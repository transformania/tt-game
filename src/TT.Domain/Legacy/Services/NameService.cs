using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Utilities;

namespace TT.Domain.Legacy.Services
{
    public static class NameService
    {
        public static Random rand = new Random();

        public static string GetRandomLastName()
        {
            var allLastNames = GetNames("FirstNames");
            return allLastNames.ElementAt((int)Math.Floor(rand.NextDouble() * allLastNames.Count));
        }

        public static string GetRandomFirstName()
        {
            var allFirstNames = GetNames("LastNames");
            return allFirstNames.ElementAt((int)Math.Floor(rand.NextDouble() * allFirstNames.Count));
        }

        private static List<string> GetNames(string firstOrLast)
        {
            return XmlResourceLoader.Load<List<string>>($"TT.Domain.XMLs.{firstOrLast}.xml");
        }

    }
}
