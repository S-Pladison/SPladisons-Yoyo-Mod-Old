using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class TheFinestHour : YoyoItem
    {
        public TheFinestHour() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 200;
            Item.knockBack = 4.5f;

            Item.shoot = ModContent.ProjectileType<TheFinestHourProjectile>();

            Item.rare = ItemRarityID.Red;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Terrarian);
            recipe.AddIngredient<Blackhole>();
            recipe.AddIngredient<MyocardialInfarction>();
            recipe.AddIngredient<TheStellarThrow>();
            recipe.AddIngredient<ResidualLight>();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }

    public class TheFinestHourProjectile : YoyoProjectile
    {
        public static Asset<Effect> TrailEffect { get; private set; }

        // ...

        public TheFinestHourProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 10f) { }

        public override void Load()
        {
            if (Main.dedServ) return;

            TrailEffect = ModAssets.GetEffect("TheFinestHour");
        }

        public override void Unload()
        {
            TrailEffect = null;
        }

        public override void YoyoSetStaticDefaults()
        {
            TrailEffect?.Value.Parameters["texture1"].SetValue(ModAssets.GetExtraTexture(27).Value);
        }

        public override bool IsSoloYoyo() => true;

        public override void OnSpawn()
        {
            /*SimpleTrail trail = new SimpleTrail(
                target: Projectile,
                length: 16 * 8,
                width: (p) => 22,
                color: (p) => Color.Lerp(new Color(255, 225, 90), new Color(115, 35, 255), p),
                effect: TrailEffect,
                additive: true
            );
            trail.SetDissolveSpeed(1f);
            PrimitiveTrailSystem.NewTrail(trail);*/
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
