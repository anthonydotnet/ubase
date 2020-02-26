using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Application.Web.App_Start.Routing
{
    public class RouteComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Insert<RouteComponent>();
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