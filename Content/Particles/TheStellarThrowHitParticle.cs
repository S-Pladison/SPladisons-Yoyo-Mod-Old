using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Common.Graphics.Primitives;
using SPladisonsYoyoMod.Utilities;
using System;
using Terraria;
using SPladisonsYoyoMod.Common.Graphics;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class TheStellarThrowHitParticle : Particle
    {
        private PrimitiveStrip trail;

        // ...

        public override string Texture => ModAssets.ParticlesPath + Name;

        public override void OnSpawn()
        {
            TimeLeft = 25;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            trail = new PrimitiveStrip(GetTrailWidth, GetTrailColor, new IPrimitiveEffect.Custom("TheStellarThrowParticleTrail"));
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

        public override bool PreDraw(DrawSystem system, ref Color lightColor, ref float scaleMult)
        {
            trail.UpdatePointsAsSimpleTrail(Position, 10);
            system.AddToLayer(DrawKey, trail);

            return true;
        }
    }
}