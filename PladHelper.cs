using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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

        public static void AddModTranslation(this Mod mod, string key, string eng, string rus = "")
        {
            ModTranslation text = mod.CreateTranslation(key);
            text.SetDefault(eng);
            if (rus != "") text.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            mod.AddTranslation(text);
        }
    }
}
