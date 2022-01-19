using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Content.Trails
{
    // Currently not used
    public class BlackholeTrail : SimpleTrail, IBlackholeSpace
    {
        public BlackholeTrail(Entity target, int length, WidthDelegate width) : base(target, length, width, (p) => Color.White, null, false)
        {
            CustomDraw = true;
        }

        public override void OnSpawn()
        {
            BlackholeSpaceSystem.Instance?.AddMetaball(this);
        }

        protected override bool PreKill()
        {
            BlackholeSpaceSystem.Instance?.RemoveMetaball(this);
            return true;
        }

        void IBlackholeSpace.DrawBlackholeSpace(SpriteBatch spriteBatch)
        {
            // ): I don't think there will be more than 2 trails in the world, so I hope that such a trick can be forgiven...

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, default, null, Main.GameViewMatrix.TransformationMatrix);
            this.Draw(spriteBatch, PrimitiveTrailSystem.Instance.TransformMatrix);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}