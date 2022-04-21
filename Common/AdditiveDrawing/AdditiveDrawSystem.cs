using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.AdditiveDrawing
{
    public class AdditiveDrawSystem : ModSystem
    {
        private static Dictionary<bool, List<AdditiveDrawData>> dataCache;
        private static RenderTarget2D target;

        public static void AddToDataCache(AdditiveDrawData data)
        {
            dataCache[data.Pixilated].Add(data);
        }

        public static void ClearDataCache()
        {
            foreach (var data in dataCache)
            {
                data.Value.Clear();
            }
        }

        public static void RecreateRenderTarget(Vector2 screenSize)
        {
            var size = (screenSize / 2).ToPoint();
            target = new RenderTarget2D(Main.graphics.GraphicsDevice, size.X, size.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        }

        public static void DrawToScreen(SpriteBatch spriteBatch)
        {
            bool canDraw = false;
            canDraw |= dataCache[false].Count != 0;
            canDraw |= ParticleSystem.ActiveParticles != 0;

            if (canDraw)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                foreach (var data in dataCache[false])
                {
                    data.Draw(spriteBatch);
                }

                ParticleSystem.DrawParticles();

                spriteBatch.End();
            }

            canDraw = false;
            canDraw |= dataCache[true].Count != 0;

            if (canDraw)
            {
                if (target == null) RecreateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                spriteBatch.Draw((Texture2D)target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }

        private static void GetDataFromEntities()
        {
            List<AdditiveDrawData> list = new();

            foreach (var proj in Main.projectile)
            {
                if (proj.ModProjectile is IDrawAdditive elem && proj.active)
                {
                    elem.DrawAdditive(list);
                }
            }

            foreach (var data in list)
            {
                dataCache[data.Pixilated].Add(data);
            }
        }

        private static void DrawToTarget()
        {
            if (dataCache[true].Count == 0) return;

            var spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;

            if (target == null) RecreateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));

            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.CreateScale(0.5f) * Main.GameViewMatrix.EffectMatrix);
            foreach (var data in dataCache[true])
            {
                data.Draw(spriteBatch);
            }
            spriteBatch.End();

            device.SetRenderTarget(null);
        }

        private static void PreDraw(GameTime time)
        {
            ClearDataCache();
        }

        // ...

        public override void Load()
        {
            dataCache = new(2);
            dataCache.Add(true, new());
            dataCache.Add(false, new());

            SPladisonsYoyoMod.PostUpdateCameraPositionEvent += () =>
            {
                GetDataFromEntities();
                DrawToTarget();
            };

            Main.OnResolutionChanged += RecreateRenderTarget;
            Main.OnPreDraw += PreDraw;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= PreDraw;
            Main.OnResolutionChanged -= RecreateRenderTarget;

            ClearDataCache();
            dataCache.Clear();

            dataCache = null;
            target = null;
        }
    }
}
