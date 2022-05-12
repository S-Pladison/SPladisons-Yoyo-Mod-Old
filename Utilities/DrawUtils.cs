using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public static class DrawUtils
    {
        public delegate void DrawStringDelegate(Vector2 position, float rotation, float height, Color color, int counter);

        // ...

        public static void BeginProjectileSpriteBatch(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, Effect effect = null, Matrix? matrix = null, bool end = true)
        {
            if (end) Main.spriteBatch.End();
            Main.spriteBatch.Begin(sortMode, blendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, matrix ?? Main.GameViewMatrix.TransformationMatrix);
        }

        public static Vector2 GetYoyoStringDrawOffset()
        {
            return Vector2.UnitY * (ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle ? -4 : 0);
        }

        public static void DrawYoyoString(Projectile projectile, Vector2 startPos, DrawStringDelegate method)
        {
            if (method == null) return;

            Player owner = Main.player[projectile.owner];
            Vector2 vector = startPos;
            float num2 = projectile.Center.X - vector.X;
            float num3 = projectile.Center.Y - vector.Y + projectile.gfxOffY;
            float num4;
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
                num2 = projectile.position.X + projectile.width * 0.5f - vector.X;
                num3 = projectile.position.Y + projectile.height * 0.5f - vector.Y;
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
                num2 = projectile.position.X + projectile.width * 0.5f - vector.X;
                num3 = projectile.position.Y + projectile.height * 0.1f - vector.Y;

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
                Color color = Color.White;
                color.A = (byte)(color.A * 0.4f);
                color = TryApplyingPlayerStringColor(owner.stringColor, color);
                color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f), color);

                method.Invoke(vector - Main.screenPosition + TextureAssets.FishingLine.Size() * 0.5f - new Vector2(6f, 0f), num4, num7, color, ++counter);
            }

            static Color TryApplyingPlayerStringColor(int playerStringColor, Color stringColor)
            {
                return (Color)(StringColorMethodInfo?.Invoke(null, new object[] { playerStringColor, stringColor }) ?? stringColor);
            }
        }

        // ...

        private static readonly MethodInfo StringColorMethodInfo = typeof(Main).GetMethod("TryApplyingPlayerStringColor", BindingFlags.NonPublic | BindingFlags.Static);
    }
}