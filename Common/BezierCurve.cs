using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPladisonsYoyoMod.Common
{
    public class BezierCurve
    {
        private static readonly List<float> factorialList = new() { 1f };

        private List<Vector2> points;

        // ...

        public BezierCurve(params Vector2[] points)
        {
            this.points = new List<Vector2>(points);
        }

        public BezierCurve(IEnumerable<Vector2> points)
        {
            this.points = new List<Vector2>(points);
        }

        public Vector2 GetPoint(float t) => BezierCurve.GetPoint(t, points);
        public List<Vector2> GetPoints(int amount) => BezierCurve.GetPoints(amount, points);

        // ...

        public static Vector2 GetPoint(float t, params Vector2[] points)
            => GetPoint(t, points.ToList());

        public static Vector2 GetPoint(float t, IEnumerable<Vector2> points)
            => GetPoint(t, points.ToList());

        public static Vector2 GetPoint(float t, List<Vector2> points)
        {
            var count = points.Count - 1;

            if (t <= 0) return points[0];
            if (t >= 1) return points[count];

            var point = Vector2.Zero;

            for (int i = 0; i < points.Count; i++)
            {
                point += Bernstein(count, i, t) * points[i];
            }

            return point;
        }

        public static List<Vector2> GetPoints(int amount, params Vector2[] points)
            => GetPoints(amount, points.ToList());

        public static List<Vector2> GetPoints(int amount, IEnumerable<Vector2> points)
            => GetPoints(amount, points.ToList());

        public static List<Vector2> GetPoints(int amount, List<Vector2> points)
        {
            var count = points.Count - 1;
            var result = new List<Vector2>();
            var perStep = 1f / (amount - 1);

            for (int i = 0; i < amount; i++)
            {
                var point = Vector2.Zero;

                for (int j = 0; j < points.Count; j++)
                {
                    point += Bernstein(count, j, perStep * i) * points[j];
                }

                result.Add(point);
            }

            return result;
        }

        // ...

        private static float Bernstein(int n, int i, float t)
            => Factorial(n) / (Factorial(i) * Factorial(n - i)) * MathF.Pow(t, i) * MathF.Pow((1 - t), (n - i));

        private static float Factorial(int n)
        {
            n = Math.Max(n, 0);

            if (n < factorialList.Count) return factorialList[n];

            for (int i = factorialList.Count; i <= n; i++)
            {
                factorialList.Add(factorialList.Last() * i);
            }

            return factorialList.Last();
        }
    }
}
