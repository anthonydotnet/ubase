using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Microsoft.Extensions.DependencyInjection;
using Application.Core.Services;
using Application.Core.App_Start;

namespace Application.Core.SEO.Composers
{
    [ComposeAfter(typeof(IoCComposer))]
    public class SitemapXmlIoCComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddTransient(typeof(ISitemapXmlGenerator), typeof(SitemapXmlGenerator));
        }
    }
}