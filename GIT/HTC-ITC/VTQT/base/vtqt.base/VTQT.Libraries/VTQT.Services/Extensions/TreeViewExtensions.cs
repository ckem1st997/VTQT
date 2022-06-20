using VTQT.Services.Helpers;

namespace VTQT
{
    public static class TreeViewExtensions
    {
        public static string ToTreeLevelString(this int level)
        {
            return TreeViewHelper.GetTreeLevelString(level);
        }
    }
}
