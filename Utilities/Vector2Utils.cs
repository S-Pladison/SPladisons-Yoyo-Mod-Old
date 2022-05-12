using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace SPladisonsYoyoMod.Utilities
{
    public static class Vector2Utils
    {
        public static float TotalDistance(this IEnumerable<Vector2> points)
        {
            if (!points.Any()) return 0f;

            float length = 0f;

            for (int i = 1; i < points.Count(); i++)
            {
                var j = i - 1;
                length += Vector2.Distance(points.ElementAt(j), points.ElementAt(i));
            }

            return length;
        }
    }
}