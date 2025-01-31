using System;
using System.Collections.Generic;
using System.Linq;
using KdTree;
using KdTree.Math;

namespace EDDCanonn.Base
{
    public class Patrols
    {
        private readonly int[] maxRanges = { 6, 24, 120, 720, 5040, 40320 };
        public Dictionary<string, KdTree<float, Patrol>> CategoryTrees { get; set; } = new Dictionary<string, KdTree<float, Patrol>>();

        public Patrols()
        {
            CategoryTrees["all"] = new KdTree<float, Patrol>(3, new FloatMath());
        }

        public void Add(string category, KdTree<float, Patrol> tree)
        {
            if (CategoryTrees.ContainsKey(category))
                return;
            CategoryTrees[category] = tree;
        }

        public List<(string category, Patrol patrol, double distance)> FindPatrolsInRange(string category, double x, double y, double z, int rangeIndex)
        {
            List<(string category, Patrol patrol, double distance)> result = new List<(string category, Patrol patrol, double distance)>();

            if (category.ToLower() == "all")
                SearchCategory("all", x, y, z, maxRanges[rangeIndex], result);
            else
                SearchCategory(category, x, y, z, maxRanges[rangeIndex], result);

            return result.OrderBy(t => t.distance).Take(500).ToList();
        }

        private void SearchCategory(string category, double x, double y, double z, double maxDistance, List<(string, Patrol, double)> result)
        {
            if (!CategoryTrees.ContainsKey(category)) return;

            float[] searchPoint = new[] { (float)x, (float)y, (float)z };
            KdTreeNode<float, Patrol>[] nodesInRange = CategoryTrees[category].RadialSearch(searchPoint, (float)maxDistance);

            foreach (KdTreeNode<float, Patrol> node in nodesInRange)
            {
                double distance = CalculateDistance(searchPoint, node.Point);
                result.Add((category, node.Value, distance));
            }
        }

        private double CalculateDistance(float[] point1, float[] point2)
        {
            double sum = 0;
            for (int i = 0; i < point1.Length; i++)
            {
                sum += Math.Pow(point1[i] - point2[i], 2);
            }
            return Math.Sqrt(sum);
        }
    }

    public class Patrol
    {
        public string PatrolType { get; set; }
        public string Type { get; set; }
        public long Id64 { get; set; } = -1;
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string Instructions { get; set; }
        public string Url { get; set; }
    }
}
