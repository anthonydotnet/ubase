using System.Linq;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Microsoft.AspNetCore.Http;
using Application.Models.CmsModels;

namespace Application.Web.App_Start
{
    public class ContentScaffoldingComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<ContentPublishedNotification, SiteContainerPublishedNotificationHandler>();
        }
    }

    public class SiteContainerPublishedNotificationHandler : INotificationHandler<ContentPublishedNotification>
    {
        private readonly IContentService _contentService;
        private readonly IDomainService _domainService;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SiteContainerPublishedNotificationHandler(IContentService contentService, IDomainService domainService, ILocalizationService localizationService, IHttpContextAccessor httpContextAccessor)
        {
            _contentService = contentService;
            _domainService = domainService;
            _localizationService = localizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            foreach (var content in notification.PublishedEntities)
            {
                if (content.ContentType.Alias.InvariantEquals(SiteContainer.ModelTypeAlias))
                {
                    var noOfChildren = _contentService.CountChildren(content.Id);

                    if (noOfChildren == 0)
                    {
                        var domain = _httpContextAccessor.HttpContext.Request.Host.Host;

                        var siteRoot = CreateSiteRoot(content, domain);

                        CreateRepositories(content);
                        CreatePages(siteRoot);

                        _contentService.SaveAndPublishBranch(content, true);
                    }
                }
            }
        }



        private IContent CreateSiteRoot(IContent siteContainer, string domain)
        {
            var siteRoot = _contentService.CreateAndSave("Website", siteContainer.Id, SiteRoot.ModelTypeAlias);

            // add hostname
            var language = _localizationService.GetAllLanguages().First();

            if (_domainService.GetByName(domain) != null)
            {
                domain = $"{domain}/{siteContainer.Name}";
            }

            _domainService.Save(new UmbracoDomain(domain)
            {
                LanguageId = language.Id,
                RootContentId = siteRoot.Id,
                DomainName = domain
            });

            return siteRoot;
        }

        private void CreatePages(IContent siteRoot)
        {
            // home 
            var home = _contentService.CreateAndSave("Home", siteRoot.Id, Home.ModelTypeAlias);
            home.SetValue("title", home.Name);
            siteRoot.SetValue("umbracoInternalRedirectId", home.GetUdi());
            _contentService.Save(siteRoot);

            // basic pages
            var error404 = _contentService.CreateAndSave("Page Not Found", siteRoot.Id, Error404.ModelTypeAlias);
            error404.SetValue("title", error404.Name);
            _contentService.Save(error404);

            var terms = _contentService.CreateAndSave("Terms And Conditions", siteRoot.Id, BasicContent.ModelTypeAlias);
            terms.SetValue("title", terms.Name);
            _contentService.Save(terms);
        }


        private void CreateRepositories(IContent siteContainer)
        {
            // create repositories
            var repository = _contentService.CreateAndSave("Data Repositories", siteContainer.Id, DataRepositories.ModelTypeAlias);

            _contentService.CreateAndSave("Authors", repository.Id, Authors.ModelTypeAlias);
            _contentService.CreateAndSave("Categories", repository.Id, Categories.ModelTypeAlias);
            _contentService.CreateAndSave("Tags", repository.Id, Tags.ModelTypeAlias);
        }
    }
}