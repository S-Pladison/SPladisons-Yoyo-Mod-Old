using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Particles;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class BellowingThunderParticle : Particle
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Particles/BellowingThunderParticle";

        public override void OnSpawn()
        {
            frameCount = 3;
            frame = Main.rand.Next(0, 3);
            timeLeft = 60;
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            rotation += 0.04f;
            scale *= 0.9f;

            if (timeLeft % 8 == 0)
            {
                frame = (++frame % frameCount);
                rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            }

            if (--timeLeft <= 0 || scale <= 0.4f)
            {
                Kill();
            }
        }

        public override Color GetAlpha(Color lightColor) => Color.White;

        protected override bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 0.5f;
            return true;
        }
    }
}
