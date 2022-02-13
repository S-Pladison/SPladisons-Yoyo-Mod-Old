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

        /*public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = new Rectangle((int)Main.screenPosition.X - 25, (int)Main.screenPosition.Y - 25, Main.screenWidth + 25, Main.screenHeight + 25);
            if (!rect.Contains((int)position.X, (int)position.Y)) return;

            var height = (int)(Texture.Height() / frameCount);
            spriteBatch.Draw(Texture.Value, position - Main.screenPosition, new Rectangle(0, height * frame, Texture.Width(), height), Color.White * (scale + 0.4f), rotation, new Vector2(Texture.Width(), height) * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);
        }*/
    }
}
