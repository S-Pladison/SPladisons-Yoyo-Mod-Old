using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class CandyCane : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + "CandyCane";

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
