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
    public class Primitives
    {
        public GraphicsDevice GraphicsDevice { get; }

        public Primitives(GraphicsDevice graphicsDevice)
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

        private readonly List<Trail> _trails;

        public class Trail
        {
            public Entity Target { get; private set; }
            public bool Pixelated { get; private set; }

            public bool active = true;

            private Vector2? _customPosition;

            private readonly float _maxLength;
            private readonly Effect _effect;
            private readonly List<Vector2> _points;

            private readonly TrailWidthDelegate _width;
            private readonly TrailColorDelegate _color;

            public Trail(Entity target, float length, TrailWidthDelegate width, TrailColorDelegate color, Effect effect = null, bool pixelated = false)
            {
                this.Target = target;

                _maxLength = length;
                _effect = effect;
                _width = width;
                _color = color;

                _points = new List<Vector2>();
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

            public void SetCustomPosition(Vector2? position) => _customPosition = position;
        }

        public delegate float TrailWidthDelegate(float progress);
        public delegate Color TrailColorDelegate(float progress);

        public Trail CreateTrail(Entity target, float length, TrailWidthDelegate width, TrailColorDelegate color) => this.CreateTrail(target, length, width, color, null);
        public Trail CreateTrail(Entity target, float length, TrailWidthDelegate width, TrailColorDelegate color, Effect effect)
        {
            Trail trail = new (target, length, width, color, effect, false);
            _trails.Add(trail);
            return trail;
        }

        public Trail CreatePixelatedTrail(Entity target, float length, TrailWidthDelegate width, TrailColorDelegate color) => this.CreatePixelatedTrail(target, length, width, color, null);
        public Trail CreatePixelatedTrail(Entity target, float length, TrailWidthDelegate width, TrailColorDelegate color, Effect effect)
        {
            Trail trail = new(target, length, width, color, effect, true);
            _trails.Add(trail);
            return trail;
        }
    }
}
