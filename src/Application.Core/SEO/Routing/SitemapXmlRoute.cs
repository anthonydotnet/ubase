using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Microsoft.AspNetCore.Rewrite;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Application.Core.SEO.Controllers;

namespace Application.Core.SEO.Routing
{
    public class SitemapXmlRouteComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // add rewrite
            var rewriteOptions = new RewriteOptions().AddRewrite("sitemap.xml", "sitemapxml", true);
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(SitemapXmlController))
                {
                    PrePipeline = app => app.UseRewriter(rewriteOptions)
                });
            });

            // add custom MVC router
            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter(nameof(SitemapXmlController))
                {
                    Endpoints = app => app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllerRoute(
                             name: nameof(SitemapXmlController),
                            pattern: "/sitemapxml",
                            defaults: new { Controller = "SitemapXml", Action = "Index" });
                    })
                });
            });
        }
    }
}