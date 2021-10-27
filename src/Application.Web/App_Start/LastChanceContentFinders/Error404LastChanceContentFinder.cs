using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Routing;
using Umbraco.Extensions;
using Application.Core.Services;

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
        private readonly IDomainService _domainService;
        private readonly ICmsService _cmsService;

        public Error404LastChanceContentFinder(IDomainService domainService, ICmsService cmsService)
        {
            _domainService = domainService;
            _cmsService = cmsService;
        }

        public bool TryFindContent(IPublishedRequestBuilder contentRequest)
        {
            var uriAuthority = contentRequest.Uri.Authority;
            var domain = _domainService.GetByName(uriAuthority);

            var notFoundNode = _cmsService.GetError404(domain.RootContentId.Value);

            if (notFoundNode != null)
            {
                contentRequest.SetPublishedContent(notFoundNode);
            }

            // return true or false depending on whether our custom 404 page was found
            return contentRequest.PublishedContent != null;
        }
    }
}