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
    public partial class PladPlayer : ModPlayer
    {
        public bool flameFlowerEquipped = false;

        public override void ResetEffects()
        {
            flameFlowerEquipped = false;
        }

        public override void ModifyWeaponCrit(Item item, ref int crit)
        {
            if (flameFlowerEquipped && ItemID.Sets.Yoyo[item.type]) crit += 12;
        }
    }
}
