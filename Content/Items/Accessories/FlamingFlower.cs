using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    [AutoloadEquip]
    public class FlamingFlower : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + nameof(FlamingFlower);

        public override void Load()
        {
            EquipLoader.AddEquipTexture(Mod, Texture + "_Head", EquipType.Head, this);
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
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

    public class FlamingFlowerMapLayer : ModMapLayer
    {
        private const float RADIUS = 16 * 90;
        private const float RADIUS2 = RADIUS - 16 * 10;

        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (WorldSystem.FlamingFlowerPosition == Point.Zero) return;

            var player = Main.LocalPlayer;
            var position = WorldSystem.FlamingFlowerPosition.ToWorldCoordinates() + new Vector2(16, 16);
            var distance = (position - player.Center).Length();
            var progress = 1f;

            if (distance > RADIUS) return;
            if (distance > RADIUS2) progress = MathHelper.SmoothStep(0, 1, (RADIUS - distance) / (RADIUS - RADIUS2));

            position = WorldSystem.FlamingFlowerPosition.ToVector2() + Vector2.UnitX;

            if (context.Draw(ModContent.Request<Texture2D>(ModAssets.ItemsPath + nameof(FlamingFlower) + "_Map").Value, position, Color.White * progress, new SpriteFrame(1, 1), 0.75f, 0.75f, Alignment.Center).IsMouseOver && progress > 0.2f)
            {
                text = Language.GetTextValue("Mods.SPladisonsYoyoMod.GameUI.FlamingFlowerMapIcon");
            }
        }
    }

    public class FlamingFlowerSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override bool IsSceneEffectActive(Player player) => player.GetPladPlayer().ZoneFlamingFlower;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (!isActive || !Main.UseHeatDistortion) return;

            player.ManageSpecialBiomeVisuals("HeatDistortion", true);

            if (!player.ZoneDesert && !player.ZoneUndergroundDesert && !player.ZoneUnderworldHeight)
            {
                Terraria.Graphics.Effects.Filters.Scene["HeatDistortion"].GetShader().UseIntensity(0.85f);
            }
        }
    }

    public class FlamingFlowerTile : ModTile
    {
        public static bool IsDousedWithWater(int x, int y) => IsDousedWithWater(Main.tile[x, y]);
        public static bool IsDousedWithWater(Tile tile) => tile.TileFrameX >= 36;

        public override string Texture => ModAssets.TilesPath + nameof(FlamingFlowerTile);

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 850;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(238, 145, 105), CreateMapEntryName());

            DustType = ModContent.DustType<Dusts.VaporDust>();
            HitSound = SoundID.Grass;
        }

        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => IsDousedWithWater(i, j);
        public override bool CanExplode(int i, int j) => IsDousedWithWater(i, j);

        public override void PlaceInWorld(int i, int j, Item item)
        {
            WorldSystem.SetFlamingFlowerPosition(new Point(i, j));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<FlamingFlower>());
            WorldSystem.SetFlamingFlowerPosition();
        }

        public override bool RightClick(int i, int j)
        {
            if (WorldGen.CanKillTile(i, j))
            {
                WorldGen.KillTile(i, j);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                }
            }

            return base.RightClick(i, j);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            var playerCenter = Main.LocalPlayer.Center;
            var tile = Main.tile[i, j];
            var tilePosition = new Vector2(i * 16, j * 16);

            if ((tilePosition - playerCenter).Length() < 16 * 17)
            {
                var modPlayer = Main.LocalPlayer.GetPladPlayer();
                modPlayer.ZoneFlamingFlower = true;
            }

            if (IsDousedWithWater(i, j) || tile.LiquidType != LiquidID.Water || tile.LiquidAmount < 64) return;

            var left = i;
            var top = j;

            if (tile.TileFrameX % 36 != 0) left--;
            if (tile.TileFrameY != 0) top--;

            static void UpdateFrameX(int x, int y)
            {
                var tile = Main.tile[x, y];
                tile.TileFrameX += 36;

                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(new Vector2(x * 16 + 2, y * 16 + 2), 12, 12, ModContent.DustType<Dusts.VaporDust>(), Scale: Main.rand.NextFloat(3f, 6f));
                    Main.dust[dust].velocity = new Vector2(0, Main.rand.NextFloat(-2.5f, -0.5f)).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
                    Main.dust[dust].rotation = Main.rand.NextFloat(-(float)Math.PI, (float)Math.PI);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient) return;

                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, x, y, tile.TileType, tile.TileFrameX, tile.TileFrameY);
            }

            UpdateFrameX(left, top);
            UpdateFrameX(left + 1, top);
            UpdateFrameX(left, top + 1);
            UpdateFrameX(left + 1, top + 1);

            SoundEngine.PlaySound(SoundID.LiquidsWaterLava, tilePosition);
        }

        public override void MouseOver(int i, int j)
        {
            var player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;

            if (IsDousedWithWater(i, j))
            {
                player.cursorItemIconID = ModContent.ItemType<FlamingFlower>();
                return;
            }

            player.cursorItemIconID = ItemID.WaterBucket;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float progress = MathHelper.Lerp(0.3f, 0.5f, (float)Math.Abs(Math.Pow(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f), 4f)));

            if (IsDousedWithWater(i, j)) progress *= 0.3f;

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

            if (!IsDousedWithWater(i, j) && Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(new Vector2(i * 16 + 2, j * 16 + 2), 12, 12, ModContent.DustType<Dusts.FlamingFlowerDust>(), Scale: Main.rand.NextFloat(0.7f, 1f));
                Main.dust[dust].velocity = new Vector2(0, Main.rand.NextFloat(-1f, -0.3f)).RotatedBy(Main.rand.NextFloat(-0.10f, 0.10f));
                Main.dust[dust].rotation = Main.rand.NextFloat(-(float)Math.PI, (float)Math.PI);
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];
            var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen) zero = Vector2.Zero;

            var texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var height = tile.TileFrameY == 36 ? 18 : 16;
            var progress = MathHelper.Lerp(0.4f, 0.9f, (float)Math.Abs(Math.Pow(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f), 4f)));
            var position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            spriteBatch.Draw(texture, position, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), Color.White * progress, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
