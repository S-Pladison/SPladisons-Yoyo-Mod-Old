using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace SPladisonsYoyoMod.Content.Items.Misc
{
    public class SpaceKey : PladItem
    {
        public override void PladSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Space Key", rus: "Космический ключ");
            this.SetTooltip(
                eng: "Unlocks a Space Chest in the dungeon",
                rus: "Открывает космический сундук в Темнице"
                );
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 26;

            Item.rare = ItemRarityID.Yellow;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 2, silver: 0, copper: 0);
            Item.maxStack = 99;
        }
    }
}
