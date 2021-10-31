using Application.Core.Services;
using System.Text;
using Application.Core.Extensions;
using Umbraco.Cms.Web.Common.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Web;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Our.Umbraco.DomainFinder;

namespace Our.Umbraco.SEOParts.Controllers
{
    public class RobotsTxtController : UmbracoPageController, IVirtualPageController
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly ICmsService _cmsService;
        private readonly IDomainFinder _domainFinder;
        Domain _domain;

        public RobotsTxtController(ILogger<RobotsTxtController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, ICmsService cmsService, IDomainFinder domainFinder)
          : base(logger, compositeViewEngine)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _cmsService = cmsService;
            _domainFinder = domainFinder;
        }

        public IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
        {
            // TODO: get root node from domain based on request URL
            var originalRequestUrl = _umbracoContextAccessor.GetRequiredUmbracoContext().OriginalRequestUrl;

            _domain = _domainFinder.GetDomain(originalRequestUrl);

            var node = _umbracoContextAccessor.GetRequiredUmbracoContext().Content.GetById(1092);

            return node;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var siteRoot = _cmsService.GetSiteRoot(_domain.ContentId);

            var siteSeoSettings = siteRoot.SiteSettings.GetElement("elementSiteSeoSettings");

            string robotsTxt;
            if (siteSeoSettings == null || siteSeoSettings.Value<bool>("disallowRobots"))
            {
                robotsTxt = "User-agent: *\nDisallow: /";
            }
            else
            {
                robotsTxt = siteSeoSettings.Value<string>("robotsTxt");
            }

            return Content(robotsTxt, "text/text", Encoding.UTF8);
        }
    }
}