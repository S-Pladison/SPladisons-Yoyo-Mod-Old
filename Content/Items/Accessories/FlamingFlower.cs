using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class FlamingFlower : PladItem
    {
        public override void PladSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Flaming Flower", rus: "Пылающий цветок");
            this.SetTooltip(eng: "12% increased yoyo critical strike chance\n" +
                            "Replaces «On Fire!» debuff with «Flaming Fragrance», which increases yoyo critical strike chance by 10%",
                            rus: "Увеличивает шанс критического попадания йо-йо на 12%\n" +
                            "Критические атаки ...");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 38;
            Item.height = 36;

            Item.rare = ItemRarityID.Orange;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetPladPlayer().flamingFlowerEquipped = true;
        }
    }

    public class FlamingFlowerTile : PladTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 850;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            this.CreateMapEntry(color: new Color(238, 145, 105), eng: "Flaming Flower", rus: "Пылающий цветок");

            this.DustType = ModContent.DustType<Dusts.VaporDust>();
            this.SoundType = SoundID.Grass;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<FlamingFlower>());

            PladSystem.FlamingFlowerPosition = Point.Zero;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Vector2 playerCenter = Main.LocalPlayer.Center;
            Vector2 tilePosition = new Vector2(i * 16, j * 16);

            if ((tilePosition - playerCenter).Length() < 16 * 17)
            {
                var modPlayer = Main.LocalPlayer.GetPladPlayer();
                modPlayer.ZoneFlamingFlower = true;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<FlamingFlower>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float progress = MathHelper.Lerp(0.3f, 0.5f, (float)Math.Abs(Math.Pow(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f), 4f)));

            r = 1f * progress;
            g = 0.4f * progress;
            b = 0.12f * progress;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (Main.gamePaused || !Main.instance.IsActive || Lighting.UpdateEveryFrame && !Main.rand.NextBool(4)) return;

            if (Main.rand.NextBool(7))
            {
                int dust = Dust.NewDust(new Vector2(i * 16 + 2, j * 16 + 2), 12, 12, ModContent.DustType<Dusts.VaporDust>(), Scale: Main.rand.NextFloat(2f, 4f));
                Main.dust[dust].velocity = new Vector2(0, Main.rand.NextFloat(-1.5f, -0.5f)).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
                Main.dust[dust].rotation = Main.rand.NextFloat(-(float)Math.PI, (float)Math.PI);
            }

            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(new Vector2(i * 16 + 2, j * 16 + 2), 12, 12, ModContent.DustType<Dusts.FlamingFlowerDust>(), Scale: Main.rand.NextFloat(0.7f, 1f));
                Main.dust[dust].velocity = new Vector2(0, Main.rand.NextFloat(-1f, -0.3f)).RotatedBy(Main.rand.NextFloat(-0.10f, 0.10f));
                Main.dust[dust].rotation = Main.rand.NextFloat(-(float)Math.PI, (float)Math.PI);
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen) zero = Vector2.Zero;

            Texture2D texture = ModContent.Request<Texture2D>(this.Texture + "_Glow").Value;
            int height = tile.frameY == 36 ? 18 : 16;
            float progress = MathHelper.Lerp(0.4f, 0.9f, (float)Math.Abs(Math.Pow(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f), 4f)));

            spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY, 16, height), Color.White * progress, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public static void DrawMapIcon(SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
        {
            if (Main.LocalPlayer == null || PladSystem.FlamingFlowerPosition == Point.Zero) return;

            Vector2 position = Common.PladSystem.FlamingFlowerPosition.ToVector2() * 16 + new Vector2(16, 16);
            float dist = (position - Main.LocalPlayer.Center).Length();

            const float maxDist = 16 * 90;
            if (dist > maxDist) return;

            Vector2 value = new Vector2(0f, (float)(-(float)Main.LocalPlayer.height / 2));
            Vector2 vec = (position + value) / 16f - mapTopLeft;
            vec *= mapScale;
            vec += mapX2Y2AndOff;
            vec = vec.Floor();

            if (mapRect == null || mapRect.Value.Contains(vec.ToPoint()))
            {
                Texture2D texture = SPladisonsYoyoMod.ExtraTextures[1].Value;
                Rectangle rectangle = texture.Frame(1, 1, 0, 0, 0, 0);

                float progress = 1;
                if (dist > (maxDist - 400)) progress = MathHelper.SmoothStep(0, 1, (maxDist - dist) / 400);

                spriteBatch.Draw(texture, vec, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * progress, 0f, rectangle.Size() / 2f, drawScale * 0.8f, SpriteEffects.None, 0f);
                if (progress > 0.2f && Utils.CenteredRectangle(vec, rectangle.Size() * drawScale).Contains(Main.MouseScreen.ToPoint()))
                {
                    mouseTextString = Language.GetTextValue("Mods.SPladisonsYoyoMod.GameUI.FlamingFlowerMapIcon");
                }
            }
        }
    }
}
