using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Our.Umbraco.Extensions.Routing;

namespace Application.Web.App_Start.Routing
{
    public class SitemapXmlRouteComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Insert<SitemapXmlRouteComponent>();
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