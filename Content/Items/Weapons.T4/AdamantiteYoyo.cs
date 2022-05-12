using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Drawing;
using SPladisonsYoyoMod.Common.Drawing.AdditionalDrawing;
using SPladisonsYoyoMod.Common.Drawing.Primitives;
using SPladisonsYoyoMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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

    public class AdamantiteYoyoProjectile : YoyoProjectile, IPostUpdateCameraPosition
    {
        public static readonly Color LightColor = new(0.3f, 0.9f, 1f);

        public static Effect TrailEffect { get; private set; }

        private PrimitiveStrip[] trails;

        // ...

        public AdamantiteYoyoProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;

            if (Main.dedServ) return;

            TrailEffect = ModAssets.GetEffect("AdamantiteYoyoTrail", AssetRequestMode.ImmediateLoad).Value;
            TrailEffect.Parameters["Texture0"].SetValue(ModAssets.GetExtraTexture(11, AssetRequestMode.ImmediateLoad).Value);
        }

        public override void OnSpawn(IEntitySource source) => InitTrails();

        public override void AI()
        {
            Projectile.rotation -= 0.15f;

            Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type];

            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                var progress = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                var drawPosTrail = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5f + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture.Value, drawPosTrail, null, lightColor * 0.15f * progress, Projectile.rotation + k * 0.35f, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        private void InitTrails()
        {
            trails = new PrimitiveStrip[2];

            for (int i = 0; i < 2; i++)
            {
                trails[i] = new PrimitiveStrip(GetTrailWidth, GetTrailColor, TrailEffect);
            }
        }

        private float GetTrailWidth(float progress) => 3f * (1 - progress * 0.4f);
        private Color GetTrailColor(float progress) => LightColor * (1 - progress) * 0.8f;

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModContent.Request<Texture2D>(Texture + "_Effect").Value;
            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.Additive, new(texture, drawPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0));

            for (int i = 0; i < 2; i++)
            {
                var pos = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY + new Vector2(3f, 0).RotatedBy(i * MathHelper.Pi + Projectile.rotation);
                var trail = trails[i];

                trail.UpdatePointsAsSimpleTrail(pos, 25, 16 * 8f);
                PrimitiveSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.Additive, trail);
            }
        }
    }
}