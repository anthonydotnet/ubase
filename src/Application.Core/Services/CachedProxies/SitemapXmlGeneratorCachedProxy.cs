using Application.Models.Models.ViewModels;
using DangEasy.Caching.MemoryCache;
using DangEasy.Interfaces.Caching;
using System.Collections.Generic;

namespace Application.Core.Services.CachedProxies
{
    public class SitemapXmlGeneratorCachedProxy : ISitemapXmlGenerator
    {
        private readonly SitemapXmlGenerator _sitemapXmlGenerator;
        private readonly ICache _cache;

        public SitemapXmlGeneratorCachedProxy(SitemapXmlGenerator sitemapXmlGenerator, ICache cache)
        {
            _sitemapXmlGenerator = sitemapXmlGenerator;
            _cache = cache;
        }

        public List<SitemapXmlItem> GetSitemap(int nodeId, string baseUrl)
        {
            var cacheKey = CacheKey.Build<CmsServiceCachedProxy, List<SitemapXmlItem>>(nodeId.ToString());

            return _cache.Get(cacheKey, () => _sitemapXmlGenerator.GetSitemap(nodeId, baseUrl));
        }
    }
}