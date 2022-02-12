using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class PaperYoyoFilterParticle : Particle, IDrawOnRenderTarget
    {
        public PaperYoyoFilterParticle(Vector2 position, Vector2? velocity = null) : base(ModContent.Request<Texture2D>(ModAssets.ParticlesPath + "SmokeParticle"), position, velocity)
        { }

        public override void OnSpawn()
        {
            timeLeft = 100;
            rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            PaperEffectSystem.Instance?.AddFilterElement(this);
        }

        protected override bool PreKill()
        {
            PaperEffectSystem.Instance?.RemoveFilterElement(this);
            return true;
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            rotation += 0.03f;
            velocity *= 0.94f;
            scale = ModUtils.GradientValue(MathHelper.Lerp, 1 - timeLeft / 100f, new[] { 0.3f, 1f, 1f, 1f, 1f, 1f, 1f, 0.9f, 0.75f, 0.4f, 0.1f });

            //Lighting.AddLight(position, Color.LightGray.ToVector3() * 0.5f * scale);

            if (--timeLeft <= 0) this.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch) { }

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture.Value, position - Main.screenPosition, null, Color.White * scale, rotation, Texture.Size() * 0.5f, scale * 0.9f, SpriteEffects.None, 0f);
        }
    }
}
