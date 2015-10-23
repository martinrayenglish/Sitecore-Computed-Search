using System.Collections.Generic;
using System.Xml;

using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Xml;
using Sitecore.Data.Items;

using Arke.Sitecore.ComputedSearch.Models;
using Arke.Sitecore.ComputedSearch.Pipelines.ComputePresentation;
using Arke.Sitecore.ComputedSearch.Utilities;

namespace Arke.Sitecore.ComputedSearch.ComputedFields.ContentSearch
{
    public class ComputedSearchContent : AbstractComputedIndexField
    {   
        private VisualCrawlerFieldMap CrawlerFieldMap { get; set; }

        public ComputedSearchContent(XmlNode configNode)
        {
            Assert.ArgumentNotNull(configNode, "configNode");

            CrawlerFieldMap = new VisualCrawlerFieldMap
            {
                CrawlerFieldMappings = new List<VisualCrawlerFieldMapping>(),
                ScopeItemId = XmlUtil.GetAttribute("scopeItemId", configNode),
                AllowNoPresentation = XmlUtil.GetAttribute("allowNoPresentation", configNode) != null &&
                                      XmlUtil.GetAttribute("allowNoPresentation", configNode)
                                          .ToLowerInvariant()
                                          .Equals("true"),
                DebugMode = global::Sitecore.Configuration.Settings.GetSetting("Arke.Sitecore.ComputedSearch.DebugMode")
                    .ToLowerInvariant()
                    .Equals("true"),
                TemplateFieldNames = XmlUtil.GetAttribute("templateFieldNames", configNode)
            };

            if (configNode.FirstChild != null)
            {
                foreach (XmlNode childNode in configNode.FirstChild.ChildNodes)
                {
                    var crawlerFieldMap = new VisualCrawlerFieldMapping
                    {
                        TemplateId = XmlUtil.GetAttribute("templateId", childNode),
                        TemplateFieldNames = XmlUtil.GetAttribute("templateFieldNames", childNode)
                    };

                    CrawlerFieldMap.CrawlerFieldMappings.Add(crawlerFieldMap);
                }
            }
        }

        public override object ComputeFieldValue(IIndexable indexable)
        {
            Item item = ComputedHelper.QuitEarlyIfPossible(indexable);

            if (item != null)
            {
                var pipelineArgs = new ComputedSearchPipelineArgs
                {
                    CurrentItem = item,
                    CrawlerFieldMap = CrawlerFieldMap
                };

                CorePipeline.Run("Arke.Sitecore.ComputedSearch.ComputePresentation", pipelineArgs);
                return pipelineArgs.ComputedContent;
            }

            return string.Empty;
        }
    }
}
