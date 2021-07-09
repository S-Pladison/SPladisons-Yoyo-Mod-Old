using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Content.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static SPladisonsYoyoMod.Common.Primitives;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class TheStellarThrow : YoyoItem
    {
        public TheStellarThrow() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "The Stellar Throw", rus: "Звездный бросок");
            this.SetTooltip(eng: "...",
                            rus: "...");
        }

        public override void YoyoSetDefaults()
        {
            Item.damage = 99;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<TheStellarThrowProjectile>();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class TheStellarThrowProjectile : YoyoProjectile
    {
        public TheStellarThrowProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "The Stellar Throw", rus: "Звездный бросок");
        }

        public override void OnSpawn()
        {
            static Color Gradient(float p)
            {
                Color ret;
                if (p <= 0.5f) ret = Color.Lerp(new Color(255, 206, 90), new Color(255, 55, 125), p * 2f);
                else ret = Color.Lerp(new Color(255, 55, 125), new Color(137, 59, 114), (p - 0.5f) * 2f);
                return ret * (1 - p);
            }

            TriangularTrail trail = new(length: 16 * 10, width: (p) => 21 * (1 - p * 0.44f), color: Gradient);
            trail.SetEffectTexture(ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_7").Value);
            trail.SetDissolveSpeed(0.15f);
            SPladisonsYoyoMod.Primitives.CreateTrail(target: Projectile, trail: trail);
        }

        public readonly Color[] DustColors = new Color[] { new Color(11, 25, 25), new Color(16, 11, 25), new Color(25, 11, 18) };

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
                    dust.color = this.DustColors[Main.rand.Next(DustColors.Length)];
                    dust.scale = Main.rand.NextFloat(0.5f, 1.7f);
                }

                dust = Main.dust[Dust.NewDust(Projectile.Center - Vector2.One * 11, 21, 21, ModContent.DustType<Dusts.StarDust>())];
                dust.velocity = -Vector2.Normalize(Projectile.velocity);
            }
        }

        public override bool PreDrawExtras()
        {
            Vector2 drawPosition = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;

            SetSpriteBatch(SpriteSortMode.Deferred, BlendState.Additive);
            var texture = ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_8").Value;
            Main.spriteBatch.Draw(texture, drawPosition, null, new Color(16, 11, 25, 150), Projectile.rotation * 0.05f, texture.Size() * 0.5f + new Vector2(0, 6), 0.3f, SpriteEffects.None, 0);

            SetSpriteBatch();
            texture = ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_5").Value;
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, 1.3f, SpriteEffects.None, 0);

            return true;
        }
    }
}
