using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Interfaces;
using System;
using System.Linq;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DrawDust(On.Terraria.Main.orig_DrawDust orig, Main main)
        {
            orig(main);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            {
                foreach (var proj in Main.projectile.ToList().FindAll(i => i.active && i.ModProjectile is IDrawAdditive))
                {
                    try
                    {
                        (proj.ModProjectile as IDrawAdditive).DrawAdditive();
                    }
                    catch (Exception e)
                    {
                        TimeLogger.DrawException(e);
                        proj.active = false;
                    }
                }

                foreach (var dust in ParticleSystem.Particles)
                {
                    dust.Draw(Main.spriteBatch);
                }
            }
            Main.spriteBatch.End();
        }
    }
}