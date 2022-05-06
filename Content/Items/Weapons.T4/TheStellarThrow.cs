using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Drawing;
using SPladisonsYoyoMod.Common.Drawing.AdditionalDrawing;
using SPladisonsYoyoMod.Common.Drawing.Primitives;
using SPladisonsYoyoMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class TheStellarThrow : YoyoItem
    {
        public TheStellarThrow() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 99;
            Item.knockBack = 2.5f;
            Item.crit = 10;

            Item.shoot = ModContent.ProjectileType<TheStellarThrowProjectile>();

            Item.rare = ItemRarityID.Pink;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class TheStellarThrowProjectile : YoyoProjectile, IPostUpdateCameraPosition
    {
        public static readonly Color[] TrailColors = new Color[] { new Color(255, 206, 90), new Color(255, 55, 125), new Color(137, 59, 114) };
        public static readonly Color[] DustColors = new Color[] { new Color(11, 25, 25), new Color(16, 11, 25), new Color(25, 11, 18) };

        private static Effect Effect { get; set; }

        // ...

        private PrimitiveStrip Trail { get; set; }

        // ...

        public TheStellarThrowProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void Load()
        {
            if (Main.dedServ) return;

            Effect = ModAssets.GetEffect("Primitive", AssetRequestMode.ImmediateLoad).Value;
            Effect.Parameters["Texture0"].SetValue(ModAssets.GetExtraTexture(7, AssetRequestMode.ImmediateLoad).Value);
        }

        public override void Unload()
        {
            Effect = null;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Trail = new PrimitiveStrip(
                width: (progress) => 18f * (1 - progress * 0.44f),
                color: (progress) => MathUtils.MultipleLerp(Color.Lerp, progress, TrailColors) * (1 - progress),
                effect: Effect
            );

            /*PrimitiveTrail.Create(Projectile, t =>
            {
                t.SetColor(new GradientTrailColor(colors: TrailColors, disappearOverTime: true));
                t.SetTip(new RoundedTrailTip(smoothness: 20));
                t.SetWidth(p => 21 * (1 - p * 0.44f));
                t.SetUpdate(new BoundedTrailUpdate(points: 15, length: 16 * 10));
                t.SetEffectTexture(ModAssets.GetExtraTexture(7, AssetRequestMode.ImmediateLoad).Value);
                t.SetDissolveSpeed(0.26f);
            });*/
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() >= 1f && Main.rand.Next((int)Projectile.velocity.Length()) > 1)
            {
                Dust dust;

                if (Main.rand.NextBool(3))
                {
                    dust = Main.dust[Dust.NewDust(Projectile.Center - Vector2.One * 11, 21, 21, ModContent.DustType<Dusts.CircleDust>())];
                    dust.velocity = -Vector2.Normalize(Projectile.velocity);
                    dust.alpha = 10;
                    dust.color = DustColors[Main.rand.Next(DustColors.Length)];
                    dust.scale = Main.rand.NextFloat(0.5f, 1.7f);
                }

                dust = Main.dust[Dust.NewDust(Projectile.Center - Vector2.One * 11, 21, 21, ModContent.DustType<Dusts.StarDust>())];
                dust.velocity = -Vector2.Normalize(Projectile.velocity);
            }
        }

        public override bool PreDrawExtras()
        {
            Main.EntitySpriteDraw(ModAssets.GetExtraTexture(5).Value, GetDrawPosition(), null, Color.White, Projectile.rotation, ModAssets.GetExtraTexture(5).Size() * 0.5f, 1.3f, SpriteEffects.None, 0);
            return true;
        }

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            var drawPosition = GetDrawPosition();
            var origin = ModAssets.GetExtraTexture(8).Size() * 0.5f + new Vector2(0, 7);
            var starRotation = Main.GlobalTimeWrappedHourly;
            var starScalePulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.15f;

            void DrawStar(Color color, float rotation, float scale)
            {
                AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.All, new(ModAssets.GetExtraTexture(8).Value, drawPosition, null, color, rotation, origin, (scale + starScalePulse) * Vector2.One, SpriteEffects.None));
            }

            DrawStar(new Color(16, 11, 25, 210), -starRotation, 0.55f);
            DrawStar(new Color(16, 11, 25, 240), starRotation, 0.35f);

            Trail.UpdatePointsAsSimpleTrail(Projectile.Center + Projectile.gfxOffY * Vector2.UnitY, 25, 16 * 7);
            PrimitiveSystem.AddToDataCache(DrawLayers.OverTiles, DrawTypeFlags.Additive, Trail);
        }
    }
}
