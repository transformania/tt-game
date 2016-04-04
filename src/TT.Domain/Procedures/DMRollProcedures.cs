using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Procedures
{
    public static class DMRollProcedures
    {
        public static string GetRoll(string actionType, string tag)
        {
            IDMRollRepository repo = new EFDMRollRepository();
            Random rand = new Random();
            double roll = rand.NextDouble();

            IEnumerable<DMRoll> options = repo.DMRolls.Where(r => r.IsLive == true && r.ActionType == actionType && r.Tags.Contains(tag));
            List<DMRoll> test = options.ToList();

            double max = options.Count();

            if (max == 0)
            {
                return "[No results found for this encounter type and tag.]";
            }

            int index = Convert.ToInt32(Math.Floor(roll * max));
            return "DM[" + actionType + ":" + tag + "]:  " + options.ElementAt(index).Message;

           

        }
    }
}