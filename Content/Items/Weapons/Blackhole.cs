using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class Blackhole : YoyoItem
    {
        public Blackhole() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<BlackholeProjectile>();

            Item.rare = ItemRarityID.Cyan;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BlackholeProjectile : YoyoProjectile
    {
        public float radiusProgress = 0;
        public float pulse = 0;

        private readonly float _radius = 16 * 9;

        public BlackholeProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override bool IsSoloYoyo() => true;

        public override void AI()
        {
            this.UpdateRadius(flag: IsReturning);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];

                if (target == null || !target.active) continue;
                if (target.friendly || target.lifeMax <= 5 || target.boss || target.dontTakeDamage || target.immortal) continue;
                if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height)) continue;

                float numX = Projectile.position.X - target.position.X - (float)(target.width / 2);
                float numY = Projectile.position.Y - target.position.Y - (float)(target.height / 2);
                float distance = (float)Math.Sqrt((double)(numX * numX + numY * numY));
                float currentRadius = _radius * radiusProgress * (this.YoyoGloveActivated ? 1.25f : 1f);

                if (distance < currentRadius)
                {
                    distance = 2f / distance;
                    numX *= distance * 3;
                    numY *= distance * 3;

                    target.velocity.X = numX;
                    target.velocity.Y = numY;
                    target.netUpdate = true;
                }
            }

            Lighting.AddLight(Projectile.Center, new Vector3(171 / 255f, 97 / 255f, 255 / 255f) * 0.45f * radiusProgress);

            this.pulse = MathHelper.SmoothStep(-0.05f, 0.05f, (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f)));
        }

        public void UpdateRadius(bool flag)
        {
            radiusProgress += !flag ? 0.05f : -0.1f;

            if (radiusProgress > 1) radiusProgress = 1;
            if (radiusProgress < 0) radiusProgress = 0;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (this.YoyoGloveActivated) damage = (int)(damage * 1.2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Shitty code... I can't do better than that :(

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            var texture = ModAssets.ExtraTextures[2].Value;
            var effect = ModAssets.BlackholeEffect.Value;
            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);

            SetSpriteBatch(SpriteSortMode.Immediate, BlendState.Additive, effect);
            Main.spriteBatch.Draw(texture, drawPosition, null, new Color(95, 65, 255) * 0.75f, Main.GlobalTimeWrappedHourly, texture.Size() * 0.5f, radiusProgress * 0.5f - pulse, SpriteEffects.None, 0);

            SetSpriteBatch(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect);
            Main.spriteBatch.Draw(texture, drawPosition, null, new Color(35, 0, 100) * 0.5f, Main.GlobalTimeWrappedHourly, texture.Size() * 0.5f, radiusProgress * 0.35f - pulse, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.Black, Main.GlobalTimeWrappedHourly, texture.Size() * 0.5f, radiusProgress * 0.2f - pulse, SpriteEffects.None, 0);

            SetSpriteBatch();
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            var texture = ModAssets.ExtraTextures[3];
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            SetSpriteBatch(SpriteSortMode.Deferred, BlendState.Additive);
            {
                Main.spriteBatch.Draw(texture.Value, drawPosition, null, Color.White * radiusProgress, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f), texture.Size() * 0.5f, radiusProgress - pulse * 5f, SpriteEffects.None, 0);
                texture = ModAssets.ExtraTextures[4];
                Main.spriteBatch.Draw(texture.Value, drawPosition, null, Color.White * radiusProgress, Projectile.rotation * 0.33f, texture.Size() * 0.5f, radiusProgress * 0.3f, SpriteEffects.None, 0);
            }
            SetSpriteBatch();

            texture = ModAssets.ExtraTextures[5];
            Main.spriteBatch.Draw(texture.Value, drawPosition, null, Color.Black, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * radiusProgress, SpriteEffects.None, 0);
        }
    }
}
