using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Interfaces;
using System;
using System.Collections.Generic;
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

    public class BlackholeProjectile : YoyoProjectile, IDrawAdditive, IDrawOnRenderTarget
    {
        public static readonly float Radius = 16 * 10;
        private float RadiusProgress { get => Projectile.localAI[1]; set => Projectile.localAI[1] = value; }

        // ...

        public BlackholeProjectile() : base(lifeTime: 14f, maxRange: 275f, topSpeed: 16f) { }

        public override bool IsSoloYoyo() => true;

        public override void OnSpawn()
        {
            BlackholeSpaceSystem.Instance.AddElement(this);
        }

        public override bool PreKill(int timeLeft)
        {
            BlackholeSpaceSystem.Instance.RemoveElement(this);
            return true;
        }

        public override void AI()
        {
            this.UpdateRadius();

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];

                if (target == null || !target.CanBeChasedBy(Projectile, false) || target.boss) continue;
                if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height)) continue;

                Vector2 vector = Projectile.Center - target.Center;
                float distance = vector.Length();
                float currentRadius = Radius * RadiusProgress * (this.YoyoGloveActivated ? 1.25f : 1f);

                if (distance < currentRadius)
                {
                    vector *= 3f / distance * 7f;

                    target.velocity = Vector2.Lerp(vector * (1 - distance / currentRadius), target.velocity, 0.5f);
                    target.netUpdate = true;
                }
            }

            Lighting.AddLight(Projectile.Center, new Vector3(171 / 255f, 97 / 255f, 255 / 255f) * 0.45f);
        }

        public void UpdateRadius()
        {
            RadiusProgress += !IsReturning ? 0.05f : -0.15f;
            RadiusProgress = Math.Clamp(RadiusProgress, 0, 1);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (this.YoyoGloveActivated) damage = (int)(damage * 1.2f);
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            for (int i = 0; i < 12; i++)
            {
                var particle = new Particles.BlackholeSpaceParticle(Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(20), Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(3));
                ParticleSystem.NewParticle(particle);
            }

            if (!this.YoyoGloveActivated) return;

            var vector = Vector2.Normalize(Projectile.Center - target.Center);
            var scaleFactor = 12f; // Vanilla: 16f

            Projectile.velocity -= vector * scaleFactor;
            Projectile.velocity *= -2f;
        }

        public override void PostDraw(Color lightColor)
        {
            var drawPosition = GetDrawPosition();
            var texture = ModAssets.GetExtraTexture(5);
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.Black, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        }

        void IDrawAdditive.DrawAdditive()
        {
            // ... Ok... 0.0125f?.. 0.025f... sin? 

            var drawPosition = GetDrawPosition();
            var texture = ModAssets.GetExtraTexture(32);
            var rotation = (float)Math.Sin(Projectile.localAI[0] * 0.0125f);
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White * rotation, (float)Math.Sin(Projectile.localAI[0] * 0.025f), texture.Size() * 0.5f, (0.4f + rotation * 0.15f) * Projectile.scale, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(31);
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White * 0.5f, -rotation * 1.5f + MathHelper.Pi, texture.Size() * 0.5f, 0.3f * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.White, rotation, texture.Size() * 0.5f, 0.2f * Projectile.scale, SpriteEffects.None, 0);
        }

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            var scale = this.RadiusProgress * (this.YoyoGloveActivated ? 1.25f : 1f) * Projectile.scale;
            var drawPosition = GetDrawPosition();
            var texture = ModAssets.GetExtraTexture(30);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, texture.Size() * 0.5f, 0.5f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, texture.Size() * 0.5f, 0.37f * scale, SpriteEffects.None, 0f);

            texture = ModAssets.GetExtraTexture(2);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, Main.GlobalTimeWrappedHourly * 2.5f, texture.Size() * 0.5f, 0.35f * (1 + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f) * scale, SpriteEffects.None, 0f);
        }
    }

    public sealed class BlackholeSpaceSystem : ModSystem
    {
        public static BlackholeSpaceSystem Instance { get => ModContent.GetInstance<BlackholeSpaceSystem>(); }
        private static readonly Color[] Colors = new Color[]
        {
            new Color(8, 9, 15),
            new Color(198, 50, 189),
            new Color(25, 25, 76)
        };

        private List<IDrawOnRenderTarget> _elems = new();
        private RenderTarget2D _target;
        private Asset<Effect> _spaceEffect;
        private Asset<Texture2D> _firstTexture;
        private Asset<Texture2D> _secondTexture;

        public int ElementsCount => _elems.Count;

        public override void Load()
        {
            _firstTexture = ModAssets.GetExtraTexture(28, AssetRequestMode.ImmediateLoad);
            _secondTexture = ModAssets.GetExtraTexture(29, AssetRequestMode.ImmediateLoad);

            if (Main.dedServ) return;

            _spaceEffect = ModAssets.GetEffect("BlackholeSpace", AssetRequestMode.ImmediateLoad);
            _spaceEffect.Value.Parameters["texture1"].SetValue(_firstTexture.Value);
            _spaceEffect.Value.Parameters["texture2"].SetValue(_secondTexture.Value);
            _spaceEffect.Value.Parameters["color0"].SetValue(Colors[0].ToVector4());
            _spaceEffect.Value.Parameters["color1"].SetValue(Colors[1].ToVector4());
            _spaceEffect.Value.Parameters["color2"].SetValue(Colors[2].ToVector4());
        }

        public override void Unload()
        {
            _elems.Clear();
            _elems = null;
            _target = null;
            _spaceEffect = null;
            _firstTexture = null;
            _secondTexture = null;
        }

        public void AddElement(IDrawOnRenderTarget elem)
        {
            if (!_elems.Contains(elem))
            {
                _elems.Add(elem);
            }
        }

        public void RemoveElement(IDrawOnRenderTarget elem)
        {
            _elems.Remove(elem);
        }

        public void RecreateRenderTarget(int width, int height)
        {
            _target = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        }

        public void DrawToTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (_elems.Count == 0) return;
            if (_target == null) this.RecreateRenderTarget(Main.screenWidth, Main.screenHeight);

            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (var elem in _elems) elem.DrawOnRenderTarget(spriteBatch);
            spriteBatch.End();

            device.SetRenderTargets(null);
        }

        public void DrawToScreen(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu || _target == null || _elems.Count == 0) return;

            var texture = (Texture2D)_target;
            var effect = _spaceEffect.Value;
            effect.Parameters["time"].SetValue((float)Main.gameTimeCache.TotalGameTime.TotalSeconds * 0.4f);
            effect.Parameters["resolution"].SetValue(texture.Size());
            effect.Parameters["offset"].SetValue(Main.screenPosition * 0.001f);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect);
            spriteBatch.Draw(texture, Vector2.Zero, null, Colors[0]);
            spriteBatch.End();
        }
    }
}