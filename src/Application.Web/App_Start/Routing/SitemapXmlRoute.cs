using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Web;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Cms.Web;
using Our.Umbraco.Extensions.Routing;

namespace Application.Web.App_Start.Routing
{
    public class SitemapXmlRouteComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Components().Append<SitemapXmlRouteComponent>();
        }
    }

    internal class SitemapXmlRouteComponent : IComponent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public SitemapXmlRouteComponent(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public void Initialize()
        {
            RouteTable.Routes.MapUmbracoRoute(
                "SitemapXml",
                "sitemap.xml",
                new
                {
                    controller = "SitemapXml",
                    action = "Index"
                },
                new RootNodeByDomainRouteHandler(_umbracoContextFactory)
            );
        }

        public void Terminate()
        {

        }
    }
}