using System.Collections.Generic;

namespace VTQT.Services.Asset
{
    public class TreePath
    {
        public string Data { get; set; }

        public string Path { get; set; }

        public bool IsRoot { get; set; }

        public List<TreePath> Children { get; set; }

        public TreePath Parents { get; set; }

        public TreePath()
        {
            Children = new List<TreePath>();
            IsRoot = false;
            Parents = null;
        }
    }
}
