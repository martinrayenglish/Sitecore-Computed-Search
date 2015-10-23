using System.Collections.Generic;
using System.Linq;

using Sitecore.Data.Items;
using Sitecore.Layouts;

namespace Arke.Sitecore.ComputedSearch.Visualization
{
    public static class VisualizationHelper
    {
        public static IEnumerable<Item> GetVisualizationDatasources(this Item item)
        {
            var list = new List<Item>();
            foreach (DeviceItem device in item.Database.Resources.Devices.GetAll())
            {
                foreach (RenderingReference renderingReference in item.Visualization.GetRenderings(device, false))
                {
                    if (renderingReference.RenderingItem == null ||
                        string.IsNullOrEmpty(renderingReference.Settings.DataSource))
                    {
                        continue;
                    }

                    var source = item.Database.GetItem(renderingReference.Settings.DataSource);
                    if (source == null)
                    {
                        continue;
                    }

                    // Add this datasource to the list
                    if (!list.Any(i => i.ID.Equals(source.ID)))
                    {
                        list.Add(source);
                    }

                    // See if the rendering parameters specify a class to evaluate data sources
                    var renderingParams = global::Sitecore.Web.WebUtil.ParseQueryString(renderingReference.RenderingItem.Parameters);
                    if (renderingParams["DatasourceEvaluator"] != null)
                    {
                        IDatasourceEvaluator evaluatorClass = global::Sitecore.Reflection.ReflectionUtil.CreateObject(renderingParams["DatasourceEvaluator"]) as IDatasourceEvaluator;
                        if (evaluatorClass != null)
                        {
                            var items = evaluatorClass.Evaluate(renderingReference);
                            if (items != null)
                            {
                                list.AddRange(items.Where(depItem => !list.Any(i => i.ID.Equals(depItem.ID))));
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}