using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Graphics;
using SPladisonsYoyoMod.Common.Graphics.Particles;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class TheStellarThrowTrailParticle : Particle
    {
        public override string Texture => ModAssets.ParticlesPath + Name;

        public override void OnSpawn()
        {
            TimeLeft = 20;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Velocity *= 0.9f;
            Rotation += 0.01f;
            Scale *= 0.885f;

            return false;
        }

        public override Color GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(DrawSystem system, ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 0.6f;
            return true;
        }
    }
}