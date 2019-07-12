using Application.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace Application.Web.Controllers.Hijacks
{
    public class HomeController : Umbraco.Web.Mvc.RenderMvcController
    {
        private readonly ICmsService _cmsService;

        public HomeController(ICmsService cmsService)
        {
            _cmsService = cmsService;
        }

        public override ActionResult Index(ContentModel model)
        {
            var node = _cmsService.GetError404Node(CurrentPage.Id);

            // Do some stuff here, then return the base method
            return base.Index(model);
        }
    }
}