using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Content.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class Blackhole : YoyoItem
    {
        public Blackhole() : base(gamepadExtraRange: 13) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<BlackholeProjectile>();

            Item.rare = ItemRarityID.Yellow;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BlackholeProjectile : YoyoProjectile
    {
        public static Asset<Effect> BlackholeEffect { get; private set; }
        private static readonly float _radius = 16 * 9;

        private float _radiusProgress = 0;
        private float _pulse = 0;

        // ...

        public BlackholeProjectile() : base(lifeTime: 14f, maxRange: 275f, topSpeed: 16f) { }

        public override bool IsSoloYoyo() => true;
        public override void Unload() => BlackholeEffect = null;

        public override void YoyoSetStaticDefaults()
        {
            if (Main.dedServ) return;

            BlackholeEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/Blackhole", AssetRequestMode.ImmediateLoad);
            BlackholeEffect.Value.Parameters["texture1"].SetValue(ModContent.Request<Texture2D>("Terraria/Images/Misc/Perlin").Value);
            BlackholeEffect.Value.Parameters["width"].SetValue(SPladisonsYoyoMod.GetExtraTextures[2].Width() / 4);
        }

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
                float currentRadius = _radius * _radiusProgress * (this.YoyoGloveActivated ? 1.25f : 1f);

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

            Lighting.AddLight(Projectile.Center, new Vector3(171 / 255f, 97 / 255f, 255 / 255f) * 0.45f * _radiusProgress);
            this._pulse = MathHelper.SmoothStep(-0.05f, 0.05f, (float)Math.Abs(Math.Sin(Projectile.localAI[0] * 0.01f)));

            if (Projectile.localAI[0] % 4 == 0 && Projectile.ai[0] != -1f)
            {
                var vector = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                var dust = Dust.NewDustPerfect(Projectile.Center + vector * 75, ModContent.DustType<BlackholeDust>(), vector.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(4, 8));
                dust.customData = this.Projectile;
            }
        }

        public void UpdateRadius(bool flag)
        {
            _radiusProgress += !flag ? 0.05f : -0.1f;

            if (_radiusProgress > 1) _radiusProgress = 1;
            if (_radiusProgress < 0) _radiusProgress = 0;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (this.YoyoGloveActivated) damage = (int)(damage * 1.2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawPosition = GetDrawPosition();
            var texture = SPladisonsYoyoMod.GetExtraTextures[2].Value;
            var effect = BlackholeEffect.Value;
            var timer = Projectile.localAI[0] * 0.025f;

            SetSpriteBatch(SpriteSortMode.Immediate, BlendState.Additive, effect);
            {
                effect.Parameters["time"].SetValue(timer);
                Main.EntitySpriteDraw(texture, drawPosition, null, new Color(95, 65, 255) * 0.75f * _radiusProgress, timer, texture.Size() * 0.5f, _radiusProgress * 0.5f - _pulse, SpriteEffects.None, 0);
            }
            SetSpriteBatch(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect);
            {
                Main.EntitySpriteDraw(texture, drawPosition, null, new Color(35, 0, 100) * 0.5f * _radiusProgress, timer, texture.Size() * 0.5f, _radiusProgress * 0.35f - _pulse, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPosition, null, Color.Black * _radiusProgress, timer, texture.Size() * 0.5f, _radiusProgress * 0.2f - _pulse, SpriteEffects.None, 0);
            }
            SetSpriteBatch();

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            var texture = SPladisonsYoyoMod.GetExtraTextures[3];
            var drawPosition = GetDrawPosition();

            SetSpriteBatch(SpriteSortMode.Deferred, BlendState.Additive);
            {
                Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White * _radiusProgress, (float)Math.Sin(Projectile.localAI[0] * 0.0125f), texture.Size() * 0.5f, (_radiusProgress - _pulse * 5f) * 1.3f, SpriteEffects.None, 0);
                texture = SPladisonsYoyoMod.GetExtraTextures[4];
                Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White * _radiusProgress, Projectile.rotation * 0.33f, texture.Size() * 0.5f, _radiusProgress * 0.3f, SpriteEffects.None, 0);
            }
            SetSpriteBatch();

            texture = SPladisonsYoyoMod.GetExtraTextures[5];
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.Black, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * _radiusProgress, SpriteEffects.None, 0);
        }
    }
}
