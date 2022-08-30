using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Items;
using SPladisonsYoyoMod.Content.Items.Accessories;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Global
{
    public class YoyoGlobalProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile projectile, bool lateInstantiation) => projectile.IsYoyo();

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            /*if (!projectile.counterweight || !Main.player[projectile.owner].GetPladPlayer().flamingFlowerEquipped) return true;

            var texture = ModContent.Request<Texture2D>("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_18");
            Main.EntitySpriteDraw(texture.Value, projectile.Center - Main.screenPosition, null, Color.White * 0.5f, projectile.rotation, texture.Size() * 0.5f, projectile.scale * 0.5f, SpriteEffects.None, 0);
            */
            return true;
        }

        // ...

        public static void ModifyYoyoLifeTime(Projectile projectile, ref float lifeTime)
        {
            if (ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] == -1f) return;

            var player = Main.player[projectile.owner];
            var modPlayer = player.GetPladPlayer();
            var lifeTimeMult = 1f;

            if (projectile.ModProjectile is YoyoProjectile yoyo) yoyo.ModifyYoyoLifeTime(ref lifeTimeMult);
            if (modPlayer.bearingEquipped) lifeTimeMult += 0.16f;
            if (modPlayer.hallowedBearingEquipped) lifeTimeMult += HallowedBearing.GetBearingBonus() / 100.0f;

            lifeTime *= Math.Max(lifeTimeMult, 0.1f);
        }

        public static void ModifyYoyoMaximumRange(Projectile projectile, ref float maxRange)
        {
            var maxRangeMult = 1f;

            if (projectile.ModProjectile is YoyoProjectile yoyo) yoyo.ModifyYoyoMaximumRange(ref maxRangeMult);

            maxRange *= Math.Max(maxRangeMult, 0.1f);
        }

        // ...

        public static readonly int YoyoAIStyle = 99;
    }
}