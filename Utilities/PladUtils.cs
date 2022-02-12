using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Globals;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public static class PladUtils
    {
        public static PladPlayer GetPladPlayer(this Player player) => player.GetModPlayer<PladPlayer>();

        public static YoyoGlobalProjectile GetYoyoGlobalProjectile(this Projectile projectile) => projectile.GetGlobalProjectile<YoyoGlobalProjectile>();
        public static YoyoGlobalItem GetYoyoGlobalItem(this Item item) => item.GetGlobalItem<YoyoGlobalItem>();

        public static List<ModSystem> GetModSystems => ((Dictionary<Mod, List<ModSystem>>)SystemsByModInfo?.GetValue(null))[SPladisonsYoyoMod.Instance] ?? new();

        // ...

        public static bool IsYoyo(this Projectile projectile) => projectile.aiStyle == YoyoGlobalProjectile.YoyoAIStyle;
        public static bool IsYoyo(this Item item) => SPladisonsYoyoMod.GetYoyos.Contains(item.type);

        // ...

        private static readonly FieldInfo SystemsByModInfo = typeof(SystemLoader).GetField("SystemsByMod", BindingFlags.NonPublic | BindingFlags.Static);
    }
}