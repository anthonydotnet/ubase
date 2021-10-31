using System.Text;
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
using System.Collections.Generic;
using Our.Umbraco.SEOParts.Extensions;

namespace Our.Umbraco.SEOParts.Controllers
{
    public class RobotsTxtController : UmbracoPageController, IVirtualPageController
    {
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IDomainFinder _domainFinder;
        Domain _domain;

        public RobotsTxtController(ILogger<RobotsTxtController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor, IDomainFinder domainFinder)
          : base(logger, compositeViewEngine)
        {
            _umbracoContextAccessor = umbracoContextAccessor;
            _domainFinder = domainFinder;
        }

        public IPublishedContent FindContent(ActionExecutingContext actionExecutingContext)
        {
            var originalRequestUrl = _umbracoContextAccessor.GetRequiredUmbracoContext().OriginalRequestUrl;

            _domain = _domainFinder.GetDomain(originalRequestUrl);

            var node = _domainFinder.GetContentByDomain(_domain);

            return node;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var siteRoot = CurrentPage;
            string robotsTxt = "User-agent: *\nDisallow: /"; // default

            if (siteRoot.HasProperty("siteSettings") && siteRoot.GetProperty("siteSettings").HasValue())
            {
                var siteSeoSettings = siteRoot.GetProperty("siteSettings").GetValue() as IEnumerable<IPublishedElement>;

                var allowRobots = siteSeoSettings.GetElementValue<bool>("elementSiteSeoSettings", "allowRobots");

                if (allowRobots)
                {
                    robotsTxt = siteSeoSettings.GetElementValue<string>("elementSiteSeoSettings", "robotsTxt");
                }
            }
            
            return Content(robotsTxt, "text/text", Encoding.UTF8);
        }
    }
}