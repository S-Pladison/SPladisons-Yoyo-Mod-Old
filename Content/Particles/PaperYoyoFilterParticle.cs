using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Drawing;
using SPladisonsYoyoMod.Common.Drawing.Particles;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class PaperYoyoFilterParticle : Particle, IDrawOnRenderTarget
    {
        public override string Texture => ModAssets.ParticlesPath + "SmokeParticle";

        public override void OnSpawn()
        {
            TimeLeft = 100;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            PaperEffectSystem.Instance.AddFilterElement(this);
        }

        public override void OnKill()
        {
            PaperEffectSystem.Instance.RemoveFilterElement(this);
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Rotation += 0.03f;
            Velocity *= 0.94f;
            Scale = MathUtils.MultipleLerp(MathHelper.Lerp, 1 - TimeLeft / 100f, new[] { 0.3f, 1f, 1f, 1f, 1f, 1f, 1f, 0.9f, 0.75f, 0.4f, 0.1f });

            minScaleForDeath = 0f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor, ref float scaleMult) => false;

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D.Value, Position - Main.screenPosition, null, Color.White * Scale, Rotation, Texture2D.Size() * 0.5f, Scale * 0.9f, SpriteEffects.None, 0f);
        }
    }
}
