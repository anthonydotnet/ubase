using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Cache;

namespace Application.Web.App_Start
{
    public class ContentScaffoldingComposer : ComponentComposer<CachePurging>
    {
    }


    public class ContentScaffoldingComponent : IComponent
    {
        public void Compose(Composition composition)
        {
        }


        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }
}