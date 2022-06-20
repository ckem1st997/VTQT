namespace VTQT.Services.Helpers
{
    public static class TreeViewHelper
    {
        public static string GetTreeLevelString(int level)
        {
            if (level <= 0)
                return "";

            var result = "";
            for (var i = 1; i <= level; i++)
            {
                result += "– ";
            }

            return result;
        }
    }
}
