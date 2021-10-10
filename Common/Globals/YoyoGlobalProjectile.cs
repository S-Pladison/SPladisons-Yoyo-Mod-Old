using SPladisonsYoyoMod.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
{
    public class YoyoGlobalProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.IsYoyo();

        // ...

        public static void ModifyYoyoLifeTime(Projectile projectile, ref float lifeTime)
        {
            if (ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] == -1f) return;

            var player = Main.player[projectile.owner];
            var modPlayer = player.GetPladPlayer();
            var lifeTimeMult = 1f;

            if (projectile.ModProjectile is YoyoProjectile yoyo) yoyo.ModifyYoyoLifeTime(ref lifeTimeMult);
            if (modPlayer.bearingEquipped) lifeTimeMult += 0.12f;

            lifeTime *= Math.Max(lifeTimeMult, 0.1f);
        }

        public static void ModifyYoyoMaximumRange(Projectile projectile, ref float maxRange)
        {
            var maxRangeMult = 1f;

            if (projectile.ModProjectile is YoyoProjectile yoyo) yoyo.ModifyYoyoMaximumRange(ref maxRangeMult);

            maxRange *= Math.Max(maxRangeMult, 0.1f);
        }

        public static readonly int YoyoAIStyle = 99;
    }
}