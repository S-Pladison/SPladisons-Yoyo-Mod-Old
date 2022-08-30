using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Graphics.Particles;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    /*public class BellowingThunderSmokeParticle : Particle
    {
        public override string Texture => ModAssets.ParticlesPath + "SmokeParticle";

        public override void OnSpawn()
        {
            TimeLeft = 360;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Scale = Main.rand.NextFloat(0.8f, 1.2f);
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Rotation += Main.rand.NextFloat(-0.5f, 0.5f);
            Velocity *= 0.975f;
            Scale *= 0.99f;

            minScaleForDeath = 0.2f;
            return false;
        }

        public override Color GetAlpha(Color lightColor) => new Color(55, 35, 170) * Scale * 0.35f;

        public override bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 0.5f;
            return true;
        }
    }*/
}
