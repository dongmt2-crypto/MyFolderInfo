using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace MyFolderInfo.Model
{
    /// <summary>
    /// Everything that walks, filters, renders and serialises a folder tree.
    /// </summary>
    /// <remarks>
    /// The two changes that matter most here:
    /// <list type="number">
    /// <item><description>
    /// XML is read with <see cref="XmlReader"/> and written with <see cref="XmlWriter"/>
    /// instead of loading a whole <see cref="XmlDocument"/> into a DOM. The tree files this
    /// app produces are routinely &gt;100 MB; the DOM approach needed several GB of RAM and
    /// simply ran the machine out of memory.
    /// </description></item>
    /// <item><description>
    /// Node lookups go through a <see cref="TreeIndex"/> instead of a linear scan per query.
    /// Child lookup used to be <c>list.Where(p =&gt; p.ParentCode.ToString() == tag.ToString())</c>,
    /// i.e. an O(n) scan with two string allocations per element, executed on every tree-node
    /// expansion - and comparing the *text* of two integers rather than the integers.
    /// </description></item>
    /// </list>
    /// </remarks>
    internal sealed class TreeLib
    {
        /// <summary>Next node code to hand out while scanning. Was <c>static</c>, which made
        /// two scans in one session silently corrupt each other's numbering.</summary>
        public int NodeCode { get; set; }

        public int ParentCode { get; set; }

        private readonly List<int> _searchPathList = new();
        private readonly TreeIndex _index = new();
        private ErrorLog _log;
        private SkipReport _skips;

        public TreeLib() { }

        public void AttachLog(ErrorLog log)
        {
            _log = log;
            _skips = null;
        }

        /// <summary>Per-walk tally of what could not be read. Zero until something is skipped.</summary>
        public int SkippedCount => _skips?.Count ?? 0;

        /// <summary>Writes the end-of-walk tally of skipped entries, if there were any.</summary>
        public void WriteScanSummary() => _skips?.WriteSummary();

        private SkipReport Skips => _skips ??= new SkipReport(_log);

        /// <summary>Resets the scan state and prepares for a fresh tree load.</summary>
        public void ResetScan()
        {
            NodeCode = 0;
            ParentCode = 0;
            _searchPathList.Clear();
            _skips = null;
        }

        #region Index

        /// <summary>
        /// Keeps a code-&gt;node and parent-&gt;children map in step with a list. The cache is
        /// keyed on the list instance and its count, and is dropped by every mutating method
        /// on this class, so the UI must funnel edits through here (it already does).
        /// </summary>
        private sealed class TreeIndex
        {
            private readonly Dictionary<int, ClassTree> _byCode = new();
            private readonly Dictionary<int, List<int>> _children = new();
            private List<ClassTree> _owner;
            private int _ownerCount = -1;

            public void Invalidate()
            {
                _owner = null;
                _ownerCount = -1;
                _byCode.Clear();
                _children.Clear();
            }

            public void Ensure(List<ClassTree> list)
            {
                if (list == null)
                {
                    Invalidate();
                    return;
                }

                if (ReferenceEquals(_owner, list) && _ownerCount == list.Count)
                {
                    return;
                }

                _byCode.Clear();
                _children.Clear();

                foreach (ClassTree node in list)
                {
                    _byCode[node.NodeCode] = node;

                    if (!_children.TryGetValue(node.ParentCode, out List<int> kids))
                    {
                        kids = new List<int>();
                        _children[node.ParentCode] = kids;
                    }

                    kids.Add(node.NodeCode);
                }

                _owner = list;
                _ownerCount = list.Count;
            }

            public bool TryGetNode(int code, out ClassTree node) => _byCode.TryGetValue(code, out node);

            /// <summary>Direct child codes of <paramref name="parentCode"/>, excluding the
            /// synthetic self-referencing root node (a folder lists itself as its own parent).</summary>
            public List<int> GetChildCodes(int parentCode)
            {
                if (!_children.TryGetValue(parentCode, out List<int> kids))
                {
                    return null;
                }

                // Small lists dominate; filter in place rather than allocating a LINQ chain.
                kids.RemoveAll(code =>
                {
                    if (!_byCode.TryGetValue(code, out ClassTree child))
                    {
                        return true;
                    }

                    return child.NodeCode == child.ParentCode;
                });

                return kids;
            }
        }

        private void EnsureIndex(List<ClassTree> list) => _index.Ensure(list);

        public void InvalidateIndex() => _index.Invalidate();

        #endregion

        #region Lookup

        /// <summary>
        /// Reads a node code off a <see cref="TreeNode"/>. Tags used to be set as <c>int</c> in
        /// some code paths and <c>string</c> in others, so every caller funnelled through
        /// <c>Convert.ToInt32(node.Tag)</c> and blew up on an unselected node.
        /// </summary>
        public static bool TryGetNodeCode(TreeNode node, out int code)
        {
            code = 0;
            return node?.Tag != null && int.TryParse(Convert.ToString(node.Tag, CultureInfo.InvariantCulture), out code);
        }

        public static int GetNodeCode(TreeNode node) => TryGetNodeCode(node, out int code) ? code : 0;

        public ClassTree GetTreeByID(int id, List<ClassTree> list)
        {
            if (TryGetTreeByID(id, list, out ClassTree node))
            {
                return node;
            }

            // Historical contract: callers dereference the result without a null check.
            return new ClassTree { NodeCode = id, ParentCode = id };
        }

        public bool TryGetTreeByID(int id, List<ClassTree> list, out ClassTree node)
        {
            node = null;
            if (list == null)
            {
                return false;
            }

            EnsureIndex(list);
            return _index.TryGetNode(id, out node);
        }

        public int GetMaxNodeCode(List<ClassTree> list)
        {
            if (list == null || list.Count == 0)
            {
                // list.Max() on an empty sequence threw InvalidOperationException, which is
                // exactly what happened when the user opened a legitimately empty tree file.
                return 0;
            }

            int max = int.MinValue;
            foreach (ClassTree node in list)
            {
                if (node.NodeCode > max)
                {
                    max = node.NodeCode;
                }
            }

            return max;
        }

        #endregion

        #region Scanning

        /// <summary>Recursively indexes a folder tree (folders only).</summary>
        /// <param name="token">Cancels the walk; checked at every folder.</param>
        /// <param name="onProgress">Optional (nodeCount, currentFolder) callback.</param>
        /// <param name="levels">
        /// How many folder generations to index, counting the picked folder as generation 1.
        /// <c>int.MaxValue</c> means "every level".
        /// </param>
        /// <param name="onlyDeepest">
        /// When true the intermediate generations are walked but not indexed: only the picked
        /// folder and the folders at <paramref name="levels"/> are added, the latter hung
        /// directly off the picked folder.
        /// </param>
        /// <param name="depth">Internal: generation of <paramref name="folderPath"/> (root == 1).</param>
        /// <param name="rootCode">Internal: node code of the picked folder, for the flattened attach.</param>
        public void GetFolder(
            string folderPath,
            int nodeCode,
            int parentCode,
            List<ClassTree> list,
            CancellationToken token = default,
            Action<int, string> onProgress = null,
            int levels = int.MaxValue,
            bool onlyDeepest = false,
            int depth = 1,
            int rootCode = 0)
        {
            token.ThrowIfCancellationRequested();

            if (!GetMyFiles.TryCreateDirectory(folderPath, out DirectoryInfo dir, out Exception dirError))
            {
                Skips.Folder(folderPath, dirError);
                return;
            }

            ParentCode = parentCode;

            bool isRoot = depth == 1;
            bool atTargetDepth = depth == levels;
            if (isRoot)
            {
                rootCode = NodeCode;
            }

            int thisCode = NodeCode;
            bool addNode = isRoot || !onlyDeepest || atTargetDepth;

            if (addNode)
            {
                // The deepest generation of a "deepest only" scan is re-hung under the picked
                // folder so the tree stays connected even though the middle generations are
                // skipped.
                int attachTo = (!onlyDeepest || isRoot) ? ParentCode : rootCode;
                ClassTree clsTree = CreateFolderNode(dir, thisCode, attachTo);
                list.Add(clsTree);
            }

            ClassData.Count = NodeCode;
            onProgress?.Invoke(NodeCode, dir.FullName);

            if (depth >= levels)
            {
                return;
            }

            List<DirectoryInfo> subFolders = GetMyFiles.GetDirectories(dir.FullName, Skips);
            subFolders.Sort(DirectoryNameComparer.Instance);

            foreach (DirectoryInfo sub in subFolders)
            {
                token.ThrowIfCancellationRequested();

                ParentCode = thisCode;
                NodeCode++;

                // The old code aborted the whole subtree at 245/247 characters and stored a
                // placeholder node named "FOO~". With longPathAware enabled we can now walk
                // the real tree instead of silently truncating it.
                EnterSubtree(sub.FullName, () => GetFolder(
                    sub.FullName, NodeCode, ParentCode, list, token, onProgress,
                    levels, onlyDeepest, depth + 1, rootCode));
            }
        }

        /// <summary>Recursively indexes a folder tree, including files.</summary>
        public void GetFolderWithFiles(
            string folderPath,
            int nodeCode,
            int parentCode,
            List<ClassTree> list,
            CancellationToken token = default,
            Action<int, string> onProgress = null,
            int levels = int.MaxValue,
            bool onlyDeepest = false,
            int depth = 1,
            int rootCode = 0)
        {
            token.ThrowIfCancellationRequested();

            if (!GetMyFiles.TryCreateDirectory(folderPath, out DirectoryInfo dir, out Exception dirError))
            {
                Skips.Folder(folderPath, dirError);
                return;
            }

            ParentCode = parentCode;

            bool isRoot = depth == 1;
            bool atTargetDepth = depth == levels;
            if (isRoot)
            {
                rootCode = NodeCode;
            }

            int thisCode = NodeCode;
            bool addNode = isRoot || !onlyDeepest || atTargetDepth;

            if (addNode)
            {
                int attachTo = (!onlyDeepest || isRoot) ? ParentCode : rootCode;
                ClassTree clsTree = CreateFolderNode(dir, thisCode, attachTo);
                list.Add(clsTree);
            }

            // Files belong to the folder itself, so a "deepest only" scan lists them only at
            // the requested generation; the usual scan lists them at every folder it indexes.
            if (!onlyDeepest || atTargetDepth)
            {
                List<FileInfo> files = GetMyFiles.GetFiles(dir.FullName, Skips);
                files.Sort(new FileInfoComparer());

                foreach (FileInfo file in files)
                {
                    token.ThrowIfCancellationRequested();

                    NodeCode++;
                    list.Add(GetMyFiles.ToClassTree(file, NodeCode, thisCode));
                }
            }

            ClassData.Count = NodeCode;
            onProgress?.Invoke(NodeCode, dir.FullName);

            if (depth >= levels)
            {
                return;
            }

            List<DirectoryInfo> subFolders = GetMyFiles.GetDirectories(dir.FullName, Skips);
            subFolders.Sort(DirectoryNameComparer.Instance);

            foreach (DirectoryInfo sub in subFolders)
            {
                token.ThrowIfCancellationRequested();

                ParentCode = thisCode;
                NodeCode++;
                EnterSubtree(sub.FullName, () => GetFolderWithFiles(
                    sub.FullName, NodeCode, ParentCode, list, token, onProgress,
                    levels, onlyDeepest, depth + 1, rootCode));
            }
        }

        /// <summary>
        /// Recurses into <paramref name="subPath"/> and, if anything at all goes wrong there,
        /// records it and carries on. One unreadable folder deep inside a mapped drive must not
        /// throw away everything the walk had already indexed.
        /// </summary>
        private void EnterSubtree(string subPath, Action step)
        {
            try
            {
                step();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex) when (GetMyFiles.IsRecoverable(ex))
            {
                Skips.Folder(subPath, ex);
            }
        }

        private static ClassTree CreateFolderNode(DirectoryInfo dir, int nodeCode, int parentCode)
        {
            DateTime created, written, accessed;
            try
            {
                created = dir.CreationTime;
                written = dir.LastWriteTime;
                accessed = dir.LastAccessTime;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                created = written = accessed = default;
            }

            return new ClassTree(
                // DirectoryInfo.Name is "" for a volume or share root, so scanning a mapped
                // network drive root ("G:\") produced a blank tree root.
                PathProbe.DisplayName(dir),
                nodeCode,
                parentCode,
                string.Empty,               // empty extension == "this is a folder"
                dir.FullName,
                "0",
                created == default ? string.Empty : created.ToString("yyyy-MM-dd"),
                written == default ? string.Empty : written.ToString("yyyy-MM-dd"),
                accessed == default ? string.Empty : accessed.ToString("yyyy-MM-dd"),
                written == default ? string.Empty : written.ToString("yyyyMMdd HH:mm:ss"),
                dir.FullName,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
        }

        private sealed class DirectoryNameComparer : IComparer<DirectoryInfo>
        {
            public static readonly DirectoryNameComparer Instance = new();

            public int Compare(DirectoryInfo x, DirectoryInfo y) =>
                string.Compare(x?.Name, y?.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion

        #region TreeView population

        /// <summary>
        /// Tag on the stand-in child that makes a folder show a "+" before its children are
        /// loaded. A <see cref="TreeView"/> only draws an expander for a node that already has
        /// <see cref="TreeNode"/> children, and the children were created in
        /// <see cref="TV1Select"/> - i.e. on <c>AfterSelect</c> - so a freshly scanned folder
        /// looked like a leaf and the only way to open it was to click the label.
        /// </summary>
        private sealed class Placeholder
        {
            public static readonly Placeholder Instance = new();

            private Placeholder() { }

            // TryGetNodeCode() runs int.TryParse(Convert.ToString(Tag)); an empty string fails
            // to parse, so a placeholder is never mistaken for a real node.
            public override string ToString() => string.Empty;
        }

        /// <summary>True when <paramref name="node"/> is the synthetic expander placeholder.</summary>
        public static bool IsPlaceholder(TreeNode node) => node?.Tag is Placeholder;

        /// <summary>True when the node has children other than the placeholder.</summary>
        private static bool HasRealChildren(TreeNode node)
        {
            if (node is null)
            {
                return false;
            }

            foreach (TreeNode child in node.Nodes)
            {
                if (!IsPlaceholder(child))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>True when the backing data says this node is a folder that has children.</summary>
        private bool HasModelChildren(int nodeCode, List<ClassTree> list)
        {
            // nodeCode must not be rejected for being 0: a full scan stores its root as
            // NodeCode=0, and a root with children has to advertise them with a "+" just like
            // any other folder, or opening such a tree shows a childless-looking root.
            if (list is null)
            {
                return false;
            }

            EnsureIndex(list);

            // A file is a leaf by definition; the scanned model marks files by their extension.
            if (_index.TryGetNode(nodeCode, out ClassTree node) && !string.IsNullOrEmpty(node.Extenstion))
            {
                return false;
            }

            List<int> childCodes = _index.GetChildCodes(nodeCode);
            return childCodes != null && childCodes.Count > 0;
        }

        /// <summary>
        /// Gives <paramref name="node"/> a placeholder child when the data says it is a folder
        /// with children, which is what makes the TreeView draw its "+" expander. No-op once
        /// the real children are there.
        /// </summary>
        private void EnsureExpander(TreeNode node, List<ClassTree> list)
        {
            if (node is null || IsPlaceholder(node) || HasRealChildren(node) || node.Nodes.Count > 0)
            {
                return;
            }

            if (!HasModelChildren(GetNodeCode(node), list))
            {
                return;
            }

            node.Nodes.Add(new TreeNode(string.Empty) { Tag = Placeholder.Instance });
        }

        /// <summary>
        /// Replaces a node's placeholder with its real children, each of which gets its own
        /// expander. Idempotent, so it is safe to call from both the expander click and the
        /// selection handler.
        /// </summary>
        private void Materialise(TreeNode node, List<ClassTree> list)
        {
            if (node is null || list is null || IsPlaceholder(node) || HasRealChildren(node))
            {
                return;
            }

            if (node.Nodes.Count == 1 && IsPlaceholder(node.Nodes[0]))
            {
                node.Nodes.Clear();
            }
            else if (node.Nodes.Count > 0)
            {
                return;
            }

            EnsureIndex(list);

            List<int> childCodes = _index.GetChildCodes(GetNodeCode(node));
            if (childCodes is null || childCodes.Count == 0)
            {
                return;
            }

            var nodes = new List<TreeNode>(childCodes.Count);
            foreach (int code in childCodes)
            {
                if (!_index.TryGetNode(code, out ClassTree child))
                {
                    continue;
                }

                nodes.Add(new TreeNode(child.NodeName) { Tag = code, ForeColor = ClassData.NodeColor });
            }

            if (nodes.Count == 0)
            {
                return;
            }

            foreach (TreeNode child in nodes)
            {
                EnsureExpander(child, list);
            }

            node.Nodes.AddRange(nodes.ToArray());
        }

        /// <summary>
        /// Public wrapper for callers that add a node by hand (the right-hand tree does) so the
        /// new node also gets a "+" when it is a folder with children.
        /// </summary>
        public void RefreshExpander(TreeNode node, List<ClassTree> list) => EnsureExpander(node, list);

        /// <summary>
        /// Materialises the children of a node the user is about to expand. Wire this to
        /// <c>TreeView.BeforeExpand</c> so clicking the "+" opens the folder without the node
        /// ever having to be selected first.
        /// </summary>
        public void BeforeExpand(TreeNode node, List<ClassTree> list) => Materialise(node, list);

        /// <summary>
        /// Lazily expands the selected node's children. Tags are always stored as <c>int</c>
        /// now; they used to be <c>int</c> in one code path and <c>string</c> in another.
        /// </summary>
        public void TV1Select(TreeView tv, List<ClassTree> list)
        {
            if (tv == null || list == null)
            {
                return;
            }

            TreeNode selected = tv.SelectedNode;
            if (selected == null || IsPlaceholder(selected))
            {
                return;
            }

            EnsureIndex(list);
            Materialise(selected, list);

            if (ClassData.Load2level == "1")
            {
                LoadSecondLevel(list, selected.Nodes);
            }
        }

        /// <summary>Pre-loads the grandchildren of the given nodes when the 2-level option is on.</summary>
        private void LoadSecondLevel(List<ClassTree> list, TreeNodeCollection parents)
        {
            if (ClassData.Load2level != "1")
            {
                return;
            }

            foreach (TreeNode parent in parents)
            {
                if (IsPlaceholder(parent) || HasRealChildren(parent))
                {
                    continue;
                }

                // Materialise() gives each child its own placeholder; that placeholder is what
                // the grandchildren hang off, so it has to go first.
                Materialise(parent, list);
            }
        }

        /// <summary>Adds the root nodes (nodes that list themselves as their own parent).</summary>
        public void AddTreeView(TreeView tv, List<ClassTree> list)
        {
            if (tv == null || list == null)
            {
                return;
            }

            EnsureIndex(list);

            foreach (ClassTree node in list)
            {
                if (node.ParentCode == node.NodeCode)
                {
                    var root = new TreeNode(node.NodeName) { Tag = node.NodeCode };
                    EnsureExpander(root, list);
                    tv.Nodes.Add(root);
                }
            }
        }

        #endregion

        #region Right-hand (second) tree

        /// <summary>Copies a node and its whole subtree from the left tree into the right one.</summary>
        public void CopySubtree(List<ClassTree> source, int newParentCode, int sourceNodeCode, List<ClassTree> target, ref int maxNodeCode)
        {
            if (source == null || target == null)
            {
                return;
            }

            EnsureIndex(source);

            if (!_index.TryGetNode(sourceNodeCode, out ClassTree origin))
            {
                return;
            }

            maxNodeCode++;
            int assignedCode = maxNodeCode;

            target.Add(origin.Clone(assignedCode, newParentCode));

            List<int> childCodes = _index.GetChildCodes(sourceNodeCode);
            if (childCodes == null)
            {
                return;
            }

            foreach (int childCode in childCodes)
            {
                CopySubtree(source, assignedCode, childCode, target, ref maxNodeCode);
            }

            InvalidateIndex();
        }

        /// <summary>Removes a node and every descendant from the backing list.</summary>
        public void RemoveTreeNode(int id, List<ClassTree> list)
        {
            if (list == null)
            {
                return;
            }

            InvalidateIndex();
            EnsureIndex(list);

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].ParentCode == id && list[i].NodeCode != list[i].ParentCode)
                {
                    RemoveTreeNode(list[i].NodeCode, list);
                }
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].NodeCode == id)
                {
                    list.RemoveAt(i);
                    break;
                }
            }

            InvalidateIndex();
        }

        #endregion

        #region Serialisation

        /// <summary>
        /// Streams the tree to disk. The previous implementation built the entire document in
        /// memory as a DOM and then saved it, which doubled peak memory on multi-hundred-MB trees.
        /// </summary>
        public void WriteXml(List<ClassTree> list, string filename)
        {
            if (list == null)
            {
                return;
            }

            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                CloseOutput = true,
            };

            using var writer = XmlWriter.Create(filename, settings);

            writer.WriteStartDocument();
            writer.WriteComment(" My Folder Info ");
            writer.WriteComment(" DongMT's soft ");

            writer.WriteStartElement("xml");

            foreach (ClassTree item in list)
            {
                WriteNode(writer, item);
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private static void WriteNode(XmlWriter writer, ClassTree item)
        {
            writer.WriteStartElement("Node");
            writer.WriteAttributeString("NodeCode", item.NodeCode.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("NodeName", item.NodeName);
            writer.WriteAttributeString("ParentCode", item.ParentCode.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Extenstion", item.Extenstion);
            writer.WriteAttributeString("OriginalUrl", item.OriginalUrl);
            writer.WriteAttributeString("Length", item.Length);
            writer.WriteAttributeString("CreationTime", item.CreationTime);
            writer.WriteAttributeString("LastWriteTime", item.LastWriteTime);
            writer.WriteAttributeString("LastAccessTime", item.LastAccessTime);
            writer.WriteAttributeString("LastWriteHourTime", item.LastWriteHourTime);
            writer.WriteAttributeString("CompareUrl", item.CompareUrl);
            writer.WriteAttributeString("Extend1", item.Extend1);
            writer.WriteAttributeString("Extend2", item.Extend2);
            writer.WriteAttributeString("Extend3", item.Extend3);
            writer.WriteAttributeString("Extend4", item.Extend4);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Streams a tree file into <paramref name="list"/>. Returns false if the file is
        /// missing or malformed instead of throwing at the call site.
        /// </summary>
        public bool ReadXml(string filename, List<ClassTree> list)
        {
            if (list == null)
            {
                return false;
            }

            list.Clear();

            if (!File.Exists(filename))
            {
                _log?.Write($"Tree file not found: {filename}");
                return false;
            }

            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                CloseInput = true,
            };

            try
            {
                using var reader = XmlReader.Create(filename, settings);

                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Element
                        || !string.Equals(reader.LocalName, "Node", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    ClassTree node = new()
                    {
                        NodeName = reader.GetAttribute("NodeName") ?? string.Empty,
                        Extenstion = reader.GetAttribute("Extenstion") ?? string.Empty,
                        OriginalUrl = reader.GetAttribute("OriginalUrl") ?? string.Empty,
                        Length = reader.GetAttribute("Length") ?? string.Empty,
                        CreationTime = reader.GetAttribute("CreationTime") ?? string.Empty,
                        LastWriteTime = reader.GetAttribute("LastWriteTime") ?? string.Empty,
                        LastAccessTime = reader.GetAttribute("LastAccessTime") ?? string.Empty,
                        LastWriteHourTime = reader.GetAttribute("LastWriteHourTime") ?? string.Empty,
                        CompareUrl = reader.GetAttribute("CompareUrl") ?? string.Empty,
                        Extend1 = reader.GetAttribute("Extend1") ?? string.Empty,
                        Extend2 = reader.GetAttribute("Extend2") ?? string.Empty,
                        Extend3 = reader.GetAttribute("Extend3") ?? string.Empty,
                        Extend4 = reader.GetAttribute("Extend4") ?? string.Empty,
                    };

                    if (!int.TryParse(reader.GetAttribute("NodeCode"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int nodeCode))
                    {
                        _log?.Write($"Skipping a <Node> with a non-numeric NodeCode in {filename}.");
                        continue;
                    }

                    node.NodeCode = nodeCode;

                    if (!int.TryParse(reader.GetAttribute("ParentCode"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int parentCode))
                    {
                        parentCode = nodeCode;
                    }

                    node.ParentCode = parentCode;

                    list.Add(node);
                }

                InvalidateIndex();
                return true;
            }
            catch (Exception ex) when (ex is XmlException or IOException or UnauthorizedAccessException)
            {
                _log?.Write($"Failed to read tree file {filename}.", ex);
                list.Clear();
                return false;
            }
        }

        #endregion

        #region Search

        /// <summary>
        /// Selects the node with <paramref name="nodeCode"/> in <paramref name="tv"/>,
        /// expanding each level on the way down.
        /// </summary>
        public void DoSearch(int nodeCode, TreeView tv, List<ClassTree> list)
        {
            if (tv == null || tv.Nodes.Count == 0 || list == null)
            {
                // tv.Nodes[0] was indexed unconditionally, so searching an empty or
                // not-yet-loaded tree threw IndexOutOfRangeException.
                return;
            }

            EnsureIndex(list);

            _searchPathList.Clear();
            CreateSearchPath(nodeCode, list);

            // _searchPathList runs leaf-first and ends with the root (CreateSearchPath stops
            // at the node that is its own parent), so the root is the LAST element. The old
            // loop walked from the last element down while already sitting on that root, so
            // the first step looked for the root among its own children, always got null and
            // returned - the search highlighted nothing, every time.
            int rootCode = _searchPathList[_searchPathList.Count - 1];
            TreeNode current = FindRoot(tv, rootCode);
            if (current == null)
            {
                return;
            }

            for (int i = _searchPathList.Count - 2; i >= 0; i--)
            {
                current = FindChild(current, _searchPathList[i]);
                if (current == null)
                {
                    return;
                }

                // Expanding is what lazily materialises the next level, so it has to happen
                // before the following FindChild.
                current.Expand();
            }

            tv.SelectedNode = current;
            current.Expand();
            current.BackColor = ClassData.SelectNodeColor;
            current.EnsureVisible();
        }

        private static TreeNode FindRoot(TreeView tv, int code)
        {
            foreach (TreeNode node in tv.Nodes)
            {
                if (TryGetNodeCode(node, out int nodeCode) && nodeCode == code)
                {
                    return node;
                }
            }

            return null;
        }

        private static TreeNode FindChild(TreeNode parent, int code)
        {
            if (parent == null)
            {
                return null;
            }

            foreach (TreeNode child in parent.Nodes)
            {
                if (TryGetNodeCode(child, out int childCode) && childCode == code)
                {
                    return child;
                }
            }

            return null;
        }

        private void CreateSearchPath(int nodeCode, List<ClassTree> list)
        {
            _searchPathList.Add(nodeCode);

            // Guard against a cycle in a hand-edited file; the old recursion had no depth limit.
            if (_searchPathList.Count > 512 || !TryGetTreeByID(nodeCode, list, out ClassTree node))
            {
                return;
            }

            if (node.NodeCode != node.ParentCode)
            {
                CreateSearchPath(node.ParentCode, list);
            }
        }

        /// <summary>True when every comma-separated term in <paramref name="substring"/> matches
        /// <paramref name="str"/> as a whole word.</summary>
        public bool IsExactlyMap(string str, string substring)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substring))
            {
                return false;
            }

            str = str.ToLower(CultureInfo.CurrentCulture);
            string[] terms = substring.Split(',');

            foreach (string rawTerm in terms)
            {
                string term = rawTerm.Trim().ToLower(CultureInfo.CurrentCulture);
                if (term.Length == 0)
                {
                    continue;
                }

                if (str == term
                    || str.EndsWith(" " + term, StringComparison.Ordinal)
                    || str.StartsWith(term + " ", StringComparison.Ordinal)
                    || str.Contains(" " + term + " ", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>True when any comma-separated term appears anywhere in <paramref name="str"/>.</summary>
        public bool IsNormalMap(string str, string substring)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substring))
            {
                return false;
            }

            str = str.ToLower(CultureInfo.CurrentCulture);
            string[] terms = substring.Split(',');

            foreach (string rawTerm in terms)
            {
                string term = rawTerm.Trim().ToLower(CultureInfo.CurrentCulture);
                if (term.Length > 0 && str.Contains(term, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Text report

        /// <summary>
        /// Writes a flat, human-readable listing. The old version re-scanned the entire list
        /// once per folder (O(n^2)) - unusable on the large trees this app is built for.
        /// </summary>
        public void WriteText2(string filename, List<ClassTree> list, bool numberNames = true)
        {
            if (list == null)
            {
                return;
            }

            EnsureIndex(list);

            using var writer = new StreamWriter(filename, append: false, Encoding.UTF8);

            string ExportName(ClassTree node, ClassTree parent)
            {
                string current = node.NodeName ?? string.Empty;
                if (numberNames || current.Count(c => c == '-') >= 2)
                {
                    return current;
                }

                if (parent is not null && parent != node)
                {
                    return parent.OriginalUrl + " - " + current;
                }

                return current;
            }

            int folderNumber = 0;

            foreach (ClassTree folder in list)
            {
                if (!string.IsNullOrEmpty(folder.Extenstion))
                {
                    continue;   // not a folder
                }

                ClassTree parentNode = _index.TryGetNode(folder.ParentCode, out ClassTree parent)
                    ? parent
                    : null;

                folderNumber++;
                if (numberNames)
                {
                    writer.WriteLine("..........................");
                    writer.WriteLine(folderNumber + ". " + folder.OriginalUrl);
                }
                else
                {
                    writer.WriteLine(ExportName(folder, parentNode));
                }

                List<int> childCodes = _index.GetChildCodes(folder.NodeCode);
                if (childCodes == null)
                {
                    continue;
                }

                int itemNumber = 0;
                foreach (int code in childCodes)
                {
                    if (!_index.TryGetNode(code, out ClassTree child))
                    {
                        continue;
                    }

                    itemNumber++;
                    writer.WriteLine(numberNames
                        ? itemNumber + ". " + child.NodeName + "     [" + child.Length + " KBs]     "
                        : ExportName(child, folder) + "     [" + child.Length + " KBs]     ");
                }
            }
        }

        #endregion
    }
}
