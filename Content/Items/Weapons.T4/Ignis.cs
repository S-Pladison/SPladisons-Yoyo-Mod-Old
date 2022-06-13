using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Drawing;
using SPladisonsYoyoMod.Common.Drawing.AdditionalDrawing;
using SPladisonsYoyoMod.Common.Drawing.Particles;
using SPladisonsYoyoMod.Common.Drawing.Primitives;
using SPladisonsYoyoMod.Content.Particles;
using SPladisonsYoyoMod.Utilities;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class Ignis : YoyoItem
    {
        public Ignis() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetStaticDefaults()
        {
            SPladisonsYoyoMod.Sets.ItemCustomInventoryScale[Type] = 0.96f;
        }

        public override void YoyoSetDefaults()
        {
            Item.width = 34;
            Item.height = 32;

            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<IgnisProjectile>();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.WoodYoyo).AddIngredient(ItemID.LivingFireBlock, 25).AddIngredient(ItemID.Torch, 99).Register();
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = TextureAssets.Item[Type];
            spriteBatch.Draw(texture.Value, Item.Center + new Vector2(0, 2) - Main.screenPosition, null, lightColor, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class IgnisProjectile : YoyoProjectile, IPostUpdateCameraPosition
    {
        public static float TrailTextureWidth { get; private set; }

        // ...

        private PrimitiveStrip trail;
        private int timer;

        // ...

        public IgnisProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void Load()
        {
            if (Main.dedServ) return;

            TrailTextureWidth = ModAssets.GetExtraTexture(17, AssetRequestMode.ImmediateLoad).Width();
        }

        public override void OnSpawn(IEntitySource source)
        {
            trail = new PrimitiveStrip(GetTrailWidth, GetTrailColor, ModAssets.GetEffect("IgnisTrail").Value);
            trail.OnUpdateEffectParameters += UpdateTrailEffect;
        }

        public override void AI()
        {
            Projectile.rotation -= 0.15f;
            timer++;

            for (int i = 0; i < 5; i++)
            {
                var position = Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(20);
                var velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(3);

                Particle.NewParticle(DrawLayers.OverWalls, DrawTypeFlags.None, ParticleSystem.ParticleType<SmokeParticle>(), position, velocity, Color.Black, 200, Main.rand.NextFloat(MathHelper.TwoPi), Main.rand.NextFloat(0.15f, 0.7f));
            }

            if (timer % 2 == 0)
            {
                var vector = Vector2.One.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                var dust = Dust.NewDustPerfect(Projectile.Center + vector * Main.rand.NextFloat(0, 24), 6, Velocity: vector * Main.rand.NextFloat(1), Scale: Main.rand.NextFloat(0.8f, 2.5f));
                dust.noGravity = true;
            }

            if (timer % 3 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IgnisSmokeProjectile>(), Projectile.damage, 0f, Projectile.owner);
            }
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * 6);
        }

        public override void YoyoSendExtraAI(BinaryWriter writer) => writer.Write(timer);
        public override void YoyoReceiveExtraAI(BinaryReader reader) => timer = reader.ReadInt32();

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type];
            var position = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;

            Main.EntitySpriteDraw(texture.Value, position, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        private void UpdateTrailEffect(Effect effect)
        {
            effect.Parameters["Time"].SetValue(-timer * 0.05f);
            effect.Parameters["Width"].SetValue(trail.Points.TotalDistance() / TrailTextureWidth);
        }

        private float GetTrailWidth(float progress) => 28f * (1 - progress * 0.2f);
        private Color GetTrailColor(float progress) => Color.Lerp(new Color(255, 196, 63), new Color(220, 7, 7), progress) * (1 - progress) * ReturningProgress;

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModAssets.GetExtraTexture(4).Value;
            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverTiles, DrawTypeFlags.Additive, new(texture, drawPosition, null, new Color(239, 137, 37), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None, 0));

            trail.UpdatePointsAsSimpleTrail(Projectile.Center + Projectile.gfxOffY * Vector2.UnitY, 30, 16 * 12f);
            PrimitiveSystem.AddToDataCache(DrawLayers.OverTiles, DrawTypeFlags.None, trail);
        }
    }

    public class IgnisSmokeProjectile : ModProjectile
    {
        public override string Texture => ModAssets.InvisiblePath;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;

            Projectile.width = 50;
            Projectile.height = Projectile.width;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }
    }
}
