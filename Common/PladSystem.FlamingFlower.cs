using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common
{
    public partial class PladSystem : ModSystem
    {
        public static Point flamingFlowerPosition;

        public void UpdateFlamingFlower()
        {
            if (flamingFlowerPosition != Point.Zero)
            {
                var tile = Main.tile[flamingFlowerPosition.X, flamingFlowerPosition.Y];
                if (tile == null || tile.type != ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>()) flamingFlowerPosition = Point.Zero;
            }

            if (flamingFlowerPosition == Point.Zero && Main.time == 0) this.GenerateFlamingFlower();
        }

        public void GenerateFlamingFlower()
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
                    flamingFlowerPosition = new Point(i, j);
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
                        if (Main.tile[x - 1, y - 3].active() || Main.tile[x + 2, y - 3].active()) continue; // Вроде как после обновы стала публичной... позже исправлю короче ( на данный момент internal а не public... )
                        if (WorldGen.SolidOrSlopedTile(x, y - 1) || WorldGen.SolidOrSlopedTile(x + 1, y - 1) || WorldGen.SolidOrSlopedTile(x, y - 2) || WorldGen.SolidOrSlopedTile(x + 1, y - 2)) continue;
                        if (Main.tile[x, y - 1].LiquidAmount > 0 || Main.tile[x + 1, y - 1].LiquidAmount > 0 || Main.tile[x, y - 2].LiquidAmount > 0 || Main.tile[x + 1, y - 2].LiquidAmount > 0) continue;
                        if (!WorldGen.SolidOrSlopedTile(x, y + 1) || Main.tile[x, y + 1].type != TileID.Stone || !WorldGen.SolidOrSlopedTile(x + 1, y + 1) || Main.tile[x + 1, y + 1].type != TileID.Stone) continue;
                        if (!WorldGen.SolidOrSlopedTile(x - 1, y) || Main.tile[x - 1, y].type != TileID.Stone || !WorldGen.SolidOrSlopedTile(x + 2, y) || Main.tile[x + 2, y].type != TileID.Stone) continue;

                        WorldGen.PlaceTile(x, y - 1, (ushort)ModContent.TileType<Content.Items.Accessories.FlamingFlowerTile>());
                        flamingFlowerPosition = new Point(x, y - 1);
                        flag = true;
                    }
                }
            }
        }
    }
}
