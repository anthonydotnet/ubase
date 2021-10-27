using Application.Core.Services.CachedProxies;
using DangEasy.Caching.MemoryCache;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Events;
using DangEasy.Interfaces.Caching;

namespace Application.Web.App_Start
{
    public class CachePurgingComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentCacheRefresherNotification, CachePurgingNotificationHandler>();
        }
    }

    public class CachePurgingNotificationHandler : INotificationHandler<ContentCacheRefresherNotification>
    {
        private readonly ICache _cache;

        public CachePurgingNotificationHandler(ICache cache)
        {
            _cache = cache;
        }

        public void Handle(ContentCacheRefresherNotification notification)
        {
            // purge all cache prefixed with CmsServiceCachedProxy
            _cache.RemoveByPrefix(typeof(CmsServiceCachedProxy).ToString());
        }
    }
}