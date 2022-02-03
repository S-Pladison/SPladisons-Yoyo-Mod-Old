using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DrawPlayers_BehindNPCs(On.Terraria.Main.orig_DrawPlayers_BehindNPCs orig, Main main)
        {
            var spriteBatch = Main.spriteBatch;
            var pts = PrimitiveTrailSystem.Instance;

            if (pts != null)
            {
                pts.UpdateTransformMatrix();

                if (pts.AlphaBlendTrails.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    pts.DrawTrails(pts.AlphaBlendTrails);
                    spriteBatch.End();
                }

                if (pts.AdditiveTrails.Count > 0)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                    pts.DrawTrails(pts.AdditiveTrails);
                    spriteBatch.End();
                }
            }

            orig(main);
        }
    }
}