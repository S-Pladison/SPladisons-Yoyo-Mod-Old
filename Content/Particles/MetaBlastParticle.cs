using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Graphics;
using SPladisonsYoyoMod.Common.Graphics.Particles;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class MetaBlastParticle : Particle, IDrawOnRenderTarget
    {
        public override string Texture => ModAssets.MiscPath + "Extra_43";

        public override void OnSpawn()
        {
            TimeLeft = 180;
            MetaBlastEffectSystem.AddElement(this);
        }

        public override void OnKill()
        {
            MetaBlastEffectSystem.RemoveElement(this);
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Rotation += 0.05f;
            Velocity *= 0.875f;
            Scale *= 0.9825f;

            minScaleForDeath = 0.01f;
            return false;
        }

        public override bool PreDraw(DrawSystem system, ref Color lightColor, ref float scaleMult) => false;

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D.Value, Position - Main.screenPosition, null, Color.White, Rotation, Texture2D.Size() * 0.5f, Scale * 0.4f, SpriteEffects.None, 0f);
        }
    }
}
