using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Particles;

namespace SPladisonsYoyoMod.Content.Particles
{
    /*public class SmokeParticle : Particle
    {
        public override string Texture => ModAssets.ParticlesPath + "SmokeParticle";

        public override void OnSpawn()
        {
            TimeLeft = 60;
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Rotation += 0.1f;
            Velocity *= 0.9f;
            Scale *= 0.99f;

            return false;
        }

        public override Color GetAlpha(Color lightColor) => lightColor * Scale * (TimeLeft / 360f);

        public override bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 2f;
            return true;
        }
    }*/
}