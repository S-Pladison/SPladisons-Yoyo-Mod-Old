using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class TitaniumYoyo : YoyoItem
    {
        public TitaniumYoyo() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 200;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<TitaniumYoyoProjectile>();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TitaniumBar, 13).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    public class TitaniumYoyoProjectile : YoyoProjectile
    {
        public TitaniumYoyoProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                var texture = TextureAssets.Projectile[Type];
                var progress = ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                var drawPosTrail = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5f + new Vector2(0f, Projectile.gfxOffY);

                Main.EntitySpriteDraw(texture.Value, drawPosTrail, null, lightColor * 0.3f * progress, Projectile.rotation + k * 0.35f, texture.Size() * 0.5f, Projectile.scale * (0.95f - (1 - progress) * 0.2f), SpriteEffects.None, 0);
            }

            return true;
        }
    }
}