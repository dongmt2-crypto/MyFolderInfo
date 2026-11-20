using System;
using System.Collections.Generic;
using System.IO;

namespace MyFolderInfo.Model
{
    internal sealed class DirectoryInfoComparer : IComparer<DirectoryInfo>
    {
        public int Compare(DirectoryInfo first, DirectoryInfo second)
        {
            return string.Compare(first?.Name, second?.Name, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    internal sealed class FileInfoComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo first, FileInfo second)
        {
            if (first == null || second == null)
            {
                return 0;
            }

            if (ClassData.SortNo != "1")
            {
                return string.Compare(first.Name, second.Name, StringComparison.CurrentCultureIgnoreCase);
            }

            int byExtension = string.Compare(first.Extension, second.Extension, StringComparison.CurrentCultureIgnoreCase);
            return byExtension != 0
                ? byExtension
                : string.Compare(first.Name, second.Name, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Fixed extension-then-name order for the picture browser, which is unaffected by the
    /// "Sort_No" option because a photo set is expected to group by type.
    /// </summary>
    internal sealed class FileInfoComparer2 : IComparer<FileInfo>
    {
        public int Compare(FileInfo first, FileInfo second)
        {
            if (first == null || second == null)
            {
                return 0;
            }

            int byExtension = string.Compare(first.Extension, second.Extension, StringComparison.CurrentCultureIgnoreCase);
            return byExtension != 0
                ? byExtension
                : string.Compare(first.Name, second.Name, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    /// <summary>
    /// Orders tree nodes. The old version sorted by extension in *both* branches, so the
    /// "sort by name / by extension" option in FrmOptions had no effect on the tree at all,
    /// and it dereferenced <c>Extenstion</c> without a null check.
    /// </summary>
    internal sealed class ClassTreeComparer : IComparer<ClassTree>
    {
        public int Compare(ClassTree first, ClassTree second)
        {
            if (first == null || second == null)
            {
                return 0;
            }

            int byName = string.Compare(first.NodeName, second.NodeName, StringComparison.CurrentCultureIgnoreCase);

            if (ClassData.SortNo != "1")
            {
                return byName;
            }

            int byExtension = string.Compare(first.Extenstion, second.Extenstion, StringComparison.CurrentCultureIgnoreCase);
            return byExtension != 0 ? byExtension : byName;
        }
    }

    internal sealed class ClassTreeComparer2 : IComparer<ClassTree>
    {
        public int Compare(ClassTree first, ClassTree second)
        {
            return string.Compare(first?.OriginalUrl, second?.OriginalUrl, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
