using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Microsoft.Extensions.DependencyInjection;


namespace Our.Umbraco.SEOParts.Composers
{
   // [ComposeAfter(typeof(IoCComposer))]
    public class SitemapXmlIoCComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient(typeof(ISitemapXmlGenerator), typeof(SitemapXmlGenerator));
        }
    }
}