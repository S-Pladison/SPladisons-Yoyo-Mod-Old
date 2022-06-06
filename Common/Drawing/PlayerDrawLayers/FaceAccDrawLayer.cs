using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Drawing.PlayerDrawLayers
{
    public class FaceAccDrawLayer : PlayerDrawLayer
    {
        public static Asset<Texture2D> EternalConfusionStarTexture => ModAssets.GetExtraTexture(13);

        // ...

        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.FaceAcc);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            var modPlayer = player.GetPladPlayer();

            if (!player.dead && modPlayer.eternalConfusionVisible)
            {
                Vector2 position = new((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height));
                position += player.headPosition + drawInfo.headVect + Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height] * player.gravDir;

                //if (player.gravDir == -1f) position.Y += 12f;

                float time = Main.GlobalTimeWrappedHourly * 2f;
                List<float> values = new()
                {
                    time % MathHelper.TwoPi,
                    (time + 1 / 3f * MathHelper.TwoPi) % MathHelper.TwoPi,
                    (time + 2 / 3f * MathHelper.TwoPi) % MathHelper.TwoPi
                };

                for (int i = 0; i < 3; i++)
                {
                    var num = values.Aggregate((x, y) => Math.Abs(x - MathHelper.Pi) < Math.Abs(y - MathHelper.Pi) ? x : y);
                    var num2 = 0.7f + Math.Abs(MathHelper.Pi - num) * 0.15f;
                    var rotProgress = num * player.direction * player.gravDir;
                    var starPosition = position + new Vector2((float)Math.Sin(rotProgress) * 15f, (float)Math.Cos(rotProgress) * 3f).RotatedBy(player.headRotation) * player.gravDir;

                    var item = new DrawData
                    (
                        EternalConfusionStarTexture.Value,
                        starPosition,
                        null,
                        player.GetImmuneAlphaPure(new Color(num2, num2, num2, 0.8f), drawInfo.shadow) * player.stealth,
                        player.headRotation,
                        EternalConfusionStarTexture.Size() * 0.5f,
                        num2 - 0.2f,
                        drawInfo.playerEffect,
                        0
                    );
                    item.shader = modPlayer.eternalConfusionDye;

                    drawInfo.DrawDataCache.Add(item);
                    values.Remove(num);

                    if (!Main.rand.NextBool(75)) continue;

                    var dust = Dust.NewDustPerfect(starPosition + Main.screenPosition + new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5)), DustID.GoldFlame, Vector2.Zero, 100, Color.White, 1f);
                    dust.noLight = true;
                    dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    dust.noGravity = true;
                    dust.shader = GameShaders.Armor.GetSecondaryShader(modPlayer.eternalConfusionDye, player);

                    drawInfo.DustCache.Add(dust.dustIndex);
                }
            }
        }
    }
}
