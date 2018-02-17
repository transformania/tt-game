using System.Collections.Generic;

namespace TT.Domain.Statics
{
    public static class ListifyHelper
    {
        /// <summary>
        /// Takes a list of strings and prints them out in readable English using the Oxford comma.
        /// </summary>
        /// <param name="list">list to convert to readable english</param>
        /// <param name="boldListItems">if true, each list item is surrounded with bold tags</param>
        /// <returns>input strings printed out in readable English</returns>
        public static string Listify(List<string> list, bool boldListItems = false)
        {
            var output = "";

            var open = "";
            var close = "";

            if (boldListItems)
            {
                open = "<b>";
                close = "</b>";
            }
               
            if (list.Count == 1)
            {
                return $"{open}{list[0]}{close}";
            }
            else if (list.Count == 2)
            {
                return $"{open}{list[0]}{close} and {open}{list[1]}{close}";
            }
            else
            {
                for (var i = 0; i < list.Count; i++)
                {
                    if (i < list.Count - 2)
                    {
                        output += $"{open}{list[i]}{close}, ";
                    }
                    else if (i < list.Count - 1)
                    {
                        output += $"{open}{list[i]}{close}, and ";
                    }
                    else
                    {
                        output += $"{open}{list[i]}{close}";
                    }
                }
            }

            return output;
        }

    }
}
