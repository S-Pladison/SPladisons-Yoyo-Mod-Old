using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Globals;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    internal static class ModUtils
    {
        public static PladPlayer GetPladPlayer(this Player player) => player.GetModPlayer<PladPlayer>();

        public static YoyoGlobalProjectile GetYoyoGlobalProjectile(this Projectile projectile) => projectile.GetGlobalProjectile<YoyoGlobalProjectile>();
        public static YoyoGlobalItem GetYoyoGlobalItem(this Item item) => item.GetGlobalItem<YoyoGlobalItem>();

        public static bool IsYoyo(this Projectile projectile) => projectile.aiStyle == YoyoGlobalProjectile.YoyoAIStyle;
        public static bool IsYoyo(this Item item) => SPladisonsYoyoMod.GetYoyos.Contains(item.type);

        // ...

        public static T GradientValue<T>(Func<T, T, float, T> method, float percent, params T[] values)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (percent >= 1) return values.Last();

            percent = Math.Max(percent, 0);
            float num = 1f / (values.Length - 1);
            int index = Math.Max(0, (int)(percent / num));

            return method.Invoke(values[index], values[index + 1], (percent - num * index) / num) ?? throw new Exception();
        }

        public static void DrawYoyoString(Projectile projectile, Vector2 startPos, DrawStringDelegate method)
        {
            if (method == null) return;

            Player owner = Main.player[projectile.owner];
            Vector2 vector = startPos;

            float num2 = projectile.Center.X - vector.X;
            float num3 = projectile.Center.Y - vector.Y;
            float num4 = (float)Math.Atan2(num3, num2) - 1.57f;

            bool flag = true;
            if (num2 == 0f && num3 == 0f)
            {
                flag = false;
            }
            else
            {
                float num6 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num6 = 12f / num6;
                num2 *= num6;
                num3 *= num6;
                vector.X -= num2 * 0.1f;
                vector.Y -= num3 * 0.1f;
                num2 = projectile.position.X + (float)projectile.width * 0.5f - vector.X;
                num3 = projectile.position.Y + (float)projectile.height * 0.5f - vector.Y;
            }

            int counter = 0;
            while (flag)
            {
                float num7 = 12f;
                float num8 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                float num9 = num8;
                if (float.IsNaN(num8) || float.IsNaN(num9))
                {
                    flag = false;
                    continue;
                }

                if (num8 < 20f)
                {
                    num7 = num8 - 8f;
                    flag = false;
                }

                num8 = 12f / num8;
                num2 *= num8;
                num3 *= num8;
                vector.X += num2;
                vector.Y += num3;
                num2 = projectile.position.X + (float)projectile.width * 0.5f - vector.X;
                num3 = projectile.position.Y + (float)projectile.height * 0.1f - vector.Y;

                if (num9 > 12f)
                {
                    float num10 = 0.3f;
                    float num11 = Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y);
                    if (num11 > 16f) num11 = 16f;

                    num11 = 1f - num11 / 16f;
                    num10 *= num11;
                    num11 = num9 / 80f;
                    if (num11 > 1f) num11 = 1f;

                    num10 *= num11;
                    if (num10 < 0f) num10 = 0f;

                    num10 *= num11;
                    num10 *= 0.5f;
                    if (num3 > 0f)
                    {
                        num3 *= 1f + num10;
                        num2 *= 1f - num10;
                    }
                    else
                    {
                        num11 = Math.Abs(projectile.velocity.X) / 3f;
                        if (num11 > 1f) num11 = 1f;

                        num11 -= 0.5f;
                        num10 *= num11;
                        if (num10 > 0f) num10 *= 2f;

                        num3 *= 1f + num10;
                        num2 *= 1f - num10;
                    }
                }

                num4 = (float)Math.Atan2(num3, num2) - 1.57f;
                Color color = Microsoft.Xna.Framework.Color.White;
                color.A = (byte)((float)(int)color.A * 0.4f);
                color = ModUtils.TryApplyingPlayerStringColor(owner.stringColor, color);
                color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f), color);

                method.Invoke(vector - Main.screenPosition + TextureAssets.FishingLine.Size() * 0.5f - new Vector2(6f, 0f), num4, num7, color, ++counter);
            }
        }

        public static Vector2 GetYoyoStringOffset()
        {
            return Vector2.UnitY * (ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle ? -4 : 0);
        }

        public static Color TryApplyingPlayerStringColor(int playerStringColor, Color stringColor)
        {
            return (Color)(StringColorMethodInfo?.Invoke(null, new object[] { playerStringColor, stringColor }) ?? stringColor);
        }

        public delegate void DrawStringDelegate(Vector2 position, float rotation, float height, Color color, int counter);

        // ...

        private static readonly MethodInfo StringColorMethodInfo = typeof(Main).GetMethod("TryApplyingPlayerStringColor", BindingFlags.NonPublic | BindingFlags.Static);
    }
}
