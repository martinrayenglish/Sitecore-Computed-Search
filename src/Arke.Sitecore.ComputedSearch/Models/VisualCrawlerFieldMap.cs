using System.Collections.Generic;

namespace Arke.Sitecore.ComputedSearch.Models
{
    public class VisualCrawlerFieldMap
    {
        public string ScopeItemId { get; set; }
        public bool AllowNoPresentation { get; set; }
        public string TemplateFieldNames { get; set; }
        public List<VisualCrawlerFieldMapping> CrawlerFieldMappings { get; set; }
        public bool DebugMode { get; set; }
    }

    public class VisualCrawlerFieldMapping
    {
        public string TemplateId { get; set; }
        public string TemplateFieldNames { get; set; }
    }
}
