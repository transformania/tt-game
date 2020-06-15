using System.Collections.Generic;
using NUnit.Framework;
using TT.Domain.Utilities;

namespace TT.Tests.Domain.Utilities
{
    [TestFixture]
    public class XmlResourceLoaderTests
    {
        [Test]
        public void Load_GivenAnXmlResource_ShouldReturnDeserialisedObject()
        {
            var resourcePath = "TT.Domain.XMLs.FirstNames.xml";

            var resource = XmlResourceLoader.Load<List<string>>(resourcePath);

            Assert.That(resource, Has.Member("Andy"));
        }

        [Test]
        public void Load_GivenAnXmlResouce_WhichDoesntExist_ShouldThrowException()
        {
            var resourcePath = "TT.Domain.DoesntExist.xml";

            Assert.That(() => XmlResourceLoader.Load<List<string>>(resourcePath),
                Throws.TypeOf<ResourceNotFoundException>());
        }
    }
}