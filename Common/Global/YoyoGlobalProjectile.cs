using SPladisonsYoyoMod.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Global
{
    public class YoyoGlobalProjectile : GlobalProjectile
    {
        public static readonly int YoyoAIStyle = 99;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.IsYoyo();

        public void ModifyYoyo(Projectile projectile, ref float lifeTime, ref float maxRange, ref float topSpeed)
        {
            bool inf = lifeTime == -1;

            float lifeTimeMult = 1f, maxRangeMult = 1f, topSpeedMult = 1f;

            if (projectile.ModProjectile is YoyoProjectile yoyo) yoyo.ModifyYoyo(ref lifeTimeMult, ref maxRangeMult, ref topSpeedMult);

            var globalYoyo = Main.player[projectile.owner]?.HeldItem?.GetYoyoGlobalItem();
            if (globalYoyo != null)
            {
                lifeTimeMult += globalYoyo.lifeTimeMult - 1;
                maxRangeMult += globalYoyo.maxRangeMult - 1;
                topSpeedMult += globalYoyo.topSpeedMult - 1;
            }

            if (lifeTimeMult < 0) lifeTimeMult = 0;
            if (maxRangeMult < 0) maxRangeMult = 0;
            if (topSpeedMult < 0) topSpeedMult = 0;

            lifeTime *= lifeTimeMult;
            maxRange *= maxRangeMult;
            topSpeed *= topSpeedMult;

            if (inf) lifeTime = -1;
        }
    }
}
