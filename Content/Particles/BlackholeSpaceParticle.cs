using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Graphics;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class BlackholeSpaceParticle : Particle, IDrawOnRenderTarget
    {
        public override string Texture => ModAssets.ParticlesPath + "HDSmokeParticle";

        public override void OnSpawn()
        {
            TimeLeft = 180;
            BlackholeEffectSystem.AddElement(this);
        }

        public override void OnKill()
        {
            BlackholeEffectSystem.RemoveElement(this);
        }

        public override bool PreUpdate(ref float minScaleForDeath)
        {
            OldPosition = Position;
            Position += Velocity;
            Rotation += 0.05f;
            Velocity *= 0.94f;
            Scale *= 0.98f;

            minScaleForDeath = 0.15f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor, ref float scaleMult) => false;

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture2D.Value, Position - Main.screenPosition, null, Color.White * Scale, Rotation, Texture2D.Size() * 0.5f, Scale * 0.5f, SpriteEffects.None, 0f);
        }
    }
}
