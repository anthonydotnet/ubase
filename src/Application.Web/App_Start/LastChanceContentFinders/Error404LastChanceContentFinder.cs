using Application.Core.Services;
using Umbraco.Core.Composing;
using Umbraco.Core.Services;
using Umbraco.Web.Routing;
using Umbraco.Web;

namespace Application.Web.App_Start.LastChanceContentFinders
{
    public class Error404ContentFinderComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            //set the last chance content finder
            composition.SetContentLastChanceFinder<Error404LastChanceContentFinder>();
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

        public bool TryFindContent(PublishedRequest contentRequest)
        {
            var uriAuthority = contentRequest.Uri.Authority;
            var domain = _domainService.GetByName(uriAuthority);

            var notFoundNode = _cmsService.GetError404(domain.RootContentId.Value);

            if (notFoundNode != null)
            {
                contentRequest.PublishedContent = notFoundNode;
            }

            // return true or false depending on whether our custom 404 page was found
            return contentRequest.PublishedContent != null;
        }
    }
}