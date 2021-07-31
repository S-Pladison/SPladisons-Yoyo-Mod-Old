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
        public BloomingDeath() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<BloomingDeathProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BloomingDeathProjectile : YoyoProjectile, IDrawCustomString
    {
        private int _key;

        public BloomingDeathProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Blooming Death", rus: "Цветущая смерть");
        }

        public override void OnSpawn()
        {
            _key = Main.rand.Next(1337);
        }

        public override void AI()
        {
            float _ = float.NaN;
            foreach (var target in Main.npc)
            {
                if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), Projectile.Center, Main.player[Projectile.owner].MountedCenter, 8, ref _))
                {
                    target.AddBuff(ModContent.BuffType<Buffs.ImprovedPoisoningDebuff>(), 60 * 7);
                }
            }
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.ImprovedPoisoningDebuff>(), 60 * 5);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.position + new Vector2((float)Projectile.width, (float)Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;

            var texture = ModAssets.ExtraTextures[12];
            var color = Lighting.GetColor((int)Projectile.Center.X / 16, (int)(Projectile.Center.Y / 16f), Color.White);

            for (int i = 0; i < 5; i++) Main.EntitySpriteDraw(texture.Value, drawPosition, null, color * 0.22f, Projectile.rotation * 0.05f + MathHelper.Pi + (MathHelper.TwoPi / 5 * i), new Vector2(9, 20), 1.22f, SpriteEffects.None, 0);
            for (int i = 0; i < 5; i++) Main.EntitySpriteDraw(texture.Value, drawPosition, null, color * 0.88f, Projectile.rotation * 0.05f + (MathHelper.TwoPi / 5 * i), new Vector2(9, 20), 1f, SpriteEffects.None, 0);

            return true;
        }

        public void DrawCustomString(Vector2 startPosition)
        {
            Vector2 offset = Vector2.Zero; // ...
            Vector2 vector = startPosition;
            vector.Y += Main.player[Projectile.owner].gfxOffY;

            Player player = Main.player[Projectile.owner];
            float num2 = Projectile.Center.X - vector.X;
            float num3 = Projectile.Center.Y - vector.Y;

            // Hand rotation
            {
                int num5 = -1;
                if (Projectile.position.X + (float)(Projectile.width / 2) < player.position.X + (float)(player.width / 2)) num5 = 1;
                num5 *= -1;
                player.itemRotation = (float)Math.Atan2(num3 * (float)num5, num2 * (float)num5);
            }

            bool flag = true;
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

            int counter = _key;
            while (flag)
            {
                float num7 = 12f;
                float num8 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
                float num9 = num8;

                if (float.IsNaN(num8) || float.IsNaN(num9))
                {
                    flag = false;
                    continue;
                }

                if (num8 < 20f)
                {
                    num7 = num8 - 8f;
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
                    float num10 = 0.3f;
                    float num11 = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
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

                int frame = Math.Min(counter - _key, 2);
                counter++;

                float num4 = (float)Math.Atan2(num3, num2) - 1.57f;
                Vector2 position = vector - Main.screenPosition + new Vector2(6, 6) - new Vector2(6f, 0f);
                Rectangle sourceRectangle = new Rectangle(0, 12 * frame, ModAssets.ExtraTextures[9].Width(), (int)num7);

                // Set string color
                Color color = Color.White;
                {
                    if (frame == 0) color = TryApplyingPlayerStringColor(player.stringColor);

                    color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f), color);
                    if (frame == 0)
                    {
                        color.A = (byte)(color.A * 0.4f);
                        color *= 0.5f;
                    }
                }

                Main.EntitySpriteDraw(color: color, texture: ModAssets.ExtraTextures[9].Value, position: position, sourceRectangle: sourceRectangle, rotation: num4, origin: new Vector2(6, 0f), scale: 1f, effects: SpriteEffects.None, worthless: 0);

                // Leafs
                {
                    num4 += (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.25f;
                    position += new Vector2(-7, 0).RotatedBy(num4);

                    if (frame != 2 || !flag) continue;

                    int flip = counter % 2 == 0 ? 1 : -1;
                    if (counter % 3 == 0 || counter % 4 == 0)
                    {
                        SpriteEffects effect = flip > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                        Main.EntitySpriteDraw(color: color * 0.75f, texture: ModAssets.ExtraTextures[10].Value, position: position, sourceRectangle: new Rectangle(0, ModAssets.ExtraTextures[10].Height() / 3 * (int)(counter % 3), ModAssets.ExtraTextures[10].Width(), ModAssets.ExtraTextures[10].Height() / 3), rotation: num4, origin: new Vector2(4 * flip, 0), scale: 1f, effects: effect, worthless: 0);
                    }

                    flip = -flip;
                    if (counter % 5 == 0 || counter % 7 == 0)
                    {
                        color = Lighting.GetColor((int)vector.X / 16, (int)(vector.Y / 16f), Color.Gray);
                        SpriteEffects effect = flip > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                        Main.EntitySpriteDraw(color: color * 0.3f, texture: ModAssets.ExtraTextures[10].Value, position: position, sourceRectangle: new Rectangle(0, ModAssets.ExtraTextures[10].Height() / 3 * (int)(counter % 3), ModAssets.ExtraTextures[10].Width(), ModAssets.ExtraTextures[10].Height() / 3), rotation: num4, origin: new Vector2(6 * flip, 0), scale: 0.92f, effects: effect, worthless: 0);
                    }
                }
            }
        }
    }
}
