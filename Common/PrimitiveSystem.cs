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

        private List<Trail> _trails;

        public PrimitiveSystem(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            this.Target = (!Main.dedServ) ? new RenderTarget2D(graphicsDevice, Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents) : null;

            _trails = new List<Trail>();
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

        public Trail CreateTrail(Entity target)
        {
            Trail trail = new Trail(target);
            _trails.Add(trail);
            return trail;
        }

        public class Trail
        {
            public Entity Target { get; private set; }

            private List<Vector2> _points;

            public Trail(Entity target)
            {
                this.Target = target;

                _points = new List<Vector2>();
            }

            public void Draw()
            {

            }

            public void Update()
            {

            }

            public void Kill()
            {

            }
        }
    }
}
