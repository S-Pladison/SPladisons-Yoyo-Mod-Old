using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Drawing.Primitives;
using System.Linq;
using Terraria;

namespace SPladisonsYoyoMod.Utilities
{
    public static class PrimitiveUtils
    {
        public static void UpdatePointsAsSimpleTrail(this PrimitiveStrip strip, Vector2 currentPosition, uint maxPoints, float? maxLength = null)
        {
            if (Main.gamePaused) return;

            var points = strip.Points;

            if (points.Any() && currentPosition == points.First()) return;

            points.Insert(0, currentPosition);

            if (points.Count > maxPoints) points.Remove(points.Last());
            if (points.Count <= 1 || maxLength == null) return;

            var length = 0f;
            var lastIndex = -1;

            for (int i = 1; i < points.Count; i++)
            {
                float dist = Vector2.Distance(points[i], points[i - 1]);
                length += dist;

                if (length > maxLength)
                {
                    lastIndex = i;
                    length -= dist;
                    break;
                }
            }

            if (lastIndex < 0) return;

            var vector = Vector2.Normalize(points[lastIndex] - points[lastIndex - 1]) * (maxLength - length) ?? Vector2.Zero;
            points.RemoveRange(lastIndex, points.Count - lastIndex);
            points.Add(points[lastIndex - 1] + vector);
        }
    }
}
