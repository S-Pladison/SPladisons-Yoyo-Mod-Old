using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace SPladisonsYoyoMod.Common
{
    public class WorldSystem : ModSystem
    {
        public static Point FlamingFlowerPosition { get; set; }

        // ...

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (index != -1)
            {
                tasks.Insert(index + 1, new PassLegacy(Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.SpaceChest"), GenerateSpaceChest));
            }
        }

        public override void PostWorldGen()
        {
            ChangeLootInChests();
        }

        public override void PostUpdateWorld()
        {
            UpdateFlamingFlower();
        }

        public override void ResetNearbyTileEffects()
        {
            Main.LocalPlayer.GetPladPlayer().ZoneFlamingFlower = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag.Add("flamingFlowerPosition", FlamingFlowerPosition.ToVector2());
        }

        public override void LoadWorldData(TagCompound tag)
        {
            FlamingFlowerPosition = tag.Get<Vector2>("flamingFlowerPosition").ToPoint();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(FlamingFlowerPosition.ToVector2());
        }

        public override void NetReceive(BinaryReader reader)
        {
            FlamingFlowerPosition = reader.ReadVector2().ToPoint();
        }

        // ...

        private static void ChangeLootInChests()
        {
            for (int chestIndex = 0; chestIndex < Main.chest.Length; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null || Main.tile[chest.x, chest.y].type != TileID.Containers) continue;

                switch (Main.tile[chest.x, chest.y].frameX / 36)
                {
                    case 12: // Living Wood Chest
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                AddItemToFirstSlotOfChest(chest, ModContent.ItemType<Content.Items.Weapons.BloomingDeath>());
                            }
                        }
                        break;
                }
            }
        }

        private static void AddItemToFirstSlotOfChest(Chest chest, int itemType, int quantity = 1)
        {
            if (chest.item.Count(i => i.type > ItemID.None) >= Chest.maxItems) return;

            var items = chest.item.ToList();

            Item item = new Item();
            item.SetDefaults(itemType);
            item.stack = quantity;

            items.Insert(0, item);
            items.Remove(items.Last());

            chest.item = items.ToArray();
        }

        private static void UpdateFlamingFlower()
        {
            if (FlamingFlowerPosition != Point.Zero)
            {
                var tile = Main.tile[FlamingFlowerPosition.X, FlamingFlowerPosition.Y];
                if (tile == null || tile.type != ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>()) FlamingFlowerPosition = Point.Zero;
            }

            if (FlamingFlowerPosition == Point.Zero && Main.time == 0) GenerateFlamingFlower();
        }

        private static void GenerateFlamingFlower()
        {
            bool flag = false;

            // Проверяем часть мира на наличие цветка
            for (int i = 200; i < Main.maxTilesX - 200; i++)
            {
                int j;
                for (j = Main.maxTilesY - 355; j < Main.maxTilesY - 195; j++)
                {
                    if (Main.tile[i, j].type == ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>())
                    {
                        flag = true;
                        break;
                    }
                }

                // Если цветок найден, то...
                if (flag)
                {
                    FlamingFlowerPosition = new Point(i, j);
                    break;
                }
            }

            // Если цветок не был найден, то...
            if (!flag)
            {
                int t = 0;
                while (t < 1000 && flag == false)
                {
                    int x = Main.rand.Next(200, Main.maxTilesX - 200);
                    int y = Main.rand.Next(Main.maxTilesY - 350, Main.maxTilesY - 200);

                    if (WorldGen.SolidOrSlopedTile(x, y) && Main.tile[x, y].type == TileID.Stone && WorldGen.SolidOrSlopedTile(x + 1, y) && Main.tile[x + 1, y].type == TileID.Stone)
                    {
                        if (Main.tile[x - 1, y - 3].IsActive || Main.tile[x + 2, y - 3].IsActive) continue; // Вроде как после обновы стала публичной... позже исправлю короче ( на данный момент internal а не public... )
                        if (WorldGen.SolidOrSlopedTile(x, y - 1) || WorldGen.SolidOrSlopedTile(x + 1, y - 1) || WorldGen.SolidOrSlopedTile(x, y - 2) || WorldGen.SolidOrSlopedTile(x + 1, y - 2)) continue;
                        if (Main.tile[x, y - 1].LiquidAmount > 0 || Main.tile[x + 1, y - 1].LiquidAmount > 0 || Main.tile[x, y - 2].LiquidAmount > 0 || Main.tile[x + 1, y - 2].LiquidAmount > 0) continue;
                        if (!WorldGen.SolidOrSlopedTile(x, y + 1) || Main.tile[x, y + 1].type != TileID.Stone || !WorldGen.SolidOrSlopedTile(x + 1, y + 1) || Main.tile[x + 1, y + 1].type != TileID.Stone) continue;
                        if (!WorldGen.SolidOrSlopedTile(x - 1, y) || Main.tile[x - 1, y].type != TileID.Stone || !WorldGen.SolidOrSlopedTile(x + 2, y) || Main.tile[x + 2, y].type != TileID.Stone) continue;

                        WorldGen.PlaceTile(x, y - 1, (ushort)ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>());
                        FlamingFlowerPosition = new Point(x, y - 1);
                        flag = true;
                    }
                }
            }
        }

        private static void GenerateSpaceChest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.SpaceChest_0");

            bool flag = false;
            while (!flag)
            {
                object dMinX = typeof(WorldGen).GetField("dMinX", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(null);
                object dMaxX = typeof(WorldGen).GetField("dMaxX", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(null);
                object dMaxY = typeof(WorldGen).GetField("dMaxY", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(null);

                int x = WorldGen.genRand.Next((int)dMinX, (int)dMaxX);
                int y = WorldGen.genRand.Next((int)Main.worldSurface, (int)dMaxY);

                if (Main.wallDungeon[(int)Main.tile[x, y].wall] && !Main.tile[x, y].IsActive)
                {
                    flag = WorldGen.AddBuriedChest(x, y, ModContent.ItemType<Content.Items.Weapons.Blackhole>(), false, 1, chestTileType: (ushort)ModContent.TileType<Content.Items.Placeables.SpaceChestTile>());
                }
            }
        }
    }
}
