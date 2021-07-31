using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using Terraria;
using Terraria.GameContent;

namespace SPladisonsYoyoMod.Content.Trails
{
    public class SimpleTrail : Primitives.Trail
    {
        protected readonly WidthDelegate _width;
        protected readonly ColorDelegate _color;

        public SimpleTrail(Entity target, int length, WidthDelegate width, ColorDelegate color, Asset<Effect> effect = null) : base(target, length, effect)
        {
            _width = width;
            _color = color;
        }

        protected override void CreateMesh()
        {
            float progress = 0f;
            float currentWidth = _width?.Invoke(progress) ?? 0;
            Color currentColor = (_color?.Invoke(progress) ?? Color.White) * _dissolveProgress;

            Vector2 normal = (_points[1] - _points[0]).SafeNormalize(Vector2.Zero) * currentWidth / 2f;
            CreateTipMesh(normal, currentWidth, currentColor);
            normal = normal.RotatedBy(MathHelper.PiOver2) * (NormalFlip ? -1f : 1f);

            AddVertex(_points[0] + normal, currentColor, new Vector2(progress, 0));
            AddVertex(_points[0] - normal, currentColor, new Vector2(progress, 1));

            for (int i = 1; i < _points.Count; i++)
            {
                progress += Vector2.Distance(_points[i], _points[i - 1]) / this.Length;

                currentWidth = _width?.Invoke(progress) ?? 0;
                currentColor = (_color?.Invoke(progress) ?? Color.White) * _dissolveProgress;
                normal = (_points[i] - _points[i - 1]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * currentWidth / 2f * (NormalFlip ? -1f : 1f);

                AddVertex(_points[i] + normal, currentColor, new Vector2(progress, 0));
                AddVertex(_points[i] - normal, currentColor, new Vector2(progress, 1));
            }
        }

        protected virtual bool NormalFlip => false;
        protected virtual void CreateTipMesh(Vector2 normal, float width, Color color) { }

        public delegate float WidthDelegate(float progress);
        public delegate Color ColorDelegate(float progress);
    }
}
