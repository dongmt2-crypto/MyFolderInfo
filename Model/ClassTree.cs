using System;
using System.Linq;

namespace MyFolderInfo.Model
{
    public class ClassTree
    {
        public string NodeName { get; set; }
        public int NodeCode { get; set; }
        public int ParentCode { get; set; }
        public string Extenstion { get; set; }
        public string OriginalUrl { get; set; }
        public string Length { get; set; }
        public string CreationTime { get; set; }
        public string LastWriteTime { get; set; }
        public string LastAccessTime { get; set; }
        public string LastWriteHourTime { get; set; }
        public string CompareUrl { get; set; }
        public string Extend1 { get; set; }
        public string Extend2 { get; set; }
        public string Extend3 { get; set; }
        public string Extend4 { get; set; }

        public ClassTree()
        { }

        /// <summary>
        /// Copy of this node re-keyed under a new parent. Used when dragging a subtree from
        /// the left tree to the right one; the original code hand-copied all fifteen fields
        /// at the call site, which is how <c>CompareUrl</c> and the Extend slots drifted apart.
        /// </summary>
        public ClassTree Clone(int nodeCode, int parentCode)
        {
            return new ClassTree
            {
                NodeCode = nodeCode,
                ParentCode = parentCode,
                NodeName = NodeName,
                Extenstion = Extenstion,
                OriginalUrl = OriginalUrl,
                Length = Length,
                CreationTime = CreationTime,
                LastWriteTime = LastWriteTime,
                LastAccessTime = LastAccessTime,
                LastWriteHourTime = LastWriteHourTime,
                CompareUrl = CompareUrl,
                Extend1 = Extend1,
                Extend2 = Extend2,
                Extend3 = Extend3,
                Extend4 = Extend4,
            };
        }

        /// <summary>True when this node represents a folder rather than a file.</summary>
        public bool IsFolder => string.IsNullOrEmpty(Extenstion);

        public ClassTree(string nodename, int nodecode, int parentcode,
            string extenstion, string originalUrl, string fLength, string fCreationTime
            , string fLastWriteTime, string fLastAccessTime, string fLastWriteHourTime, string fcompareUrl,
            string str12, string str13, string str14, string str15
            )
        {
            NodeName = nodename;
            NodeCode = nodecode;
            ParentCode = parentcode;
            Extenstion = extenstion;
            OriginalUrl = originalUrl;
            Length = fLength;
            CreationTime = fCreationTime;
            LastWriteTime = fLastWriteTime;
            LastAccessTime = fLastAccessTime;
            LastWriteHourTime = fLastWriteHourTime;
            CompareUrl = fcompareUrl;
            Extend1 = str12;
            Extend2 = str13;
            Extend3 = str14;
            Extend4 = str15;
        }
    }
}
