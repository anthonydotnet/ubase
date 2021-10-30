using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Our.Umbraco.DomainFinder
{
    public interface IDomainFinder
    {
        Domain GetDomain(string absoluteUri);
        Domain GetDomain(Uri uri);
        IEnumerable<Domain> GetDomains(Uri uri);
        IPublishedContent GetContentByDomain(Domain domain);
    }

    // forked from callumbwhyte/umbraco-routing-extensions
    public class DomainFinder : IDomainFinder
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public DomainFinder(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
        }

        private IEnumerable<Domain> GetDomains()
        {
            using (UmbracoContextReference umbContextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                try
                {
                    return umbContextRef.UmbracoContext.Domains.GetAll(true);
                }
                catch
                {
                    // case where Umbraco is not set up yet.
                    return new List<Domain>();
                }
            }
        }

        public Domain GetDomain(string absoluteUri)
        {
            Uri uri;

            if (!absoluteUri.StartsWith("http"))
            {
                // this is a hack, however Umbraco allows non-scheme hostnames to be stored
                uri = new Uri($"https://{absoluteUri}", UriKind.Absolute); 
            }
            else
            {
                uri = new Uri(absoluteUri, UriKind.Absolute);
            }

            return GetDomain(uri);
        }

        public Domain GetDomain(Uri uri)
        {
            var baseDomains = GetDomains(uri);

            return baseDomains.FirstOrDefault();
        }

        public IEnumerable<Domain> GetDomains(Uri uri)
        {
            var domains = GetDomains();

            var uriWithSlash = uri.EndPathWithSlash();

            // DomainAndUri handles parsing cases such as lack of scheme
            var domainsAndUris = domains.Select(x => new DomainAndUri(x, uri));

            var uriWithoutPort = uriWithSlash.IsDefaultPort ? uriWithSlash : uriWithSlash.WithoutPort();
            var baseDomains = GetBaseDomains(domainsAndUris, uriWithSlash);

            return baseDomains;
        }

        public IPublishedContent GetContentByDomain(Domain domain)
        {
            if (domain.ContentId < 1)
            {
                return default(IPublishedContent);
            }

            using (UmbracoContextReference umbContextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                return umbContextRef.UmbracoContext.Content.GetById(domain.ContentId);
            }
        }

        private IEnumerable<Domain> GetBaseDomains(IEnumerable<DomainAndUri> domainAndUris, Uri uri)
        {
            var baseDomains = domainAndUris.Where(x => x.Uri.EndPathWithSlash().IsBaseOf(uri));

            return baseDomains;
        }
    }
}