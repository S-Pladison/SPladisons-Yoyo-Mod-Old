using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Particles;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class BellowingThunderSmokeParticle : Particle
    {
        public override string Texture => ModAssets.ParticlesPath + "SmokeParticle";

        public override void OnSpawn()
        {
            timeLeft = 360;
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            scale = Main.rand.NextFloat(0.8f, 1.2f);
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            rotation += Main.rand.NextFloat(-0.5f, 0.5f);
            velocity *= 0.975f;
            scale *= 0.99f;

            if (--timeLeft <= 0 || scale <= 0.2f) this.Kill();
        }

        public override Color GetAlpha(Color lightColor) => new Color(55, 35, 170) * scale * 0.35f;

        protected override bool PreDraw(ref Color lightColor, ref float scaleMult)
        {
            scaleMult = 0.5f;
            return true;
        }

        /*public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = new Rectangle((int)Main.screenPosition.X - 25, (int)Main.screenPosition.Y - 25, Main.screenWidth + 25, Main.screenHeight + 25);
            if (!rect.Contains((int)position.X, (int)position.Y)) return;

            Color color = new Color(55, 35, 170) * scale * 0.35f;
            spriteBatch.Draw(Texture.Value, position - Main.screenPosition, null, color, rotation, Texture.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);
        }*/
    }
}
