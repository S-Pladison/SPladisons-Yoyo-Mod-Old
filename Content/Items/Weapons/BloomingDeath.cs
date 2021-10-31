using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Interfaces;
using System;
using Terraria;
using Terraria.GameContent;
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
            this.DrawCustomString(Main.player[Projectile.owner].MountedCenter);

            var drawPosition = GetDrawPosition();
            var texture = SPladisonsYoyoMod.GetExtraTextures[12];
            var color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16f), new Color(230, 230, 230, 230));

            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(texture.Value, drawPosition, null, color * 0.15f, Projectile.rotation * 0.05f + (MathHelper.TwoPi / 5 * i), new Vector2(texture.Width() * 0.5f, texture.Height()), 1f, SpriteEffects.None, 0);
            }

            return true;
        }

        private void DrawCustomString(Vector2 startPosition)
        {
            var offset = Vector2.Zero; // ...
            var vector = startPosition;
            vector.Y += Main.player[Projectile.owner].gfxOffY;

            var player = Main.player[Projectile.owner];
            var num2 = Projectile.Center.X - vector.X;
            var num3 = Projectile.Center.Y - vector.Y;

            // Hand rotation
            {
                int num5 = -1;
                if (Projectile.position.X + (float)(Projectile.width / 2) < player.position.X + (float)(player.width / 2)) num5 = 1;
                num5 *= -1;
                player.itemRotation = (float)Math.Atan2(num3 * (float)num5, num2 * (float)num5);
            }

            var flag = true;
            if (num2 == 0f && num3 == 0f) flag = false;
            else
            {
                float num6 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                num6 = 12f / num6;
                num2 *= num6;
                num3 *= num6;
                vector.X -= num2 * 0.1f;
                vector.Y -= num3 * 0.1f;
                num2 = Projectile.position.X + Projectile.width * 0.5f - vector.X;
                num3 = Projectile.position.Y + Projectile.height * 0.5f - vector.Y;
            }

            var counter = (int)Projectile.localAI[1];
            while (flag)
            {
                float num8 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                float num9 = num8;

                if (float.IsNaN(num8) || float.IsNaN(num9))
                {
                    flag = false;
                    continue;
                }

                if (num8 < 20f)
                {
                    flag = false;
                }

                num8 = 12f / num8;
                num2 *= num8;
                num3 *= num8;
                vector.X += num2;
                vector.Y += num3;
                num2 = Projectile.position.X + Projectile.width * 0.5f - vector.X - offset.X;
                num3 = Projectile.position.Y + Projectile.height * 0.1f - vector.Y - offset.Y;

                if (num9 > 12f)
                {
                    var num10 = 0.3f;
                    var num11 = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
                    if (num11 > 16f) num11 = 16f;

                    num11 = 1f - num11 / 16f;
                    num10 *= num11;
                    num11 = num9 / 80f;
                    if (num11 > 1f) num11 = 1f;

                    num10 *= num11;
                    if (num10 < 0f) num10 = 0f;

                    num10 *= num11;
                    num10 *= 0.5f;

                    if (num3 > 0f)
                    {
                        num3 *= 1f + num10;
                        num2 *= 1f - num10;
                    }
                    else
                    {
                        num11 = Math.Abs(Projectile.velocity.X) / 3f;
                        if (num11 > 1f) num11 = 1f;

                        num11 -= 0.5f;
                        num10 *= num11;
                        if (num10 > 0f) num10 *= 2f;

                        num3 *= 1f + num10;
                        num2 *= 1f - num10;
                    }
                }

                counter++;
                var num4 = (float)Math.Atan2(num3, num2) - 1.57f;
                var position = vector - Main.screenPosition + new Vector2(6, 6) - new Vector2(6f, 0f);
                var color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f), Color.White);
                var colorProgress = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f + counter * 0.3f);
                var texture = SPladisonsYoyoMod.GetExtraTextures[10];

                // Leafs
                {
                    num4 += (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.25f;
                    position += new Vector2(-7, 0).RotatedBy(num4);

                    var flip = counter % 2 == 0 ? 1 : -1;
                    var rectangle = new Rectangle(0, texture.Height() / 3 * (int)(counter % 3), texture.Width(), texture.Height() / 3);

                    if (counter % 3 == 0 || counter % 4 == 0)
                    {
                        SpriteEffects effect = flip > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                        Main.EntitySpriteDraw(color: color * 0.4f * colorProgress, texture: texture.Value, position: position, sourceRectangle: rectangle, rotation: num4, origin: new Vector2(4 * flip, 0), scale: 1f, effects: effect, worthless: 0);
                    }

                    flip = -flip;
                    if (counter % 5 == 0 || counter % 7 == 0)
                    {
                        SpriteEffects effect = flip > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                        Main.EntitySpriteDraw(color: color * 0.15f * colorProgress, texture: texture.Value, position: position, sourceRectangle: rectangle, rotation: num4, origin: new Vector2(6 * flip, 0), scale: 0.92f, effects: effect, worthless: 0);
                    }
                }
            }
        }
    }
}
