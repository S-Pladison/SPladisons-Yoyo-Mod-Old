using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Interfaces;
using SPladisonsYoyoMod.Content.Trails;
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
            recipe.AddIngredient<TheStellarThrow>();
            recipe.AddIngredient<ResidualLight>();
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }

    public class TheFinestHourProjectile : YoyoProjectile, IDrawAdditive
    {
        public static Asset<Effect> TrailEffect { get; private set; }

        // ...

        public TheFinestHourProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 10f) { }

        public override void Load()
        {
            if (Main.dedServ) return;

            TrailEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/TheFinestHour");
        }

        public override void Unload()
        {
            TrailEffect = null;
        }

        public override void YoyoSetStaticDefaults()
        {
            TrailEffect?.Value.Parameters["texture1"].SetValue(SPladisonsYoyoMod.GetExtraTextures[27].Value);
        }

        public override bool IsSoloYoyo() => true;

        public override void OnSpawn()
        {
            SimpleTrail trail = new SimpleTrail(
                target: Projectile,
                length: 16 * 8,
                width: (p) => 22,
                color: (p) => Color.Lerp(new Color(255, 225, 90), new Color(115, 35, 255), p),
                effect: TrailEffect,
                additive: true
            );
            trail.SetDissolveSpeed(1f);
            PrimitiveTrailSystem.NewTrail(trail);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        void IDrawAdditive.DrawAdditive()
        {
            Main.EntitySpriteDraw(SPladisonsYoyoMod.GetExtraTextures[8].Value, GetDrawPosition(), null, new Color(251, 239, 223), Projectile.rotation * 0.15f, SPladisonsYoyoMod.GetExtraTextures[8].Size() * 0.5f, Projectile.scale * 0.25f, SpriteEffects.None, 0);
        }
    }
}
