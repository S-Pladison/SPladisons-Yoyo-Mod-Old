using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DrawProj_DrawYoyoString(On.Terraria.Main.orig_DrawProj_DrawYoyoString orig, Main main, Projectile projectile, Vector2 mountedCenter)
        {
            if (!ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle || !projectile.IsYoyo())
            {
                orig(main, projectile, mountedCenter);
                return;
            }

            Player owner = Main.player[projectile.owner];
            mountedCenter += ModUtils.GetYoyoStringOffset();
            mountedCenter.Y += owner.gfxOffY;

            ModUtils.DrawYoyoString(projectile, mountedCenter, (position, rotation, height, color, i) =>
            {
                Main.EntitySpriteDraw(
                    color: color * Math.Min(i / 12.0f, 0.5f),
                    texture: TextureAssets.FishingLine.Value,
                    position: position,
                    sourceRectangle: new Rectangle(0, 0, TextureAssets.FishingLine.Width(), (int)height),
                    rotation: rotation,
                    origin: new Vector2((float)TextureAssets.FishingLine.Width() * 0.5f, 0f),
                    scale: 1f,
                    effects: SpriteEffects.None,
                    worthless: 0
                );
            });

            // Part of the vanilla code
            if (!projectile.counterweight)
            {
                Vector2 vec = projectile.Center - mountedCenter;
                int num5 = -1;

                if (projectile.position.X + (float)(projectile.width / 2) < owner.position.X + (float)(owner.width / 2))
                {
                    num5 = 1;
                }

                num5 *= -1;
                owner.itemRotation = (float)Math.Atan2(vec.Y * (float)num5, vec.X * (float)num5);
            }
        }
    }
}