namespace VTQT.Elastic.Helpers
{
    public class ElasticHelper
    {
        public static readonly int MaxPageSize = 200;

        public static class ConnectionNames
        {
            public static readonly string Default = "Default";
        }

        public static class BoostValues
        {
            public static readonly double Important = 10;

            public static readonly double Code = 5;
        }
    }
}
