using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;
using Application.Core.Services;
using Application.Core.App_Start;
using Our.Umbraco.DomainFinder;

namespace Application.Web.App_Start.LastChanceContentFinders
{
    [ComposeAfter(typeof(IoCComposer))]
    public class Error404ContentFinderComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.SetContentLastChanceFinder<Error404LastChanceContentFinder>();
        }
    }

    public class Error404LastChanceContentFinder : IContentLastChanceFinder
    {
        private readonly ICmsService _cmsService;
        private readonly IDomainFinder _domainFinder;

        public Error404LastChanceContentFinder(ICmsService cmsService, IDomainFinder domainFinder)
        {
            _cmsService = cmsService;
            _domainFinder = domainFinder;
        }

        public bool TryFindContent(IPublishedRequestBuilder contentRequest)
        {
            var domain = _domainFinder.GetDomain(contentRequest.Uri);

            if (domain == null)
            {
                return false;
            }

            var notFoundNode = _cmsService.GetError404(domain.ContentId);

            if (notFoundNode != null)
            {
                contentRequest.SetPublishedContent(notFoundNode);
            }

            // return true or false depending on whether our custom 404 page was found
            return contentRequest.PublishedContent != null;
        }
    }
}