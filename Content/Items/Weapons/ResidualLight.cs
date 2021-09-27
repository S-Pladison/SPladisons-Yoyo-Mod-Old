using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Content.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class ResidualLight : YoyoItem
    {
        public ResidualLight() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<ResidualLightProjectile>();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.2f * Item.scale);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    public class ResidualLightProjectile : YoyoProjectile
    {
        public static Asset<Effect> ResidualLightEffect { get; private set; }

        // ...

        public ResidualLightProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 12f) { }

        public override bool IsSoloYoyo() => true;

        public override void Load()
        {
            if (Main.dedServ) return;

            ResidualLightEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/ResidualLight");
        }

        public override void Unload()
        {
            ResidualLightEffect = null;
        }

        public override void YoyoSetStaticDefaults()
        {
            ResidualLightEffect?.Value.Parameters["texture1"].SetValue(SPladisonsYoyoMod.GetExtraTextures[20].Value);
        }

        public override void OnSpawn()
        {
            TriangularTrail trail = new
            (
                target: Projectile,
                length: 16 * 10,
                width: (p) => 20,
                color: (p) => Color.White * (1 - p),
                effect: ResidualLightEffect,
                blendState: BlendState.Additive,
                tipLength: 3
            );
            trail.SetDissolveSpeed(0.2f);
            trail.SetMaxPoints(20);
            PrimitiveTrailSystem.NewTrail(trail);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.2f * Projectile.scale);
        }

        public override void ModifyYoyo(ref float lifeTime, ref float maxRange, ref float topSpeed, bool infinite)
        {
            if (!Main.dayTime) maxRange += 0.2f;
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ResidualLightHitProjectile>(), (int)(YoyoGloveActivated ? Projectile.damage * 0.5f : 0), 0, Projectile.owner);
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(ref Color lightColor)
        {
            SetSpriteBatch(SpriteSortMode.Deferred, BlendState.Additive);
            {
                var texture = SPladisonsYoyoMod.GetExtraTextures[4];
                Main.EntitySpriteDraw(texture.Value, GetDrawPosition(), null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None, 0);
            }
            SetSpriteBatch();
            return true;
        }
    }

    public class ResidualLightHitProjectile : PladProjectile
    {
        private readonly Color[] _colors = new Color[] { new Color(252, 222, 252), new Color(202, 243, 248), new Color(254, 234, 185) };
        private Color _color;

        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Misc/Extra_0";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.friendly = true;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn()
        {
            Projectile.rotation += Main.rand.NextFloat(MathHelper.TwoPi);

            _color = _colors[Main.rand.Next(_colors.Length)];
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f;
            Projectile.scale = ModUtils.GradientValue<float>(MathHelper.Lerp, 1 - Projectile.timeLeft / 20f, new float[] { 1f, 1.2f, 0.6f, 0f });

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.3f * Projectile.scale);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var position = GetDrawPosition();
            SetSpriteBatch(SpriteSortMode.Deferred, BlendState.Additive);
            {
                var texture = SPladisonsYoyoMod.GetExtraTextures[21];
                Main.spriteBatch.Draw(texture.Value, position, null, _color, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.6f, SpriteEffects.None, 0);

                texture = SPladisonsYoyoMod.GetExtraTextures[3];
                Main.spriteBatch.Draw(texture.Value, position, null, _color, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }
            SetSpriteBatch();
            return false;
        }
    }
}
