using Sitecore.Diagnostics;

using Arke.Sitecore.ComputedSearch.Extensions;

namespace Arke.Sitecore.ComputedSearch.Pipelines.ComputePresentation
{
    public class EnsureSitePresentation : IComputePresentationProcessor
    {
        public void Process(ComputedSearchPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.CurrentItem.HasPresentationDetails() &&
                args.CurrentItem.Paths.LongID.Contains(args.CrawlerFieldMap.ScopeItemId))
            {
                return;
            }

            if (!args.CrawlerFieldMap.AllowNoPresentation && !args.CurrentItem.Paths.LongID.Contains(args.CrawlerFieldMap.ScopeItemId))
            {
                args.AbortPipeline();
            }
        }
    }
}
