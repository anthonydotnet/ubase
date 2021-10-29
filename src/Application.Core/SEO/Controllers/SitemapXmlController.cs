using Application.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Our.Umbraco.DomainFinder;

namespace Application.Core.SEO.Controllers
{
    public class SitemapXmlController : UmbracoPageController, IVirtualPageController
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDomainFinder _domainFinder;
        private readonly ISitemapXmlGenerator _sitemapXmlGenerator;

        Domain _domain;
        public SitemapXmlController(ILogger<RobotsTxtController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IHttpContextAccessor httpContextAccessor, IDomainFinder domainFinder, ISitemapXmlGenerator sitemapXmlGenerator)
          : base(logger, compositeViewEngine)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _domainFinder = domainFinder;
            _sitemapXmlGenerator = sitemapXmlGenerator;
        }

        public IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
        {
            var originalRequestUrl = _umbracoContextAccessor.GetRequiredUmbracoContext().OriginalRequestUrl;

            _domain = _domainFinder.GetDomain(originalRequestUrl);

            var node = _umbracoContextAccessor.GetRequiredUmbracoContext().Content.GetById(_domain.ContentId);

            return node;
        }

        public IActionResult Index()
        {
            var scheme = _umbracoContextAccessor.GetRequiredUmbracoContext().OriginalRequestUrl.GetLeftPart(UriPartial.Scheme);

            var sitemap = _sitemapXmlGenerator.GetSitemap(_domain, scheme);

            _httpContextAccessor.GetRequiredHttpContext().Response.Headers.Clear();

            return new ContentResult()
            {
                Content = sitemap,
                ContentType = "text/xml",
                StatusCode = 200
            };

        }
    }
}