using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using Terraria;

namespace SPladisonsYoyoMod.Content.Trails
{
    public class TriangularTrail : SimpleTrail
    {
        protected int _tipLength;

        public TriangularTrail(Entity target, int length, WidthDelegate width, ColorDelegate color, Asset<Effect> effect = null, int? tipLength = null) : base(target, length, width, color, effect)
        {
            _tipLength = tipLength ?? -1;
        }

        protected override int ExtraTrianglesCount => 1;
        protected override bool NormalFlip => true;

        protected override void CreateTipMesh(Vector2 normal, float width, Color color)
        {
            AddVertex(_points[0] - Vector2.Normalize(normal) * (_tipLength > 0 ? _tipLength : width * 0.5f), color, new Vector2(0, 0.5f));
        }
    }
}