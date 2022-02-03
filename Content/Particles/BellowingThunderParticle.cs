using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class BellowingThunderParticle : Particle
    {
        public BellowingThunderParticle(Vector2 position, Vector2? velocity = null) : base(ModContent.Request<Texture2D>(ModAssets.ParticlesPath + "BellowingThunderParticle"), position, velocity)
        { }

        public override void OnSpawn()
        {
            frameCount = 3;
            frame = Main.rand.Next(0, 3);

            timeLeft = 60;
            rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
            scale = Main.rand.NextFloat(0.8f, 1.2f);
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

            if (--timeLeft <= 0 || scale <= 0.4f) this.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = new Rectangle((int)Main.screenPosition.X - 25, (int)Main.screenPosition.Y - 25, Main.screenWidth + 25, Main.screenHeight + 25);
            if (!rect.Contains((int)position.X, (int)position.Y)) return;

            var height = (int)(Texture.Height() / frameCount);
            spriteBatch.Draw(Texture.Value, position - Main.screenPosition, new Rectangle(0, height * frame, Texture.Width(), height), Color.White * (scale + 0.4f), rotation, new Vector2(Texture.Width(), height) * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);
        }
    }
}
