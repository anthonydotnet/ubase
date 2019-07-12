using Application.Core.Services.CachedProxies;
using DangEasy.Caching.MemoryCache;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Application.Web.App_Start
{
    public class CachePurgingComposer : ComponentComposer<CachePurging>
    {
    }


    public class CachePurging : IComponent
    {
        public void Compose(Composition composition)
        {
            ContentCacheRefresher.CacheUpdated += ContentCacheRefresher_CacheUpdated;
        }

        private void ContentCacheRefresher_CacheUpdated(ContentCacheRefresher sender, Umbraco.Core.Cache.CacheRefresherEventArgs e)
        {
            Cache.Instance.RemoveByPrefix(typeof(CmsServiceCachedProxy).ToString());
        }

        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }
}