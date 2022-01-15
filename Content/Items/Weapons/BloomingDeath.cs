using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class BloomingDeath : YoyoItem
    {
        public BloomingDeath() : base(gamepadExtraRange: 6) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 12;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<BloomingDeathProjectile>();

            Item.rare = ItemRarityID.Blue;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BloomingDeathProjectile : YoyoProjectile
    {
        public BloomingDeathProjectile() : base(lifeTime: 7f, maxRange: 170f, topSpeed: 11f) { }

        public override void OnSpawn()
        {
            Projectile.localAI[1] = Main.rand.Next(1337);
        }

        public override void AI()
        {
            float _ = float.NaN;
            foreach (var target in Main.npc)
            {
                if (target == null || !target.active) continue;
                if (target.type != NPCID.TargetDummy && (target.friendly || target.lifeMax <= 5 || target.dontTakeDamage || target.immortal)) continue;

                if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), Projectile.Center, Main.player[Projectile.owner].MountedCenter, 8, ref _))
                {
                    target.AddBuff(ModContent.BuffType<Buffs.ImprovedPoisoningDebuff>(), 60 * 7);
                }
            }

            Projectile.rotation -= 0.25f;

            if (Projectile.velocity.Length() >= 1f && Main.rand.Next((int)(Projectile.velocity.Length())) > 1 && Main.rand.NextBool(3))
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, 115, Vector2.Normalize(-Projectile.velocity).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(1f, 3f), 140, default, Main.rand.NextFloat(0.4f, 0.8f));
                dust.noLight = true;
            }
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.ImprovedPoisoningDebuff>(), 60 * 5);

            for (int i = 0; i < 7; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, 115, Vector2.Normalize(-Projectile.velocity).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(1f, 3f), 140, default, Main.rand.NextFloat(0.4f, 0.8f));
                dust.noLight = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var stringPosition = Main.player[Projectile.owner].MountedCenter + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY + ModUtils.GetYoyoStringOffset();
            ModUtils.DrawYoyoString(Projectile, stringPosition, this.DrawStrangeString);

            var drawPosition = GetDrawPosition();
            var color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16f), new Color(230, 230, 230, 230));
            var texture = SPladisonsYoyoMod.GetExtraTextures[12];

            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(texture.Value, drawPosition, null, color * 0.15f, Projectile.rotation * 0.05f + (MathHelper.TwoPi / 5 * i), new Vector2(texture.Width() * 0.5f, texture.Height()), 1f, SpriteEffects.None, 0);
            }

            return true;
        }

        private void DrawStrangeString(Vector2 position, float rotation, float height, Color color, int i)
        {
            i += (int)Projectile.localAI[1];
            rotation += (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.25f;
            position += new Vector2(-7, 0).RotatedBy(rotation);

            var texture = SPladisonsYoyoMod.GetExtraTextures[10];
            var colorProgress = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f + i * 0.3f);
            var flip = i % 2 == 0 ? 1 : -1;
            var rectangle = new Rectangle(0, texture.Height() / 3 * (int)(i % 3), texture.Width(), texture.Height() / 3);

            if (i % 3 == 0 || i % 4 == 0)
            {
                SpriteEffects effect = flip > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Main.EntitySpriteDraw(
                    color: color * Math.Min(i / 12.0f, 0.4f) * colorProgress,
                    texture: texture.Value,
                    position: position,
                    sourceRectangle: rectangle,
                    rotation: rotation,
                    origin: new Vector2(4 * flip, 0),
                    scale: 1f,
                    effects: effect,
                    worthless: 0
                );
            }

            flip = -flip;

            if (i % 5 == 0 || i % 7 == 0)
            {
                SpriteEffects effect = flip > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Main.EntitySpriteDraw(
                    color: color * Math.Min(i / 12.0f, 0.15f) * colorProgress,
                    texture: texture.Value,
                    position: position,
                    sourceRectangle: rectangle,
                    rotation: rotation,
                    origin: new Vector2(6 * flip, 0),
                    scale: 0.92f,
                    effects: effect,
                    worthless: 0
                );
            }
        }
    }
}
