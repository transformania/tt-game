using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace tfgame.Statics
{
    public static class TrustStatics
    {

       

        // also banned names
        public static string[] ReservedNames = {
           "Judoo",
           "Testbot",
           "Psychopath",
           "Admin",
           "Administrator",
           "Lindella",
           "Hitler",
           "Jewdewfae",
           "Wuffie",
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