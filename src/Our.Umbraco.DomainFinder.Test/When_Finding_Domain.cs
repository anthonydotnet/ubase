using Xunit;
using Moq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Routing;
using System.Collections.Generic;
using Umbraco.Cms.Core.PublishedCache;

namespace Our.Umbraco.DomainFinder.Test
{
    public class When_Finding_Domain
    {
        Mock<IUmbracoContextFactory> _umbracoContextFactoryMock;
        UmbracoContextReference _umbracoContextReference;
        Mock<IUmbracoContext> _umbracoContextMock;
        Mock<IDomainCache> _domainCacheMock;

        public When_Finding_Domain()
        {
            _umbracoContextFactoryMock = new Mock<IUmbracoContextFactory>();
            _umbracoContextMock = new Mock<IUmbracoContext>();
            _domainCacheMock = new Mock<IDomainCache>();

            var umbracoContextAccessor = new Mock<IUmbracoContextAccessor>();
            _umbracoContextReference = new UmbracoContextReference(_umbracoContextMock.Object, true, umbracoContextAccessor.Object);
            _umbracoContextFactoryMock.Setup(x => x.EnsureUmbracoContext()).Returns(_umbracoContextReference);
            _umbracoContextMock.Setup(x => x.Domains).Returns(_domainCacheMock.Object);
        }

        [Theory]
        [InlineData("mydomain.com/", "mydomain.com/")]
        [InlineData("mydomain.com", "mydomain.com/")]
        [InlineData("https://mydomain.com", "https://mydomain.com")] 
        [InlineData("https://mydomain.com/", "https://mydomain.com/")]
        [InlineData("https://mydomain.com:1234", "https://mydomain.com:1234/")]
        [InlineData("https://mydomain.com:1234/", "https://mydomain.com:1234/")]
        [InlineData("https://mydomain.com", "https://mydomain.com/about-us/")]
        [InlineData("https://mydomain.com/", "https://mydomain.com/about-us/")]
        [InlineData("https://mydomain.com/fr", "https://mydomain.com/fr/")]
        [InlineData("https://mydomain.com/fr/", "https://mydomain.com/fr/")]
        [InlineData("https://mydomain.com/ch/fr", "https://mydomain.com/ch/fr/")]
        [InlineData("https://mydomain.com/ch/fr/", "https://mydomain.com/ch/fr/")]
        [InlineData("https://mydomain.com/ch/fr/", "https://mydomain.com/ch/fr/about-us/")]
        public void Domain_Is_Found(string cachedDomain, string domainToFind )
        {
            var domains = new List<Domain>()
            {
                new Domain (123, cachedDomain, 1092, "en-UK", false)
            };
            _domainCacheMock.Setup(x => x.GetAll(true)).Returns(domains);

            var domainFinder = new DomainFinder(_umbracoContextFactoryMock.Object);

            var res = domainFinder.GetDomain(domainToFind);

            Assert.NotNull(res);
            Assert.Equal(domains[0].ContentId, res.ContentId);
        }


        [Theory]
        [InlineData("https://mydomain.com/ch/fr/", "https://mydomain.com/ch/fr")]
        [InlineData("https://mydomain.com/ch/fr/", "https://mydomain.com")]
        [InlineData("https://mydomain.com:1234/", "mydomain.com:1234")]
        [InlineData("mydomain.com:1234/", "mydomain.com:1234")]
        [InlineData("mydomain.com/", "mydomain.com")]
        [InlineData("https://mydomain.com:1234", "mydomain.com:1234")]
        public void Domain_Is_Not_Found(string cachedDomain, string domainToFind)
        {
            var domains = new List<Domain>()
            {
                new Domain (123, cachedDomain, 1092, "en-UK", false)
            };
            _domainCacheMock.Setup(x => x.GetAll(true)).Returns(domains);

            var domainFinder = new DomainFinder(_umbracoContextFactoryMock.Object);

            var res = domainFinder.GetDomain(domainToFind);

            Assert.Null(res);
        }
    }
}
