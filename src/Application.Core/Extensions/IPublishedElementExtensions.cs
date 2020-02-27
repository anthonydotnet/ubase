using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Application.Core.Extensions
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
                var value = element.Value<T>(propertyAlias);

                return value;
            }

            return default;
        }


        public static object Value(this IEnumerable<IPublishedElement> items, string doctypeAlias, string propertyAlias)
        {
            var element = items.FirstOrDefault(x => x.ContentType.Alias == doctypeAlias);

            if (element != null)
            {
                var value = element.Value(propertyAlias);

                return value;
            }

            return default;
        }
    }
}
