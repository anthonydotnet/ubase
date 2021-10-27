using Application.Models.CmsModels;
using DangEasy.Caching.MemoryCache;
using DangEasy.Interfaces.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace Application.Core.Services.CachedProxies
{
    public class CmsServiceCachedProxy : ICmsService
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly ICmsService _cmsService;
        private readonly ICache _cache;

        public CmsServiceCachedProxy(IUmbracoContextFactory umbracoContextFactory, CmsService cmsService, ICache cache)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _cmsService = cmsService;
            _cache = cache;
        }

        /// <summary>
        /// Uses a Dictionary to store ALL the root nodes in cache.
        ///
        /// Uses the IDs in the Path property of the given node to get the Root node from the cached Dictionary.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public SiteRoot GetSiteRoot(int nodeId)
        {
            var cacheKey = CacheKey.Build<CmsServiceCachedProxy, Dictionary<int, SiteRoot>>("-1");

            var sites = _cache.Get<Dictionary<int, SiteRoot>>(cacheKey);

            SiteRoot siteNode;

            if (sites != null)
            {
                // Use the IDs in the Path property to get the Root node from the cached Dictionary.
                IPublishedContent node;
                using (UmbracoContextReference umbContextRef = _umbracoContextFactory.EnsureUmbracoContext())
                {
                    var contentCache = umbContextRef.UmbracoContext.Content;
                    node = umbContextRef.UmbracoContext.Content.GetById(nodeId);
                }

                var pathIds = node.Path.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));

                foreach (var id in pathIds)
                {
                    if (sites.ContainsKey(id))
                    {
                        siteNode = sites[id];

                        return siteNode;
                    }
                }
            }

            // get here if there were no cached Site nodes, OR the Site node was not found in the dictionary
            sites = new Dictionary<int, SiteRoot>();
            siteNode = _cmsService.GetSiteRoot(nodeId);

            if (siteNode != null)
            {
                // GetSiteNode might return null
                sites.Add(siteNode.Id, siteNode);
            }

            _cache.Add(cacheKey, sites);

            return siteNode;
        }

        public Home GetHome(int nodeId)
        {
            var siteNode = GetSiteRoot(nodeId);
            var cacheKey = CacheKey.Build<CmsServiceCachedProxy, Home>(siteNode.Id.ToString());

            return _cache.Get(cacheKey, () => _cmsService.GetHome(siteNode.Id));
        }

        public Error404 GetError404(int nodeId)
        {
            var siteNode = GetSiteRoot(nodeId);
            var cacheKey = CacheKey.Build<CmsServiceCachedProxy, Error404>(siteNode.Id.ToString());

            return _cache.Get(cacheKey, () => _cmsService.GetError404(siteNode.Id));
        }
    }
}