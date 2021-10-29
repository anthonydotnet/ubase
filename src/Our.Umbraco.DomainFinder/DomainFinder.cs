using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Our.Umbraco.DomainFinder
{
    public interface IDomainFinder
    {
        Domain GetDomain(string absoluteUri);
        Domain GetDomain(Uri uri);
    }

    public class DomainFinder : IDomainFinder
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        IEnumerable<Domain> _domains;

        //public DomainFinder(IUmbracoContext umbracoContext)
        //{
        //    _domains = umbracoContext.Domains.GetAll(true);
        //}

        public DomainFinder(IUmbracoContextFactory umbracoContextFactory)
        {
            _umbracoContextFactory = umbracoContextFactory;
            using (UmbracoContextReference umbContextRef = _umbracoContextFactory.EnsureUmbracoContext())
            {
                _domains = umbContextRef.UmbracoContext.Domains.GetAll(true);
            }
        }


        public Domain GetDomain(string absoluteUri)
        {
            Uri uri;
            string url = absoluteUri;
            var parsed = Uri.TryCreate(absoluteUri, UriKind.Absolute, out uri);

            if (parsed && uri.Scheme.Contains("http"))
            {
                return GetDomain(uri);
            }

            return QueryDomainCache(url);
        }


        public Domain GetDomain(Uri uri)
        {
            // get domain without scheme (eg. http:// or https://)
            // var url = uri.AbsoluteUri.Replace(uri.GetLeftPart(UriPartial.Scheme), string.Empty);

            var url = uri.AbsoluteUri;
            return QueryDomainCache(url);
        }

        private Domain QueryDomainCache(string url)
        {
            Domain domain = default(Domain);

            // trim from the end until domain is encountered
            var parts = url.Split("/", StringSplitOptions.RemoveEmptyEntries).Reverse();
            foreach (var part in parts)
            {
                domain = _domains.FirstOrDefault(x => url.StartsWith(x.Name));
                if (domain == null)
                {
                    // try without trailing slash
                    url = url.TrimEnd("/");
                    domain = _domains.FirstOrDefault(x => url.StartsWith(x.Name));
                }

                if (domain != null) { break; }

                url = url.TrimEnd(part);
            }

            return domain;
        }
    }
}