using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Global
{
    public class PladGlobalProjectile : GlobalProjectile
    {
        public static readonly int YoyoAIStyle = 99;

        public void ModifyYoyo(Projectile projectile, ref float lifeTime, ref float maxRange, ref float topSpeed)
        {
            float lifeTimeMult = 1f, maxRangeMult = 1f, topSpeedMult = 1f;

            if (projectile.ModProjectile is YoyoProjectile yoyo) yoyo.ModifyYoyo(ref lifeTimeMult, ref maxRangeMult, ref topSpeedMult);

            if (lifeTimeMult < 0) lifeTimeMult = 0;
            if (maxRangeMult < 0) maxRangeMult = 0;
            if (topSpeedMult < 0) topSpeedMult = 0;

            lifeTime *= lifeTimeMult;
            maxRange *= maxRangeMult;
            topSpeed *= topSpeedMult;
        }
    }
}
