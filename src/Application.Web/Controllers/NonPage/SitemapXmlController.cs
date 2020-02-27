using Application.Core.Services;
using System.Web.Mvc;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web;
using Application.Core.Extensions;
using Umbraco.Core.Models.PublishedContent;
using System.Collections.Generic;
using System.Linq;
using Application.Models.Models.CmsModels;
using Umbraco.Core;
using System;

namespace Application.Web.Controllers.NonPage
{
    public class SitemapXmlController : RenderMvcController
    {
        private readonly IDomainService _domainService;
        private readonly ICmsService _cmsService;
        private string _baseUrl;

        public SitemapXmlController(IDomainService domainService, ICmsService cmsService)
        {
            _domainService = domainService;
            _cmsService = cmsService;
        }

        public ActionResult Index()
        {
            _baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var uriAuthority = Request.Url.Authority;
            var domain = _domainService.GetByName(uriAuthority);

            var siteRoot = _cmsService.GetSiteRoot(domain.RootContentId.Value);
            var homeId = siteRoot.Value<GuidUdi>("umbracoInternalRedirectId");
            var home = Umbraco.Content(homeId.Guid) as Home;

            var sitemapItems = new List<SitemapItem>();
            sitemapItems = ProcessSitemapItems(sitemapItems, new List<IPublishedContent>() { home });
            sitemapItems = ProcessSitemapItems(sitemapItems, siteRoot.Children().Where(x => x.Id != home.Id));

            Response.ClearHeaders();
            Response.ContentType = "text/xml";

            return View("SitemapXml", sitemapItems);
        }


        public List<SitemapItem> ProcessSitemapItems(List<SitemapItem> sitemapItems, IEnumerable<IPublishedContent> nodes)
        {
            foreach (var node in nodes)
            {
                if (CanProcessNode(node))
                {
                    sitemapItems.Add(CreateSitemapItem(node));
                    if (node.Children.Any())
                    {
                        sitemapItems = ProcessSitemapItems(sitemapItems, node.Children);
                    }
                }
            }

            return sitemapItems;
        }

        public SitemapItem CreateSitemapItem(IPublishedContent node, string defaultPath = null)
        {
            var item = new SitemapItem
            {
                ChangeFreq = GetValueOrDefault(node, "seoFrequency", "monthly"),
                Priority = GetValueOrDefault(node, "seoPriority", "0.5"),
                Url = $"{_baseUrl}/{defaultPath ?? node.UrlSegment()}",
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

        public class SitemapItem
        {
            public string ChangeFreq { get; set; }
            public string Priority { get; set; }
            public string Url { get; set; }
            public string LastModified { get; set; }
        }
    }
}