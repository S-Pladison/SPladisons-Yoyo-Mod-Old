using SPladisonsYoyoMod.Common.Misc;
using SPladisonsYoyoMod.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Terraria_Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main main)
        {
            var matrix = PrimitiveTrailSystem.GetTransformMatrix();
            var projs = Main.projectile.ToList().FindAll(i => i.active && i.ModProjectile is PladProjectile);

            if (PrimitiveTrailSystem.AlphaBlendTrails.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                {
                    foreach (var trail in PrimitiveTrailSystem.AlphaBlendTrails) trail.Draw(Main.spriteBatch, matrix);
                }
                Main.spriteBatch.End();
            }

            if (projs.Count > 0 || PrimitiveTrailSystem.AdditiveBlendTrails.Count > 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                {
                    foreach (var trail in PrimitiveTrailSystem.AdditiveBlendTrails)
                    {
                        trail.Draw(Main.spriteBatch, matrix);
                    }

                    foreach (var proj in projs)
                    {
                        try
                        {
                            (proj.ModProjectile as PladProjectile).DrawAdditive();
                        }
                        catch (Exception e)
                        {
                            TimeLogger.DrawException(e);
                            proj.active = false;
                        }
                    }
                }
                Main.spriteBatch.End();
            }

            orig(main);
        }
    }
}