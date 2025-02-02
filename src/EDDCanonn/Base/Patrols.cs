using System;
using System.Collections.Generic;
using System.Linq;
using KdTree;
using KdTree.Math;


namespace EDDCanonn.Base
{
    public class Patrols
    {
        public Dictionary<string, KdTree<float, Patrol>> CategoryTrees { get; set; } = new Dictionary<string, KdTree<float, Patrol>>();

        public Patrols()
        {
            CategoryTrees["all"] = new KdTree<float, Patrol>(3, new FloatMath());
        }

        private readonly object _addLock = new object();
        public void Add(string category, KdTree<float, Patrol> tree)
        {
            lock (_addLock)
            {
                if (CategoryTrees.ContainsKey(category))
                    return;
                CategoryTrees[category] = tree;

                foreach (KdTreeNode<float, Patrol> node in tree)
                    CategoryTrees["all"].Add(node.Point, node.Value);
            }
        }

        public List<(string category, Patrol patrol, double distance)> FindPatrolsInRange(string category, double x, double y, double z, double maxDistance)
        {
            List<(string category, Patrol patrol, double distance)> result = new List<(string category, Patrol patrol, double distance)>();

                SearchCategory(category, x, y, z, maxDistance, result);

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
        public Patrol(float x, float y, float z, string instructions, string url, long id64 = -1, string patrolType = null)
        {
            X = x;
            Y = y;
            Z = z;
            Instructions = instructions;
            Url = url;
            Id64 = id64;
            PatrolType = patrolType;
        }

        public string PatrolType { get; set; }
        public long Id64 { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string Instructions { get; set; }
        public string Url { get; set; }
    }
}
