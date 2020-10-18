using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{

    public class LocationNode
    {
        public string dbName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string NorthNode { get; set; }
        public string EastNode { get; set; }
        public string SouthNode { get; set; }
        public string WestNode { get; set; }

        public string ParentNode { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int Fx { get; set; }

        public void CalculateFx()
        {
            this.Fx = this.G + this.H;
        }

        public bool IsOnClosedList(List<LocationNode> closedList)
        {
            var x = closedList.Count(l => l.dbName == dbName);
            return x == 1;
        }

        public bool IsOnOpenList(List<LocationNode> openList)
        {
            var x = openList.Count(l => l.dbName == dbName);
            return x == 1;
        }

        public void MoveToOpenList(List<LocationNode> openList)
        {
            // assert that it's not already in the open list
            var possibleOld = openList.FirstOrDefault(n => n.dbName == this.dbName);
            // it isn't in the list yet, so add it in.  Otherwise do nothing.
            if (possibleOld == null)
            {
                openList.Add(this);
            }
        }

        public void MoveToClosedList(List<LocationNode> openList, List<LocationNode> closedList)
        {
          //  openList = openList.Where(n => n.dbName != this.dbName).ToList();
            closedList.Add(this);
            var toRemove = openList.FirstOrDefault(l => l.dbName == this.dbName);
            openList.Remove(toRemove);
        }

        public bool IsNextToFinish(string finish)
        {
            if (this.NorthNode == finish ||
            this.EastNode == finish ||
            this.SouthNode == finish ||
            this.WestNode == finish)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CalculateH(LocationNode finish)
        {
            this.H = Math.Abs(this.X - finish.X) + Math.Abs(this.Y - finish.Y);
        }
    }

    public static class PathfindingProcedures
    {
        private static List<string> CalculatePath(Location start, Location end)
        {
            if (start == null || end == null || start.dbName == end.dbName)
            {
                return new List<string>();
            }

            var nodes = new List<LocationNode>();

            // populate the location nodes
            foreach (var loc in LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != ""))
            {
                var addMe = new LocationNode
                {
                    dbName = loc.dbName,
                    NorthNode = loc.Name_North,
                    EastNode = loc.Name_East,
                    SouthNode = loc.Name_South,
                    WestNode = loc.Name_West,
                    X = loc.X,
                    Y = loc.Y,
                    H = -1,
                };
                nodes.Add(addMe);
            }

            // we may want to filter out certain nodes based on other criteria, but we're not doing that now.

            var startingNode = nodes.FirstOrDefault(n => n.dbName == start.dbName);
            var endingNode = nodes.FirstOrDefault(n => n.dbName == end.dbName);
            var openList = new List<LocationNode>();
            var closedList = new List<LocationNode>();

            startingNode.CalculateH(endingNode);

            // add the first set of neighbors to the open list
            //LocationNode northNode = nodes.FirstOrDefault(n => n.dbName == startingNode.NorthNode);
            //LocationNode eastNode = nodes.FirstOrDefault(n => n.dbName == startingNode.EastNode);
            //LocationNode southNode = nodes.FirstOrDefault(n => n.dbName == startingNode.SouthNode);
            //LocationNode westNode = nodes.FirstOrDefault(n => n.dbName == startingNode.WestNode);

            //if (northNode != null)
            //{
            //    northNode.MoveToOpenList(openList);
            //}
            //if (eastNode != null)
            //{
            //    eastNode.MoveToOpenList(openList);
            //}
            //if (southNode != null)
            //{
            //    southNode.MoveToOpenList(openList);
            //}
            //if (westNode != null)
            //{
            //    westNode.MoveToOpenList(openList);
            //}



          //  bool done = Search(startingNode, nodes, openList, closedList, endingNode);
            var breakoutMax = 1000;
            var breakoutCurrent = 0;
            var done = false;

            while (!done && breakoutCurrent < breakoutMax)
            {

                LocationNode nextNode;
                if (breakoutCurrent == 0) {
                    nextNode = startingNode;
                } else {
                    nextNode = openList.OrderBy(n => n.Fx).First();
                }

                done = Search(nextNode, nodes, openList, closedList, endingNode);
                breakoutCurrent++;
            }

            // we should now have the shortest path, so get a list of the nodes we went through and turn it to a big string.
            var next = end.dbName;
            var pathList = new List<string>();

            while (next != start.dbName)
            {
                pathList.Add(next);
                var current = closedList.FirstOrDefault(n => n.dbName == next);
                var parent = closedList.FirstOrDefault(n => n.dbName == current.ParentNode);
                next = parent.dbName;
            }

            return pathList;
        }

        public static List<string> GetMovementPath(Location start, Location end)
        {
            var pathList = CalculatePath(start, end);
            pathList.Reverse();
            return pathList;
        }

        public static int GetNumSteps(Location start, Location end)
        {
            return CalculatePath(start, end).Count;
        }

        public static bool Search(LocationNode currentNode, List<LocationNode> nodes, List<LocationNode> openList, List<LocationNode> closedList, LocationNode finish)
        {

            // move this node to the closed list
            currentNode.MoveToClosedList(openList, closedList);

            if (currentNode.IsNextToFinish(finish.dbName))
            {
                finish.ParentNode = currentNode.dbName;
                finish.MoveToClosedList(openList, closedList);
                return true;
            }

            // get the eligible neighboring nodes
            var Neighbors = new List<LocationNode>();
            var northNode = nodes.FirstOrDefault(n => n.dbName == currentNode.NorthNode);
            var eastNode = nodes.FirstOrDefault(n => n.dbName == currentNode.EastNode);
            var southNode = nodes.FirstOrDefault(n => n.dbName == currentNode.SouthNode);
            var westNode = nodes.FirstOrDefault(n => n.dbName == currentNode.WestNode);

            if (northNode != null && !northNode.IsOnClosedList(closedList))
            {
                Neighbors.Add(northNode);
                if (northNode.ParentNode.IsNullOrEmpty())
                {
                    northNode.ParentNode = currentNode.dbName;
                }
            }
            if (eastNode != null && !eastNode.IsOnClosedList(closedList))
            {
                Neighbors.Add(eastNode);
                if (eastNode.ParentNode.IsNullOrEmpty())
                {
                    eastNode.ParentNode = currentNode.dbName;
                }
            }
            if (southNode != null && !southNode.IsOnClosedList(closedList))
            {
                Neighbors.Add(southNode);
                if (southNode.ParentNode.IsNullOrEmpty())
                {
                    southNode.ParentNode = currentNode.dbName;
                }
            }
            if (westNode != null && !westNode.IsOnClosedList(closedList))
            {
                Neighbors.Add(westNode);
                if (westNode.ParentNode.IsNullOrEmpty())
                {
                    westNode.ParentNode = currentNode.dbName;
                }
            }

            // now compute the f(x) and change the parent if needed
            foreach (var neighbor in Neighbors)
            {

                if (neighbor.H == -1)
                {
                    neighbor.CalculateH(finish);
                }

                // if G of the current node to its neighbor plus travel cost is less than neighbor's G
                // then change the neighbor's parent.
                if (neighbor.IsOnOpenList(openList))
                {
                    if (currentNode.G + 1 < neighbor.G)
                    {
                        neighbor.ParentNode = currentNode.dbName;
                    }
                }

                // otherwise set neighbor's G to the total movement cost and calculate its new Fx
                else
                {
                    neighbor.MoveToOpenList(openList);
                    neighbor.G = currentNode.G + 1;
                    
                }
                neighbor.CalculateFx();

            }

            return false;

        }

    }

}


