using SPladisonsYoyoMod.Content.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
{
    public class PladGlobalTile : GlobalTile
    {
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            bool flag = true;

            if (WorldGen.InWorld(i, j - 1))
            {
                var topTile = Main.tile[i, j - 1];
                flag &= CanKillOrExplodeTile(i, j, topTile);
            }

            return flag;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            bool flag = true;

            if (WorldGen.InWorld(i, j - 1))
            {
                var topTile = Main.tile[i, j - 1];
                flag &= CanKillOrExplodeTile(i, j, topTile);
            }

            return flag;
        }

        // ...

        public static bool CanKillOrExplodeTile(int i, int j, Tile topTile)
        {
            if (topTile.TileType == ModContent.TileType<FlamingFlowerTile>())
            {
                return FlamingFlowerTile.IsDousedWithWater(topTile);
            }

            return true;
        }
    }
}
