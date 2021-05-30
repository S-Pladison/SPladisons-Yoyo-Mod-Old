using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common
{
    public class PladPlayer : ModPlayer
    {
        public bool flamingFlowerEquipped = false;

        public override void ResetEffects()
        {
            flamingFlowerEquipped = false;
        }

        public override void ModifyWeaponCrit(Item item, ref int crit)
        {
            if (flamingFlowerEquipped && ItemID.Sets.Yoyo[item.type]) crit += 12;
        }
    }
}
