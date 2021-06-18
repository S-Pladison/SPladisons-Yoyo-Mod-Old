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
        public bool desecratedDiceEquipped;
        public bool flamingFlowerEquipped;

        public override void ResetEffects()
        {
            desecratedDiceEquipped = false;
            flamingFlowerEquipped = false;
        }
    }
}
