using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace SPladisonsYoyoMod.Common.PlayerDrawLayers
{
    public class AfterFaceAccDrawLayer : PlayerDrawLayer
    {
        private static Asset<Texture2D> StarsOverHeadTexture => ModAssets.ExtraTextures[13];

        public override Position GetDefaultPosition() => new AfterParent(Terraria.DataStructures.PlayerDrawLayers.FaceAcc);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            var player = drawInfo.drawPlayer;
            var modPlayer = player.GetPladPlayer();

            if (modPlayer.starsOverHeadVisible)
            {
                Vector2 position = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f));
                position += player.headPosition + drawInfo.headVect + Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height] * player.gravDir;

                if (player.gravDir == -1f) position.Y += 12f;

                float time = Main.GlobalTimeWrappedHourly * 2f;
                List<float> values = new()
                {
                    time % MathHelper.TwoPi,
                    (time + 1 / 3f * MathHelper.TwoPi) % MathHelper.TwoPi,
                    (time + 2 / 3f * MathHelper.TwoPi) % MathHelper.TwoPi
                };

                for (int i = 0; i < 3; i++)
                {
                    float num = values.Aggregate((x, y) => Math.Abs(x - MathHelper.Pi) < Math.Abs(y - MathHelper.Pi) ? x : y);
                    float num2 = 0.7f + Math.Abs(MathHelper.Pi - num) * 0.15f;
                    values.Remove(num);

                    DrawData item = new DrawData(
                        StarsOverHeadTexture.Value,
                        position + new Vector2((float)Math.Sin(num) * 15f, -28 + (float)Math.Cos(num) * 3f).RotatedBy(player.headRotation) * player.Directions,
                        null,
                        player.GetImmuneAlphaPure(new Color(num2, num2, num2, 0.8f), drawInfo.shadow) * player.stealth,
                        player.headRotation,
                        StarsOverHeadTexture.Size() * 0.5f,
                        num2 - 0.2f,
                        drawInfo.playerEffect,
                        0
                    );
                    item.shader = modPlayer.starsOverHeadDye;

                    drawInfo.DrawDataCache.Add(item);
                }
            }
        }
    }
}
