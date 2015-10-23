using Sitecore.Data.Items;
using Sitecore.Pipelines;

using Arke.Sitecore.ComputedSearch.Models;

namespace Arke.Sitecore.ComputedSearch.Pipelines.ComputePresentation
{
    public class ComputedSearchPipelineArgs : PipelineArgs
    {   
        public Item CurrentItem { get; set; }
        public string ComputedContent { get; set; }
        public VisualCrawlerFieldMap CrawlerFieldMap { get; set; }
    }
}
