using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures
{
    public static class DungeonProcedures
    {

        public static IEnumerable<Location> GenerateDungeon()
        {
            // get a random name
            List<string> adjectives = new List<string>();
            List<string> nouns = new List<string>();

            var serializer = new XmlSerializer(typeof(List<string>));
            string path = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/DungeonAdjectives.xml");
            using (var reader = XmlReader.Create(path))
            {
                adjectives = (List<string>)serializer.Deserialize(reader);
            }

            string path2 = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/DungeonNouns.xml");
            using (var reader = XmlReader.Create(path2))
            {
                nouns = (List<string>)serializer.Deserialize(reader);
            }

            List<Location> maze = new List<Location>();


            // create base location
            int intname = 0;
            int breakout = 0;
            Location baseNode = new Location
            {
                dbName = "dungeon_" + intname.ToString(),
                CovenantController = -1,
                Description = "",
                Region = "dungeon",
                X = 0,
                Y = 0,
                Name = intname.ToString(),
            };

            maze.Add(baseNode);
            intname++;

            Random rand = new Random();


            // loop until dungeon is built
            while (intname < 100 && breakout < 1000)
            {

                // get a random existing location
                double max = maze.Count();

                double num = rand.NextDouble();

                int index = Convert.ToInt32(Math.Floor(num * max));
                Location baseLocation = maze.ElementAt(index);

                double xory = rand.NextDouble();

                int newX = baseLocation.X;
                int newY = baseLocation.Y;

                if (xory > .5)
                {
                    double xroll = rand.NextDouble();
                    if (xroll < .5)
                    {
                        newX -= 1;
                    }
                    else
                    {
                        newX += 1;
                    }
                }
                else
                {
                    double yroll = rand.NextDouble();
                    if (yroll < .5)
                    {
                        newY -= 1;
                    }
                    else
                    {
                        newY += 1;
                    }

                }

                // get coordinates for a random neighbor
                Location possible = maze.FirstOrDefault(l => l.X == newX && l.Y == newY);

                // we have a location already, so try over
                if (possible != null)
                {
                    breakout++;
                    continue;
                }

                // no location:  make a new one here
                else
                {
                    Location newblock = new Location
                    {
                        dbName = "dungeon_" + intname.ToString(),
                        X = newX,
                        Y = newY,
                        Name = intname.ToString(),
                        CovenantController = -1,
                        Region = "dungeon",
                    };

                    // connects to the right
                    if (newblock.X == baseLocation.X + 1 && newblock.Y == baseLocation.Y)
                    {
                        newblock.Name_West = baseLocation.dbName;
                        baseLocation.Name_East = newblock.dbName;
                    }

                    // connects to the left
                    else if (newblock.X == baseLocation.X + -1 && newblock.Y == baseLocation.Y)
                    {
                        newblock.Name_East = baseLocation.dbName;
                        baseLocation.Name_West = newblock.dbName;
                    }


                     // connects to the north
                    else if (newblock.X == baseLocation.X && newblock.Y == baseLocation.Y + 1)
                    {
                        newblock.Name_South = baseLocation.dbName;
                        baseLocation.Name_North = newblock.dbName;
                    }

                    else if (newblock.X == baseLocation.X && newblock.Y == baseLocation.Y - 1)
                    {
                        newblock.Name_North = baseLocation.dbName;
                        baseLocation.Name_South = newblock.dbName;
                    }

                    maze.Add(newblock);


                }

                intname++;
            }

            double randomBeforeNoWall = .35;
            maze.Reverse();

            // break out some random walls
            foreach (Location loc in maze)
            {
                double breakRoll = rand.NextDouble();

                // eastern neighbor
                if (breakRoll < .25 * randomBeforeNoWall && loc.Name_East == null)
                {
                    Location neighbor = maze.FirstOrDefault(l => l.X == loc.X + 1 && l.Y == loc.Y);
                    if (neighbor != null)
                    {
                        loc.Name_East = neighbor.dbName;
                        neighbor.Name_West = loc.dbName;
                    }
                }

                // western neighbor
                else if (breakRoll < .5 * randomBeforeNoWall && loc.Name_West == null)
                {
                    Location neighbor = maze.FirstOrDefault(l => l.X == loc.X - 1 && l.Y == loc.Y);
                    if (neighbor != null)
                    {
                        loc.Name_West = neighbor.dbName;
                        neighbor.Name_East = loc.dbName;
                    }
                }

                // northern neighbor
                else if (breakRoll < .75 * randomBeforeNoWall && loc.Name_North == null)
                {
                    Location neighbor = maze.FirstOrDefault(l => l.X == loc.X && l.Y == loc.Y + 1);
                    if (neighbor != null)
                    {
                        loc.Name_North = neighbor.dbName;
                        neighbor.Name_South = loc.dbName;
                    }
                }

                // southern neighbor
                else if (breakRoll < 1.0 * randomBeforeNoWall && loc.Name_South == null)
                {
                    Location neighbor = maze.FirstOrDefault(l => l.X == loc.X && l.Y == loc.Y - 1);
                    if (neighbor != null)
                    {
                        loc.Name_South = neighbor.dbName;
                        neighbor.Name_North = loc.dbName;
                    }
                }

            }

            // assign names
            foreach (Location loc in maze)
            {
                int connectionCount = 0;
                if (loc.Name_East != null)
                {
                    connectionCount++;
                }
                if (loc.Name_West != null)
                {
                    connectionCount++;
                }
                if (loc.Name_North != null)
                {
                    connectionCount++;
                }
                if (loc.Name_South != null)
                {
                    connectionCount++;
                }

                if (connectionCount == 1)
                {
                    loc.Name = "Chamber of the ";
                }
                else if (connectionCount == 2)
                {
                    loc.Name = "Passageway of the ";
                }
                else if (connectionCount == 3)
                {
                    loc.Name = "Junction of the ";
                }
                else if (connectionCount == 4)
                {
                    loc.Name = "Crossing of the ";
                }

                // get random adjective
                double maxAdj = adjectives.Count();
                double num = rand.NextDouble();
                int adjindex = Convert.ToInt32(Math.Floor(num * maxAdj));
                string adjToUse = adjectives.ElementAt(adjindex);
                loc.Name += adjToUse + " ";
                adjectives.Remove(adjToUse);

                // get random noun
                double maxNoun = nouns.Count();
                num = rand.NextDouble();
                int nounindex = Convert.ToInt32(Math.Floor(num * maxNoun));
                string nounToUse = nouns.ElementAt(nounindex);
                loc.Name += nounToUse;
                nouns.Remove(nounToUse);
            }


         


          

            List<Location> old = LocationsStatics.LocationList.GetLocation.ToList();

            // clear out the old dungeon from memory
            old = old.Where(l => l.Region != "dungeon").ToList();
            old = old.Concat(maze).ToList();

            LocationsStatics.LocationList.GetLocation = old;

            return maze;
        }


    }
}