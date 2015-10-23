using System;

using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Arke.Sitecore.ComputedSearch.Utilities
{
    public static class ComputedHelper
    {
        public static Database MasterDatabase
        {
            get
            {
                return global::Sitecore.Configuration.Factory.GetDatabase("master");
            }
        }

        public static Item QuitEarlyIfPossible(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");

            SitecoreIndexableItem scIndexable = indexable as SitecoreIndexableItem;

            if (scIndexable == null)
            {
                return null;
            }

            Item item = scIndexable;

            // optimization to reduce indexing time by skipping this logic for items in the Core database
            if ((string.Compare(item.Database.Name, "core", StringComparison.OrdinalIgnoreCase) == 0) ||
                (item.Paths.IsMediaItem) ||
                (!item.Paths.IsContentItem))
                return null;

            return item;
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex).ToLowerInvariant();
        }
    }
}
