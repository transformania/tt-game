using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Serialization;
using TT.Domain.Utilities;

namespace TT.Domain.ViewModels
{
    public class Location
    {
        public string dbName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsSafe { get; set; }
        public string ImageUrl { get; set; }
        public string Name_North { get; set; }
        public string Name_East { get; set; }
        public string Name_South { get; set; }
        public string Name_West { get; set; }
        public string Region { get; set; }
        public int CovenantController { get; set; }
        public float TakeoverAmount { get; set; }

        public string FriendlyName_North { get; set; }
        public string FriendlyName_East { get; set; }
        public string FriendlyName_South { get; set; }
        public string FriendlyName_West { get; set; }

        public string GetDescription()
        {
            if (Region == "dungeon")
                return "You are wandering in the shifting corridors of the multidimensional dungeon beneath the town, lost amid an ocean of forgotten places and histories in this world and countless others.  Mortal minds must tread carefully down here... danger is around every corner, not just from the various demonic inhabitants of this twisted realm but of other mages eager to prove their superiority over you, possibly even wearing you as another trophy of their conquests.";

            try
            {
                var location = XmlResourceLoader.Load<Location>(string.Format("TT.Domain.XMLs.LocationDescriptions.{0}.xml", dbName));
                return location.Description;
            }
            catch (ResourceNotFoundException)
            {
                return "ERROR:  The description for this location was not able to be loaded.  Please report this bug at http://luxianne.com/forum/viewforum.php?f=5.";
            }
        }
    }
}