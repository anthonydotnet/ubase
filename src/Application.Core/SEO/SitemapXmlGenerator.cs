using Application.Models.CmsModels;
using Application.Models.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Application.Core.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using System.Text;
using Umbraco.Cms.Core.Routing;

namespace Application.Core.Services
{
    public interface ISitemapXmlGenerator
    {
        string GetSitemap(Domain domain, string scheme);
    }

    public class SitemapXmlGenerator : ISitemapXmlGenerator
    {
        private readonly ICmsService _cmsService;

        public SitemapXmlGenerator(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        public string GetSitemap(Domain domain, string scheme)
        {
            var sitemapItems = new List<SitemapXmlItem>();

            var siteRoot = _cmsService.GetSiteRoot(domain.ContentId);
            var home = _cmsService.GetHome(domain.ContentId);

            var baseUrl = domain.Name.Contains(scheme) ? domain.Name : $"{scheme}{domain.Name}";
           // sitemapItems = ProcessSitemapItems(baseUrl, sitemapItems, new List<IPublishedContent>() { siteRoot });
            sitemapItems = ProcessSitemapItems(baseUrl, sitemapItems, siteRoot.Children.Where(x => x.Id != home.Id));

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
            foreach (var node in nodes)
            {
                if (CanProcessNode(node))
                {
                    sitemapItems.Add(CreateSitemapItem( baseUrl, node));
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
            var settings = node.GetProperty("pageSettings").GetValue() as IEnumerable <IPublishedElement>;

            string value;
            if (settings != null)
            {
                var element = settings.FirstOrDefault(x => x.ContentType.Alias == ElementSeoSettings.ModelTypeAlias);

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
            var item = page as IPageSettingsMixin;

            if (page.ContentType.Alias == Error404.ModelTypeAlias)
            {
                return false;
            }

            if (item?.PageSettings != null)
            {
                var hide = item.PageSettings.GetElementValue<bool>(ElementSeoSettings.ModelTypeAlias, "hideFromSitemapXml");
                if (!hide)
                {
                    return true;
                }
            }
            else if (item == null && page.TemplateId != 0)
            {
                return true;
            }

            return false;
        }
    }
}