using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Interfaces;
using SPladisonsYoyoMod.Content.Trails;
using System;
using Terraria;
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

    public class TheStellarThrowProjectile : YoyoProjectile, IDrawAdditive
    {
        public static readonly Color[] TrailColors = new Color[] { new Color(255, 206, 90), new Color(255, 55, 125), new Color(137, 59, 114) };
        public static readonly Color[] DustColors = new Color[] { new Color(11, 25, 25), new Color(16, 11, 25), new Color(25, 11, 18) };

        // ...

        public TheStellarThrowProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void OnSpawn()
        {
            TriangularTrail trail = new
            (
                target: Projectile,
                length: 16 * 10,
                width: (progress) => 21 * (1 - progress * 0.44f),
                color: (progress) => ModUtils.GradientValue<Color>(method: Color.Lerp, percent: progress, values: TrailColors) * (1 - progress),
                additive: true
            );
            trail.SetEffectTexture(SPladisonsYoyoMod.GetExtraTextures[7].Value);
            trail.SetDissolveSpeed(0.26f);
            PrimitiveTrailSystem.NewTrail(trail);
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() >= 1f && Main.rand.Next((int)Projectile.velocity.Length()) > 1)
            {
                Dust dust;

                if (Main.rand.Next(3) == 0)
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
            Main.EntitySpriteDraw(SPladisonsYoyoMod.GetExtraTextures[5].Value, GetDrawPosition(), null, Color.White, Projectile.rotation, SPladisonsYoyoMod.GetExtraTextures[5].Size() * 0.5f, 1.3f, SpriteEffects.None, 0);
            return true;
        }

        void IDrawAdditive.DrawAdditive()
        {
            var drawPosition = GetDrawPosition();
            var origin = SPladisonsYoyoMod.GetExtraTextures[8].Size() * 0.5f + new Vector2(0, 6);
            var starRotation = Main.GlobalTimeWrappedHourly;
            var starScalePulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.15f;

            void DrawStar(Color color, float rotation, float scale)
            {
                Main.EntitySpriteDraw(SPladisonsYoyoMod.GetExtraTextures[8].Value, drawPosition, null, color, rotation, origin, scale + starScalePulse, SpriteEffects.None, 0);
            }

            DrawStar(new Color(16, 11, 25, 90), -starRotation, 0.5f);
            DrawStar(new Color(16, 11, 25, 210), starRotation, 0.3f);
        }
    }
}
