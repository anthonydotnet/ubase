using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Our.Umbraco.Extensions.Routing;

namespace Application.Web.App_Start.Routing
{
    public class RobotsTxtRouteComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Insert<RobotsTxtRouteComponent>();
        }
    }

    internal class RobotsTxtRouteComponent : IComponent
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public RobotsTxtRouteComponent(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        public void Initialize()
        {
            RouteTable.Routes.MapUmbracoRoute(
                "RobotsTxt",
                "robots.txt",
                new
                {
                    controller = "RobotsTxt",
                    action = "Render"
                },
                new RootNodeByDomainRouteHandler(_umbracoContextFactory)
            );
        }

        public void Terminate()
        {

        }
    }
}