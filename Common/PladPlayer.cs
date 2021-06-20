using SPladisonsYoyoMod.Content.Items;
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
        public bool bearingEquipped;
        public bool desecratedDiceEquipped;
        public bool flamingFlowerEquipped;

        public override void ResetEffects()
        {
            bearingEquipped = false;
            desecratedDiceEquipped = false;
            flamingFlowerEquipped = false;
        }

        public override void UpdateEquips()
        {
            /*if (Player.counterWeight > 0)
            {
                this.counterWeight = 556 + Main.rand.Next(6);
                this.yoyoGlove = true;
                this.yoyoString = true;
            }

            if (currentItem.type >= 3309 && currentItem.type <= 3314)
            {
                this.counterWeight = 556 + currentItem.type - 3309;
            }*/
        }
    }
}
