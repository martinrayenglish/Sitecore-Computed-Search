using System;
using System.Linq;
using System.Text;

using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace Arke.Sitecore.ComputedSearch.Pipelines.ComputePresentation
{
    public class GetItemContent : IComputePresentationProcessor
    {
        public void Process(ComputedSearchPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.CurrentItem, "args.CurrentItem");

            args.ComputedContent = ItemContent(args);
        }

        /// <summary>
        /// Crawls fields on an item and includes their content in the index
        /// </summary>
        public static string ItemContent(ComputedSearchPipelineArgs args)
        {
            var result = new StringBuilder();
            args.CurrentItem.Fields.ReadAll();

            foreach (var field in args.CurrentItem.Fields.Where(ShouldIndexField))
            {   
                if (args.CrawlerFieldMap.TemplateFieldNames.ToLowerInvariant()
                                        .Split('|')
                                        .Contains(field.Name.ToLowerInvariant()))
                {
                    if (args.CrawlerFieldMap.DebugMode)
                    {
                        Log.Info(
                            string.Format("[Computed Search] '{0}' field name match found for item '{1}' of template type '{2}'", field.Name,
                                args.CurrentItem.Name, args.CurrentItem.TemplateName), typeof(GetItemContent));
                    }

                    result.AppendLine(field.Value);
                }
            }

            return result.ToString();
        }

        private static bool ShouldIndexField(Field field)
        {
            //process non-empty text fields that are not part of the standard template
            //return !field.Name.StartsWith("__") && IsTextField(field) && !String.IsNullOrEmpty(field.Value);
            return !field.Name.StartsWith("__") && !String.IsNullOrEmpty(field.Value);
        }

    }
}
