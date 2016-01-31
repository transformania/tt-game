using System;
using System.Collections.Generic;
using FluentAssertions;
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

            resource.Should().Contain("Andy");
        }

        [Test]
        public void Load_GivenAnXmlResouce_WhichDoesntExist_ShouldThrowException()
        {
            var resourcePath = "TT.Domain.DoesntExist.xml";

            Action action = () => XmlResourceLoader.Load<List<string>>(resourcePath);

            action.ShouldThrow<ResourceNotFoundException>();
        }
    }
}