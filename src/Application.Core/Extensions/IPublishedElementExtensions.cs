using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models.PublishedContent;

namespace Application.Core.Extensions
{
    public static class IPublishedElementExtensions
    {
        public static IPublishedElement GetElement(this IEnumerable<IPublishedElement> items, string alias)
        {
            var element = items.FirstOrDefault(x => x.ContentType.Alias == alias);

            return element;
        }

    }
}
