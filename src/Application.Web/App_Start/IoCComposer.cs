using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Core.Services;
using DangEasy.Caching.MemoryCache;
using Application.Core.Services.CachedProxies;

namespace Application.Web.App_Start
{
    public class IoCComposer : IComposer
    {
        private IConfiguration _config;
        public void Compose(IUmbracoBuilder builder)
        {
            _config = builder.Config;

            RegisterBuilders(builder);
            RegisterServices(builder);
            RegisterCachedServices(builder);
        }


        private  void RegisterBuilders(IUmbracoBuilder builder)
        {

        }

        private  void RegisterServices(IUmbracoBuilder builder)
        {

        }

        private  void RegisterCachedServices(IUmbracoBuilder builder)
        {
            if (_config.GetValue<bool>("uBase:ServiceCacheEnabled"))
            {
                builder.Services.AddTransient(typeof(DangEasy.Interfaces.Caching.ICache), typeof(Cache));

                builder.Services.AddTransient(typeof(ICmsService), typeof(CmsServiceCachedProxy));
                builder.Services.AddTransient(typeof(CmsService), typeof(CmsService));

                //builder.Services.AddTransient(typeof(SitemapXmlGenerator), typeof(SitemapXmlGenerator));
                //builder.Services.AddTransient(typeof(ISitemapXmlGenerator), typeof(SitemapXmlGeneratorCachedProxy));
            }
            else
            {
                builder.Services.AddTransient(typeof(ICmsService), typeof(CmsService));
                // builder.Services.AddTransient(typeof(ISitemapXmlGenerator), typeof(SitemapXmlGenerator));
            }
        }
    }
}