using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Graphics;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Common.Graphics.Primitives;
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

        public override void YoyoSetStaticDefaults()
        {
            SPladisonsYoyoMod.Sets.ItemCustomInventoryScale[Type] = 1f;
        }

        public override void YoyoSetDefaults()
        {
            Item.width = 38;
            Item.height = 26;

            Item.damage = 43;
            Item.knockBack = 2.5f;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<BlackholeProjectile>();

            Item.rare = ItemRarityID.Yellow;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BlackholeProjectile : YoyoProjectile, IDrawOnRenderTarget, IDrawOnDifferentLayers
    {
        public static float Radius { get; } = 16 * 10;

        public float RadiusProgress { get => Projectile.localAI[1]; set => Projectile.localAI[1] = value; }

        private PrimitiveStrip trail;
        private int timer;

        // ...

        public BlackholeProjectile() : base(lifeTime: 14f, maxRange: 275f, topSpeed: 16f) { }

        public override bool IsSoloYoyo() => true;

        public override void OnSpawn(IEntitySource source)
        {
            BlackholeEffectSystem.AddElement(this);

            trail = new PrimitiveStrip
            (
                width: p => 22 * (1 - p * 0.7f),
                color: p => new Color(209, 112, 224) * (1 - p),
                effect: new IPrimitiveEffect.Default(ModAssets.GetExtraTexture(11), false),
                headTip: null,
                tailTip: null
            );
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

            timer++;

            Lighting.AddLight(Projectile.Center, new Vector3(171 / 255f, 97 / 255f, 255 / 255f) * 0.45f);

            var currentRadius = Radius * RadiusProgress * (YoyoGloveActivated ? 1.25f : 1f);
            var targets = NPCUtils.NearestNPCs(
                center: Projectile.Center,
                radius: currentRadius,
                predicate: npc =>
                    npc.CanBeChasedBy(Projectile, false) &&
                    !npc.boss &&
                    !NPCID.Sets.ShouldBeCountedAsBoss[npc.type] &&
                    Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height)
            );

            foreach (var target in targets)
            {
                var npc = target.npc;
                var distance = target.distance;
                var vector = Projectile.Center - npc.Center;

                vector *= 3f / distance * 5f;

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
            var scaleFactor = 15f; // Vanilla: 16f

            Projectile.velocity -= vector * scaleFactor;
            Projectile.velocity *= -2f;
        }

        public override void PostDraw(Color lightColor)
        {
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModAssets.GetExtraTexture(5);
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, Color.Black, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);
        }

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            var scale = RadiusProgress * (YoyoGloveActivated ? 1.25f : 1f) * Projectile.scale;
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModAssets.GetExtraTexture(30);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, texture.Size() * 0.5f, 0.45f * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, 0f, texture.Size() * 0.5f, 0.40f * scale, SpriteEffects.None, 0f);

            texture = ModAssets.GetExtraTexture(2);
            spriteBatch.Draw(texture.Value, drawPosition, null, Color.White, Main.GlobalTimeWrappedHourly * 2.5f, texture.Size() * 0.5f, 0.35f * (1 + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f) * scale, SpriteEffects.None, 0f);
        }

        void IDrawOnDifferentLayers.DrawOnDifferentLayers(DrawSystem system)
        {
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModAssets.GetExtraTexture(32);
            var rotation = (float)Math.Sin(timer * 0.0125f);
            var scale = Projectile.scale;
            var drawData = new DefaultDrawData(texture.Value, drawPosition, null, Color.White * rotation, (float)Math.Sin(timer * 0.025f), texture.Size() * 0.5f, (0.4f + rotation * 0.15f) * scale, SpriteEffects.None);
            system.AddToLayer(DrawLayers.Dusts, DrawTypeFlags.All, drawData);

            texture = ModAssets.GetExtraTexture(31);
            drawData = new DefaultDrawData(texture.Value, drawPosition, null, Color.White, rotation, texture.Size() * 0.5f, 0.28f * scale, SpriteEffects.None);
            system.AddToLayer(DrawLayers.Dusts, DrawTypeFlags.All, drawData);

            trail.UpdatePointsAsSimpleTrail(Projectile.Center + Projectile.gfxOffY * Vector2.UnitY, 10, 16 * 7 * ReturningProgress);
            system.AddToLayer(DrawLayers.Tiles, DrawTypeFlags.All, trail);
        }
    }

    [Autoload(Side = ModSide.Client)]
    public sealed class BlackholeEffectSystem : ModSystem
    {
        private List<IDrawOnRenderTarget> elems = new();
        private RenderTarget2D target = null;

        // ...

        public static void AddElement(IDrawOnRenderTarget elem)
        {
            var elems = ModContent.GetInstance<BlackholeEffectSystem>().elems;
            if (!elems.Contains(elem)) elems.Add(elem);
        }

        public static void RemoveElement(IDrawOnRenderTarget elem)
        {
            ModContent.GetInstance<BlackholeEffectSystem>().elems.Remove(elem);
        }

        // ...

        public override void PostSetupContent()
        {
            SPladisonsYoyoMod.Events.OnPostUpdateCameraPosition += DrawToTarget;
            SPladisonsYoyoMod.Events.OnResolutionChanged += RecreateRenderTarget;
            DrawSystem.AddPostDrawLayerMethod(DrawLayers.Walls, DrawToScreen);
        }

        public override void Unload()
        {
            elems.Clear();
            elems = null;
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
            if (!elems.Any()) return;
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

        private void DrawToScreen(SpriteBatch spriteBatch)
        {
            if (target == null || !elems.Any()) return;

            var texture = (Texture2D)target;
            var effect = ModAssets.GetEffect("BlackholeBackground").Value;

            effect.Parameters["Time"].SetValue((float)Main.gameTimeCache.TotalGameTime.TotalSeconds * 0.4f);
            effect.Parameters["Resolution"].SetValue(texture.Size());
            effect.Parameters["Offset"].SetValue(Main.screenPosition * 0.001f);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(texture, Vector2.Zero, null, new Color(8, 9, 15), 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}