using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Items.Misc;
using SPladisonsYoyoMod.Content.Items.Weapons;
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
            if (Main.time == 0 && Main.dayTime)
            {
                UpdateFlamingFlower();
            }
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
                if (chest == null || (int)Main.tile[chest.x, chest.y].TileType != TileID.Containers) continue;

                switch (Main.tile[chest.x, chest.y].TileFrameX / 36)
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

            Item item = new();
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
                if (tile == null || (int)tile.TileType != ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>())
                {
                    FlamingFlowerPosition = Point.Zero;
                }
            }

            if (FlamingFlowerPosition == Point.Zero && !SearchFlamingFlower())
            {
                GenerateFlamingFlower();
            }
        }

        private static bool SearchFlamingFlower()
        {
            for (int i = 200; i < Main.maxTilesX - 200; i++)
            {
                int j;
                for (j = Main.maxTilesY - 355; j < Main.maxTilesY - 195; j++)
                {
                    var tile = Main.tile[i, j];
                    if ((int)tile.TileType == ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>() && tile.TileFrameX == 0 && tile.TileFrameY == 0)
                    {
                        FlamingFlowerPosition = new Point(i, j);
                        return true;
                    }
                }
            }
            return false;
        }

        private static void GenerateFlamingFlower()
        {
            int t = 0;
            while (t < 1000)
            {
                int x = Main.rand.Next(200, Main.maxTilesX - 200);
                int y = Main.rand.Next(Main.maxTilesY - 350, Main.maxTilesY - 200);

                if (WorldGen.SolidOrSlopedTile(x, y) && (int)Main.tile[x, y].TileType == TileID.Stone && WorldGen.SolidOrSlopedTile(x + 1, y) && (int)Main.tile[x + 1, y].TileType == TileID.Stone)
                {
                    if (Main.tile[x - 1, y - 3].HasTile || Main.tile[x + 2, y - 3].HasTile) continue;
                    if (WorldGen.SolidOrSlopedTile(x, y - 1) || WorldGen.SolidOrSlopedTile(x + 1, y - 1) || WorldGen.SolidOrSlopedTile(x, y - 2) || WorldGen.SolidOrSlopedTile(x + 1, y - 2)) continue;
                    if (Main.tile[x, y - 1].LiquidAmount > 0 || Main.tile[x + 1, y - 1].LiquidAmount > 0 || Main.tile[x, y - 2].LiquidAmount > 0 || Main.tile[x + 1, y - 2].LiquidAmount > 0) continue;
                    if (!WorldGen.SolidOrSlopedTile(x, y + 1) || (int)Main.tile[x, y + 1].TileType != TileID.Stone || !WorldGen.SolidOrSlopedTile(x + 1, y + 1) || (int)Main.tile[x + 1, y + 1].TileType != TileID.Stone) continue;
                    if (!WorldGen.SolidOrSlopedTile(x - 1, y) || (int)Main.tile[x - 1, y].TileType != TileID.Stone || !WorldGen.SolidOrSlopedTile(x + 2, y) || (int)Main.tile[x + 2, y].TileType != TileID.Stone) continue;

                    WorldGen.PlaceTile(x, y - 1, (ushort)ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>());
                    FlamingFlowerPosition = new Point(x, y - 1);
                    return;
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

                if (Main.wallDungeon[(int)Main.tile[x, y].WallType] && !Main.tile[x, y].HasTile)
                {
                    flag = WorldGen.AddBuriedChest(x, y, ModContent.ItemType<Blackhole>(), false, 1, chestTileType: (ushort)ModContent.TileType<SpaceChestTile>());
                }
            }
        }
    }
}
