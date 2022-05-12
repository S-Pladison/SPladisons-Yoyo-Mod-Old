using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Drawing.Particles
{
    public abstract class Particle : ModTexturedType
    {
        public static Particle NewParticle<T>(Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f) where T : Particle
            => NewParticle<T>(DrawLayers.OverDusts, DrawTypeFlags.None, position, velocity, color, alpha, rotation, scale);

        public static Particle NewParticle<T>(DrawLayers layer, DrawTypeFlags flags, Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f) where T : Particle
            => NewParticle<T>(new DrawKey(layer, flags), position, velocity, color, alpha, rotation, scale);

        public static Particle NewParticle<T>(DrawKey drawKey, Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f) where T : Particle
            => NewParticle(drawKey, ParticleSystem.ParticleType<T>(), position, velocity, color, alpha, rotation, scale);

        public static Particle NewParticle(int type, Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f)
            => NewParticle(DrawLayers.OverDusts, DrawTypeFlags.None, type, position, velocity, color, alpha, rotation, scale);

        public static Particle NewParticle(DrawLayers layer, DrawTypeFlags flags, int type, Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f)
            => NewParticle(new DrawKey(layer, flags), type, position, velocity, color, alpha, rotation, scale);

        public static Particle NewParticle(DrawKey drawKey, int type, Vector2 position, Vector2? velocity = null, Color? color = null, int alpha = 0, float rotation = 0f, float scale = 1f)
        {
            if (Main.dedServ) return null;

            var particleInstance = ParticleSystem.GetParticleInstance(type);

            if (particleInstance == null) return null;

            var newParticle = (Particle)Activator.CreateInstance(particleInstance.GetType());
            newParticle.Position = position;
            newParticle.Velocity = velocity ?? Vector2.Zero;
            newParticle.Color = color ?? Color.White;
            newParticle.Alpha = alpha;
            newParticle.Rotation = rotation;
            newParticle.Scale = scale;

            newParticle.DrawKey = drawKey;
            newParticle.Type = particleInstance.Type;
            newParticle.Texture2D = particleInstance.Texture2D;
            newParticle.OnSpawn();

            ParticleSystem.particles[drawKey].Add(newParticle);

            return newParticle;
        }

        // ...

        public int Type { get; private set; }
        public Asset<Texture2D> Texture2D { get; private set; }
        public DrawKey DrawKey { get; private set; }

        public Color Color;
        public Vector2 Position;
        public Vector2 OldPosition;
        public Vector2 Velocity;

        public float Rotation;
        public float Scale = 1f;

        public int Alpha;
        public int TimeLeft = 60;
        public int Frame;
        public int FrameCount = 1;

        // ...

        public virtual void OnSpawn() { }
        public virtual void OnKill() { }

        public void Update()
        {
            float minScaleForDeath = 0f;

            if (PreUpdate(ref minScaleForDeath))
            {
                OldPosition = Position;
                Position += Velocity;
                Velocity *= 0.975f;
                Scale *= 0.975f;
            }
            PostUpdate(minScaleForDeath);

            if (--TimeLeft <= 0 || Scale <= minScaleForDeath)
            {
                Kill();
            }
        }

        public virtual void PostUpdate(float minDeathScale) { }
        public virtual bool PreUpdate(ref float minDeathScale) => true;

        public Color GetColor(Color newColor)
        {
            int num = Color.R - (byte.MaxValue - newColor.R);
            int num2 = Color.G - (byte.MaxValue - newColor.G);
            int num3 = Color.B - (byte.MaxValue - newColor.B);
            int num4 = Color.A - (byte.MaxValue - newColor.A);

            num = Math.Clamp(num, 0, 255);
            num2 = Math.Clamp(num2, 0, 255);
            num3 = Math.Clamp(num3, 0, 255);
            num4 = Math.Clamp(num4, 0, 255);

            return new Color(num, num2, num3, num4);
        }

        public virtual Color GetAlpha(Color lightColor)
        {
            float num = (255 - Alpha) / 255f;

            return new(lightColor.R * num, lightColor.G * num, lightColor.B * num, lightColor.A - Alpha);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            var lightColor = Lighting.GetColor((int)(Position.X + 4.0) / 16, (int)(Position.Y + 4.0) / 16);
            var scaleMult = 1f;

            if (PreDraw(ref lightColor, ref scaleMult))
            {
                var height = Texture2D.Height() / FrameCount;
                var rect = new Rectangle(0, height * Frame, Texture2D.Width(), height);
                var origin = rect.Size() * 0.5f;
                var alphaColor = GetAlpha(lightColor);
                spritebatch.Draw(Texture2D.Value, Position - Main.screenPosition, rect, GetColor(alphaColor), Rotation, origin, Scale * scaleMult, SpriteEffects.None, 0f);
            }
            PostDraw(lightColor, scaleMult);
        }

        public virtual void PostDraw(Color lightColor, float scaleMult) { }
        public virtual bool PreDraw(ref Color lightColor, ref float scaleMult) => true;

        public void Kill()
        {
            OnKill();
            ParticleSystem.particles[DrawKey].Remove(this);
        }

        // ...

        protected sealed override void Register()
        {
            ModTypeLookup<Particle>.Register(this);

            Type = ParticleSystem.particleInstances.Count;
            Texture2D ??= !string.IsNullOrEmpty(Texture) ? ModContent.Request<Texture2D>(this.Texture, AssetRequestMode.ImmediateLoad) : TextureAssets.MagicPixel;

            ParticleSystem.particleInstances.Add(Type, this);
        }
    }
}
