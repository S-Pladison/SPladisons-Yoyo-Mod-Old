using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Drawing;
using SPladisonsYoyoMod.Common.Drawing.AdditionalDrawing;
using SPladisonsYoyoMod.Common.Drawing.Particles;
using SPladisonsYoyoMod.Content.Particles;
using SPladisonsYoyoMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
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

    public class BlackholeProjectile : YoyoProjectile, IDrawOnRenderTarget, IPostUpdateCameraPosition
    {
        private const float RADIUS = 16 * 10;
        private float RadiusProgress { get => Projectile.localAI[1]; set => Projectile.localAI[1] = value; }

        // ...

        public BlackholeProjectile() : base(lifeTime: 14f, maxRange: 275f, topSpeed: 16f) { }

        public override bool IsSoloYoyo() => true;

        public override void OnSpawn(IEntitySource source)
        {
            BlackholeEffectSystem.AddElement(this);
        }

        public override bool PreKill(int timeLeft)
        {
            BlackholeEffectSystem.RemoveElement(this);
            return true;
        }

        public override void AI()
        {
            RadiusProgress += !IsReturning ? 0.05f : -0.15f;
            RadiusProgress = Math.Clamp(RadiusProgress, 0, 1);

            Lighting.AddLight(Projectile.Center, new Vector3(171 / 255f, 97 / 255f, 255 / 255f) * 0.45f);

            var currentRadius = RADIUS * RadiusProgress * (YoyoGloveActivated ? 1.25f : 1f);
            var targets = NPCUtils.NearestNPCs(
                center: Projectile.Center,
                radius: currentRadius,
                predicate: npc => npc.CanBeChasedBy(Projectile, false) &&
                           !npc.boss &&
                           Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height)
            );

            foreach (var target in targets)
            {
                var npc = target.npc;
                var distance = target.distance;
                var vector = Projectile.Center - npc.Center;

                vector *= 3f / distance * 7f;

                npc.velocity = Vector2.Lerp(vector * (1 - distance / currentRadius), npc.velocity, 0.5f);
                npc.netUpdate = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (YoyoGloveActivated) damage = (int)(damage * 1.2f);
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            if (!IsReturning)
            {
                for (int i = 0; i < 12; i++)
                {
                    var position = Projectile.Center + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(20);
                    var velocity = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(3);
                    Particle.NewParticle(ParticleSystem.ParticleType<BlackholeSpaceParticle>(), position, velocity, Color.White);
                }
            }

            if (!YoyoGloveActivated) return;

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

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            var scale = RadiusProgress * (YoyoGloveActivated ? 1.25f : 1f) * Projectile.scale;
            var drawPosition = GetDrawPosition();
            var texture = ModAssets.GetExtraTexture(30);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, texture.Size() * 0.5f, 0.45f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, texture.Size() * 0.5f, 0.40f * scale, SpriteEffects.None, 0f);

            texture = ModAssets.GetExtraTexture(2);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, Main.GlobalTimeWrappedHourly * 2.5f, texture.Size() * 0.5f, 0.35f * (1 + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f) * scale, SpriteEffects.None, 0f);
        }

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            var drawPosition = GetDrawPosition();
            var texture = ModAssets.GetExtraTexture(32);
            var rotation = (float)Math.Sin(Projectile.localAI[0] * 0.0125f);
            var scale = Projectile.scale * Vector2.One;
            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.All, new(texture.Value, drawPosition, null, Color.White * rotation, (float)Math.Sin(Projectile.localAI[0] * 0.025f), texture.Size() * 0.5f, (0.4f + rotation * 0.15f) * scale, SpriteEffects.None));

            texture = ModAssets.GetExtraTexture(31);
            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.All, new(texture.Value, drawPosition, null, Color.White, rotation, texture.Size() * 0.5f, 0.28f * scale, SpriteEffects.None));
        }
    }

    public sealed class BlackholeEffectSystem : ModSystem
    {
        private static readonly Color[] Colors = new Color[]
        {
            new Color(8, 9, 15),
            new Color(198, 50, 189),
            new Color(25, 25, 76)
        };

        private List<IDrawOnRenderTarget> elems;
        private RenderTarget2D target;
        private Asset<Effect> spaceEffect;
        private Asset<Texture2D> firstTexture;
        private Asset<Texture2D> secondTexture;

        // ...

        public static void AddElement(IDrawOnRenderTarget elem)
        {
            var list = ModContent.GetInstance<BlackholeEffectSystem>().elems;
            if (!list.Contains(elem)) list.Add(elem);
        }

        public static void RemoveElement(IDrawOnRenderTarget elem)
        {
            ModContent.GetInstance<BlackholeEffectSystem>().elems.Remove(elem);
        }

        // ...

        public override void Load()
        {
            firstTexture = ModAssets.GetExtraTexture(28, AssetRequestMode.ImmediateLoad);
            secondTexture = ModAssets.GetExtraTexture(29, AssetRequestMode.ImmediateLoad);
            elems = new();

            if (Main.dedServ) return;

            spaceEffect = ModAssets.GetEffect("BlackholeSpace", AssetRequestMode.ImmediateLoad);
            spaceEffect.Value.Parameters["texture1"].SetValue(firstTexture.Value);
            spaceEffect.Value.Parameters["texture2"].SetValue(secondTexture.Value);
            spaceEffect.Value.Parameters["color0"].SetValue(Colors[0].ToVector4());
            spaceEffect.Value.Parameters["color1"].SetValue(Colors[1].ToVector4());
            spaceEffect.Value.Parameters["color2"].SetValue(Colors[2].ToVector4());
        }

        public override void PostSetupContent()
        {
            Main.OnResolutionChanged += RecreateRenderTarget;
            SPladisonsYoyoMod.PostUpdateCameraPositionEvent += DrawToTarget;
            DrawingManager.AddCustomMethodToLayer(DrawLayers.OverWalls, DrawToScreen);
        }

        public override void Unload()
        {
            Main.OnResolutionChanged -= RecreateRenderTarget;

            elems.Clear();
            elems = null;
            target = null;
            spaceEffect = null;
            firstTexture = null;
            secondTexture = null;
        }

        public override void OnWorldUnload()
        {
            elems.Clear();
        }

        // ...

        private void RecreateRenderTarget(Vector2 screenSize)
        {
            var size = (screenSize / 2).ToPoint();
            target = new RenderTarget2D(Main.graphics.GraphicsDevice, size.X, size.Y);
        }

        private void DrawToTarget()
        {
            if (elems.Count == 0) return;
            if (target == null) RecreateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));

            var spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.CreateScale(0.5f) * Main.GameViewMatrix.EffectMatrix);
            foreach (var elem in elems) elem.DrawOnRenderTarget(spriteBatch);
            spriteBatch.End();

            device.SetRenderTargets(null);
        }

        private void DrawToScreen(SpriteBatch spriteBatch, DrawLayers _)
        {
            if (target == null || !elems.Any()) return;

            var texture = (Texture2D)target;
            var effect = spaceEffect.Value;
            effect.Parameters["time"].SetValue((float)Main.gameTimeCache.TotalGameTime.TotalSeconds * 0.4f);
            effect.Parameters["resolution"].SetValue(texture.Size());
            effect.Parameters["offset"].SetValue(Main.screenPosition * 0.001f);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture, Vector2.Zero, null, Colors[0], 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}