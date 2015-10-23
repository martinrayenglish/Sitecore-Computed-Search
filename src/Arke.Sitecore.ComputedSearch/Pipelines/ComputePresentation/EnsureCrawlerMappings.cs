using Sitecore.Diagnostics;
using Sitecore.StringExtensions;

namespace Arke.Sitecore.ComputedSearch.Pipelines.ComputePresentation
{
    public class EnsureCrawlerMappings : IComputePresentationProcessor
    {
        public void Process(ComputedSearchPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.CrawlerFieldMap.ScopeItemId.IsNullOrEmpty())
            {   
                args.AbortPipeline();
            }
        }
    }
}
