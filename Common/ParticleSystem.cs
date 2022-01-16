using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common
{
    public class ParticleSystem : ModSystem
    {
        internal static readonly List<Particle> Particles = new List<Particle>();

        // ...

        public override void PostUpdateDusts()
        {
            foreach (var particle in Particles.ToArray()) particle.Update();
        }

        public static void NewParticle(Asset<Texture2D> texture, int timeLeft, Vector2 position, Vector2? velocity = null, float rotation = 0f, float scale = 1f)
        {
            Particle particle = new Particle(texture, timeLeft, position, velocity, rotation, scale);
            ParticleSystem.NewParticle(particle);
        }

        public static void NewParticle(Particle particle)
        {
            Particles.Add(particle);
        }

        // ...

        public class Particle
        {
            public Asset<Texture2D> Texture { get; protected set; }

            public Vector2 position;
            public Vector2 oldPosition;
            public Vector2 velocity;

            public float rotation;
            public float scale;

            public int timeLeft;
            public int frame;
            public int frameCount = 1;

            public Particle(Asset<Texture2D> texture, int timeLeft, Vector2 position, Vector2? velocity = null, float rotation = 0f, float scale = 1f)
            {
                this.Texture = texture;

                this.timeLeft = timeLeft;
                this.position = position;
                this.velocity = velocity ?? Vector2.Zero;
                this.rotation = rotation;
                this.scale = scale;
            }

            public virtual void Update()
            {
                oldPosition = position;
                position += velocity;

                velocity *= 0.975f;
                scale *= 0.975f;

                if (--timeLeft <= 0 || scale <= 0.1f) this.Kill();
            }

            public virtual void Draw(SpriteBatch spriteBatch)
            {
                var rect = new Rectangle((int)Main.screenPosition.X - 25, (int)Main.screenPosition.Y - 25, Main.screenWidth + 25, Main.screenHeight + 25);
                if (!rect.Contains((int)position.X, (int)position.Y)) return;

                var height = (int)(Texture.Height() / frameCount);
                spriteBatch.Draw(Texture.Value, position - Main.screenPosition, new Rectangle(0, height * frame, Texture.Width(), height), Color.White * 0.95f, rotation, new Vector2(Texture.Width(), height) * 0.5f, scale, SpriteEffects.None, 0f);
            }

            public void Kill()
            {
                Particles.Remove(this);
            }
        }
    }
}
