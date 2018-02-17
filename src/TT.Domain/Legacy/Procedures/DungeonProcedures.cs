using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Statics;
using TT.Domain.Utilities;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class DungeonProcedures
    {

        public static IEnumerable<Location> GenerateDungeon()
        {
            // get a random name
            var adjectives = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.DungeonAdjectives.xml");
            var nouns = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.DungeonNouns.xml");
            var maze = new List<Location>();


            // create base location
            var intname = 0;
            var breakout = 0;
            var baseNode = new Location
            {
                dbName = "dungeon_" + intname,
                CovenantController = -1,
                Description = "",
                Region = "dungeon",
                X = 0,
                Y = 0,
                Name = intname.ToString(),
            };

            maze.Add(baseNode);
            intname++;

            var rand = new Random();


            // loop until dungeon is built
            while (intname < 100 && breakout < 1000)
            {

                // get a random existing location
                double max = maze.Count();

                var num = rand.NextDouble();

                var index = Convert.ToInt32(Math.Floor(num * max));
                var baseLocation = maze.ElementAt(index);

                var xory = rand.NextDouble();

                var newX = baseLocation.X;
                var newY = baseLocation.Y;

                if (xory > .5)
                {
                    var xroll = rand.NextDouble();
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
                    var yroll = rand.NextDouble();
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
                var possible = maze.FirstOrDefault(l => l.X == newX && l.Y == newY);

                // we have a location already, so try over
                if (possible != null)
                {
                    breakout++;
                    continue;
                }

                // no location:  make a new one here
                else
                {
                    var newblock = new Location
                    {
                        dbName = "dungeon_" + intname,
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

            var randomBeforeNoWall = .35;
            maze.Reverse();

            // break out some random walls
            foreach (var loc in maze)
            {
                var breakRoll = rand.NextDouble();

                // eastern neighbor
                if (breakRoll < .25 * randomBeforeNoWall && loc.Name_East == null)
                {
                    var neighbor = maze.FirstOrDefault(l => l.X == loc.X + 1 && l.Y == loc.Y);
                    if (neighbor != null)
                    {
                        loc.Name_East = neighbor.dbName;
                        neighbor.Name_West = loc.dbName;
                    }
                }

                // western neighbor
                else if (breakRoll < .5 * randomBeforeNoWall && loc.Name_West == null)
                {
                    var neighbor = maze.FirstOrDefault(l => l.X == loc.X - 1 && l.Y == loc.Y);
                    if (neighbor != null)
                    {
                        loc.Name_West = neighbor.dbName;
                        neighbor.Name_East = loc.dbName;
                    }
                }

                // northern neighbor
                else if (breakRoll < .75 * randomBeforeNoWall && loc.Name_North == null)
                {
                    var neighbor = maze.FirstOrDefault(l => l.X == loc.X && l.Y == loc.Y + 1);
                    if (neighbor != null)
                    {
                        loc.Name_North = neighbor.dbName;
                        neighbor.Name_South = loc.dbName;
                    }
                }

                // southern neighbor
                else if (breakRoll < 1.0 * randomBeforeNoWall && loc.Name_South == null)
                {
                    var neighbor = maze.FirstOrDefault(l => l.X == loc.X && l.Y == loc.Y - 1);
                    if (neighbor != null)
                    {
                        loc.Name_South = neighbor.dbName;
                        neighbor.Name_North = loc.dbName;
                    }
                }

            }

            // assign names
            foreach (var loc in maze)
            {
                var connectionCount = 0;
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
                var num = rand.NextDouble();
                var adjindex = Convert.ToInt32(Math.Floor(num * maxAdj));
                var adjToUse = adjectives.ElementAt(adjindex);
                loc.Name += adjToUse + " ";
                adjectives.Remove(adjToUse);

                // get random noun
                double maxNoun = nouns.Count();
                num = rand.NextDouble();
                var nounindex = Convert.ToInt32(Math.Floor(num * maxNoun));
                var nounToUse = nouns.ElementAt(nounindex);
                loc.Name += nounToUse;
                nouns.Remove(nounToUse);
            }


         


          

            var old = LocationsStatics.LocationList.GetLocation.ToList();

            // clear out the old dungeon from memory
            old = old.Where(l => l.Region != "dungeon").ToList();
            old = old.Concat(maze).ToList();

            LocationsStatics.LocationList.GetLocation = old;

            return maze;
        }


    }
}