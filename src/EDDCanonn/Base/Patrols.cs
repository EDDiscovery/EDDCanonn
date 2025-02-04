/*
 * Copyright © 2022-2022 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KdTree;
using KdTree.Math;

namespace EDDCanonn.Base
{
    public class Patrols
    {
        public Dictionary<string, KdTree<double, Patrol>> CategoryTrees { get; set; } = new Dictionary<string, KdTree<double, Patrol>>();

        public Patrols()
        {
            CategoryTrees["all"] = new KdTree<double, Patrol>(3, new DoubleMath());
        }

        private readonly object _addLock = new object();
        public void Add(string category, KdTree<double, Patrol> tree)
        {
            lock (_addLock)
            {
                if (CategoryTrees.ContainsKey(category))
                    return;
                CategoryTrees[category] = tree;

                foreach (KdTreeNode<double, Patrol> node in tree)
                    CategoryTrees["all"].Add(node.Point, node.Value);
            }
        }

        public List<(string category, Patrol patrol, double distance)> FindPatrolsInRange(string category, double x, double y, double z, double maxDistance)
        {
            List<(string category, Patrol patrol, double distance)> result = new List<(string category, Patrol patrol, double distance)>();

                SearchCategory(category, x, y, z, maxDistance, result);

            return result.ToList();
        }

        private void SearchCategory(string category, double x, double y, double z, double maxDistance, List<(string, Patrol, double)> result)
        {
            if (!CategoryTrees.ContainsKey(category)) return;

            double[] searchPoint = new[] { x, y, z };
            KdTreeNode<double, Patrol>[] nodesInRange = CategoryTrees[category].RadialSearch(searchPoint, maxDistance);

            foreach (KdTreeNode<double, Patrol> node in nodesInRange)
            {
                double distance = CalculateDistance(searchPoint, node.Point);
                result.Add((category, node.Value, distance));
            }
        }

        private double CalculateDistance(double[] point1, double[] point2)
        {
            double sum = 0;
            for (int i = 0; i < point1.Length; i++)
            {
                sum += Math.Pow(point1[i] - point2[i], 2);
            }
            return Math.Round(Math.Sqrt(sum),3);
        }
    }

    public class Patrol
    {

        public Patrol(string patrolType, string category, double x, double y, double z, string instructions, string url)
        {
            PatrolType = patrolType;
            this.category = category;
            X = x;
            Y = y;
            Z = z;
            Instructions = instructions;
            Url = url;
        }

        public Patrol(string category, double x, double y, double z, string instructions, string url)
        {
            this.category = category;
            X = x;
            Y = y;
            Z = z;
            Instructions = instructions;
            Url = url;
        }

        public string PatrolType { get; set; }
        public string category {  get; set; }
        public long Id64 { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string Instructions { get; set; }
        public string Url { get; set; }
    }
}
