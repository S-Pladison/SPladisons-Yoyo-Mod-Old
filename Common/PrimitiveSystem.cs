using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SPladisonsYoyoMod.Common
{
    public class PrimitiveSystem
    {
        public GraphicsDevice GraphicsDevice { get; private set; }

        private readonly List<Trail> _trails;

        public PrimitiveSystem(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;

            _trails = new List<Trail>();
        }

        public void DrawTrails(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (var trail in _trails) trail.Draw();
            spriteBatch.End();
        }

        public void UpdateTrails()
        {
            foreach (var trail in _trails) trail.Update();
        }

        public Trail CreateTrail(Entity target, float length, Func<float, float> widthFunc, Func<float, Color> colorFunc)
        {
            return this.CreateTrail(target, length, widthFunc, colorFunc, null);
        }

        public Trail CreateTrail(Entity target, float length, Func<float, float> widthFunc, Func<float, Color> colorFunc, Effect effect)
        {
            Trail trail = new Trail(target, length, widthFunc, colorFunc, effect);
            _trails.Add(trail);
            return trail;
        }

        public class Trail
        {
            public Entity Target { get; private set; }

            public bool active = true;

            private readonly float _maxLength;
            private readonly List<Vector2> _points;

            private readonly Func<float, float> _widthFunc; // (progress) => return width;
            private readonly Func<float, Color> _colorFunc; // (progress) => return color;

            private readonly Effect _effect;

            public Trail(Entity target, float length, Func<float, float> widthFunc, Func<float, Color> colorFunc, Effect effect = null)
            {
                this.Target = target;

                _maxLength = length;
                _points = new List<Vector2>();

                _widthFunc = widthFunc;
                _colorFunc = colorFunc;

                _effect = effect;
            }

            public void Draw()
            {
                if (!active || _points.Count <= 1) return;

                // ...
            }

            public void Update()
            {
                if (this.Target == null || !this.Target.active || !active)
                {
                    this.Kill();
                    return;
                }

                // ...
            }

            public void Kill()
            {
                SPladisonsYoyoMod.Primitives?._trails.Remove(this);
            }
        }
    }
}
