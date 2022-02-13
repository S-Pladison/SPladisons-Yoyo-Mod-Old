using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Content.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class PaperYoyo : YoyoItem
    {
        public PaperYoyo() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<PaperYoyoProjectile>();

            Item.rare = ItemRarityID.Green;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class PaperYoyoProjectile : YoyoProjectile
    {
        private float RadiusProgress { get => Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        private int HitCounter { get; set; } = 0;

        // ...

        public PaperYoyoProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 11f) { }

        public override bool IsSoloYoyo() => true;

        public override void YoyoSetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
        }

        public override void AI()
        {
            this.UpdateRadius();

            Projectile.rotation -= 0.25f;

            /* var particle = new Particles.PaperYoyoParticle(Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(20), Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(3));
             ParticleSystem.NewParticle(particle);*/
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            if (HitCounter++ % 3 == 0)
            {
                var vector = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                var dust = Dust.NewDustPerfect(Projectile.Center + vector * Main.rand.NextFloat(25f, 30f) - new Vector2(25, 19), ModContent.DustType<Dusts.PaperYoyoHitDust>(), vector * Main.rand.NextFloat(0.5f, 3f));
                dust.rotation = Main.rand.NextFloat(-0.5f, 0.5f);
                dust.scale = 1f;
            }

            for (int i = 0; i < 8; i++)
            {
                var position = Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(15);
                var velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(3);
                Particle.NewParticle(ParticleSystem.ParticleType<PaperYoyoFilterParticle>(), position, velocity, Color.White);
            }

            for (int i = 0; i < 5; i++)
            {
                var position = Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(15);
                var velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(1f, 2.5f);
                Particle.NewParticle(ParticleSystem.ParticleType<PaperYoyoBubbleParticle>(), position, velocity, Color.White);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                var texture = TextureAssets.Projectile[Type];
                var progress = ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                var drawPosTrail = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5f + new Vector2(0f, Projectile.gfxOffY);

                Main.EntitySpriteDraw(texture.Value, drawPosTrail, null, lightColor * 0.3f * progress, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * (0.95f - (1 - progress) * 0.2f), SpriteEffects.None, 0);
            }

            return true;
        }

        public void UpdateRadius()
        {
            RadiusProgress += !IsReturning ? 0.05f : -0.15f;
            RadiusProgress = Math.Clamp(RadiusProgress, 0, 1);
        }
    }

    public sealed class PaperEffectSystem : ModSystem, IOnResizeScreen
    {
        public static PaperEffectSystem Instance { get => ModContent.GetInstance<PaperEffectSystem>(); }
        public static readonly string SceneName = "SPladisonsYoyoMod:Paper";

        private List<IDrawOnRenderTarget> _filterElems = new();
        private List<IDrawOnRenderTarget> _bubbleElems = new();
        private RenderTarget2D _filterTarget;
        private RenderTarget2D _bubbleTarget;
        private Asset<Effect> _effect;

        public int FilterElementsCount => _filterElems.Count;
        public int BubbleElementsCount => _bubbleElems.Count;

        public override void Load()
        {
            if (Main.dedServ) return;

            _effect = ModAssets.GetEffect("Paper", AssetRequestMode.ImmediateLoad);

            Filters.Scene[SceneName] = new(new ScreenShaderData(new Ref<Effect>(_effect.Value), "Paper"), EffectPriority.VeryHigh);
            Filters.Scene[SceneName].GetShader().Shader.Parameters["contrast"].SetValue(0.1f);
        }

        public override void Unload()
        {
            _filterElems.Clear();
            _filterElems = null;
            _bubbleElems.Clear();
            _bubbleElems = null;

            _filterTarget = null;
            _bubbleTarget = null;

            _effect = null;
        }

        public override void PostUpdateEverything()
        {
            var filter = Filters.Scene[SceneName];
            filter.GetShader().UseImage(this.GetFilterTexture());

            if (_filterElems.Count == 0)
            {
                if (filter.IsActive())
                {
                    Filters.Scene.Deactivate(SceneName);
                }
                return;
            }

            if (!filter.IsActive())
            {
                Filters.Scene.Activate(SceneName);
            }
        }

        public void AddFilterElement(IDrawOnRenderTarget elem)
        {
            if (!_filterElems.Contains(elem))
            {
                _filterElems.Add(elem);
            }
        }

        public void AddBubbleElement(IDrawOnRenderTarget elem)
        {
            if (!_bubbleElems.Contains(elem))
            {
                _bubbleElems.Add(elem);
            }
        }

        public void RemoveFilterElement(IDrawOnRenderTarget elem)
        {
            _filterElems.Remove(elem);
        }

        public void RemoveBubbleElement(IDrawOnRenderTarget elem)
        {
            _bubbleElems.Remove(elem);
        }

        public Texture2D GetFilterTexture() => _filterElems.Count > 0 ? ((Texture2D)_filterTarget ?? ModAssets.GetExtraTexture(0).Value) : ModAssets.GetExtraTexture(0).Value;

        public void RecreateRenderTargets(int width, int height)
        {
            _filterTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
            _bubbleTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
        }

        public void DrawToTarget(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            if (_filterTarget == null || _bubbleTarget == null)
            {
                RecreateRenderTargets(Main.screenWidth, Main.screenHeight);
            }

            if (_filterElems.Count > 0)
            {
                device.SetRenderTarget(_filterTarget);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                foreach (var elem in _filterElems) elem.DrawOnRenderTarget(spriteBatch);
                spriteBatch.End();
            }

            if (_bubbleElems.Count > 0)
            {
                device.SetRenderTargets(_bubbleTarget);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (var elem in _bubbleElems) elem.DrawOnRenderTarget(spriteBatch);
                spriteBatch.End();
            }

            device.SetRenderTargets(null);
        }

        public void DrawToScreen(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu || _bubbleTarget == null || _bubbleElems.Count == 0) return;

            var texture = (Texture2D)_bubbleTarget;
            var effect = _effect.Value;
            effect.Parameters["uScreenResolution"].SetValue(texture.Size());
            /*effect.Parameters["time"].SetValue((float)Main.gameTimeCache.TotalGameTime.TotalSeconds * 0.4f);
            effect.Parameters["resolution"].SetValue(texture.Size());
            effect.Parameters["offset"].SetValue(Main.screenPosition * 0.001f);*/

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect);
            spriteBatch.Draw(texture, Vector2.Zero, null, Color.White);
            spriteBatch.End();
        }

        void IOnResizeScreen.OnResizeScreen(int width, int height)
        {
            RecreateRenderTargets(width, height);
        }
    }
}