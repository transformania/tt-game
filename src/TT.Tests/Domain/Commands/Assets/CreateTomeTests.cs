﻿using System;
using NUnit.Framework;
using FluentAssertions;
using System.Linq;
using TT.Domain.Commands.Assets;
using TT.Domain.Entities.Assets;
using TT.Tests.Builders.Item;
using TT.Domain;

namespace TT.Tests.Domain.Commands.Assets
{
    [TestFixture]
    public class CreateTomeTests : TestBase
    {
        [Test]
        public void Should_create_new_tome()
        {

            var item = new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave();
            var cmd = new CreateTome { Text = "This is a tome.", BaseItemId = item.Id };

            var tome = Repository.Execute(cmd);

            DataContext.AsQueryable<Tome>().Count(cr =>
                cr.Id == tome.Id &&
                cr.Text == "This is a tome." &&
                cr.BaseItem.Id == item.Id)
            .Should().Be(1);

        }

        [Test]
        public void Should_throw_error_when_text_is_empty()
        {
            var item = new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave();
            var cmd = new CreateTome { Text = "", BaseItemId = item.Id };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("No text"));
        }

        [Test]
        public void Should_throw_error_when_base_item_id_is_0()
        {
            var cmd = new CreateTome { Text = "tome text", BaseItemId = 0 };

            var action = new Action(() => { Repository.Execute(cmd); });

            action.ShouldThrowExactly<DomainException>().WithMessage(string.Format("No base item was provided"));
        }
    }
}