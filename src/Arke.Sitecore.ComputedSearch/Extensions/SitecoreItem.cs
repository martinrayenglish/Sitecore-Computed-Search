using Sitecore.Data.Items;

namespace Arke.Sitecore.ComputedSearch.Extensions
{
    public static class SitecoreItem
    {
        public static bool HasPresentationDetails(this Item item)
        {
            if (item == null) return false;

            return item.Fields[global::Sitecore.FieldIDs.LayoutField] != null
                   && !string.IsNullOrEmpty(item.Fields[global::Sitecore.FieldIDs.LayoutField].Value);
        }
    }
}