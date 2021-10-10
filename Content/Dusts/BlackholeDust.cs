using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using System;

namespace SPladisonsYoyoMod.Content.Dusts
{
    public class BlackholeDust : PladDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = 0;
            dust.frame = new Microsoft.Xna.Framework.Rectangle(0, 0, 9, 9);
        }

        public override bool Update(Dust dust)
        {
            if (dust.customData != null && dust.customData is Projectile projectile && projectile.active)
            {
                dust.position += projectile.position - projectile.oldPosition;

                var value = dust.position - projectile.Center;
                var num = value.Length();
                value /= num;

                dust.scale = Math.Min(dust.scale, num / 24f - 1f);
                dust.velocity -= value * (100f / Math.Max(50f, num));
                dust.scale *= 0.965f;

                if (projectile.Hitbox.Contains((int)dust.position.X, (int)dust.position.Y))
                {
                    dust.active = false;
                }
            }
            else
            {
                dust.scale *= 0.85f;
                dust.velocity *= 0.85f;
            }

            dust.position += dust.velocity;
            dust.rotation = dust.velocity.ToRotation();

            if (dust.scale <= 0.1f) dust.active = false;

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(220, 220, 220, 220) * dust.scale;
    }
}
