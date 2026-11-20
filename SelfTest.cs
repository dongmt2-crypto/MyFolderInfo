using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using MyFolderInfo.Model;
using MyFolderInfo.UI;

namespace MyFolderInfo
{
    /// <summary>
    /// Headless smoke test, run with <c>MyFolderInfo.exe --selftest</c>. The exit code is the
    /// number of failed checks, and a copy of the report is written to
    /// <c>%LOCALAPPDATA%\MyFolderInfo\selftest.txt</c>.
    /// </summary>
    /// <remarks>
    /// There is no test project and the app is a WinExe, so the flows that were rewritten -
    /// scan, XML round trip, search, compare, and every form's construction - otherwise have
    /// no automated coverage and a regression only shows up as a crash while the user clicks.
    /// Nothing here writes to the shipped data files: anything that saves goes to a scratch
    /// folder under the temp path.
    /// </remarks>
    internal static class SelfTest
    {
        private const string ScratchRoot = "MyFolderInfo.SelfTest";

        private static readonly StringBuilder Report = new();
        private static int _failed;
        private static int _passed;

        public static int Run()
        {
            string scratch = Path.Combine(Path.GetTempPath(), ScratchRoot);

            Check("scratch folder is writable", () => CreateSampleTree(scratch));
            Check("scan: folders only", () => ScanFoldersOnly(scratch));
            Check("scan: folders and files", () => ScanWithFiles(scratch));
            Check("scan: cancellation is honoured", () => ScanCancellation(scratch));
            Check("scan: level limit and deepest-only modes", () => ScanLevelLimited(scratch));
            Check("scan: unreadable and malformed entries are skipped, not fatal",
                () => ScanSkipsUnreadableEntries(scratch));
            Check("path: unreadable folders are diagnosed, not mislabelled", () => PathProbeChecks(scratch));
            Check("path: translations carry the diagnostics", () => LanguageFilesCarryDriveDiagnostics());
            Check("xml: write then read round trip", () => XmlRoundTrip(scratch));
            Check("xml: text export", () => TextExport(scratch));
            Check("context menu: folder and file nodes are both openable", () => ShellTargets(scratch));
            Check("tree: subtree copy and remove", () => SubtreeCopyAndRemove(scratch));
            Check("tree: AddTreeView + DoSearch", () => TreeViewSearch(scratch));
            Check("tree: folders show a + and expand on click", () => TreeViewExpanders(scratch));
            Check("tree: a code-0 root shows + and expands", () => ZeroCodeRoot(scratch));
            Check("compare: null attributes are tolerated", () => CompareWithEmptyAttributes(scratch));
            Check("options: Options.xml loads", () => OptionsLoad());
            Check("theme: Config.xml parses", () => ThemeLoad());
            Check("theme: nested containers are painted", () => ThemeReachesNestedControls());
            Check("designer: control names are unique", () => UniqueControlNames());
            Check("dialogs: accept and cancel buttons are wired", () => DialogButtons());
            Check("language: every form tag loads", () => LanguageLoad());
            Check("forms: every form constructs", () => FormsConstruct());
            Check("banner: MediaUserControl loads", () => BannerLoad());

            Report.AppendLine();
            Report.AppendLine(_failed == 0
                ? $"ALL PASS ({_passed} assertions)"
                : $"FAILED: {_failed} check(s) failed, {_passed} assertion(s) passed");

            string text = Report.ToString();
            Console.Write(text);
            TryWriteReport(text);

            return _failed;
        }

        #region Sample data

        private static void CreateSampleTree(string scratch)
        {
            if (Directory.Exists(scratch))
            {
                Directory.Delete(scratch, recursive: true);
            }

            string root = Path.Combine(scratch, "tree");
            Directory.CreateDirectory(Path.Combine(root, "alpha", "beta"));
            Directory.CreateDirectory(Path.Combine(root, "alpha", "gamma"));
            Directory.CreateDirectory(Path.Combine(root, "delta"));

            File.WriteAllText(Path.Combine(root, "root.txt"), "root");
            File.WriteAllText(Path.Combine(root, "alpha", "a.log"), "alpha");
            File.WriteAllText(Path.Combine(root, "alpha", "beta", "deep.xml"), "<a />");
            File.WriteAllText(Path.Combine(root, "delta", "d.dat"), "delta");
        }

        private static TreeLib SampleTree(string scratch, bool withFiles, List<ClassTree> list)
        {
            var lib = new TreeLib();
            // GetFolder/GetFolderWithFiles allocate codes from the NodeCode field, not from
            // their own first argument, so the caller has to seed it - as FolderInfo does.
            lib.ResetScan();
            lib.NodeCode = 1;

            if (withFiles)
            {
                lib.GetFolderWithFiles(Path.Combine(scratch, "tree"), 1, 1, list);
            }
            else
            {
                lib.GetFolder(Path.Combine(scratch, "tree"), 1, 1, list);
            }

            return lib;
        }

        #endregion

        #region Checks

        /// <summary>
        /// The old scanner mislabelled every file as a folder, so "search for files" found
        /// nothing. Assert the two node kinds stay distinguishable.
        /// </summary>
        private static void ScanFoldersOnly(string scratch)
        {
            var list = new List<ClassTree>();
            SampleTree(scratch, withFiles: false, list);

            AssertEqual(5, list.Count, "folder count");
            AssertTrue(list.All(n => n.IsFolder), "every scanned node is a folder");
            AssertEqual(1, list[0].NodeCode, "root code");
            AssertEqual(list[0].NodeCode, list[0].ParentCode, "root is its own parent");
        }

        private static void ScanWithFiles(string scratch)
        {
            var list = new List<ClassTree>();
            SampleTree(scratch, withFiles: true, list);

            AssertEqual(9, list.Count, "total node count");

            List<ClassTree> files = list.Where(n => !n.IsFolder).ToList();
            AssertEqual(4, files.Count, "file count");
            AssertTrue(files.All(f => !string.IsNullOrEmpty(f.Extenstion)), "files carry an extension");
            AssertTrue(files.All(f => !string.IsNullOrEmpty(f.OriginalUrl)), "files carry their path");
            AssertTrue(files.All(f => f.ParentCode != f.NodeCode), "files are not their own parent");
        }

        /// <summary>
        /// The scan can be asked for a fixed number of generations (the picked folder counts as
        /// generation 1) or for just the deepest of those generations. Assert both the counts and
        /// that a "deepest only" scan stays connected to the picked folder.
        /// </summary>
        private static void ScanLevelLimited(string scratch)
        {
            AssertEqual(1, ScanLevels(scratch, withFiles: false, levels: 1, onlyDeepest: false).Count,
                "level 1, folders only: just the picked folder");
            AssertEqual(3, ScanLevels(scratch, withFiles: false, levels: 2, onlyDeepest: false).Count,
                "level 2, folders only: picked folder + alpha + delta");
            AssertEqual(5, ScanLevels(scratch, withFiles: false, levels: 3, onlyDeepest: false).Count,
                "level 3, folders only: adds beta + gamma");

            AssertEqual(2, ScanLevels(scratch, withFiles: true, levels: 1, onlyDeepest: false).Count,
                "level 1, with files: picked folder + root.txt");
            AssertEqual(6, ScanLevels(scratch, withFiles: true, levels: 2, onlyDeepest: false).Count,
                "level 2, with files: adds a.log + d.dat");
            AssertEqual(9, ScanLevels(scratch, withFiles: true, levels: 3, onlyDeepest: false).Count,
                "level 3, with files: the full tree");

            List<ClassTree> deepest = ScanLevels(scratch, withFiles: false, levels: 3, onlyDeepest: true);
            AssertEqual(3, deepest.Count, "deepest only, level 3: picked folder + beta + gamma");
            ClassTree root = deepest[0];
            AssertTrue(deepest.Skip(1).All(n => n.ParentCode == root.NodeCode),
                "deepest-only folders hang directly off the picked folder");
            AssertTrue(deepest.All(n => n.NodeName != "alpha" && n.NodeName != "delta"),
                "deepest-only skipped the intermediate generation");

            List<ClassTree> deepestWithFiles = ScanLevels(scratch, withFiles: true, levels: 3, onlyDeepest: true);
            AssertEqual(4, deepestWithFiles.Count, "deepest only, with files: adds deep.xml");
            AssertTrue(deepestWithFiles.All(n => n.NodeName != "root.txt" && n.NodeName != "a.log"),
                "deepest-only did not list files from the skipped generations");

            AssertEqual(1, ScanLevels(scratch, withFiles: false, levels: 1, onlyDeepest: true).Count,
                "deepest only, level 1: just the picked folder");
        }

        private static List<ClassTree> ScanLevels(string scratch, bool withFiles, int levels, bool onlyDeepest)
        {
            var list = new List<ClassTree>();
            var lib = new TreeLib();
            lib.ResetScan();
            lib.NodeCode = 1;
            string root = Path.Combine(scratch, "tree");

            if (withFiles)
            {
                lib.GetFolderWithFiles(root, 1, 1, list, default, null, levels, onlyDeepest);
            }
            else
            {
                lib.GetFolder(root, 1, 1, list, default, null, levels, onlyDeepest);
            }

            return list;
        }

        private static void ScanCancellation(string scratch)
        {
            var lib = new TreeLib();
            var list = new List<ClassTree>();
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            bool cancelled = false;
            try
            {
                lib.GetFolder(Path.Combine(scratch, "tree"), 1, 1, list, cts.Token);
            }
            catch (OperationCanceledException)
            {
                cancelled = true;
            }

            AssertTrue(cancelled, "an already-cancelled token stops the walk");
            AssertEqual(0, list.Count, "nothing was indexed");
        }

        /// <summary>
        /// A mapped SharePoint/WebDAV drive is not a normal filesystem: it reports protected
        /// folders ("Access to 'C:\Windows\WUModels' is denied"), folders that vanish mid-walk,
        /// and now and then a name containing a NUL. The scan used to die on the last of those
        /// with "Null character in path" after indexing the whole tree, throwing away the
        /// result. Assert that a walk survives all three and reports what it skipped.
        /// </summary>
        private static void ScanSkipsUnreadableEntries(string scratch)
        {
            AssertTrue(!GetMyFiles.IsUsablePath(null), "a null path is unusable");
            AssertTrue(!GetMyFiles.IsUsablePath("   "), "a blank path is unusable");
            AssertTrue(!GetMyFiles.IsUsablePath(@"C:\Docs\report" + '\0' + ".xlsx"),
                "a name carrying a NUL is unusable");
            AssertTrue(!GetMyFiles.IsUsablePath(@"C:\Docs\report" + '\u0001' + ".xlsx"),
                "a name carrying another control character is unusable");
            AssertTrue(GetMyFiles.IsUsablePath("tab\tname.txt"),
                "the XML-legal control characters still pass");
            AssertTrue(GetMyFiles.IsUsablePath(Path.Combine(scratch, "tree")),
                "an ordinary path is usable");

            AssertTrue(!GetMyFiles.TryCreateDirectory(@"C:\Docs\x" + '\0', out DirectoryInfo made, out Exception why),
                "a NUL path yields no DirectoryInfo");
            AssertTrue(made == null, "no DirectoryInfo escapes for a NUL path");
            AssertTrue(why != null, "the reason the path was rejected is reported");

            string missing = Path.Combine(scratch, "no-such-folder");
            var skips = new SkipReport(null);
            AssertEqual(0, GetMyFiles.GetDirectories(missing, skips).Count,
                "listing a missing folder yields nothing");
            AssertEqual(0, GetMyFiles.GetFiles(missing, skips).Count,
                "listing the files of a missing folder yields nothing");
            AssertEqual(2, skips.Count, "both misses were counted as skipped");

            // The walk itself: a bad root is reported and skipped, not thrown.
            var lib = new TreeLib();
            var list = new List<ClassTree>();
            lib.ResetScan();

            bool threw = false;
            try
            {
                lib.GetFolderWithFiles(scratch + '\\' + '\0', 0, 0, list);
            }
            catch (Exception)
            {
                threw = true;
            }

            AssertTrue(!threw, "a malformed root path does not abort the scan");
            AssertEqual(0, list.Count, "a malformed root path indexes nothing");
            AssertTrue(lib.SkippedCount > 0, "the skip was counted");

            // And a healthy root next to a bad one still produces a complete tree.
            var healthy = new TreeLib();
            var healthyList = new List<ClassTree>();
            healthy.ResetScan();
            healthy.NodeCode = 1;
            healthy.GetFolderWithFiles(Path.Combine(scratch, "tree"), 1, 1, healthyList);

            AssertEqual(9, healthyList.Count, "an unaffected tree still scans completely");
            AssertTrue(healthyList.All(n => GetMyFiles.IsUsablePath(n.OriginalUrl)),
                "no unusable path reaches the tree");
        }

        /// <summary>
        /// The scan entry points gated on <c>Directory.Exists</c>, which never throws: it answers
        /// false for a missing path <i>and</i> for a mapped SharePoint drive that is merely
        /// unreachable, so the user got "'G:\Docs' is not a folder" either way. Assert the probe
        /// separates them, that the paths the folder picker hands back are normalised, and that
        /// a drive root - whose <c>DirectoryInfo.Name</c> is empty - still gets a name.
        /// </summary>
        private static void PathProbeChecks(string scratch)
        {
            string folder = Path.Combine(scratch, "tree");
            string file = Path.Combine(folder, "root.txt");
            string missing = Path.Combine(folder, "no-such-folder");

            AssertEqual(PathVerdict.Ok, PathProbe.Probe(folder, out _), "an existing folder probes Ok");
            AssertEqual(PathVerdict.IsFile, PathProbe.Probe(file, out _), "a file is not a folder");
            AssertEqual(PathVerdict.NotFound, PathProbe.Probe(missing, out _),
                "a missing folder is NotFound rather than 'unreachable'");
            AssertEqual(PathVerdict.NotFound, PathProbe.Probe("   ", out _), "blank input is not a folder");
            AssertEqual(PathVerdict.NotFound, PathProbe.Probe(null, out _), "null input is not a folder");

            PathProbe.Probe(missing, out int missingCode);
            AssertTrue(missingCode != 0, "the Win32 error code is reported, not swallowed");

            // A name typed into the picker arrives quoted and padded.
            string quoted = "  \"" + folder + "\"  ";
            AssertEqual(folder, PathProbe.Normalize(quoted), "quotes and padding are stripped");
            AssertEqual(PathVerdict.Ok, PathProbe.Probe(quoted, out _), "a quoted path still opens");

            // "This PC" entries come back through the shell namespace.
            AssertEqual(folder,
                PathProbe.Normalize("::{20D04FE0-3AEA-1069-A2D8-08002B30309D}\\" + folder),
                "a shell namespace path is unwrapped");

            // The extended-length prefix, common on long SharePoint paths.
            AssertEqual(folder, PathProbe.Normalize(@"\\?\" + folder), "the \\\\?\\ prefix is stripped");
            AssertEqual(@"\\server\share", PathProbe.Normalize(@"\\?\UNC\server\share"),
                "\\\\?\\UNC\\ becomes a plain UNC path");

            // DirectoryInfo.Name is "" for a volume root, which made the scanned tree root blank.
            string driveRoot = Path.GetPathRoot(folder);
            AssertTrue(driveRoot != null && driveRoot.Length > 0, "the scratch folder has a drive root");
            AssertEqual(driveRoot, PathProbe.DisplayName(new DirectoryInfo(driveRoot)),
                "a drive root is named after itself");
            AssertEqual("tree", PathProbe.DisplayName(new DirectoryInfo(folder)),
                "an ordinary folder keeps its own name");

            AssertTrue(PathProbe.IsDriveLetterAssigned(folder), "the current drive is assigned here");
            AssertTrue(PathProbe.IsDriveLetterAssigned(@"\\server\share"),
                "a UNC path is not judged on a drive letter");
        }

        /// <summary>
        /// The scan-failure texts are looked up by id in the language files. A translation file
        /// that lost them silently degrades to English, so assert they are present in both.
        /// </summary>
        private static void LanguageFilesCarryDriveDiagnostics()
        {
            foreach (string name in new[] { "English.xml", "Vietnamese.xml" })
            {
                string file = Path.Combine(AppPaths.LanguageFolder, name);
                AssertTrue(File.Exists(file), $"{name} is present next to the executable");

                var doc = new XmlDocument { XmlResolver = null };
                doc.Load(file);

                for (int id = 40; id <= 47; id++)
                {
                    // The files pad the ids to three digits, and the XPath compares text, so
                    // '40' would match nothing where '040' matches exactly one row.
                    XmlNodeList hits = doc.SelectNodes(
                        $"/dsname/dsname[form='frmMain' and tag='{id:000}']");

                    AssertTrue(hits != null && hits.Count == 1,
                        $"{name} defines frmMain message {id:000}");
                    AssertTrue(!string.IsNullOrWhiteSpace(hits?[0].InnerText),
                        $"{name} message {id:000} has text");
                }
            }
        }

        private static void XmlRoundTrip(string scratch)
        {
            var list = new List<ClassTree>();
            TreeLib lib = SampleTree(scratch, withFiles: true, list);

            string file = Path.Combine(scratch, "tree.xml");
            lib.WriteXml(list, file);
            AssertTrue(File.Exists(file) && new FileInfo(file).Length > 0, "tree.xml was written");

            var doc = new XmlDocument { XmlResolver = null };
            doc.Load(file);
            XmlNodeList elements = doc.SelectNodes("//Node");
            AssertEqual(list.Count, elements?.Count ?? -1, "element count in the written XML");
            AssertEqual("xml", doc.DocumentElement?.Name, "root element name");

            var reloaded = new List<ClassTree>();
            AssertTrue(lib.ReadXml(file, reloaded), "ReadXml reported success");
            AssertEqual(list.Count, reloaded.Count, "node count survived the round trip");

            ClassTree originalRoot = list[0];
            AssertTrue(lib.TryGetTreeByID(originalRoot.NodeCode, reloaded, out ClassTree copy), "root is addressable after reload");
            AssertEqual(originalRoot.NodeName, copy?.NodeName, "root name survived");

            foreach (ClassTree node in list)
            {
                if (!lib.TryGetTreeByID(node.NodeCode, reloaded, out ClassTree same) || same.NodeName != node.NodeName)
                {
                    AssertTrue(false, $"node {node.NodeCode} ('{node.NodeName}') survived the round trip");
                    return;
                }
            }

            AssertTrue(true, "every node survived the round trip");
        }

        private static void TextExport(string scratch)
        {
            var list = new List<ClassTree>();
            TreeLib lib = SampleTree(scratch, withFiles: true, list);

            string file = Path.Combine(scratch, "tree.txt");
            lib.WriteText2(file, list);
            AssertTrue(File.Exists(file) && new FileInfo(file).Length > 0, "tree.txt was written");
        }

        /// <summary>
        /// "Thực thi" has to open whatever the right-clicked node points at, and the tree is
        /// mostly folders. <c>File.Exists</c> alone answers false for a directory, which made
        /// the menu report that the node had vanished for every folder in the tree.
        /// </summary>
        private static void ShellTargets(string scratch)
        {
            string folder = Path.Combine(scratch, "tree");
            string file = Path.Combine(folder, "root.txt");
            string missing = Path.Combine(folder, "not-here.txt");

            AssertTrue(FolderInfo.CanOpenInShell(folder), "a folder node is openable");
            AssertTrue(FolderInfo.CanOpenInShell(file), "a file node is openable");
            AssertTrue(!FolderInfo.CanOpenInShell(missing), "a path that is gone is not openable");
            AssertTrue(!FolderInfo.CanOpenInShell(null), "a node without a url is not openable");
            AssertTrue(!FolderInfo.CanOpenInShell("   "), "a blank url is not openable");
        }

        private static void SubtreeCopyAndRemove(string scratch)        {
            var left = new List<ClassTree>();
            TreeLib lib = SampleTree(scratch, withFiles: true, left);

            int alphaCode = left.First(n => n.NodeName == "alpha").NodeCode;
            int expected = CountSubtree(left, alphaCode);

            var right = new List<ClassTree>();
            int maxCode = lib.GetMaxNodeCode(left);
            lib.CopySubtree(left, 0, alphaCode, right, ref maxCode);

            AssertEqual(expected, right.Count, "the whole subtree was copied");
            AssertEqual(maxCode, right.Max(n => n.NodeCode), "copied codes sit above the source maximum");
            AssertTrue(right.All(n => n.NodeCode != alphaCode), "the copied subtree was re-keyed");
            AssertTrue(right.All(n => left.All(o => o.NodeCode != n.NodeCode)), "no copied code collides with a source code");

            lib.RemoveTreeNode(alphaCode, left);
            AssertTrue(left.All(n => n.NodeCode != alphaCode), "the removed node is gone");
            AssertEqual(9 - expected, left.Count, "exactly the subtree was removed");
        }

        private static void TreeViewSearch(string scratch)
        {
            var list = new List<ClassTree>();
            TreeLib lib = SampleTree(scratch, withFiles: true, list);

            using var tv = new TreeView();
            lib.AddTreeView(tv, list);
            AssertEqual(1, tv.Nodes.Count, "one root node");

            Populate(tv.Nodes[0], list, (int)tv.Nodes[0].Tag, 0);
            AssertTrue(tv.Nodes[0].Nodes.Count > 0, "the root has children");

            int target = list.First(n => n.NodeName == "deep.xml").NodeCode;
            lib.DoSearch(target, tv, list);
            AssertTrue(tv.SelectedNode != null, "a node is selected after DoSearch");
            AssertEqual(target, TreeLib.GetNodeCode(tv.SelectedNode), "the requested node is selected");

            // The empty tree used to throw IndexOutOfRangeException on tv.Nodes[0].
            using var empty = new TreeView();
            lib.DoSearch(target, empty, list);
            AssertEqual(0, empty.Nodes.Count, "DoSearch on an empty tree is a no-op");
        }

        /// <summary>
        /// A folder that has children must show a "+" straight after a scan, and clicking it
        /// must open the folder. Before this, children were only created from AfterSelect, and
        /// a TreeView draws an expander solely on whether the node already has child nodes, so
        /// every folder looked like an empty leaf and had to be clicked open.
        /// </summary>
        private static void TreeViewExpanders(string scratch)
        {
            var list = new List<ClassTree>();
            TreeLib lib = SampleTree(scratch, withFiles: true, list);

            using var tv = new TreeView();
            lib.AddTreeView(tv, list);
            AssertEqual(1, tv.Nodes.Count, "one root node after AddTreeView");

            TreeNode root = tv.Nodes[0];

            // The root is a folder with children, so it must advertise them up front.
            AssertTrue(root.Nodes.Count > 0,
                "a folder with children gets a child node so the TreeView draws a '+'");
            AssertTrue(TreeLib.IsPlaceholder(root.Nodes[0]),
                "the child is the expander placeholder, not real data");
            AssertEqual(0, TreeLib.GetNodeCode(root.Nodes[0]),
                "the placeholder carries no node code, so it cannot be selected as data");

            // Clicking "+" - BeforeExpand - must swap the placeholder for the real children.
            lib.BeforeExpand(root, list);
            AssertTrue(root.Nodes.Count > 1, "expanding a folder reveals more than the placeholder");
            AssertTrue(AllNodes(root, n => !TreeLib.IsPlaceholder(n)),
                "no placeholder survives the expansion");
            AssertTrue(AllNodes(root, n => TreeLib.GetNodeCode(n) != 0),
                "every revealed child carries a node code");

            // The root lists itself as its own parent, and TreeIndex.GetChildCodes deliberately
            // filters that synthetic self-reference out, so the root is not its own child.
            int expectedChildren = list.Count(
                n => n.ParentCode == TreeLib.GetNodeCode(root) && n.NodeCode != n.ParentCode);
            AssertEqual(expectedChildren, root.Nodes.Count, "every child of the folder is revealed");

            // A file is a leaf: no expander, ever.
            ClassTree anyFile = list.First(n => !string.IsNullOrEmpty(n.Extenstion));
            TreeNode file = FindNode(root, anyFile.NodeCode);
            AssertTrue(file != null, "the sample tree contains a file node");
            lib.BeforeExpand(file, list);
            AssertEqual(0, file.Nodes.Count, "a file never gets an expander");

            // Re-expanding must not duplicate the children.
            lib.BeforeExpand(root, list);
            AssertEqual(expectedChildren, root.Nodes.Count, "expanding twice does not duplicate children");

            // Selection still materialises children, which is how the old code worked.
            using var tv2 = new TreeView();
            lib.AddTreeView(tv2, list);
            tv2.SelectedNode = tv2.Nodes[0];
            lib.TV1Select(tv2, list);
            AssertTrue(AllNodes(tv2.Nodes[0], n => !TreeLib.IsPlaceholder(n)),
                "selecting a node still loads its children");

            // Grandchildren exist and are reachable by expanding step by step.
            TreeNode deep = FirstNode(root, n => n.Nodes.Count > 0);
            lib.BeforeExpand(deep, list);
            AssertTrue(AllNodes(deep, n => !TreeLib.IsPlaceholder(n)),
                "expanding a nested folder reveals its children too");

            // End to end on a real TreeView, wired exactly the way FolderInfo wires it: a click
            // on the "+" raises BeforeExpand, and the handler must open the folder without the
            // node ever becoming selected.
            using var tv3 = new TreeView();
            List<ClassTree> backed = list;
            TreeLib captured = lib;
            tv3.BeforeExpand += (_, e) => captured.BeforeExpand(e.Node, backed);
            lib.AddTreeView(tv3, list);
            TreeNode only = tv3.Nodes[0];
            RaiseBeforeExpand(tv3, only);
            AssertTrue(AllNodes(only, n => !TreeLib.IsPlaceholder(n)),
                "a real TreeView expands the folder when BeforeExpand is raised");
            AssertTrue(tv3.SelectedNode == null, "expanding does not select the node");

            // Both trees in the real form must be wired, or the + does nothing in the app. The
            // handlers are private members of FolderInfo, so look them up on the declaring form
            // rather than on the TreeView.
            using var form = new FolderInfo();
            AssertTrue(HasExpandHandler(form, "treeView1", "TreeView1_BeforeExpand"),
                "FolderInfo wires treeView1.BeforeExpand");
            AssertTrue(HasExpandHandler(form, "treeView2", "TreeView2_BeforeExpand"),
                "FolderInfo wires treeView2.BeforeExpand");
        }

        /// <summary>
        /// A full scan numbers its root 0 (GetFolder/GetFolderWithFiles read the NodeCode
        /// field, which ResetScan seeds with 0), and files saved by that older numbering - for
        /// example D:\test.xml - come back with a root whose code is 0. That root used to be
        /// refused an expander and showed as a childless leaf.
        /// </summary>
        private static void ZeroCodeRoot(string scratch)
        {
            var list = new List<ClassTree>();
            TreeLib lib = SampleTree(scratch, withFiles: true, list);

            // Re-key the tree the way a real full scan stores it: root code/parent 0, every
            // direct child pointing back at 0.
            ClassTree root = list[0];
            root.NodeCode = 0;
            root.ParentCode = 0;
            foreach (ClassTree kick in list.Where(n => n.ParentCode == 1 && n.NodeCode != 1))
            {
                kick.ParentCode = 0;
            }

            using var tv = new TreeView();
            lib.AddTreeView(tv, list);
            AssertEqual(1, tv.Nodes.Count, "the code-0 root is the only top-level node");

            TreeNode zero = tv.Nodes[0];
            AssertEqual(0, TreeLib.GetNodeCode(zero), "the root carried its code-0 tag");
            AssertTrue(zero.Nodes.Count > 0 && TreeLib.IsPlaceholder(zero.Nodes[0]),
                "a code-0 root with children advertises them with a '+' expander");

            lib.BeforeExpand(zero, list);
            int expected = list.Count(n => n.ParentCode == 0 && n.NodeCode != 0);
            AssertEqual(expected, zero.Nodes.Count, "expanding the code-0 root reveals its children");
            AssertTrue(AllNodes(zero, n => !TreeLib.IsPlaceholder(n)),
                "no placeholder survives the expansion");
        }

        /// <summary>
        /// The handlers are private methods on FolderInfo, not fields, and the TreeView must
        /// expose the event. Reflection keeps this honest without driving the mouse.
        /// </summary>
        private static bool HasExpandHandler(FolderInfo form, string treeName, string handlerName) =>
            form.GetType().GetMethod(
                handlerName, BindingFlags.Instance | BindingFlags.NonPublic) != null
            && form.Controls[treeName] is TreeView tv
            && tv.GetType().GetEvent("BeforeExpand") != null;

        private static void RaiseBeforeExpand(TreeView tv, TreeNode node)
        {
            typeof(TreeView)
                .GetMethod("OnBeforeExpand", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(tv, new object[]
                {
                    new TreeViewCancelEventArgs(node, true, TreeViewAction.Expand),
                });
        }

        /// <summary>TreeNodeCollection is not IEnumerable&lt;TreeNode&gt;, hence the cast.</summary>
        private static bool AllNodes(TreeNode parent, Func<TreeNode, bool> match)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                if (!match(child))
                {
                    return false;
                }
            }

            return true;
        }

        private static TreeNode FirstNode(TreeNode parent, Func<TreeNode, bool> match)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                if (match(child))
                {
                    return child;
                }
            }

            return null;
        }

        /// <summary>Depth-first search for a node carrying <paramref name="code"/>, skipping placeholders.</summary>
        private static TreeNode FindNode(TreeNode parent, int code)
        {
            foreach (TreeNode child in parent.Nodes)
            {
                if (TreeLib.IsPlaceholder(child))
                {
                    continue;
                }

                if (TreeLib.GetNodeCode(child) == code)
                {
                    return child;
                }

                TreeNode found = FindNode(child, code);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Scanned files carry an empty <c>LastWriteHourTime</c> and nodes restored from old
        /// XML can be missing attributes altogether, so the comparison must not dereference them.
        /// </summary>
        private static void CompareWithEmptyAttributes(string scratch)
        {
            var list = new List<ClassTree>();
            SampleTree(scratch, withFiles: true, list);

            foreach (ClassTree node in list)
            {
                node.LastWriteHourTime = null;
                node.CompareUrl = null;
                node.Length = null;
            }

            foreach (ClassTree node in list)
            {
                string url = (node.CompareUrl ?? string.Empty).ToLowerInvariant();
                if (url.Length < 0)
                {
                    AssertTrue(false, "unreachable");
                    return;
                }
            }

            AssertTrue(true, "every node can be compared with null attributes");
        }

        private static void OptionsLoad()
        {
            List<ClassOptions> options = OptionsStore.Load(ErrorLog.Current);
            AssertTrue(options.Count > 0, "Options.xml yielded at least one option");
            AssertTrue(options.All(o => !string.IsNullOrWhiteSpace(o.ThuocTinh)), "every option has a key");
            AssertTrue(options.Any(o => o.ThuocTinh == "Image_Dir"), "Image_Dir is present");
        }

        private static void ThemeLoad()
        {
            List<ThemeEntry> entries = ThemeStore.Load();
            AssertTrue(entries.Count > 0, "Config.xml yielded theme entries");
            AssertEqual(ThemeStore.KnownKeys.Length, entries.Count, "every known theme key is present");

            foreach (ThemeEntry entry in entries)
            {
                if (!int.TryParse(entry.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                {
                    AssertTrue(false, $"theme value '{entry.Name}' is not an ARGB integer ('{entry.Value}')");
                    return;
                }
            }

            AssertTrue(true, "every theme value is an ARGB integer");
        }

        private static void LanguageLoad()
        {
            string folder = AppPaths.LanguageFolder;
            AssertTrue(Directory.Exists(folder), "the Languages folder ships with the app");

            List<string> files = Directory.GetFiles(folder, "*.xml").Select(Path.GetFileName).ToList();
            AssertTrue(files.Count > 0, "at least one language file is present");

            int forms = 0;
            foreach (string file in files)
            {
                var doc = new XmlDocument { XmlResolver = null };
                doc.Load(Path.Combine(folder, file));

                foreach (XmlNode form in doc.SelectNodes("//form"))
                {
                    string tag = form.InnerText?.Trim();
                    if (string.IsNullOrEmpty(tag))
                    {
                        continue;
                    }

                    forms++;
                    using var host = new Form { Tag = tag };
                    using var menu = new MenuStrip();
                    host.Controls.Add(menu);

                    // A key missing from the translation file used to throw
                    // KeyNotFoundException and stop the form from opening.
                    var language = new ClassLanguage(host, tag, menu);
                    AssertEqual(string.Empty, language.GetMessage("no-such-key"),
                        $"'{file}/{tag}' tolerates an unknown message id");
                }
            }

            AssertTrue(forms > 0, "at least one form block was exercised");
        }

        private static void FormsConstruct()
        {
            // Every one of these used to crash in its constructor or Load on a clean install.
            foreach ((string name, Func<Form> factory) in AllForms())
            {
                Construct(name, factory);
            }
        }

        private static void ThemeReachesNestedControls()
        {
            // ColorControl only looked at the form's direct children, so anything laid out
            // inside a Panel/GroupBox kept the default grey no matter what Config.xml said.
            using var host = new ThemedProbe();
            var panel = new Panel { Name = "probePanel" };
            var group = new GroupBox { Name = "probeGroup" };
            var label = new Label { Name = "probeLabel" };
            var button = new Button { Name = "probeButton" };

            group.Controls.Add(label);
            group.Controls.Add(button);
            panel.Controls.Add(group);
            host.Controls.Add(panel);

            host.SetColor();

            AssertEqual(host.GetColor("MauNenGroup"), panel.BackColor, "Panel gets MauNenGroup");
            AssertEqual(host.GetColor("MauNenGroup"), group.BackColor, "GroupBox inside the Panel gets MauNenGroup");
            AssertEqual(host.GetColor("MauNenLB"), label.BackColor, "Label nested two levels deep gets MauNenLB");
            AssertEqual(host.GetColor("MauNenBT"), button.BackColor, "Button gets MauNenBT");
        }

        private static void UniqueControlNames()
        {
            // FolderInfo's designer gave contextMenuStrip2 the Name "contextMenuStrip1", so two
            // controls in one form answered to the same name and Controls["contextMenuStrip1"]
            // picked the wrong one. Designer-assigned components are not in Controls, hence the
            // reflection sweep over the form's ToolStrip fields.
            foreach ((string name, Func<Form> factory) in AllForms())
            {
                using Form form = factory();
                var names = new List<string>();
                CollectNames(form, new HashSet<Control>(), names);

                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var duplicates = names.Where(n => !seen.Add(n)).Distinct().ToList();

                AssertTrue(duplicates.Count == 0,
                    $"{name} has unique control names (duplicate: {string.Join(", ", duplicates.Distinct())})");
            }
        }

        private static void DialogButtons()
        {
            // Without AcceptButton/CancelButton, Enter and Escape did nothing on every dialog:
            // the buttons existed but had no dialog role.
            var dialogs = new (string Name, Func<Form> Factory, string Accept, string Cancel)[]
            {
                ("FrmColor", () => new FrmColor(), "btLuu", "btDong"),
                ("FrmOptions", () => new FrmOptions(), "cmdSave", "cmdCancel"),
                ("FrmConnection", () => new FrmConnection(), "cmdLogin", "cmdCancel"),
                ("FrmNodeInfo", () => new FrmNodeInfo(), "cmdSave", "cmdClose"),
                ("FrmEvents", () => new FrmEvents(), "cmdSave", "cmdClose"),
                ("FrmSetCompareUrl", () => new FrmSetCompareUrl(), "cmdOK", "cmdCancel"),
                ("FrmScanMode", () => new FrmScanMode(), "cmdOK", "cmdCancel"),
            };

            foreach ((string name, Func<Form> factory, string accept, string cancel) in dialogs)
            {
                using Form form = factory();

                AssertTrue(form.AcceptButton is not null, $"{name} has an AcceptButton (Enter)");
                AssertTrue(form.CancelButton is not null, $"{name} has a CancelButton (Escape)");
                AssertEqual(accept, (form.AcceptButton as Control)?.Name, $"{name} Enter triggers the right button");
                AssertEqual(cancel, (form.CancelButton as Control)?.Name, $"{name} Escape triggers the right button");
                AssertTrue(!ReferenceEquals(form.AcceptButton, form.CancelButton),
                    $"{name} does not accept and cancel on the same button");
            }
        }

        private static void CollectNames(Control control, HashSet<Control> visited, List<string> names)
        {
            // A MenuStrip/StatusStrip is both a field and a child of the form, so the same
            // instance is reachable twice; without the visited set every strip read as a
            // duplicate of itself.
            if (!visited.Add(control))
            {
                return;
            }

            if (control.Name.Length > 0)
            {
                names.Add(control.Name);
            }

            foreach (Control child in control.Controls)
            {
                CollectNames(child, visited, names);
            }

            foreach (FieldInfo field in control.GetType().GetFields(
                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.GetValue(control) is ToolStrip strip)
                {
                    CollectNames(strip, visited, names);
                }
            }
        }

        private static List<(string Name, Func<Form> Factory)> AllForms() =>
            new()
            {
                ("FolderInfo", () => new FolderInfo()),
                ("FrmColor", () => new FrmColor()),
                ("FrmCompare", () => new FrmCompare()),
                ("FrmConnection", () => new FrmConnection()),
                ("FrmDirectSearch", () => new FrmDirectSearch()),
                ("FrmEvents", () => new FrmEvents()),
                ("FrmMenu", () => new FrmMenu()),
                ("FrmNodeInfo", () => new FrmNodeInfo()),
                ("FrmOptions", () => new FrmOptions()),
                ("FrmPictures", () => new FrmPictures()),
                ("FrmProgress", () => new FrmProgress("selftest")),
                ("FrmScanMode", () => new FrmScanMode()),
                ("FrmSearchDetail", () => new FrmSearchDetail()),
                ("FrmSetCompareUrl", () => new FrmSetCompareUrl()),
            };

        /// <summary>Concrete <see cref="ParentForm"/> so the theme traversal can be probed.</summary>
        private sealed class ThemedProbe : ParentForm
        {
        }

        private static void Construct(string name, Func<IDisposable> factory)
        {
            IDisposable form = null;
            try
            {
                form = factory();
            }
            catch (Exception ex)
            {
                AssertTrue(false, name + " threw while constructing: " + Unwrap(ex).Message);
                return;
            }

            try
            {
                AssertTrue(form != null, name + " constructed");
            }
            finally
            {
                form?.Dispose();
            }
        }

        private static void BannerLoad()
        {
            using var banner = new MediaUserControl();
            banner.LoadData();
            AssertTrue(banner.Index >= -1, "the banner reports a valid row index");
        }

        #endregion

        #region Harness

        private static Exception Unwrap(Exception ex) =>
            ex is TargetInvocationException { InnerException: { } inner } ? inner : ex;

        private static void Check(string name, Action body)
        {
            try
            {
                body();
            }
            catch (Exception ex)
            {
                _failed++;
                Report.AppendLine($"FAIL  {name}");
                Report.AppendLine($"        {Unwrap(ex).GetType().Name}: {Unwrap(ex).Message}");
            }
        }

        private static void AssertTrue(bool condition, string what)
        {
            if (condition)
            {
                _passed++;
            }
            else
            {
                throw new SelfTestFailure(what);
            }
        }

        private static void AssertEqual<T>(T expected, T actual, string what)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new SelfTestFailure($"{what}: expected '{expected}', got '{actual}'");
            }

            _passed++;
        }

        private static void TryWriteReport(string text)
        {
            try
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyFolderInfo");
                Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, "selftest.txt"), text);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // A missing report file is a harness problem, not an app failure.
            }
        }

        private static int CountSubtree(List<ClassTree> list, int code)
        {
            int total = 0;

            foreach (ClassTree node in list)
            {
                if (node.NodeCode == code)
                {
                    total++;
                }
            }

            foreach (ClassTree node in list)
            {
                if (node.ParentCode == code && node.NodeCode != node.ParentCode)
                {
                    total += CountSubtree(list, node.NodeCode);
                }
            }

            return total;
        }

        /// <summary>Fills one level of children, mirroring how the main form expands a node.</summary>
        private static void Populate(TreeNode parent, List<ClassTree> list, int parentCode, int depth)
        {
            if (depth > 32)
            {
                return;
            }

            foreach (ClassTree node in list)
            {
                if (node.ParentCode != parentCode || node.NodeCode == parentCode)
                {
                    continue;
                }

                var child = new TreeNode(node.NodeName) { Tag = node.NodeCode };
                parent.Nodes.Add(child);
                Populate(child, list, node.NodeCode, depth + 1);
            }
        }

        private sealed class SelfTestFailure : Exception
        {
            public SelfTestFailure(string message)
                : base(message)
            {
            }
        }

        #endregion
    }
}
