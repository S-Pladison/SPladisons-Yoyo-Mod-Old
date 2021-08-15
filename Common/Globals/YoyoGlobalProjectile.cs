using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
{
    public class YoyoGlobalProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.IsYoyo();

        // ...

        public static void ModifyYoyoStats(Projectile projectile, ref float lifeTime, ref float maxRange, ref float topSpeed, bool infinite)
        {
            var player = Main.player[projectile.owner];
            var modPlayer = player.GetPladPlayer();

            if (!infinite)
            {
                if (modPlayer.bearingEquipped) lifeTime += 0.12f;
            }
        }

        public static readonly int YoyoAIStyle = 99;
    }
}
