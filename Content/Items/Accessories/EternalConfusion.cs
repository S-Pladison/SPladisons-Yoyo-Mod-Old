using Terraria;
using Terraria.ID;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class EternalConfusion : PladItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.width = 40;
            Item.height = 36;

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetPladPlayer();

            modPlayer.eternalConfusionEquipped = true;
            modPlayer.eternalConfusionVisible = !hideVisual;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetPladPlayer().eternalConfusionVisible = true;
        }
    }
}
