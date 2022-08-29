using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Particles;
using System;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class HeartParticle : Particle
    {
        public override string Texture => ModAssets.ParticlesPath + nameof(HeartParticle);

        public override void OnSpawn()
        {
            TimeLeft = 70;
            FrameCount = 2;
            Frame = Main.rand.NextBool(3) ? 1 : 0;
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Velocity *= 0.9f;
            Rotation *= 0.95f;
            Scale *= 0.98f;

            Lighting.AddLight(Position, Color.ToVector3() * (TimeLeft / (float)InitTimeLeft) * 0.15f);

            return false;
        }

        public override Color GetAlpha(Color lightColor) => Color.White * (TimeLeft / (float)InitTimeLeft) * 1.15f;

        public override bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 0.25f * MathF.Sin(TimeLeft / (float)InitTimeLeft * MathF.PI);
            return true;
        }
    }
}