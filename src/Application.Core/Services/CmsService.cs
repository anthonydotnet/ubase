using Application.Models.CmsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web;
using Umbraco.Extensions;

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
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public CmsService(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public SiteRoot GetSiteRoot(int currentNodeId)
        {
            IPublishedContent node;
            using (UmbracoContextReference umbContextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                node = umbContextRef.UmbracoContext.Content.GetById(currentNodeId);
            }

            if (node == null)
            {
                throw new Exception($"Node with id {currentNodeId} is null.");
            }

            var foundNode = node.AncestorsOrSelf().SingleOrDefault(x => x.ContentType.Alias == SiteRoot.ModelTypeAlias);
            var siteNode = foundNode as SiteRoot;

            if (siteNode == null)
            {
                throw new Exception($"Site Node was not found.");
            }

            return siteNode;
        }

        public Home GetHome(int currentNodeId)
        {
            var siteNode = GetSiteRoot(currentNodeId);
            var home = siteNode.Children.FirstOrDefault(x => x.ContentType.Alias == Home.ModelTypeAlias);

            return home as Home;
        }

        public Error404 GetError404(int currentNodeId)
        {
            var siteNode = GetSiteRoot(currentNodeId);
            var notFoundNode = siteNode.Children.FirstOrDefault(x => x.ContentType.Alias == Error404.ModelTypeAlias);

            return notFoundNode as Error404;
        }
    }
}