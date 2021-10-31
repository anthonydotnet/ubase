using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Our.Umbraco.SEOParts.Extensions
{
    public static class IPublishedElementExtensions
    {
        public static IPublishedElement GetElement(this IEnumerable<IPublishedElement> items, string doctypeAlias)
        {
            var element = items.FirstOrDefault(x => x.ContentType.Alias == doctypeAlias);

            return element;
        }

        public static T GetElementValue<T>(this IEnumerable<IPublishedElement> items, string doctypeAlias, string propertyAlias)
        {
            var element = items.FirstOrDefault(x => x.ContentType.Alias == doctypeAlias);

            if(element != null)
            {
                var value = element.GetProperty(propertyAlias).Value(default);

                return (T)value;
            }

            return default;
        }


        public static object Value(this IEnumerable<IPublishedElement> items, string doctypeAlias, string propertyAlias)
        {
            var element = items.FirstOrDefault(x => x.ContentType.Alias == doctypeAlias);

            if (element != null)
            {
                var value = element.GetProperty(propertyAlias).Value(default);

                return value;
            }

            return default;
        }
    }
}
