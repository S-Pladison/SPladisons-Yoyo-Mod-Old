using System.Reflection;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace SPladisonsYoyoMod.Common
{
    public partial class PladSystem : ModSystem
    {
        public static void GenerateSpaceChest(GenerationProgress progress, GameConfiguration configuration)
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
