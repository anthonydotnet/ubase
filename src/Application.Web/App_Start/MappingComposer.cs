using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Application.Web.App_Start
{
    public class MappingComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Components().Append<MappingComponent>();
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