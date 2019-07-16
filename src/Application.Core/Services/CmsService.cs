using Application.Models.Models.CmsModels;
using System.Linq;
using Umbraco.Core.Logging;
using Umbraco.Web;

namespace Application.Core.Services
{
    public interface ICmsService
    {
        Site GetSiteNode(int nodeId);

        HomePage GetHomeNode(int nodeId);

        Error404 GetError404Node(int nodeId);

        //TagContainer GetTagContainer(int currentNodeId);
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

        public Site GetSiteNode(int currentNodeId)
        {
            var node = _umbracoHelper.Content(currentNodeId);
            if (node == null)
            {
                _logger.Warn<CmsService>($"1.Node with id {currentNodeId} is null");
                return null;
            }

            var siteNode = node.AncestorsOrSelf().SingleOrDefault(x => x.ContentType.Alias == Site.ModelTypeAlias) as Site;

            if (siteNode == null)
            {
                _logger.Warn<CmsService>("siteNode is null");
            }

            return siteNode;
        }

        public HomePage GetHomeNode(int currentNodeId)
        {
            var siteNode = GetSiteNode(currentNodeId);
            return siteNode.Children<HomePage>().FirstOrDefault();
        }

        public Error404 GetError404Node(int currentNodeId)
        {
            var homeNode = GetSiteNode(currentNodeId);
            var notFoundNode = homeNode.Children<Error404>().FirstOrDefault();

            return notFoundNode;
        }
    }
}