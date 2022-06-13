using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class TrueBellowingThunder : YoyoItem
    {
        public TrueBellowingThunder() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<TrueBellowingThunderProjectile>();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient<BellowingThunder>();
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 20);
            recipe.AddIngredient(ItemID.SoulofSight, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }

    public class TrueBellowingThunderProjectile : YoyoProjectile
    {
        public TrueBellowingThunderProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }
    }
}