using Xunit;
using Moq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Core.Routing;
using System.Collections.Generic;
using Umbraco.Cms.Core.PublishedCache;
using System.Linq;

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
        [InlineData("mydomain.com", "http://mydomain.com")]
        [InlineData("mydomain.com", "https://mydomain.com")]
        [InlineData("mydomain.com/fr/", "https://mydomain.com/fr/")]
        [InlineData("mydomain.com/ch/fr/", "https://mydomain.com/ch/fr/")]
        public void AbsoluteUri_Without_Scheme_Is_Found(string cachedDomain, string domainToFind )
        {
            RunGenericTest(cachedDomain, domainToFind);
        }


        [Theory]
        [InlineData("https://mydomain.com/", "https://mydomain.com/")]
        [InlineData("https://mydomain.com", "https://mydomain.com/")]
        public void AbsoluteUri_With_Trailing_Slash_Is_Truncated_Until_Domain_Is_Found(string cachedDomain, string domainToFind)
        {
            RunGenericTest(cachedDomain, domainToFind);
        }

        [Theory]
        [InlineData("https://mydomain.com/fr", "https://mydomain.com/fr/")]
        [InlineData("https://mydomain.com/fr/", "https://mydomain.com/fr/")]
        [InlineData("https://mydomain.com/ch/fr", "https://mydomain.com/ch/fr/")]
        [InlineData("https://mydomain.com/ch/fr/", "https://mydomain.com/ch/fr/")]
        public void AbsoluteUri_With_Path_Segment_Is_Found(string cachedDomain, string domainToFind)
        {
            RunGenericTest(cachedDomain, domainToFind);
        }


        [Theory]
        [InlineData("https://mydomain.com/", "https://mydomain.com")]
        [InlineData("https://mydomain.com/ch/fr/", "https://mydomain.com/ch/fr")]
        [InlineData("https://mydomain.com:1234/", "https://mydomain.com:1234")]
        [InlineData("localhost:1234/", "https://localhost:1234")]
        [InlineData("localhost:1234/ch/fr", "http://localhost:1234/ch/fr")]
        public void AbsoluteUri_Without_Trailing_Slash_Is_Found(string cachedDomain, string domainToFind)
        {
            RunGenericTest(cachedDomain, domainToFind);
        }



        [Theory]
        [InlineData("https://mydomain.com", "https://mydomain.com/about-us/")]
        [InlineData("https://mydomain.com", "https://mydomain.com/news/my-article/")]
        [InlineData("https://mydomain.com/ch/fr", "https://mydomain.com/ch/fr/news/my-article/")]
        public void AbsoluteUri_With_Path_Segment_Is_Truncated_Until_Domain_Is_Found(string cachedDomain, string domainToFind)
        {
            RunGenericTest(cachedDomain, domainToFind);
        }


        [Theory]
        [InlineData("https://mydomain.com:1234", "https://mydomain.com:1234/")]
        [InlineData("https://mydomain.com:1234/", "https://mydomain.com:1234/")]
        public void AbsoluteUri_With_Port_Is_Found(string cachedDomain, string domainToFind)
        {
            RunGenericTest(cachedDomain, domainToFind);
        }

        [Theory]
        [InlineData("https://mydomain.com/ch/fr/", "https://mydomain.com")]
        [InlineData("http://mydomain.com:1234", "https://mydomain.com:1234")]
        [InlineData("https://mydomain.com:1234", "http://mydomain.com:1234")]
        [InlineData("https://mydomain.com:1234", "http://mydomain.com:9999")]
        public void Domain_Is_Not_Found(string cachedDomain, string domainToFind)
        {
            var domains = new List<Domain>()
            {
                new Domain (123, cachedDomain, 1092, "en-US", false)
            };
            _domainCacheMock.Setup(x => x.GetAll(true)).Returns(domains);

            var domainFinder = new DomainFinder(_umbracoContextFactoryMock.Object);

            var res = domainFinder.GetDomain(domainToFind);

            Assert.Null(res);
        }


        [Fact]
        public void AbsoluteUri_Has_Multiple_Found()
        {
            var domains = new List<Domain>()
            {
                new Domain (123, "http://localhost/ch", 1092, "en-US", false),
                new Domain (1234, "http://localhost/ch/fr", 1093, "en-US", false)
            };
            _domainCacheMock.Setup(x => x.GetAll(true)).Returns(domains);

            var domainFinder = new DomainFinder(_umbracoContextFactoryMock.Object);

            var res = domainFinder.GetDomains(new System.Uri("http://localhost/ch/fr/about-us"));

            Assert.True(res.Count() == 2);
        }


        private void RunGenericTest(string cachedDomain, string domainToFind)
        {
            var domains = new List<Domain>()
            {
                new Domain (123, cachedDomain, 1092, "en-US", false)
            };
            _domainCacheMock.Setup(x => x.GetAll(true)).Returns(domains);

            var domainFinder = new DomainFinder(_umbracoContextFactoryMock.Object);

            var res = domainFinder.GetDomain(domainToFind);

            Assert.NotNull(res);
            Assert.Equal(domains[0].ContentId, res.ContentId);
        }
    }
}
