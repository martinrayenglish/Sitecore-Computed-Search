using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

using Arke.Sitecore.ComputedSearch.Visualization;

namespace Arke.Sitecore.ComputedSearch.Pipelines.ComputePresentation
{
    public class GetVisualizationContent : IComputePresentationProcessor
    {
        public void Process(ComputedSearchPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.CurrentItem, "args.CurrentItem");

            args.ComputedContent += VisualizationContent(args);
        }

        /// <summary>
        /// Crawls the renderings on an item and includes their content in the index
        /// </summary>
        public static string VisualizationContent(ComputedSearchPipelineArgs args)
        {
            IEnumerable<Item> dataSources = args.CurrentItem.GetVisualizationDatasources();

            var result = new StringBuilder();
            foreach (var dataSource in dataSources)
            {
                //Check datasource item
                if (args.CrawlerFieldMap.CrawlerFieldMappings.Any(t => t.TemplateId.Equals(dataSource.TemplateID.ToString())))
                {
                    if (args.CrawlerFieldMap.DebugMode)
                    {
                        global::Sitecore.Diagnostics.Log.Info(
                                string.Format("[Computed Search] {0} dataSources found for item '{1}' that match template '{2}'", dataSources.Count(),
                                    args.CurrentItem.Name, dataSource.TemplateID), typeof(GetVisualizationContent));
                    }

                    dataSource.Fields.ReadAll();
                    foreach (var field in dataSource.Fields.Where(ShouldIndexField))
                    {
                        if (
                            args.CrawlerFieldMap.CrawlerFieldMappings.Any(
                                t =>
                                    t.TemplateFieldNames.ToLowerInvariant()
                                        .Split('|')
                                        .Contains(field.Name.ToLowerInvariant())))
                        {
                            result.AppendLine(field.Value);
                        }
                    }
                }

                //Special case for when you have a datasource item that is referencing other items
                dataSource.Fields.ReadAll();
                foreach (var field in dataSource.Fields.Where(ShouldIndexField))
                {
                    //Possibly to a check for certain field types here
                    var fieldItemValues = field.Value.Split('|');

                    foreach (var targetItemId in fieldItemValues)
                    {
                        var targetItem = Factory.GetDatabase("master").GetItem(targetItemId);

                        if (targetItem == null)
                        {
                            continue;
                        }

                        if (!args.CrawlerFieldMap.CrawlerFieldMappings.Any(
                            t => t.TemplateId.Equals(targetItem.TemplateID.ToString())))
                        {
                            continue;
                        }

                        targetItem.Fields.ReadAll();

                        foreach (var targetField in targetItem.Fields.Where(ShouldIndexField))
                        {
                            if (!args.CrawlerFieldMap.CrawlerFieldMappings.Any(
                                t =>
                                    t.TemplateFieldNames.ToLowerInvariant()
                                        .Split('|')
                                        .Contains(targetField.Name.ToLowerInvariant())))
                            {
                                continue;
                            }

                            if (args.CrawlerFieldMap.DebugMode)
                            {
                                global::Sitecore.Diagnostics.Log.Info(
                                    string.Format("[Computed Search] '{0}' field name match found items referenced by datasource of item '{1}' of template type '{2}'", targetField.Name,
                                        args.CurrentItem.Name, targetItem.TemplateName), typeof(GetVisualizationContent));
                            }

                            result.AppendLine(targetField.Value);
                        }
                    }
                }

                //Check datasource item descendents
                foreach (var childItem in dataSource.Axes.GetDescendants())
                {
                    if (args.CrawlerFieldMap.CrawlerFieldMappings.Any(t => t.TemplateId.Equals(childItem.TemplateID.ToString())))
                    {
                        if (args.CrawlerFieldMap.DebugMode)
                        {
                            global::Sitecore.Diagnostics.Log.Info(
                                string.Format("[Computed Search] {0} dataSources found for the descendents of item '{1}' that match template '{2}'", dataSources.Count(),
                                    args.CurrentItem.Name, dataSource.TemplateName), typeof(GetVisualizationContent));
                        }

                        childItem.Fields.ReadAll();
                        foreach (var field in childItem.Fields.Where(ShouldIndexField))
                        {
                            if (
                                args.CrawlerFieldMap.CrawlerFieldMappings.Any(
                                    t =>
                                        t.TemplateFieldNames.ToLowerInvariant()
                                            .Split('|')
                                            .Contains(field.Name.ToLowerInvariant())))
                            {
                                if (args.CrawlerFieldMap.DebugMode)
                                {
                                    global::Sitecore.Diagnostics.Log.Info(
                                        string.Format("[Computed Search] '{0}' field name match found for the descendents of item '{1}' of template type '{2}'", field.Name,
                                            args.CurrentItem.Name, dataSource.TemplateName), typeof(GetVisualizationContent));
                                }

                                result.AppendLine(field.Value);
                            }
                        }
                    }
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
