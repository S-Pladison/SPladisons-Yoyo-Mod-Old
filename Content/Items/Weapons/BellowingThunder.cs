using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class BellowingThunder : YoyoItem
    {
        public BellowingThunder() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<BellowingThunderProjectile>();

            Item.rare = ItemRarityID.LightPurple;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BellowingThunderProjectile : YoyoProjectile
    {
        public BellowingThunderProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public int Counter
        {
            get => (int)Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(145, 198, 249).ToVector3() * 0.2f);

            Counter++;
            if (Counter >= 20)
            {
                Counter = 0;
                _effect.Reset();
                return;
            }

            _effect.Update(Counter);
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 drawPosition = Projectile.position + Projectile.Size / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;

            SetSpriteBatch(SpriteSortMode.Deferred, BlendState.Additive);
            {
                _effect.Draw(drawPosition, 0.45f * Projectile.scale);
                Main.EntitySpriteDraw(ModAssets.ExtraTextures[23].Value, drawPosition, null, new Color(137, 90, 248) * 0.4f, 0f, ModAssets.ExtraTextures[23].Size() * 0.5f, Projectile.scale * 0.1f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(ModAssets.ExtraTextures[21].Value, Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition, null, new Color(137, 90, 248) * 0.2f, Projectile.rotation, ModAssets.ExtraTextures[21].Size() * 0.5f, Projectile.scale * 0.6f, SpriteEffects.None, 0);
            }
            SetSpriteBatch();

            var glowTexture = ModContent.Request<Texture2D>(this.Texture + "_Glowmask");
            Main.EntitySpriteDraw(glowTexture.Value, drawPosition, null, Color.White, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        }

        private readonly LightningEffect _effect = new();

        // ...

        #region Lightning Effect
        private class LightningEffect
        {
            private bool _active;
            private int _time;
            private int _frame;
            private float _rotation;
            private SpriteEffects _spriteEffects;

            public LightningEffect() => this.Reset();

            public void Draw(Vector2 position, float scale)
            {
                if (!_active) return;

                var texture = ModAssets.ExtraTextures[22];
                Rectangle rectangle = new(_time * 96, _frame * 96, 96, 96);
                Main.spriteBatch.Draw(texture.Value, position, rectangle, Color.White, _rotation, new Vector2(48, 48), scale, _spriteEffects, 0);
            }

            public void Update(int counter)
            {
                if (counter % 3 == 0) _time++;
                if (_time >= 4) _active = false;
            }

            public void Reset()
            {
                _active = true;
                _time = 0;
                _frame = Main.rand.Next(4);
                _rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                _spriteEffects = Main.rand.NextBool(2) ? SpriteEffects.None : SpriteEffects.FlipVertically;
            }
        }
        #endregion
    }

    public class BellowingThunderLightingProjectile : PladProjectile
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Misc/Extra_0";

        /*public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            //ScreenShakeSystem.NewScreenShake(position: Projectile.Center, power: 3f, range: 16 * 35, time: 50);
        }*/
    }
}
