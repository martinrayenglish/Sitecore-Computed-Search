using Sitecore.Diagnostics;

namespace Arke.Sitecore.ComputedSearch.Pipelines.ComputePresentation
{
    public class SanitizeSearchContent : IComputePresentationProcessor
    {
        public void Process(ComputedSearchPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.CurrentItem, "args.CurrentItem");

            args.ComputedContent = StripTagsCharArray(args.ComputedContent);

        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        private static string StripTagsCharArray(string source)
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
