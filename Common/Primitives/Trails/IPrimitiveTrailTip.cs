using Microsoft.Xna.Framework;
using Terraria;

namespace SPladisonsYoyoMod.Common.Primitives.Trails
{
    public interface IPrimitiveTrailTip
    {
        int ExtraTriangles { get; }

        void CreateTipMesh(PrimitiveTrail trail, Vector2 position, Vector2 normal, Color color);
    }

    public class WithoutTrailTip : IPrimitiveTrailTip
    {
        int IPrimitiveTrailTip.ExtraTriangles => 0;

        void IPrimitiveTrailTip.CreateTipMesh(PrimitiveTrail trail, Vector2 position, Vector2 normal, Color color) { }
    }

    public class TriangularTrailTip : IPrimitiveTrailTip
    {
        protected float tipLength;

        public TriangularTrailTip(float? length = null)
        {
            tipLength = length ?? -1;
        }

        int IPrimitiveTrailTip.ExtraTriangles => 1;

        void IPrimitiveTrailTip.CreateTipMesh(PrimitiveTrail trail, Vector2 position, Vector2 normal, Color color)
        {
            float length = tipLength >= 0 ? tipLength : normal.Length();

            trail.AddVertex(position - Vector2.Normalize(normal).RotatedBy(-MathHelper.PiOver2) * length, color, new Vector2(0, 0.5f));
            trail.AddVertex(position - normal, color, new Vector2(0, 0));
            trail.AddVertex(position + normal, color, new Vector2(0, 1));
        }
    }

    public class RoundedTrailTip : IPrimitiveTrailTip
    {
        protected uint tipSmoothness;

        public RoundedTrailTip(uint smoothness = 10)
        {
            tipSmoothness = smoothness;
        }

        int IPrimitiveTrailTip.ExtraTriangles => (int)tipSmoothness;

        void IPrimitiveTrailTip.CreateTipMesh(PrimitiveTrail trail, Vector2 position, Vector2 normal, Color color)
        {
            float angle = MathHelper.Pi / tipSmoothness;
            float currentAngle = 0;

            for (int i = 0; i < tipSmoothness; i++)
            {
                trail.AddVertex(position, color, new Vector2(0.5f, 0.5f));
                trail.AddVertex(position + normal.RotatedBy(currentAngle), color, new Vector2(1, 1));

                currentAngle += angle;

                trail.AddVertex(position + normal.RotatedBy(currentAngle), color, new Vector2(0, 1));
            }
        }
    }
}