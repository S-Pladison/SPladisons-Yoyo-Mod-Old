using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Particles
{
    public abstract class Particle : ModTexturedType
    {
        public static Particle NewParticle<T>(Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f) where T : Particle
        {
            int type = ParticleSystem.ParticleType<T>();
            return NewParticle(type, position, velocity, color, alpha, rotation, scale);
        }

        public static Particle NewParticle(int type, Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f)
        {
            if (Main.dedServ) return null;

            var particleInstance = ParticleSystem.GetParticleInstance(type);
            if (particleInstance == null) return null;

            if (ParticleSystem.ActiveParticles == ParticleSystem.MaxParticles)
            {
                ParticleSystem.particles.RemoveAt(0);
            }

            var newParticle = (Particle)Activator.CreateInstance(particleInstance.GetType());
            newParticle.position = position;
            newParticle.velocity = velocity ?? Vector2.Zero;
            newParticle.color = color ?? Color.White;
            newParticle.alpha = alpha;
            newParticle.rotation = rotation;
            newParticle.scale = scale;
            newParticle.Type = type;
            newParticle.LoadTexture();
            newParticle.OnSpawn();

            ParticleSystem.AddParticle(newParticle);
            return newParticle;
        }

        // ...

        public int Type { get; private set; }
        public Asset<Texture2D> Texture2D { get; private set; }

        public Color color;
        public Vector2 position;
        public Vector2 oldPosition;
        public Vector2 velocity;

        public float rotation;
        public float scale;

        public int alpha;
        public int timeLeft;
        public int frame;
        public int frameCount = 1;

        protected Particle()
        {

        }

        public void Kill()
        {
            this.OnKill();
            ParticleSystem.RemoveParticle(this);
        }

        public void Draw()
        {
            var lightColor = Lighting.GetColor((int)(position.X + 4.0) / 16, (int)(position.Y + 4.0) / 16);
            var scaleMult = 1f;

            if (PreDraw(ref lightColor, ref scaleMult))
            {
                var height = (int)(Texture2D.Height() / frameCount);
                var rect = new Rectangle(0, height * frame, Texture2D.Width(), height);
                var origin = rect.Size() * 0.5f;
                var alphaColor = GetAlpha(lightColor);
                Main.spriteBatch.Draw(Texture2D.Value, position - Main.screenPosition, rect, GetColor(alphaColor), rotation, origin, scale * scaleMult, SpriteEffects.None, 0f);
            }
            PostDraw(lightColor, scaleMult);
        }

        public Color GetColor(Color newColor)
        {
            int num = (int)(color.R - (byte.MaxValue - newColor.R));
            int num2 = (int)(color.G - (byte.MaxValue - newColor.G));
            int num3 = (int)(color.B - (byte.MaxValue - newColor.B));
            int num4 = (int)(color.A - (byte.MaxValue - newColor.A));

            num = Math.Clamp(num, 0, 255);
            num2 = Math.Clamp(num2, 0, 255);
            num3 = Math.Clamp(num3, 0, 255);
            num4 = Math.Clamp(num4, 0, 255);

            return new Color(num, num2, num3, num4);
        }

        public virtual Color GetAlpha(Color lightColor)
        {
            float num = (255 - alpha) / 255f;
            return new Color(lightColor.R * num, lightColor.G * num, lightColor.B * num, lightColor.A - alpha);
        }

        public virtual void OnSpawn()
        {

        }

        public virtual void Update()
        {
            oldPosition = position;
            position += velocity;
            velocity *= 0.975f;
            scale *= 0.975f;

            if (--timeLeft <= 0 || scale <= 0.1f)
            {
                Kill();
            }
        }

        protected virtual void OnKill()
        {

        }

        protected virtual void PostDraw(Color lightColor, float scaleMult)
        {

        }

        protected virtual bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            return true;
        }

        protected sealed override void Register()
        {
            ModTypeLookup<Particle>.Register(this);

            Type = ParticleSystem.particleInstances.Count;
            LoadTexture();

            ParticleSystem.particleInstances.Add(Type, this);
        }

        protected void LoadTexture()
        {
            Texture2D ??= (!string.IsNullOrEmpty(this.Texture)) ? ModContent.Request<Texture2D>(this.Texture, AssetRequestMode.ImmediateLoad) : TextureAssets.MagicPixel;
        }
    }
}