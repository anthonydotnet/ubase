using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using System.Text;
using Umbraco.Cms.Core.Routing;
using Our.Umbraco.DomainFinder;
using Our.Umbraco.SEOParts.Models;
using Our.Umbraco.SEOParts.Extensions;
using Microsoft.Extensions.Configuration;

namespace Our.Umbraco.SEOParts
{
    public interface ISitemapXmlGenerator
    {
        string GetSitemap(Domain domain, string scheme);
    }

    public class SitemapXmlGenerator : ISitemapXmlGenerator
    {
        IDomainFinder _domainFinder;
        private readonly IConfiguration _config;
        private IEnumerable<string> _excludeAliases;

        public SitemapXmlGenerator(IDomainFinder domainFinder, IConfiguration config)
        {
            _domainFinder = domainFinder;
            _config = config;

            var temp = _config.GetValue<string>("SEO:ExcludeAliases");
            _excludeAliases = temp != null ? temp.Split(",") : new List<string>();
        }

        public string GetSitemap(Domain domain, string scheme)
        {
            var sitemapItems = new List<SitemapXmlItem>();
            
            var siteRoot = _domainFinder.GetContentByDomain(domain);
            //var home = _cmsService.GetHome(domain.ContentId);

            var baseUrl = domain.Name.Contains(scheme) ? domain.Name : $"{scheme}{domain.Name}";
            sitemapItems = ProcessSitemapItems(baseUrl, sitemapItems, new List<IPublishedContent>() { siteRoot });
            sitemapItems = ProcessSitemapItems(baseUrl, sitemapItems, siteRoot.Children);

            // sitemapItems.First().Url = baseUrl; // ensure first node is the base url

            var sb = new StringBuilder($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");
            foreach (var item in sitemapItems)
            {
                sb.AppendLine("<url>");
                sb.AppendLine($"<loc>{item.Url}</loc>");
                sb.AppendLine($"<lastmod>{item.LastModified}</lastmod>");
                sb.AppendLine($"<changefreq>{item.ChangeFreq}</changefreq>");
                sb.AppendLine($"<priority>{item.Priority}</priority>");
                sb.AppendLine("</url>");
            }
            sb.AppendLine("</urlset>");

            return sb.ToString();
        }

        private List<SitemapXmlItem> ProcessSitemapItems(string baseUrl, List<SitemapXmlItem> sitemapItems, IEnumerable<IPublishedContent> nodes)
        {
            var filtered = nodes.Where(x => !_excludeAliases.Contains(x.ContentType.Alias));

            foreach (var node in filtered)
            {
                if (CanProcessNode(node))
                {
                    sitemapItems.Add(CreateSitemapItem(baseUrl, node));
                    if (node.Children.Any())
                    {
                        sitemapItems = ProcessSitemapItems(baseUrl, sitemapItems, node.Children);
                    }
                }
            }

            return sitemapItems;
        }

        private SitemapXmlItem CreateSitemapItem(string baseUrl, IPublishedContent node, string defaultPath = null)
        {
            var item = new SitemapXmlItem
            {
                ChangeFreq = GetValueOrDefault(node, "seoFrequency", "monthly"),
                Priority = GetValueOrDefault(node, "seoPriority", "0.5"),
                Url = $"{baseUrl}/{defaultPath ?? node.UrlSegment}",
                LastModified = string.Format("{0:s}+00:00", node.UpdateDate),
            };

            return item;
        }


        private string GetValueOrDefault(IPublishedContent node, string alias, string defaultValue)
        {
            if (!node.HasProperty("pageSettings") || !node.GetProperty("pageSettings").HasValue())
            {
                return defaultValue;
            }

            var settings = node.GetProperty("pageSettings").GetValue() as IEnumerable<IPublishedElement>;

            string value;
            if (settings != null)
            {
                var element = settings.FirstOrDefault(x => x.ContentType.Alias == "elementSeoSettings");

                if (element == null)
                {
                    return defaultValue;
                }

                value = element.GetProperty(alias).GetValue() as string;
                if (string.IsNullOrEmpty(value))
                {
                    value = defaultValue;
                }
            }
            else
            {
                value = defaultValue;
            }

            return value;
        }

        private bool CanProcessNode(IPublishedContent page)
        {
            if (page.HasProperty("pageSettings") && page.GetProperty("pageSettings").HasValue())
            {
                var pageSettings = page.GetProperty("pageSettings").GetValue() as IEnumerable<IPublishedElement>;

                var hide = pageSettings.GetElementValue<bool>("elementSeoSettings", "hideFromSitemapXml");

                if (!hide)
                {
                    return true;
                }
            }
            else if (page.TemplateId != 0)
            {
                return true;
            }

            return false;
        }
    }
}