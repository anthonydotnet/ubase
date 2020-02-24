using Application.Models.Models.CmsModels;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace Application.Core.Services
{
    public interface ICmsService
    {
        SiteRoot GetSiteRoot(int nodeId);

        Home GetHome(int nodeId);

        Error404 GetError404(int nodeId);
    }

    public class CmsService : ICmsService
    {
        private readonly UmbracoHelper _umbracoHelper;
        private readonly ILogger _logger;

        public CmsService(UmbracoHelper umbracoHelper, ILogger logger)
        {
            _umbracoHelper = umbracoHelper;
            _logger = logger;
        }

        public SiteRoot GetSiteRoot(int currentNodeId)
        {
            var node = _umbracoHelper.Content(currentNodeId);
            if (node == null)
            {
                _logger.Warn<CmsService>($"1.Node with id {currentNodeId} is null");
                return null;
            }

            var siteNode = node.AncestorsOrSelf().SingleOrDefault(x => x.ContentType.Alias == SiteRoot.ModelTypeAlias) as SiteRoot;

            if (siteNode == null)
            {
                _logger.Warn<CmsService>("siteNode is null");
            }

            return siteNode;
        }

        public Home GetHome(int currentNodeId)
        {
            var siteNode = GetSiteRoot(currentNodeId);
            return siteNode.Children<Home>().FirstOrDefault();
        }

        public Error404 GetError404(int currentNodeId)
        {
            var homeNode = GetSiteRoot(currentNodeId);
            var notFoundNode = homeNode.Children<Error404>().FirstOrDefault();

            return notFoundNode;
        }
    }
}