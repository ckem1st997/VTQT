using Nest;
using VTQT.Elastic.Documents.Common;
using VTQT.Elastic.Helpers;

namespace VTQT.Elastic.Mapping
{
    public static class CommonMapping
    {
        public static void MapCommon(this ElasticClient client)
        {
            if (!client.Indices.Exists(ElasticIndexHelper.GetIndexName<LogDoc>()).Exists)
            {
                var resLog = client.Indices.Create(ElasticIndexHelper.GetIndexName<LogDoc>(), c => c
                    .Settings(settings => settings
                        .Setting("index.mapping.total_fields.limit", ElasticIndexHelper.LimitTotalFields)
                        .Analysis(analysis => analysis
                            .Analyzers(analyzers => analyzers
                                .Custom(ElasticAnalyzerHelper.Custom.StandardAnalyzerName, custom =>
                                    ElasticAnalyzerHelper.Custom.StandardAnalyzer)
                            )
                        )
                    )
                    .Map<LogDoc>(m => m
                        .AutoMap()
                        .Properties(p => p
                            .Text(t => t
                                .Name(x => x.AppType)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.LogLevelText)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.ShortMessage)
                                .Analyzer(ElasticAnalyzerHelper.Custom.StandardAnalyzerName)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.FullMessage)
                                .Analyzer(ElasticAnalyzerHelper.Custom.StandardAnalyzerName)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.Action)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.UserId)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.UserName)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.PageUrl)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.ReferrerUrl)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.HttpMethod)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.MachineName)
                                .Analyzer(ElasticAnalyzerHelper.Custom.StandardAnalyzerName)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(x => x.IpAddress)
                                .Fields(fs => fs
                                    .Keyword(k => k
                                        .Name(ElasticFieldHelper.KeywordName)
                                        .IgnoreAbove(256)
                                    )
                                )
                            )

                        //.Object<object>(o => o
                        //    .Name(x => x.Data)
                        //    .AutoMap()
                        //)
                        )
                    )
                );
            }
        }
    }
}
