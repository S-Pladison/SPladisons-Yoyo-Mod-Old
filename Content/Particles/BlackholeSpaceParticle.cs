using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Particles
{
    public class BlackholeSpaceParticle : ParticleSystem.Particle, IBlackholeSpace
    {
        public BlackholeSpaceParticle(Vector2 position, Vector2? velocity = null) :
        base(ModContent.Request<Texture2D>("SPladisonsYoyoMod/Assets/Textures/Particles/SmokeParticle"), 180, position, velocity, 0f, 1f)
        { }

        public override void OnSpawn()
        {
            BlackholeSpaceSystem.Instance?.AddElement(this);
        }

        protected override bool PreKill()
        {
            BlackholeSpaceSystem.Instance?.RemoveElement(this);
            return true;
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

        public override void Draw(SpriteBatch spriteBatch) { }

        void IBlackholeSpace.DrawBlackholeSpace(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture.Value, position - Main.screenPosition, null, Color.White * scale, rotation, Texture.Size() * 0.5f, scale * 0.5f, SpriteEffects.None, 0f);
        }
    }
}
