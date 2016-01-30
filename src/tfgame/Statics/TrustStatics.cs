using System.Linq;

namespace tfgame.Statics
{
    public static class TrustStatics
    {

       

        // also banned names
        public static string[] ReservedNames = {
           "Judoo",
           "Juderp",
           "Testbot",
           "Psychopath",
           "Admin",
           "Administrator",
           "Moderator",
           "Lindella",
           "Hitler",
           "Jewdewfae",
           "Wuffie",
           "Lovebringer",
           "Narcissa"
           };


        public static string NameIsReserved(string name)
        {

            if (ReservedNames.Contains(name)) {
                return name;
            } else {
                return "";
            }
        }

       

    }

}