﻿using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Item;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries.Item
{
    [TestFixture]
    public class GetItemSourceTests : TestBase
    {
        [Test]
        public void Should_fetch_tome_by_id()
        {
            new ItemBuilder().With(cr => cr.Id, 23)
                .With(cr => cr.DbName, "dbName")
                .With(cr => cr.FriendlyName, "Hello!")
                .With(cr => cr.IsUnique, true)
                .BuildAndSave();

            var cmd = new GetItemSource(23);

            var item = DomainRegistry.Repository.Find(cmd);

            item.Id.Should().Equals(23);
            item.DbName.Should().BeEquivalentTo("dbName");
            item.FriendlyName.Should().BeEquivalentTo("Hello!");
            item.IsUnique.Should().Be(true);
        }
    }
}