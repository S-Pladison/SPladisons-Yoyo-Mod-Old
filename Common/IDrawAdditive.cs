using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Particles;
using System;
using System.Linq;
using Terraria;

namespace SPladisonsYoyoMod.Common
{
    public interface IDrawAdditive
    {
        void DrawAdditive();

        // ...

        public static void Draw()
        {
            var projs = Main.projectile.ToList().FindAll(i => i.active && i.ModProjectile is IDrawAdditive);

            if (projs.Count == 0 && ParticleSystem.ActiveParticles == 0) return;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            {
                foreach (var proj in projs)
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

                ParticleSystem.DrawParticles();
            }
            Main.spriteBatch.End();
        }
    }
}
