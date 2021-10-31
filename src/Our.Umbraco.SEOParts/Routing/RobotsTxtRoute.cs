using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Our.Umbraco.SEOParts.Controllers;

namespace Application.Core.SEO.Routing
{
    public class RobotsTxtRouteComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // add rewrite
            var rewriteOptions = new RewriteOptions().AddRewrite("robots.txt", "robotstxt", true);
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(RobotsTxtController))
                {
                    PrePipeline = app => app.UseRewriter(rewriteOptions)
                });
            });
            
            // add custom MVC router
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(RobotsTxtController))
                {
                    Endpoints = app => app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllerRoute(
                             name: nameof(RobotsTxtController),
                            pattern: "/robotstxt",
                            defaults: new { Controller = "RobotsTxt", Action = "Index" });
                    })
                });
            });
          
        }
    }
}