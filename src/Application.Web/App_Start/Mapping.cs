using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Application.Web.App_Start
{
    public class MappingComposer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<MappingComponent>();
        }
    }

    public class MappingComponent : IComponent
    {
        public void Initialize()
        {
            // Add your automapper here
        }

        public void Terminate()
        {
        }
    }
}