using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class PaperYoyoBubbleParticle : Particle, IDrawOnRenderTarget
    {
        public override string Texture => ModAssets.MiscPath + "Extra_33";

        public override void OnSpawn()
        {
            timeLeft = 85;
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            PaperEffectSystem.Instance.AddBubbleElement(this);
        }

        protected override void OnKill()
        {
            PaperEffectSystem.Instance.RemoveBubbleElement(this);
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            rotation += 0.01f;
            velocity *= 0.94f;
            scale = MathUtils.MultipleLerp(MathHelper.Lerp, 1 - timeLeft / 85f, new[] { 0.3f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.1f });

            if (--timeLeft <= 0) this.Kill();
        }

        protected override bool PreDraw(ref Color lightColor, ref float scaleMult) => false;

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D.Value, position - Main.screenPosition, null, Color.White, rotation, Texture2D.Size() * 0.5f, scale * 0.9f, SpriteEffects.None, 0f);
        }
    }
}
