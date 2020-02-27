using Application.Models.Models.CmsModels;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;

namespace Application.Web.App_Start
{
    public class ContentScaffoldingComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Components().Append<ContentScaffoldingComponent>();
        }
    }

    public class ContentScaffoldingComponent : IComponent
    {
        public void Initialize()
        {
            ContentService.Published += ContentService_Published; ;
        }

        private void ContentService_Published(IContentService contentService, ContentPublishedEventArgs e)
        {
            foreach (var content in e.PublishedEntities)
            {
                if (content.ContentType.Alias.InvariantEquals(SiteContainer.ModelTypeAlias))
                {
                    var noOfChildren = contentService.CountChildren(content.Id);

                    if (noOfChildren == 0)
                    {
                        var domainService = Current.Services.DomainService;
                        var localizationService = Current.Services.LocalizationService;
                        var domain = HttpContext.Current.Request.Url.Host;

                        var siteRoot = CreateSiteRoot(contentService, content, domainService, localizationService, domain);

                        CreateRepositories(contentService, content);
                        CreatePages(contentService, siteRoot);

                        contentService.SaveAndPublishBranch(content, true);
                    }
                }
            }
        }

        private IContent CreateSiteRoot(IContentService contentService, IContent siteContainer, IDomainService domainService, ILocalizationService localizationService, string domain)
        {
            var siteRoot = contentService.CreateAndSave("Website", siteContainer.Id, SiteRoot.ModelTypeAlias);

            // add hostname
            var language = localizationService.GetAllLanguages().First();

            if (domainService.GetByName(domain) != null)
            {
                domain = $"{domain}/{siteContainer.Name}";
            }

            domainService.Save(new UmbracoDomain(domain)
            {
                LanguageId = language.Id,
                RootContentId = siteRoot.Id,
                DomainName = domain
            });

            return siteRoot;
        }

        private void CreatePages(IContentService contentService, IContent siteRoot)
        {
            // home 
            var home = contentService.CreateAndSave("Home", siteRoot.Id, Home.ModelTypeAlias);
            home.SetValue("title", home.Name);
            siteRoot.SetValue("umbracoInternalRedirectId", home.GetUdi());
            contentService.Save(siteRoot, raiseEvents: false);

            // basic pages
            var error404 = contentService.CreateAndSave("Page Not Found", siteRoot.Id, Error404.ModelTypeAlias);
            error404.SetValue("title", error404.Name);
            contentService.Save(error404, raiseEvents: false);

            var terms = contentService.CreateAndSave("Terms And Conditions", siteRoot.Id, BasicContent.ModelTypeAlias);
            terms.SetValue("title", terms.Name);
            contentService.Save(terms, raiseEvents: false);
        }


        private void CreateRepositories(IContentService contentService, IContent siteContainer)
        {
            // create repositories
            var repository = contentService.CreateAndSave("Data Repositories", siteContainer.Id, DataRepositories.ModelTypeAlias);
            
            contentService.CreateAndSave("Authors", repository.Id, Authors.ModelTypeAlias);
            contentService.CreateAndSave("Categories", repository.Id, Categories.ModelTypeAlias);
            contentService.CreateAndSave("Tags", repository.Id, Tags.ModelTypeAlias);
        }

        public void Terminate()
        {
        }
    }
}