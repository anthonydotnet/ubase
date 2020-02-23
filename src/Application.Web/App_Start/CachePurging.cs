using Umbraco.Core;
using Umbraco.Core.Composing;
using Application.Core.Services.CachedProxies;
using DangEasy.Caching.MemoryCache;
using Umbraco.Web.Cache;
using Umbraco.Core.Cache;

namespace Application.Web.App_Start
{
    public class CachePurgingComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<CachePurgingComponent>();
        }
    }

    public class CachePurgingComponent : IComponent
    {
        public void Compose(Composition composition)
        {
        }

        public void Initialize()
        {
            ContentCacheRefresher.CacheUpdated += ContentCacheRefresher_CacheUpdated;
        }

        private void ContentCacheRefresher_CacheUpdated(ContentCacheRefresher sender, CacheRefresherEventArgs e)
        {
            Cache.Instance.RemoveByPrefix(typeof(CmsServiceCachedProxy).ToString());
        }

        public void Terminate()
        {
        }
    }
}