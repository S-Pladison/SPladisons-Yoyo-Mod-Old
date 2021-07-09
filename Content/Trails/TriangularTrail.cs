using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPladisonsYoyoMod.Content.Trails
{
    public class TriangularTrail : SimpleTrail
    {
        protected int _tipLength;

        protected override int ExtraTriangleCount => 1;

        public TriangularTrail(int length, WidthDelegate width, ColorDelegate color, Effect effect = null, int? tipLength = null) : base(length, width, color, effect)
        {
            _tipLength = tipLength ?? -1;
        }

        protected override void DrawHead(float width, Color color, Vector2 normal)
        {
            Vector2 vector = _tipLength == -1 ? normal : (Vector2.Normalize(normal) * _tipLength);
            this.AddVertex(_points[0] + vector, color, new Vector2(0, 0.5f));
        }
    }
}
