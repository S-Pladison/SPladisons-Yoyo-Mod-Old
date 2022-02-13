using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class PaperYoyoFilterParticle : Particle, IDrawOnRenderTarget
    {
        public override string Texture => ModAssets.ParticlesPath + "SmokeParticle";

        public override void OnSpawn()
        {
            timeLeft = 100;
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            PaperEffectSystem.Instance.AddFilterElement(this);
        }

        protected override void OnKill()
        {
            PaperEffectSystem.Instance.RemoveFilterElement(this);
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            rotation += 0.03f;
            velocity *= 0.94f;
            scale = MathUtils.MultipleLerp(MathHelper.Lerp, 1 - timeLeft / 100f, new[] { 0.3f, 1f, 1f, 1f, 1f, 1f, 1f, 0.9f, 0.75f, 0.4f, 0.1f });

            if (--timeLeft <= 0) this.Kill();
        }

        protected override bool PreDraw(ref Color lightColor, ref float scaleMult) => false;

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D.Value, position - Main.screenPosition, null, Color.White * scale, rotation, Texture2D.Size() * 0.5f, scale * 0.9f, SpriteEffects.None, 0f);
        }
    }
}
