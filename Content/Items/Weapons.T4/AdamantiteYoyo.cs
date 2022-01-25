using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class AdamantiteYoyo : YoyoItem
    {
        public AdamantiteYoyo() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<AdamantiteYoyoProjectile>();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.AdamantiteBar, 12).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    public class AdamantiteYoyoProjectile : YoyoProjectile
    {
        public AdamantiteYoyoProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }
    }
}