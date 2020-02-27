using Application.Core.Services;
using System.Text;
using System.Web.Mvc;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using Umbraco.Web;
using Application.Core.Extensions;

namespace Application.Web.Controllers.NonPage
{
    public class RobotsTxtController : RenderMvcController
    {
        private readonly IDomainService _domainService;
        private readonly ICmsService _cmsService;

        public RobotsTxtController(IDomainService domainService, ICmsService cmsService)
        {
            _domainService = domainService;
            _cmsService = cmsService;
        }

        public ActionResult Render()
        {
            var uriAuthority = Request.Url.Authority;
            var domain = _domainService.GetByName(uriAuthority);

            var siteRoot = _cmsService.GetSiteRoot(domain.RootContentId.Value);

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