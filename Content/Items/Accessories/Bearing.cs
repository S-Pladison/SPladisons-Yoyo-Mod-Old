using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class Bearing : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + "Bearing";

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 40;
            Item.height = 36;

            Item.rare = ItemRarityID.Blue;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetPladPlayer().bearingEquipped = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddRecipeGroup(RecipeGroupID.IronBar, 7).AddTile(TileID.Anvils).Register();
        }
    }
}
