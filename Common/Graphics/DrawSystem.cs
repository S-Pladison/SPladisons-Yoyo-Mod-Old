using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Graphics
{
    [Autoload(Side = ModSide.Client)]
    public class DrawSystem : ILoadable
    {
        public delegate void PostDrawLayerDelegate(SpriteBatch spriteBatch);

        public static Matrix TransformMatrix { get; private set; }

        private Matrix pixelatedMatrix = Matrix.CreateScale(0.5f);
        private Dictionary<DrawKey, List<IDrawData>> drawDataDict;
        private Dictionary<DrawKey, RenderTarget2D> renderTargetDict;
        private Dictionary<DrawLayers, PostDrawLayerDelegate> postDrawMethodsDict;

        // ...

        void ILoadable.Load(Mod mod)
        {
            drawDataDict = new();
            renderTargetDict = new();
            postDrawMethodsDict = new();

            foreach (DrawLayers layer in Enum.GetValues(typeof(DrawLayers)))
            {
                foreach (DrawTypeFlags flags in Enum.GetValues(typeof(DrawTypeFlags)))
                {
                    var key = new DrawKey(layer, flags);

                    if (!drawDataDict.ContainsKey(key))
                    {
                        drawDataDict.Add(key, new List<IDrawData>());
                        renderTargetDict.Add(key, null);
                    }
                }

                if (!postDrawMethodsDict.ContainsKey(layer))
                {
                    postDrawMethodsDict.Add(layer, (s) => { });
                }
            }

            SPladisonsYoyoMod.Events.OnPostDraw += ClearDrawData;
            SPladisonsYoyoMod.Events.OnResolutionChanged += RecreateRenderTargets;
            SPladisonsYoyoMod.Events.OnPostUpdateCameraPosition += DrawOnRenderTargets;

            Main.QueueMainThreadAction(() => RecreateRenderTargets(new Vector2(Main.screenWidth, Main.screenHeight)));

            On.Terraria.Main.DoDraw_WallsAndBlacks += (orig, main) =>
            {
                orig(main);

                var spriteBatch = Main.spriteBatch;
                var spriteBatchInfo = new SpriteBatchInfo(spriteBatch);

                spriteBatch.End();
                DrawLayer(DrawLayers.Walls, Main.spriteBatch);
                spriteBatchInfo.Begin(spriteBatch);
            };

            On.Terraria.Main.DoDraw_Tiles_Solid += (orig, main) =>
            {
                orig(main);
                DrawLayer(DrawLayers.Tiles, Main.spriteBatch);
            };

            On.Terraria.Main.DrawDust += (orig, main) =>
            {
                orig(main);
                DrawLayer(DrawLayers.Dusts, Main.spriteBatch);
            };

            On.Terraria.Main.DrawBackgroundBlackFill += (orig, main) =>
            {
                orig(main);
                UpdateTransformMatrix();
            };
        }

        void ILoadable.Unload()
        {
            postDrawMethodsDict.Clear();
            postDrawMethodsDict = null;

            ClearDrawData();

            drawDataDict.Clear();
            drawDataDict = null;

            ClearRenderTargets();

            renderTargetDict.Clear();
            renderTargetDict = null;
        }

        private void DrawLayer(DrawLayers layer, SpriteBatch spriteBatch)
        {
            void DrawLayer(DrawTypeFlags drawType, BlendState blendState)
            {
                var key = new DrawKey(layer, drawType);

                if (!drawDataDict[key].Any()) return;

                spriteBatch.Begin(SpriteSortMode.Deferred, blendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw(renderTargetDict[key], new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                spriteBatch.End();
            }

            DrawLayer(DrawTypeFlags.None, BlendState.AlphaBlend);
            DrawLayer(DrawTypeFlags.Pixelated, BlendState.AlphaBlend);
            DrawLayer(DrawTypeFlags.Additive, BlendState.Additive);
            DrawLayer(DrawTypeFlags.All, BlendState.Additive);

            postDrawMethodsDict[layer].Invoke(spriteBatch);
        }

        private void DrawOnRenderTargets()
        {
            var spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;

            foreach (var (key, _) in renderTargetDict)
            {
                if (!drawDataDict[key].Any()) continue;

                var target = renderTargetDict[key];
                var drawType = key.DrawType;
                var blendState = drawType.HasFlag(DrawTypeFlags.Additive) ? BlendState.Additive : BlendState.AlphaBlend;
                var matrix = drawType.HasFlag(DrawTypeFlags.Pixelated) ? pixelatedMatrix : Matrix.Identity;

                device.SetRenderTarget(target);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, matrix);
                foreach (var data in drawDataDict[key]) data.Draw(spriteBatch);
                spriteBatch.End();

                device.SetRenderTarget(null);
            }
        }

        private void ClearDrawData(GameTime _ = null)
        {
            foreach (var (_, value) in drawDataDict)
            {
                value.Clear();
            }
        }

        private void ClearRenderTargets()
        {
            foreach (var (key, _) in renderTargetDict)
            {
                renderTargetDict[key] = null;
            }
        }

        private void RecreateRenderTargets(Vector2 size)
        {
            int width = (int)size.X;
            int height = (int)size.Y;

            int halfWidth = width / 2;
            int halfHeight = height / 2;

            foreach (var (key, _) in renderTargetDict)
            {
                renderTargetDict[key] = key.DrawType.HasFlag(DrawTypeFlags.Pixelated)
                    ? new RenderTarget2D(Main.graphics.GraphicsDevice, halfWidth, halfHeight)
                    : new RenderTarget2D(Main.graphics.GraphicsDevice, width, height);
            }
        }

        private void UpdateTransformMatrix()
        {
            TransformMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
            TransformMatrix *= Matrix.CreateTranslation(Main.screenWidth / 2, Main.screenHeight / -2, 0);
            TransformMatrix *= Matrix.CreateRotationZ(MathHelper.Pi);
            TransformMatrix *= Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 1000);
        }

        public void AddToLayer(DrawLayers layer, DrawTypeFlags drawType, IDrawData data)
            => AddToLayer(new DrawKey(layer, drawType), data);

        public void AddToLayer(DrawKey drawKey, IDrawData data)
            => drawDataDict[drawKey].Add(data);

        // ...

        public struct DrawKey
        {
            public DrawLayers Layer;
            public DrawTypeFlags DrawType;

            public DrawKey(DrawLayers layer, DrawTypeFlags drawType)
            {
                Layer = layer;
                DrawType = drawType;
            }
        }

        // ...

        public static void AddPostDrawLayerMethod(DrawLayers layer, PostDrawLayerDelegate drawMethod)
        {
            if (drawMethod is not null)
            {
                ModContent.GetInstance<DrawSystem>().postDrawMethodsDict[layer] += drawMethod;
            }
        }

        public static void GetDrawData()
        {
            var instance = ModContent.GetInstance<DrawSystem>();

            foreach (var proj in Main.projectile)
            {
                if (proj.active && proj.ModProjectile is IDrawOnDifferentLayers obj)
                {
                    obj.DrawOnDifferentLayers(instance);
                }
            }

            foreach (var particle in ParticleSystem.particles)
            {
                particle.Draw(instance);
            }
        }
    }
}