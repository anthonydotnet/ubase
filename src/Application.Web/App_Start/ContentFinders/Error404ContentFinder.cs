using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application.Web.App_Start.ContentFinders
{
    public class Error404ContentFinder : IContentLastChanceFinder
    {
        private readonly IDomainService _domainService;
        private readonly ICmsService _cmsService;

        public My404ContentFinder(IDomainService domainService, ICmsService cmsService)
        {
            _domainService = domainService;
            _cmsService = cmsService;
        }

        public bool TryFindContent(PublishedRequest contentRequest)
        {
            //find the root node with a matching domain to the incoming request
            var url = contentRequest.Uri.ToString();
            var allDomains = _domainService.GetAll(true);
            var domain = allDomains.Where(f => f.DomainName == contentRequest.Uri.Authority || f.DomainName == "https://" + contentRequest.Uri.Authority).FirstOrDefault();

            var siteId = domain != null ? domain.RootContentId : allDomains.FirstOrDefault().RootContentId;
            var siteRoot = contentRequest.UmbracoContext.Content.GetById(false, siteId ?? -1);
            if (siteRoot == null)
            {
                return false;
            }

            IPublishedContent notFoundNode = _cmsService.GetError404();

            if (notFoundNode != null)
            {
                contentRequest.PublishedContent = notFoundNode;
            }

            // return true or false depending on whether our custom 404 page was found
            return contentRequest.PublishedContent != null;
        }
    }
}