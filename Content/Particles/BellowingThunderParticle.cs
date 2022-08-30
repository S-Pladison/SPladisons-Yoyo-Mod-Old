using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Graphics.Particles;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    /*public class BellowingThunderParticle : Particle
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Particles/BellowingThunderParticle";

        public override void OnSpawn()
        {
            FrameCount = 3;
            Frame = Main.rand.Next(0, 3);
            TimeLeft = 60;
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Rotation += 0.04f;
            Scale *= 0.9f;

            if (TimeLeft % 8 == 0)
            {
                Frame = (++Frame % FrameCount);
                Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            }

            minScaleForDeath = 0.4f;
            return false;
        }

        public override Color GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 0.5f;
            return true;
        }
    }*/
}
