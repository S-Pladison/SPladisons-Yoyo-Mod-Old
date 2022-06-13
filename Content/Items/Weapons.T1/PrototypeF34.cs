using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Drawing;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class PrototypeF34 : YoyoItem
    {
        public PrototypeF34() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<PrototypeF34Projectile>();

            Item.rare = ItemRarityID.Green;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class PrototypeF34Projectile : YoyoProjectile, IDrawOnRenderTarget
    {
        public PrototypeF34Projectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetDefaults()
        {
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            PrototypeF34EffectSystem.AddElement(this);
        }

        public override bool PreKill(int timeLeft)
        {
            PrototypeF34EffectSystem.RemoveElement(this);
            return true;
        }

        void IDrawOnRenderTarget.DrawOnRenderTarget(SpriteBatch spriteBatch)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            var position = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition + zero;
            var texture = ModAssets.GetExtraTexture(1);
            spriteBatch.Draw(texture.Value, position, null, Color.White, 0f, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
        }
    }

    [Autoload(Side = ModSide.Client)]
    public sealed class PrototypeF34EffectSystem : ModSystem
    {
        public static Effect Effect { get; private set; }

        private List<IDrawOnRenderTarget> elems;
        private RenderTarget2D elemTarget;
        private RenderTarget2D swapTileTarget;

        // ...

        public static void AddElement(IDrawOnRenderTarget elem)
        {
            var elems = ModContent.GetInstance<PrototypeF34EffectSystem>().elems;
            if (!elems.Contains(elem)) elems.Add(elem);
        }

        public static void RemoveElement(IDrawOnRenderTarget elem)
        {
            ModContent.GetInstance<PrototypeF34EffectSystem>().elems.Remove(elem);
        }

        // ...

        public override void Load()
        {
            var device = Main.graphics.GraphicsDevice;
            elems = new List<IDrawOnRenderTarget>();

            Main.QueueMainThreadAction(() => InitTarget(device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight));

            Main.OnRenderTargetsInitialized += InitTarget;
            Main.OnRenderTargetsReleased += ReleaseTarget;
            SPladisonsYoyoMod.Events.OnPostDraw += PostDraw;
            SPladisonsYoyoMod.Events.OnPostUpdateCameraPosition += () =>
            {
                if (swapTileTarget is null || !elems.Any()) return;

                var main = Main.instance;
                var device = main.GraphicsDevice;
                var sb = Main.spriteBatch;

                device.SetRenderTarget(swapTileTarget);
                device.Clear(Color.Transparent);

                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, Effect, Matrix.Identity);
                sb.Draw(main.tileTarget, Vector2.Zero, Color.White);
                foreach (var elem in elems) elem.DrawOnRenderTarget(sb);
                sb.End();

                device.SetRenderTarget(null);

                (main.tileTarget, swapTileTarget) = (swapTileTarget, main.tileTarget);
            };

            //Effect = ModAssets.GetEffect("PrototypeF34", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void Unload()
        {
            Main.OnRenderTargetsInitialized -= InitTarget;
            Main.OnRenderTargetsReleased -= ReleaseTarget;

            elems.Clear();
            elems = null;
        }

        private void InitTarget(int width, int height)
        {
            var device = Main.graphics.GraphicsDevice;
            elemTarget = new RenderTarget2D(device, width, height, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
            swapTileTarget = new RenderTarget2D(device, width, height, false, device.PresentationParameters.BackBufferFormat, DepthFormat.None);
        }

        private void ReleaseTarget()
        {
            swapTileTarget?.Dispose();
        }

        private void PostDraw(GameTime _ = null)
        {
            if (swapTileTarget is null || !elems.Any()) return;

            var main = Main.instance;
            (main.tileTarget, swapTileTarget) = (swapTileTarget, main.tileTarget);
        }
    }
}