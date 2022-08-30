using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Graphics;
using SPladisonsYoyoMod.Common.Graphics.Primitives;
using SPladisonsYoyoMod.Common.Graphics.Particles;
using SPladisonsYoyoMod.Content.Particles;
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

        public override void AddRecipes()
        {
            var r = CreateRecipe();
            r.AddIngredient(ItemID.Starfury);
            r.AddIngredient(ItemID.Code1);
            r.AddIngredient(ItemID.SoulofSight, 15);
            r.AddIngredient(ItemID.SoulofFlight, 40);
            r.AddIngredient(ItemID.CobaltBar, 7);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();

            r = CreateRecipe();
            r.AddIngredient(ItemID.Starfury);
            r.AddIngredient(ItemID.Code1);
            r.AddIngredient(ItemID.SoulofSight, 15);
            r.AddIngredient(ItemID.SoulofFlight, 40);
            r.AddIngredient(ItemID.PalladiumBar, 7);
            r.AddTile(TileID.MythrilAnvil);
            r.Register();
        }
    }

    public class TheStellarThrowProjectile : YoyoProjectile, IDrawOnDifferentLayers
    {
        public static readonly Color[] ProjectileColors = new Color[] { new Color(255, 206, 90), new Color(255, 55, 125), new Color(137, 59, 114) };
        public static readonly Color[] DustColors = new Color[] { new Color(11, 25, 25), new Color(16, 11, 25), new Color(25, 11, 18) };

        // ...

        private PrimitiveStrip trail;
        private int timer;

        // ...

        public TheStellarThrowProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void OnSpawn(IEntitySource source)
        {
            trail = new PrimitiveStrip
            (
                width: GetTrailWidth,
                color: GetTrailColor,
                effect: new IPrimitiveEffect.Custom("TheStellarThrowTrail", UpdateTrailEffect),
                headTip: null,
                tailTip: null
            );
        }

        public override void AI()
        {
            timer++;

            if (Projectile.velocity.Length() >= 1f && Main.rand.Next((int)Projectile.velocity.Length()) > 1)
            {
                if (Main.rand.NextBool(3))
                {
                    var dust = Main.dust[Dust.NewDust(Projectile.Center - Vector2.One * 11, 21, 21, ModContent.DustType<Dusts.CircleDust>())];
                    dust.velocity = -Vector2.Normalize(Projectile.velocity);
                    dust.alpha = 10;
                    dust.color = DustColors[Main.rand.Next(DustColors.Length)];
                    dust.scale = Main.rand.NextFloat(0.5f, 1.7f);
                }

                if (Main.rand.NextBool(3))
                {
                    var position = Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(20);
                    var velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(2);
                    Particle.NewParticle<TheStellarThrowTrailParticle>(DrawLayers.Dusts, DrawTypeFlags.Additive, position, velocity);
                }
            }
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 7; i++)
            {
                var position = Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(20);
                var velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(2, 4);
                Particle.NewParticle<TheStellarThrowHitParticle>(DrawLayers.Dusts, DrawTypeFlags.Additive, position, velocity);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            return true;
        }

        private void UpdateTrailEffect(EffectParameterCollection parameter)
        {
            parameter["Time"].SetValue(-timer * 0.05f);
            parameter["Repeats"].SetValue(trail.Points.TotalDistance() / 100f);
        }

        private float GetTrailWidth(float progress) => 44f * (1 - progress * 0.45f);
        private Color GetTrailColor(float progress) => MathUtils.MultipleLerp(Color.Lerp, progress, ProjectileColors) * (1 - progress);

        void IDrawOnDifferentLayers.DrawOnDifferentLayers(DrawSystem system)
        {
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModAssets.GetExtraTexture(8);
            var origin = texture.Size() * 0.5f + new Vector2(0, 7);
            var starRotation = Main.GlobalTimeWrappedHourly;
            var starScalePulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.15f;

            void DrawStar(Color color, float rotation, float scale)
            {
                system.AddToLayer(DrawLayers.Dusts, DrawTypeFlags.All, new DefaultDrawData(texture.Value, drawPosition, null, color, rotation, origin, (scale + starScalePulse) * Projectile.scale, SpriteEffects.None));
            }

            DrawStar(new Color(16, 11, 25, 220), -starRotation, 0.55f);
            DrawStar(new Color(16, 11, 25, 255), starRotation, 0.35f);

            texture = ModAssets.GetExtraTexture(4);
            system.AddToLayer(DrawLayers.Dusts, DrawTypeFlags.Additive, new DefaultDrawData(texture.Value, drawPosition, null, ProjectileColors[1] * 0.3f, 0f, texture.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None));

            texture = ModAssets.GetExtraTexture(3);
            system.AddToLayer(DrawLayers.Dusts, DrawTypeFlags.Additive, new DefaultDrawData(texture.Value, drawPosition, null, ProjectileColors[1] * 0.6f, 0f, texture.Size() * 0.5f, Projectile.scale * 1.4f, SpriteEffects.None));

            texture = ModAssets.GetExtraTexture(34);
            system.AddToLayer(DrawLayers.Dusts, DrawTypeFlags.Additive, new DefaultDrawData(texture.Value, drawPosition, null, Color.White * 0.9f, 0f, texture.Size() * 0.5f, Projectile.scale * 0.15f + starScalePulse * 0.2f, SpriteEffects.None));

            trail.UpdatePointsAsSimpleTrail(Projectile.Center + Projectile.gfxOffY * Vector2.UnitY, 20, 16 * 9 * ReturningProgress);
            system.AddToLayer(DrawLayers.Tiles, DrawTypeFlags.None, trail);
        }
    }
}