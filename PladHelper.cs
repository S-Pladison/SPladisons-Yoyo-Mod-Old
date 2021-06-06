using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public static class PladHelper
    {
        public static bool active(this Tile tile) // Временно
        {
            return (tile.sTileHeader & 32) == 32;
        }

        public static PladPlayer GetPladPlayer(this Player player) => player.GetModPlayer<PladPlayer>();
        public static PladGlobalProjectile GetPladGlobalProjectile(this Projectile projectile) => projectile.GetGlobalProjectile<PladGlobalProjectile>();

        public static bool IsYoyo(this Projectile projectile) => projectile.aiStyle == PladGlobalProjectile.YoyoAIStyle;
        public static bool IsYoyo(this Item item)
        {
            if (ItemID.Sets.Yoyo[item.type]) return true;

            if (item.shoot > ProjectileID.None)
            {
                var proj = new Projectile();
                proj.SetDefaults(item.shoot);

                if (proj.IsYoyo()) return true;
            }

            return false;
        }

        public static void AddModTranslation(this Mod mod, string key, string eng, string rus = "")
        {
            ModTranslation text = mod.CreateTranslation(key);
            text.SetDefault(eng);
            if (rus != "") text.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            mod.AddTranslation(text);
        }
    }
}
