using System;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Utilities;

namespace TT.Tests.Domain.Utilities
{
    [TestFixture]
    public class XmlResourceLoaderTests
    {
        [Test]
        public void Load_GivenAnXmlResource_ShouldReturnXml()
        {
            var resourcePath = "TT.Domain.XMLs.FirstNames.xml";

            var xml = XmlResourceLoader.Load(resourcePath);

            xml.Should().Contain("<string>Andy</string>");
        }

        [Test]
        public void Load_GivenAnXmlResouce_WhichDoesntExist_ShouldThrowException()
        {
            var resourcePath = "TT.Domain.DoesntExist.xml";

            Action action = () => XmlResourceLoader.Load(resourcePath);

            action.ShouldThrow<ResourceNotFoundException>();
        }
    }
}