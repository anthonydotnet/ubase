using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Application.Web.App_Start.Routing
{
    public class RouteComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Components().Append<RouteComponent>();
        }
    }

    internal class RouteComponent : IComponent
    {
        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }
}