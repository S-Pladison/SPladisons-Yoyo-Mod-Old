using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Misc;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class SoulFilledFlame : YoyoItem
    {
        public SoulFilledFlame() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<SoulFilledFlameProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class SoulFilledFlameProjectile : YoyoProjectile, ISoulFilledFlame
    {
        public SoulFilledFlameProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Soul-Filled Flame", rus: "Наполненное душой пламя");

            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 3;
        }

        public override void OnSpawn()
        {
            SoulFilledFlameEffect.Instance?.AddElement(this);

            for (int i = 0; i < Projectile.oldPos.Length; i++) Projectile.oldPos[i] = Projectile.position;
        }

        private float _dustRotation = 0f;

        public override void AI()
        {
            _dustRotation *= 0.9f;

            float dist = Vector2.Distance(Projectile.oldPos.Last(), Projectile.position);
            if (dist >= 1f)
            {
                _dustRotation = (Projectile.oldPos.Last() - Projectile.position).ToRotation() - MathHelper.PiOver2;
                dist = 1 + (dist * 0.1f);
            }
            else dist = 1f;

            var particle = SoulFilledFlameEffect.Instance?.CreateParticle(position: Projectile.Center, scale: 1f, timeLeft: 40, color: Color.Blue, updateAction: UpdateBlueParticles, layerDepth: 1f);
            particle.velocity = new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * 15f) * 0.5f, -1f);

            Vector2 offset = new Vector2(13, 0).RotatedBy(_dustRotation);

            if (Main.rand.Next(1) == 0)
            {
                particle = SoulFilledFlameEffect.Instance?.CreateParticle(position: Projectile.Center + offset, scale: 0.05f * dist, timeLeft: 70, color: Color.Black, updateAction: UpdateRedParticles, layerDepth: 0);
                particle.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), -1f);
            }

            offset = new Vector2(-13, 0).RotatedBy(_dustRotation);

            if (Main.rand.Next(1) == 0)
            {
                particle = SoulFilledFlameEffect.Instance?.CreateParticle(position: Projectile.Center + offset, scale: 0.05f * dist, timeLeft: 70, color: Color.Black, updateAction: UpdateRedParticles, layerDepth: 0);
                particle.velocity = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), -1f);
            }

            /*var dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.SoulFilledFlameDust>(), new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * 15f), -2f));
            dust.customData = 15;
            dust.frame = new Rectangle(0, 0, 1, 1);
            dust.scale = Main.rand.NextFloat(0.9f, 1.1f);
            dust.color = new Color(52, 219, 206);

            dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.SoulFilledFlameDust>(), new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly * 15f), -2f));
            dust.customData = 12;
            dust.frame = new Rectangle(1, 1, 1, 1);
            dust.scale = Main.rand.NextFloat(0.1f, 0.2f);
            dust.color = new Color(42, 142, 120);*/
        }

        public override void Kill(int timeLeft)
        {
            SoulFilledFlameEffect.Instance?.RemoveElement(this);
        }

        public void DrawSoulFilledFlame(SpriteBatch spriteBatch)
        {

        }

        private static void UpdateBlueParticles(Particle particle)
        {
            particle.timeLeft--;
            particle.velocity.X *= 0.9f;
            particle.position += particle.velocity;
        }

        private static void UpdateRedParticles(Particle particle)
        {
            particle.timeLeft--;
            particle.velocity.X *= 0.9f;
            particle.position += particle.velocity;
            particle.scale += 0.03f;
        }
    }
}
