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
        public RenderTarget2D Target { get; private set; }
        public GraphicsDevice GraphicsDevice { get; private set; }

        private readonly List<Trail> _trails;

        public PrimitiveSystem(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            this.Target = (!Main.dedServ) ? new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents) : null;

            _trails = new List<Trail>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(this.Target, Main.screenPosition, Color.White);
            spriteBatch.End();
        }

        public void Render()
        {
            GraphicsDevice.SetRenderTarget(Target);
            GraphicsDevice.Clear(Color.Transparent);

            foreach (var trail in _trails) trail.Draw();

            GraphicsDevice.SetRenderTarget(null);
        }

        public void UpdateTrails()
        {
            foreach (var trail in _trails) trail.Update();
        }

        public Trail CreateTrail(Entity target, int maxPoints, Func<float, float> widthFunc, Func<float, Color> colorFunc)
        {
            Trail trail = new Trail(target, maxPoints, widthFunc, colorFunc);
            _trails.Add(trail);
            return trail;
        }

        public class Trail
        {
            public Entity Target { get; private set; }

            public bool active = true;

            private readonly int _maxPoints;
            private readonly List<Vector2> _points;

            private readonly Func<float, float> _widthFunc; // (progress) => return width
            private readonly Func<float, Color> _colorFunc; // (progress) => return color

            public Trail(Entity target, int maxPoints, Func<float, float> widthFunc, Func<float, Color> colorFunc)
            {
                this.Target = target;

                _maxPoints = maxPoints;
                _points = new List<Vector2>();

                _widthFunc = widthFunc;
            }

            public void Draw()
            {
                
            }

            public void Update()
            {
                if (this.Target == null || !this.Target.active || !active)
                {
                    this.Kill();
                    return;
                }

                _points.Insert(0, Target.Center);
                if (_points.Count >= _maxPoints) _points.Remove(_points.Last());
            }

            public void Kill()
            {
                SPladisonsYoyoMod.Primitives?._trails.Remove(this);
            }
        }
    }
}
