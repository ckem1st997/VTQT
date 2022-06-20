using Nest;

namespace VTQT.Elastic.Helpers
{
    public static class ElasticAnalyzerHelper
    {
        public static class Custom
        {
            #region Standard

            public static readonly string StandardAnalyzerName = "custom_standard_analyzer";

            public static CustomAnalyzerDescriptor StandardAnalyzer = new CustomAnalyzerDescriptor()
                .CharFilters(new[] { "html_strip" })
                .Tokenizer("standard")
                .Filters(new[] { "lowercase", "asciifolding" });

            #endregion
        }
    }
}
