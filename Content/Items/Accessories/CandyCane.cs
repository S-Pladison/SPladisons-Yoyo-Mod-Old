using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class CandyCane : PladItem
    {
        public override void PladSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Candy Cane", rus: "Карамельная трость");
            this.SetTooltip(eng: "...",
                            rus: "...");
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 18;
            Item.height = 36;

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetPladPlayer().candyCaneEquipped = true;
        }
    }
}
