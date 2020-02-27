using Application.Models.Models.CmsModels;
using Application.Models.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Application.Core.Extensions;
using Umbraco.Core;

namespace Application.Core.Services
{
    public interface ISitemapXmlGenerator
    {
        List<SitemapXmlItem> GetSitemap(int nodeId, string baseUrl);
    }

    public class SitemapXmlGenerator : ISitemapXmlGenerator
    {
        private readonly ICmsService _cmsService;

        public SitemapXmlGenerator(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        public List<SitemapXmlItem> GetSitemap(int nodeId, string baseUrl)
        {
            var sitemapItems = new List<SitemapXmlItem>();

            var siteRoot = _cmsService.GetSiteRoot(nodeId);
            var home = _cmsService.GetHome(nodeId);

            sitemapItems = ProcessSitemapItems(baseUrl, sitemapItems, new List<IPublishedContent>() { home });
            sitemapItems = ProcessSitemapItems(baseUrl, sitemapItems, siteRoot.Children().Where(x => x.Id != home.Id));

            return sitemapItems;
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
                Url = $"{baseUrl}/{defaultPath ?? node.UrlSegment()}",
                LastModified = string.Format("{0:s}+00:00", node.UpdateDate),
            };

            return item;
        }


        private string GetValueOrDefault(IPublishedContent node, string alias, string defaultValue)
        {
            var settings = node.Value<IEnumerable<IPublishedElement>>("pageSettings");

            string value;
            if (settings != null)
            {
                var element = settings.FirstOrDefault(x => x.ContentType.Alias == ElementSeoSettings.ModelTypeAlias);

                if (element == null)
                {
                    return defaultValue;
                }

                value = element.Value<string>(alias);
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