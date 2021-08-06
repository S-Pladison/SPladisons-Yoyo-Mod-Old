using SPladisonsYoyoMod.Common.Globals;
using SPladisonsYoyoMod.Content.Items;
using System;
using Terraria;
using Terraria.ID;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class ModHooks
    {
        private static void On_Terraria_Projectile_AI_099_2(On.Terraria.Projectile.orig_AI_099_2 orig, Projectile projectile)
        {
            // The percentages will not be calculated correctly if someone does the same :(

            void SetYoyoStats(float lifeTime, float maxRange, float topSpeed)
            {
                ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = lifeTime;
                ProjectileID.Sets.YoyosMaximumRange[projectile.type] = maxRange;
                ProjectileID.Sets.YoyosTopSpeed[projectile.type] = topSpeed;
            }

            float[] oldValues = new float[] { ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type], ProjectileID.Sets.YoyosMaximumRange[projectile.type], ProjectileID.Sets.YoyosTopSpeed[projectile.type] };
            (float lifeTimeMult, float maxRangeMult, float topSpeedMult) = GetYoyoStatsMult(projectile);

            SetYoyoStats(oldValues[0] * lifeTimeMult, oldValues[1] * maxRangeMult, oldValues[2] * topSpeedMult);
            orig(projectile);
            SetYoyoStats(oldValues[0], oldValues[1], oldValues[2]);
        }

        private static (float, float, float) GetYoyoStatsMult(Projectile projectile)
        {
            float lifeTime = 1f, maxRange = 1f, topSpeed = 1f;
            bool infinite = lifeTime == -1f;

            if (projectile.ModProjectile is YoyoProjectile yoyo) yoyo.ModifyYoyo(ref lifeTime, ref maxRange, ref topSpeed, infinite);
            YoyoGlobalProjectile.ModifyYoyo(projectile, ref lifeTime, ref maxRange, ref topSpeed, infinite);

            const float minMult = 0.1f;
            if (lifeTime < minMult) lifeTime = minMult;
            if (maxRange < minMult) maxRange = minMult;
            if (topSpeed < minMult) topSpeed = minMult;

            if (infinite) lifeTime = -1f;

            return (lifeTime, maxRange, topSpeed);
        }
    }
}
