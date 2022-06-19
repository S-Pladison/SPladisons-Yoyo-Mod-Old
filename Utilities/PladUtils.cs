using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Globals;
using Terraria;
using Terraria.ID;

namespace SPladisonsYoyoMod
{
    public static class PladUtils
    {
        public static PladPlayer GetPladPlayer(this Player player) => player.GetModPlayer<PladPlayer>();

        public static YoyoGlobalProjectile GetYoyoGlobalProjectile(this Projectile projectile) => projectile.GetGlobalProjectile<YoyoGlobalProjectile>();
        public static YoyoGlobalItem GetYoyoGlobalItem(this Item item) => item.GetGlobalItem<YoyoGlobalItem>();

        public static bool IsYoyo(this Projectile projectile) => projectile.aiStyle == YoyoGlobalProjectile.YoyoAIStyle;
        public static bool IsYoyo(this Item item)
        {
            if (ItemID.Sets.Yoyo[item.type]) return true;
            if (item.shoot < ProjectileID.Count) return false;

            var proj = new Projectile();
            proj.SetDefaults(item.shoot);

            return proj.IsYoyo();
        }
    }
}