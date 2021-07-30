using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SPladisonsYoyoMod.Content.Trails
{
    public class RetrowaveTrail : SimpleTrail
    {
        protected int _tipLength;

        public RetrowaveTrail(int length, WidthDelegate width, ColorDelegate color, Asset<Effect> effect = null) : base(length, width, color, effect) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_points.Count <= 1) return;
            _vertices.Clear();

            float progress = 0f;
            float currentWidth = _width?.Invoke(progress) ?? 0;
            Color currentColor = (_color?.Invoke(progress) ?? Color.White) * _dissolveProgress;

            Vector2 normal = (_points[1] - _points[0]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * currentWidth / 2f;

            this.AddVertex(_points[0] + normal, currentColor, new Vector2(0, 0));
            this.AddVertex(_points[0] - normal, currentColor, new Vector2(0, 1));

            for (int i = 1; i < _points.Count; i++)
            {
                progress += Vector2.Distance(_points[i], _points[i - 1]) / this.Length;

                currentWidth = _width?.Invoke(progress) ?? 0;
                currentColor = (_color?.Invoke(progress) ?? Color.White) * _dissolveProgress;
                normal = (_points[i] - _points[i - 1]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * currentWidth / 2f;

                this.AddVertex(_points[i] + normal, currentColor, new Vector2(i % 3 / 4f, 0));
                this.AddVertex(_points[i] - normal, currentColor, new Vector2(i % 3 / 4f, 1));
            }

            _effect.Value.Parameters["transformMatrix"].SetValue(Primitives.GetTransformMatrix());

            var graphics = Main.instance.GraphicsDevice;
            foreach (var pass in _effect.Value.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, _vertices.ToArray(), 0, (_points.Count - 1) * 2);
            }
        }
    }
}
