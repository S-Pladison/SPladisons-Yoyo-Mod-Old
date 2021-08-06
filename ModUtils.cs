using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Globals;
using System;
using System.Linq;
using Terraria;

namespace SPladisonsYoyoMod
{
    internal static class ModUtils
    {
        internal static PladPlayer GetPladPlayer(this Player player) => player.GetModPlayer<PladPlayer>();

        internal static YoyoGlobalProjectile GetYoyoGlobalProjectile(this Projectile projectile) => projectile.GetGlobalProjectile<YoyoGlobalProjectile>();
        internal static YoyoGlobalItem GetYoyoGlobalItem(this Item item) => item.GetGlobalItem<YoyoGlobalItem>();

        internal static bool IsYoyo(this Projectile projectile) => projectile.aiStyle == YoyoGlobalProjectile.YoyoAIStyle;
        internal static bool IsYoyo(this Item item) => SPladisonsYoyoMod.GetYoyos.Contains(item.type);

        // ...

        internal static T GradientValue<T>(Func<T, T, float, T> method, float percent, params T[] values)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (percent >= 1) return values.Last();

            percent = Math.Max(percent, 0);
            float num = 1f / (values.Length - 1);
            int index = Math.Max(0, (int)(percent / num));

            return method.Invoke(values[index], values[index + 1], (percent - num * index) / num) ?? throw new Exception();
        }
    }
}
