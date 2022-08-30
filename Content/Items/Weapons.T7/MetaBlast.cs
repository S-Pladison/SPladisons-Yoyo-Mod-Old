using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common.Graphics;
using SPladisonsYoyoMod.Common.Graphics.Particles;
using SPladisonsYoyoMod.Content.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class MetaBlast : YoyoItem
    {
        public MetaBlast() : base(gamepadExtraRange: 21) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<MetaBlastProjectile>();

            Item.rare = ItemRarityID.Cyan;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Code2);
            recipe.AddIngredient<PrototypeF34>();
            recipe.AddIngredient(ItemID.BeetleHusk, 3);
            recipe.AddIngredient(ItemID.ShroomiteBar, 7);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddTile(TileID.Autohammer);
            recipe.Register();
        }
    }

    public class MetaBlastProjectile : YoyoProjectile, IDrawOnDifferentLayers, IDrawOnRenderTarget
    {
        public const int MAX_CHARGE_VALUE = 60 * 7;

        public static readonly SoundStyle DeviceBeepingSound = new(ModAssets.SoundsPath + "DeviceBeeping", SoundType.Sound);

        public bool IsCharged => charge == MAX_CHARGE_VALUE;

        private int charge = 0;

        // ...

        public MetaBlastProjectile() : base(lifeTime: -1f, maxRange: 400f, topSpeed: 16f) { }

        public override bool IsSoloYoyo() => true;

        public override void YoyoSetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void OnSpawn(IEntitySource source)
        {
            MetaBlastEffectSystem.AddElement(this);
        }

        public override bool PreKill(int timeLeft)
        {
            MetaBlastEffectSystem.RemoveElement(this);
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * Projectile.scale * MathHelper.Lerp(0.1f, 0.3f, charge / (float)MAX_CHARGE_VALUE));

            Projectile.rotation -= 0.15f;

            if (Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center) > 1300)
            {
                Projectile.Kill();
                return;
            }

            ChargeAI();
        }

        public void ChargeAI()
        {
            if (!IsCharged)
            {
                charge++;

                if (!IsCharged) return;

                SoundEngine.PlaySound(DeviceBeepingSound, Projectile.Center);
            }

            if (!Main.mouseRight) return;

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MetaBlastExplosionProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            charge = -170;
            Projectile.netUpdate = true;
        }

        public override bool? CanDamage()
        {
            if (charge < 0) return false;
            return base.CanDamage();
        }

        public override bool ShouldUpdatePosition()
        {
            return charge >= 0;
        }

        public override void YoyoSendExtraAI(BinaryWriter writer)
        {
            writer.Write(charge);
        }

        public override void YoyoReceiveExtraAI(BinaryReader reader)
        {
            charge = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type];
            var origin = texture.Size() * 0.5f;

            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                var progress = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                var pos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5f + new Vector2(0f, Projectile.gfxOffY);

                Main.EntitySpriteDraw(texture.Value, pos, null, lightColor * 0.3f * progress, Projectile.rotation, origin, Projectile.scale * (0.95f - (1 - progress) * 0.2f), SpriteEffects.None, 0);
            }

            return true;
        }

        void IDrawOnDifferentLayers.DrawOnDifferentLayers(DrawSystem system)
        {
            var texture = ModAssets.GetExtraTexture(40);
            var scaleMult = MathHelper.Lerp(0.4f, 1.3f, charge / (float)MAX_CHARGE_VALUE) * Projectile.scale;
            var position = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var data = new DefaultDrawData(texture.Value, position, null, Color.Black * 0.4f * (charge / (float)MAX_CHARGE_VALUE), 0f, texture.Size() * 0.5f, 0.125f * scaleMult, SpriteEffects.None);
            system.AddToLayer(DrawLayers.Walls, DrawTypeFlags.None, data);

            texture = ModAssets.GetExtraTexture(43);
            var origin = texture.Size() * 0.5f;
            data = new DefaultDrawData(texture.Value, position, null, Color.White, Projectile.rotation, origin, Projectile.scale * 0.6f, SpriteEffects.None);
            system.AddToLayer(DrawLayers.Tiles, DrawTypeFlags.Additive, data);

            if (!IsCharged) return;

            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                var progress = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                var pos = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5f + new Vector2(0f, Projectile.gfxOffY);
                data = new(texture.Value, pos, null, Color.White * progress, Projectile.rotation, origin, Projectile.scale * (0.6f - (1 - progress) * 0.2f), SpriteEffects.None);
                system.AddToLayer(DrawLayers.Tiles, DrawTypeFlags.Additive, data);
            }
        }

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            var texture = ModAssets.GetExtraTexture(40);
            var red = new Color(255, 0, 0);
            var green = new Color(0, 255, 0) * Projectile.scale;
            var position = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition - new Vector2(1);
            var scaleMult = MathHelper.Lerp(0.4f, 1.3f, charge / (float)MAX_CHARGE_VALUE) * Projectile.scale;
            spriteBatch.Draw(texture.Value, position, null, red, 0f, texture.Size() * 0.5f, 0.125f * scaleMult, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(42);
            spriteBatch.Draw(texture.Value, position, null, green, 0f, texture.Size() * 0.5f, 0.5f * scaleMult, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(30);
            spriteBatch.Draw(texture.Value, position, null, green * 0.6f, 0f, texture.Size() * 0.5f, 0.2f * scaleMult, SpriteEffects.None, 0);
        }
    }

    public class MetaBlastExplosionProjectile : ModProjectile, IDrawOnDifferentLayers, IDrawOnRenderTarget
    {
        public const int MAX_RADIUS = 16 * 11;

        public static readonly SoundStyle ExplosionDropSound = new(ModAssets.SoundsPath + "ExplosionDrop", SoundType.Sound);
        public static readonly SoundStyle PoofSound = new(ModAssets.SoundsPath + "Poof", SoundType.Sound);

        private LightningEffect lightningEffect;

        // ...

        public override string Texture => ModAssets.InvisiblePath;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = MAX_RADIUS * 2;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.1f;

            lightningEffect = new();

            MetaBlastEffectSystem.AddElement(this);
            SoundEngine.PlaySound(ExplosionDropSound, Projectile.Center);

            for (int i = 0; i < 35; i++)
            {
                Particle.NewParticle<MetaBlastParticle>(Projectile.Center, new Vector2(Main.rand.NextFloat(5, 10), 0).RotatedBy(i / 35.0 * MathHelper.TwoPi));
            }
        }

        public override bool PreKill(int timeLeft)
        {
            MetaBlastEffectSystem.RemoveElement(this);
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * Projectile.scale * 0.5f);

            Projectile.localAI[0] += 1;

            if (Projectile.localAI[0] >= 16)
            {
                Projectile.localAI[0] = 0;
                lightningEffect.Reset();
            }

            lightningEffect.Update((int)Projectile.localAI[0]);

            if (Projectile.timeLeft > 170)
            {
                Projectile.scale = Math.Min(Projectile.scale + 0.1f, 1f);
                return;
            }

            if (Projectile.timeLeft <= 5)
            {
                if (Projectile.timeLeft == 5) SoundEngine.PlaySound(PoofSound, Projectile.Center);

                Projectile.scale = Math.Max(Projectile.scale - 0.2f, 0f);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Vector2.Distance(target.Center, Projectile.Center) > MAX_RADIUS * Projectile.scale) return false;
            return base.CanHitNPC(target);
        }

        void IDrawOnDifferentLayers.DrawOnDifferentLayers(DrawSystem system)
        {
            var texture = ModAssets.GetExtraTexture(40);
            var position = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var data = new DefaultDrawData(texture.Value, position, null, Color.Black * 0.6f * Projectile.scale, 0f, texture.Size() * 0.5f, 0.67f * (Projectile.scale + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.02f), SpriteEffects.None);
            system.AddToLayer(DrawLayers.Walls, DrawTypeFlags.None, data);
        }

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            var texture = ModAssets.GetExtraTexture(40);
            var red = new Color(255 * Projectile.scale, 0, 0);
            var green = new Color(0, 255 * Projectile.scale, 0);
            var position = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var scale = Projectile.scale + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.02f;
            spriteBatch.Draw(texture.Value, position, null, red, 0f, texture.Size() * 0.5f, 0.67f * scale, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(41);
            spriteBatch.Draw(texture.Value, position, null, green, 0f, texture.Size() * 0.5f, 0.3725f * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture.Value, position, null, green * 0.8f, 0f, texture.Size() * 0.5f, 0.125f * scale, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(23);
            spriteBatch.Draw(texture.Value, position, null, green * 0.5f, 0f, texture.Size() * 0.5f, 0.4f * scale, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(30);
            spriteBatch.Draw(texture.Value, position, null, green * 0.8f, 0f, texture.Size() * 0.5f, 1.25f * scale, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(21);
            spriteBatch.Draw(texture.Value, position, null, green, 0f, texture.Size() * 0.5f, 0.7f * scale, SpriteEffects.None, 0);

            texture = ModAssets.GetExtraTexture(44);
            spriteBatch.Draw(texture.Value, position, null, green * 0.3f, Main.GlobalTimeWrappedHourly * 8f + Projectile.whoAmI * 0.5f, texture.Size() * 0.5f, 0.825f * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture.Value, position, null, green * 0.4f, Main.GlobalTimeWrappedHourly * 5f, texture.Size() * 0.5f, 0.775f * scale, SpriteEffects.None, 0);

            lightningEffect.Draw(spriteBatch, position, green, scale * 1.5f);
        }

        // ...

        private class LightningEffect
        {
            private bool active;
            private int time;
            private int frame;
            private float rotation;
            private SpriteEffects spriteEffects;

            public LightningEffect()
            {
                Reset();
            }

            public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale)
            {
                if (!active) return;

                var rectangle = new Rectangle(time * 96, frame * 96, 96, 96);
                spriteBatch.Draw(ModAssets.GetExtraTexture(22).Value, position, rectangle, color, rotation, new Vector2(48, 48), scale, spriteEffects, 0);
            }

            public void Update(int counter)
            {
                if (counter % 3 == 0) time++;
                if (time >= 4) active = false;
            }

            public void Reset()
            {
                active = true;
                time = 0;
                frame = Main.rand.Next(4);
                rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                spriteEffects = Main.rand.NextBool(2) ? SpriteEffects.None : SpriteEffects.FlipVertically;
            }
        }
    }


    [Autoload(Side = ModSide.Client)]
    public sealed class MetaBlastEffectSystem : ModSystem
    {
        public static readonly string SceneName = "SPladisonsYoyoMod:MetaBlastFilter";

        private List<IDrawOnRenderTarget> elems;
        private RenderTarget2D target;
        private Texture2D invisibleTexture;
        private Texture2D noiseTexture;
        private Texture2D colorTexture;

        // ...

        public static void AddElement(IDrawOnRenderTarget elem)
        {
            var elems = ModContent.GetInstance<MetaBlastEffectSystem>().elems;
            if (!elems.Contains(elem)) elems.Add(elem);
        }

        public static void RemoveElement(IDrawOnRenderTarget elem)
        {
            ModContent.GetInstance<MetaBlastEffectSystem>().elems.Remove(elem);
        }

        // ...

        public override void Load()
        {
            elems = new();
            invisibleTexture = ModAssets.GetExtraTexture(0, AssetRequestMode.ImmediateLoad).Value;
            noiseTexture = ModAssets.GetExtraTexture(15, AssetRequestMode.ImmediateLoad).Value;
            colorTexture = ModAssets.GetExtraTexture(45, AssetRequestMode.ImmediateLoad).Value;

            SPladisonsYoyoMod.Events.OnPostUpdateCameraPosition += DrawToTarget;
            SPladisonsYoyoMod.Events.OnResolutionChanged += RecreateRenderTarget;
        }

        public override void PostSetupContent()
        {
            EffectLoader.CreateSceneFilter("MetaBlastFilter", EffectPriority.VeryHigh);
        }

        public override void PostUpdateEverything()
        {
            var filter = Filters.Scene[SceneName];
            var shader = filter.GetShader();

            shader.UseImage(elems.Any() ? (target ?? invisibleTexture) : invisibleTexture, 0);
            shader.UseImage(noiseTexture, 1);
            shader.UseImage(colorTexture, 2);

            if (!elems.Any())
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

        public void RecreateRenderTarget(Vector2 screenSize)
        {
            var size = screenSize.ToPoint();
            target = new RenderTarget2D(Main.graphics.GraphicsDevice, size.X, size.Y);
        }

        public void DrawToTarget()
        {
            if (!elems.Any()) return;

            if (target == null)
            {
                RecreateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));
            }

            var spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            foreach (var elem in elems) elem.DrawOnRenderTarget(spriteBatch);
            spriteBatch.End();

            device.SetRenderTarget(null);
        }
    }
}