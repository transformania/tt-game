using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.Statics
{
    public static class TrustStatics
    {

        public static int[] MultisAllowed = {
              3800, //  Luxianne
             3490, // Mizuho
             224, // Lexam
             254, // Addie (Lexam's girlfriend)
             4481, // Lilith (Luxianne's alt)
             5931, // Mitsuho (Mizuho's PvP alt)
             6195, // Mirin (Cronos landstrider's GF)
             2158, // Cronos Landstrider
             2323, // transform (Cronos's alt)
             1001, // Allyham_Lurra 
             1196, // Balloon_Lover (Puppet_Ally from forums request) 
             1761, // Jessica_Balloons (Puppet_Ally from forums request) 
             6541, // Marked (Puppet_Ally from forums request) 
             7241, // Laryx (Singularity from forums request) 
             7294, // Makitk (Singularity from forums request) 
             7178, // Jessica Dahl from forums request
             7125, // Juva Daent (Jessica Dahl's roommate)
             8329, // Saiko Sosumi (from forums request)
             8348, // Sethra Solus (from forums request)

          };

        public static int[] DonatingAccount = {
             69, // me
             232, // Varn (Lucy Gautreau)
             1922, // Jess Ganes
             8151, // Cloudette
             6214, // Crissa Kentavr,
             7196, // Teranika	Darkfire
             481, // Elyn
             4411, // Kaitlyn Halliwell
             77, // Cara Doom
             6515, // Seras Tearfall
             9618, // Trixie1231
                                              };
       
        public static int[] Artists = {
           69, // Judoo
           72, // Nymic-TF
           76, // jBovinne
           272, // Wrenzephyr2
           1117, // Meddle
           3800, // Luxianne
           4481, // Lillith (Luxianne's alt)
           2090, // Danaume Rook
           1214, // Matt Fox (Goldendawn69)
           6024, // Camren Doss (http://babblingfaces.deviantart.com/art/Elvish-Bard-472692427)
           6196, // spacedande (http://spacedande.deviantart.com/)
           
           
           };

        public static int[] Proofreaders = {
           69, // Judoo
           3800, // Luxianne
           4481, // Lillith (Luxianne's alt)
           3490, // Tsubaki / Mizuho
           481, // Elynsynos
           251, // Arrhae
           1214, // Matt Fox (Goldendawn69)
           1117, // Meddle
           2047, // Kevin Gates
           4988, // Animate Mefreely
           232, // Varn (Lucy Gautreau ingame)
           };

        // also banned names
        public static string[] ReservedNames = {
           "Judoo",
           "Testbot",
           "Psychopath",
           "Admin",
           "Administrator",
           "Lindella",
           "Hitler",
           };

        public static bool PlayerIsProofreader(int MembershipId)
        {
            if (Proofreaders.Contains(MembershipId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PlayerIsContributor(int MembershipId)
        {
            if (DonatingAccount.Contains(MembershipId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string NameIsReserved(string name)
        {

            if (ReservedNames.Contains(name)) {
                return name;
            } else {
                return "";
            }
        }

        public static bool AccountCanHaveMultis(int membershipId) {
            if (MultisAllowed.Contains(membershipId)) {
                return true;
            } else {
                return false;
            }
        }

    }

}