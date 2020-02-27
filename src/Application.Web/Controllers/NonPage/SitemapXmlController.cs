using Application.Core.Services;
using System.Web.Mvc;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using System;

namespace Application.Web.Controllers.NonPage
{
    public class SitemapXmlController : RenderMvcController
    {
        private readonly IDomainService _domainService;
        private readonly ISitemapXmlGenerator _sitemapXmlGenerator;

        public SitemapXmlController(IDomainService domainService, ISitemapXmlGenerator sitemapXmlGenerator)
        {
            _domainService = domainService;
            _sitemapXmlGenerator = sitemapXmlGenerator;
        }

        public ActionResult Index()
        {
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var domain = _domainService.GetByName(Request.Url.Authority);
            var sitemapItems = _sitemapXmlGenerator.GetSitemap(domain.RootContentId.Value, baseUrl);

            Response.ClearHeaders();
            Response.ContentType = "text/xml";

            return View("SitemapXml", sitemapItems);
        }
    }
}