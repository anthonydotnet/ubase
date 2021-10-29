using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace Application.Web.Controllers.Areas.Hijacks.Controllers
{
    public class HomeController : RenderController
    {
        public HomeController(ILogger<HomeController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor)
           : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }


        public override IActionResult Index()
        {
            // you are in control here!

            // return a 'model' to the selected template/view for this page.
            return CurrentTemplate(CurrentPage);
        }
    }
}