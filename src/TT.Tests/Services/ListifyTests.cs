using System.Collections.Generic;
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
            var list = new List<string> {"Bob"};

            Assert.That(ListifyHelper.Listify(list), Is.EqualTo("Bob"));
        }

        [Test]
        public void should_print_out_one_item_correctly_with_bolding()
        {
            var list = new List<string> {"Bob"};

            Assert.That(ListifyHelper.Listify(list, true), Is.EqualTo("<b>Bob</b>"));
        }

        [Test]
        public void should_print_out_two_items_correctly()
        {
            var list = new List<string> {"Bob", "Jane"};

            Assert.That(ListifyHelper.Listify(list), Is.EqualTo("Bob and Jane"));
        }

        [Test]
        public void should_print_out_two_items_correctly_with_bolding()
        {
            var list = new List<string> {"Bob", "Jane"};

            Assert.That(ListifyHelper.Listify(list, true), Is.EqualTo("<b>Bob</b> and <b>Jane</b>"));
        }

        [Test]
        public void should_print_out_three_items_correctly()
        {
            var list = new List<string> {"Bob", "Jane", "George"};

            Assert.That(ListifyHelper.Listify(list), Is.EqualTo("Bob, Jane, and George"));
        }

        [Test]
        public void should_print_out_three_items_correctly_with_bolding()
        {
            var list = new List<string> {"Bob", "Jane", "George"};

            Assert.That(ListifyHelper.Listify(list, true), Is.EqualTo("<b>Bob</b>, <b>Jane</b>, and <b>George</b>"));
        }

        [Test]
        public void should_print_out_four_items_correctly()
        {
            var list = new List<string> {"Melissa", "Bob", "Jane", "George"};

            Assert.That(ListifyHelper.Listify(list), Is.EqualTo("Melissa, Bob, Jane, and George"));
        }
    }
}
