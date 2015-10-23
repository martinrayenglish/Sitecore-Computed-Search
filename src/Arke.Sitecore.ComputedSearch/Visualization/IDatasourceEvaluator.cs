using System.Collections.Generic;

using Sitecore.Layouts;

namespace Arke.Sitecore.ComputedSearch.Visualization
{
    public interface IDatasourceEvaluator
    {
        IEnumerable<global::Sitecore.Data.Items.Item> Evaluate(RenderingReference renderingReference);
    }
}
