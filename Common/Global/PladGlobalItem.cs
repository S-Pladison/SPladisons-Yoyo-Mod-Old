using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Global
{
    public class PladGlobalItem : GlobalItem
    {
        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            if (ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle && ItemID.Sets.Yoyo[item.type])
            {
                float rotation = player.itemRotation * player.gravDir - 1.57079637f * player.direction;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, rotation);
            }
        }
    }
}
