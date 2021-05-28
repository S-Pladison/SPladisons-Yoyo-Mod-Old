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

        private List<Trail> _trails;

        public PrimitiveSystem(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;

            _trails = new List<Trail>();
        }

        public void Draw()
        {
            foreach (var trail in _trails) trail.Draw();
        }

        public Trail CreateTrail(Entity target) // Лучше сделать target не обязательным, чтобы можно было устанавливать кастомные позиции трейла
        {
            Trail trail = new Trail(target);
            _trails.Add(trail);
            return trail;
        }

        public class Trail
        {
            public Entity Target { get; private set; }

            public Trail(Entity target)
            {
                this.Target = target;
            }

            public void Draw()
            {

            }
        }
    }
}
