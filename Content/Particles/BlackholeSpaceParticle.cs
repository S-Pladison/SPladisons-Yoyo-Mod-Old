using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class BlackholeSpaceParticle : Particle, IDrawOnRenderTarget
    {
        public override string Texture => ModAssets.ParticlesPath + "SmokeParticle";

        public override void OnSpawn()
        {
            timeLeft = 180;
            BlackholeSpaceSystem.Instance.AddElement(this);
        }

        protected override void OnKill()
        {
            BlackholeSpaceSystem.Instance.RemoveElement(this);
        }

        public override void Update()
        {
            oldPosition = position;
            position += velocity;
            rotation += 0.05f;
            velocity *= 0.94f;
            scale *= 0.98f;

            if (--timeLeft <= 0 || scale <= 0.15f) this.Kill();
        }

        protected override bool PreDraw(ref Color lightColor, ref float scaleMult) => false;

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D.Value, position - Main.screenPosition, null, Color.White * scale, rotation, Texture2D.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);
        }
    }
}
