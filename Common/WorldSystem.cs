using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Items.Misc;
using SPladisonsYoyoMod.Content.Items.Weapons;
using StructureHelper;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
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
        public static Point FlamingFlowerPosition { get; private set; }

        // ...

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));

            if (index != -1)
            {
                tasks.Insert(index + 1, new PassLegacy(Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.SpaceChest_1"), GenerateSpaceChest));
            }

            index = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));

            if (index != -1)
            {
                tasks.Insert(index + 1, new PassLegacy(Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.FlamingFlower_1"), GenerateFlamingFlower));
            }
        }

        public override void PostWorldGen()
        {
            ChangeLootInChests();
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

        // ...

        public static void SetFlamingFlowerPosition(Point? position = null)
        {
            FlamingFlowerPosition = position ?? Point.Zero;
        }

        // ...

        private static void ChangeLootInChests()
        {
            for (int chestIndex = 0; chestIndex < Main.chest.Length; chestIndex++)
            {
                var chest = Main.chest[chestIndex];

                if (chest == null || Main.tile[chest.x, chest.y].TileType != TileID.Containers) continue;

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

        private static void GenerateFlamingFlower(GenerationProgress progress, GameConfiguration _)
        {
            progress.Message = Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.FlamingFlower_2");

            int t = 0;

            bool SolidOrSlopedStone(int x, int y) => WorldGen.InWorld(x, y) && WorldGen.SolidOrSlopedTile(x, y) && Main.tile[x, y].TileType == TileID.Stone;
            bool HasTileOrLiquid(int x, int y) => !WorldGen.InWorld(x, y) || (Main.tile[x, y].HasTile || Main.tile[x, y - 1].LiquidAmount > 0);

            while (t < 1000)
            {
                int x = Main.rand.Next(200, Main.maxTilesX - 200);
                int y = Main.rand.Next(Main.maxTilesY - 350, Main.maxTilesY - 200);

                // Blocks under flower
                if (!SolidOrSlopedStone(x, y) || !SolidOrSlopedStone(x + 1, y + 1)) continue;

                // Place of the supposed position of the flower
                if (HasTileOrLiquid(x, y - 2) || HasTileOrLiquid(x + 1, y - 1)) continue;

                // Corners
                if (HasTileOrLiquid(x - 2, y - 7) || HasTileOrLiquid(x + 3, y - 7)) continue;

                // Left and right sides
                if (HasTileOrLiquid(x - 4, y - 1) || HasTileOrLiquid(x + 5, y - 1)) continue;

                // Main.tile[x, y] - first tile under flower
                var position = new Point16(x - 5, y - 10);

                if (Generator.GenerateMultistructureRandom("Assets/Structures/FlamingFlower", position, SPladisonsYoyoMod.Instance))
                {
                    FlamingFlowerPosition = new Point(x, y - 1);
                    return;
                }
            }
        }

        private static void GenerateSpaceChest(GenerationProgress progress, GameConfiguration _)
        {
            progress.Message = Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.SpaceChest_2");

            bool flag = false;

            while (!flag)
            {
                object dMinX = typeof(WorldGen).GetField("dMinX", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(null);
                object dMaxX = typeof(WorldGen).GetField("dMaxX", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(null);
                object dMaxY = typeof(WorldGen).GetField("dMaxY", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(null);

                int x = WorldGen.genRand.Next((int)dMinX, (int)dMaxX);
                int y = WorldGen.genRand.Next((int)Main.worldSurface, (int)dMaxY);

                if (Main.wallDungeon[Main.tile[x, y].WallType] && !Main.tile[x, y].HasTile)
                {
                    flag = WorldGen.AddBuriedChest(x, y, ModContent.ItemType<Blackhole>(), false, 1, chestTileType: (ushort)ModContent.TileType<SpaceChestTile>());
                }
            }
        }
    }
}
