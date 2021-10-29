using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Core.Services;
using DangEasy.Caching.MemoryCache;
using Application.Core.Services.CachedProxies;
using Our.Umbraco.DomainFinder;

namespace Application.Core.App_Start
{
    public class IoCComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            RegisterBuilders(builder);
            RegisterServices(builder);
            RegisterCachedServices(builder);
        }


        private  void RegisterBuilders(IUmbracoBuilder builder)
        {

        }

        private  void RegisterServices(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton(typeof(IDomainFinder), typeof(DomainFinder));
        }

        private  void RegisterCachedServices(IUmbracoBuilder builder)
        {
            if (builder.Config.GetValue<bool>("Cache:ServiceCacheEnabled"))
            {
                builder.Services.AddTransient(typeof(DangEasy.Interfaces.Caching.ICache), typeof(Cache));

                builder.Services.AddTransient(typeof(CmsService), typeof(CmsService));
                builder.Services.AddTransient(typeof(ICmsService), typeof(CmsServiceCachedProxy));
            }
            else
            {
                builder.Services.AddTransient(typeof(ICmsService), typeof(CmsService));
            }
        }
    }
}