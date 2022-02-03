using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var particle = new Particle(texture, position, velocity)
            {
                timeLeft = timeLeft,
                rotation = rotation,
                scale = scale
            };
            ParticleSystem.NewParticle(particle);
        }

        public static void NewParticle(Particle particle)
        {
            if (Main.dedServ) return;

            Particles.Add(particle);
            particle.OnSpawn();
        }
    }

    public class Particle
    {
        public Asset<Texture2D> Texture { get; protected set; }

        public Color color;

        public Vector2 position;
        public Vector2 oldPosition;
        public Vector2 velocity;

        public float rotation;
        public float scale;

        public int timeLeft;
        public int frame;
        public int frameCount = 1;

        // ...

        public Particle(Asset<Texture2D> texture, Vector2 position, Vector2? velocity = null)
        {
            this.Texture = texture;

            this.color = Color.White * 0.95f;
            this.timeLeft = 60 * 3;
            this.position = position;
            this.velocity = velocity ?? Vector2.Zero;
            this.rotation = 0f;
            this.scale = 1f;
        }

        // ...

        public virtual void OnSpawn() { }

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
            spriteBatch.Draw(Texture.Value, position - Main.screenPosition, new Rectangle(0, height * frame, Texture.Width(), height), color, rotation, new Vector2(Texture.Width(), height) * 0.5f, scale, SpriteEffects.None, 0f);
        }

        protected virtual bool PreKill() { return true; }

        // ...

        public void Kill()
        {
            if (this.PreKill())
            {
                // ...
            }

            ParticleSystem.Particles.Remove(this);
        }
    }
}
