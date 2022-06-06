using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Drawing.Particles;
using SPladisonsYoyoMod.Common.Drawing.Primitives;
using SPladisonsYoyoMod.Utilities;
using System;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class TheStellarThrowHitParticle : Particle, IPostUpdateCameraPosition
    {
        public static Effect Effect { get; private set; }

        // ...

        private PrimitiveStrip trail;

        // ...

        public override string Texture => ModAssets.ParticlesPath + Name;

        public override void Load()
        {
            var texture = ModAssets.GetExtraTexture(35, AssetRequestMode.ImmediateLoad);

            Effect = ModAssets.GetEffect("TheStellarThrowParticleTrail", AssetRequestMode.ImmediateLoad).Value;
            Effect.Parameters["Texture0"].SetValue(texture.Value);
        }

        public override void Unload()
        {
            Effect = null;
        }

        public override void OnSpawn()
        {
            TimeLeft = 25;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            trail = new PrimitiveStrip(GetTrailWidth, GetTrailColor, Effect);
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Velocity *= 0.9f;
            Velocity.Y += 0.02f;
            Rotation += 0.04f;
            Scale *= 0.9f;

            return false;
        }

        public override Color GetAlpha(Color lightColor) => Color.White;

        private float GetTrailWidth(float progress) => 32f * (1 - progress * 0.5f) * Scale;
        private Color GetTrailColor(float progress) => Color.White * (1 - MathF.Pow(progress, 1.5f));

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            trail.UpdatePointsAsSimpleTrail(Position, 10);
            PrimitiveSystem.AddToDataCache(DrawKey.Layer, DrawKey.Flags, trail);
        }
    }
}