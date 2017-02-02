using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain.Statics;

namespace TT.Tests.Services
{
    [TestFixture]
    public class ListifyTests : TestBase
    {
        [Test]
        public void should_print_out_one_item_correctly()
        {
            var list = new List<string>();
            list.Add("Bob");

            var output = ListifyHelper.Listify(list);
            output.Should().Be("Bob");

        }

        [Test]
        public void should_print_out_one_item_correctly_with_bolding()
        {
            var list = new List<string>();
            list.Add("Bob");

            var output = ListifyHelper.Listify(list, true);
            output.Should().Be("<b>Bob</b>");

        }

        [Test]
        public void should_print_out_two_items_correctly()
        {
            var list = new List<string>();
            list.Add("Bob");
            list.Add("Jane");

            var output = ListifyHelper.Listify(list);
            output.Should().Be("Bob and Jane");

        }

        [Test]
        public void should_print_out_two_items_correctly_with_bolding()
        {
            var list = new List<string>();
            list.Add("Bob");
            list.Add("Jane");

            var output = ListifyHelper.Listify(list, true);
            output.Should().Be("<b>Bob</b> and <b>Jane</b>");

        }

        [Test]
        public void should_print_out_three_items_correctly()
        {
            var list = new List<string>();
            list.Add("Bob");
            list.Add("Jane");
            list.Add("George");

            var output = ListifyHelper.Listify(list);
            output.Should().Be("Bob, Jane, and George");

        }

        [Test]
        public void should_print_out_three_items_correctly_with_bolding()
        {
            var list = new List<string>();
            list.Add("Bob");
            list.Add("Jane");
            list.Add("George");

            var output = ListifyHelper.Listify(list, true);
            output.Should().Be("<b>Bob</b>, <b>Jane</b>, and <b>George</b>");

        }

        [Test]
        public void should_print_out_four_items_correctly()
        {
            var list = new List<string>();
            list.Add("Melissa");
            list.Add("Bob");
            list.Add("Jane");
            list.Add("George");

            var output = ListifyHelper.Listify(list);
            output.Should().Be("Melissa, Bob, Jane, and George");

        }
    }
}
