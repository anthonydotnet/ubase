using Application.Core.Services;
using System.Web.Mvc;
using Umbraco.Web.Models;

namespace Application.Web.Controllers.Hijacks
{
    public class BasicContentController : Umbraco.Web.Mvc.RenderMvcController
    {
        private readonly ICmsService _cmsService;

        public BasicContentController(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        public override ActionResult Index(ContentModel model)
        {
            // Do some stuff here
            return base.Index(model);
        }
    }
}