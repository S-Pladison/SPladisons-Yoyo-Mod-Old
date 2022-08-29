using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Graphics.Primitives;
using System.Collections.Generic;
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

        private PrimitiveStrip[] trails;

        // ...

        public AdamantiteYoyoProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }

        public override void OnSpawn(IEntitySource source) => InitTrails();

        public override void AI()
        {
            Projectile.rotation -= 0.15f;

            Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.1f);

            var adamantitePlayer = Main.player[Projectile.owner].GetModPlayer<AdamantiteYoyoPlayer>();

            if (adamantitePlayer.BuffIsActive)
            {
                Projectile.localAI[1] = MathHelper.Min(Projectile.localAI[1] + 0.035f, 1f);
                return;
            }

            Projectile.localAI[1] = MathHelper.Max(Projectile.localAI[1] - 0.1f, 0f);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            var adamantitePlayer = Main.player[Projectile.owner].GetModPlayer<AdamantiteYoyoPlayer>();
            adamantitePlayer.AddNPCTimer(target);

            if (!adamantitePlayer.BuffIsActive) return;

            damage = (int)(damage * 1.33f);
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
            /*trails = new PrimitiveStrip[2];
            var effect = ModAssets.GetEffect("AdamantiteYoyoTrail").Value;

            for (int i = 0; i < 2; i++)
            {
                trails[i] = new PrimitiveStrip(GetTrailWidth, GetTrailColor, effect);
            }*/
        }

        private float GetTrailWidth(float progress) => 3f * (1 - progress * 0.4f);
        private Color GetTrailColor(float progress) => LightColor * (1 - progress) * 0.8f;

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            /*var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModContent.Request<Texture2D>(Texture + "_Effect").Value;

            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.Additive, new(texture, drawPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0));

            var mult = Projectile.localAI[1];
            texture = ModAssets.GetExtraTexture(33).Value;

            for (int i = 0; i < 3; i++)
            {
                var position = drawPosition + new Vector2(2 * mult, 0).RotatedBy(MathHelper.TwoPi / 3f * i + Main.GlobalTimeWrappedHourly);
                AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverTiles, DrawTypeFlags.Additive, new(texture, position, null, LightColor * 0.7f * mult, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0));
            }

            for (int i = 0; i < 2; i++)
            {
                var pos = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY + new Vector2(3f, 0).RotatedBy(i * MathHelper.Pi + Projectile.rotation);
                var trail = trails[i];

                trail.UpdatePointsAsSimpleTrail(pos, 5, 16f);
                PrimitiveSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.Additive, trail);
            }*/
        }
    }

    public class AdamantiteYoyoPlayer : ModPlayer
    {
        private readonly Dictionary<NPC, int> timers = new();

        public override void PostUpdate()
        {
            foreach (var npc in timers.Keys)
            {
                timers[npc]--;

                if (!npc.active || timers[npc] <= 0)
                {
                    timers.Remove(npc);
                }
            }
        }

        public void AddNPCTimer(NPC npc)
        {
            timers[npc] = 60 * 8;
        }

        public bool BuffIsActive => timers.Count <= 2;
    }
}