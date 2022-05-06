using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Drawing
{
    // It's totally fucked up :l
    // Don't kill me, I'm just learning

    [Autoload(Side = ModSide.Client)]
    public class DrawingManager : ILoadable
    {
        private static readonly List<DrawKey> possibleKeys = new();
        private static readonly List<DrawKey> pixelatedKeys = new();

        private static readonly Dictionary<DrawLayers, RenderTarget2D> pixelatedRenderTargets = new();
        private static readonly Dictionary<DrawKey, List<DrawData>> drawDatas = new();
        private static readonly Dictionary<DrawKey, bool> canDrawDataBooleans = new();
        private static readonly Dictionary<DrawLayers, CustomDrawDelegate> customDrawMethods = new();

        // ...

        void ILoadable.Load(Mod mod)
        {
            foreach (DrawLayers layer in Enum.GetValues(typeof(DrawLayers)))
            {
                foreach (DrawTypeFlags flags in Enum.GetValues(typeof(DrawTypeFlags)))
                {
                    var key = new DrawKey(layer, flags);

                    if (drawDatas.ContainsKey(key)) continue;

                    possibleKeys.Add(key);
                    drawDatas.Add(key, new List<DrawData>());
                    canDrawDataBooleans.Add(key, false);

                    if (!key.Pixelated) continue;

                    pixelatedKeys.Add(key);
                }

                customDrawMethods.Add(layer, null);
                pixelatedRenderTargets.Add(layer, null);
            }

            Main.OnResolutionChanged += ClearRenderTargets;
            Main.OnPostDraw += ResetCanDrawDataDictionary;
            SPladisonsYoyoMod.PostUpdateCameraPositionEvent += DrawLayersToTarget;
        }

        void ILoadable.Unload()
        {
            Main.OnResolutionChanged -= ClearRenderTargets;
            Main.OnPostDraw -= ResetCanDrawDataDictionary;

            customDrawMethods.Clear();
            canDrawDataBooleans.Clear();
            drawDatas.Clear();
            pixelatedRenderTargets.Clear();

            pixelatedKeys.Clear();
            possibleKeys.Clear();
        }

        // ...

        public static IReadOnlyList<DrawKey> GetAllPossibleKeys() => possibleKeys;

        public static void AddToEachExistingVariant(CanDrawDelegate canDraw, DrawDelegate draw, byte? priority = null)
        {
            foreach (var key in possibleKeys)
            {
                AddToLayer(key, canDraw, draw, priority);
            }
        }

        public static void AddToEachLayer(DrawTypeFlags drawFlags, CanDrawDelegate canDraw, DrawDelegate draw, byte? priority = null)
        {
            foreach (DrawKey key in possibleKeys)
            {
                if (!key.Flags.HasFlag(drawFlags)) continue;

                AddToLayer(key, canDraw, draw, priority);
            }
        }

        public static void AddToLayer(DrawLayers layer, DrawTypeFlags drawFlags, CanDrawDelegate canDraw, DrawDelegate draw, byte? priority = null)
        {
            AddToLayer(new DrawKey(layer, drawFlags), canDraw, draw, priority);
        }

        public static void AddToLayer(DrawKey key, CanDrawDelegate canDraw, DrawDelegate draw, byte? priority = null)
        {
            if (Main.dedServ) return;

            canDraw ??= (_) => false;
            draw ??= (_, _) => { };

            var data = new DrawData(canDraw, draw);
            data.Priority = priority ?? 122;

            var list = drawDatas[key];
            list.Add(data);
            list.Sort((x, y) => x.Priority.CompareTo(y.Priority));
        }

        public static void AddCustomMethodToLayer(DrawLayers layer, CustomDrawDelegate draw)
        {
            if (Main.dedServ) return;
            customDrawMethods[layer] += draw;
        }

        public static void DrawLayer(DrawLayers layer)
        {
            var spriteBatch = Main.spriteBatch;
            customDrawMethods[layer]?.Invoke(spriteBatch, layer);

            var key = new DrawKey(layer, DrawTypeFlags.None);
            DrawLayer_NotPixelated(spriteBatch, key, BlendState.AlphaBlend);

            key.Flags = DrawTypeFlags.Pixelated;
            DrawLayer_Pixelated(spriteBatch, key, BlendState.AlphaBlend);

            key.Flags = DrawTypeFlags.Additive;
            DrawLayer_NotPixelated(spriteBatch, key, BlendState.Additive);

            key.Flags = DrawTypeFlags.All;
            DrawLayer_Pixelated(spriteBatch, key, BlendState.Additive);
        }

        // ...

        private static void DrawLayer_NotPixelated(SpriteBatch spriteBatch, DrawKey key, BlendState blendState)
        {
            UpdateCanDrawDataDictionary(key);
            if (!CanBeginSpriteBatch(key)) return;

            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (var data in drawDatas[key]) data.Draw(spriteBatch, key);
            spriteBatch.End();
        }

        private static void DrawLayer_Pixelated(SpriteBatch spriteBatch, DrawKey key, BlendState blendState)
        {
            if (!CanBeginSpriteBatch(key)) return;

            var target = pixelatedRenderTargets[key.Layer] ??= RecreateRenderTarget(spriteBatch.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            spriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private static void DrawLayersToTarget()
        {
            var spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;
            var matrix = Matrix.CreateScale(0.5f);

            foreach (var key in pixelatedKeys)
            {
                UpdateCanDrawDataDictionary(key);
                if (!CanBeginSpriteBatch(key)) continue;

                var target = pixelatedRenderTargets[key.Layer] ??= RecreateRenderTarget(spriteBatch.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                var blendState = key.Additive ? BlendState.Additive : BlendState.AlphaBlend;

                device.SetRenderTarget(target);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, matrix * Main.GameViewMatrix.EffectMatrix);
                foreach (var data in drawDatas[key]) data.Draw(spriteBatch, key);
                spriteBatch.End();

                device.SetRenderTarget(null);
            }
        }

        private static bool CanBeginSpriteBatch(DrawKey key) => canDrawDataBooleans[key];

        private static void UpdateCanDrawDataDictionary(DrawKey key)
        {
            foreach (var data in drawDatas[key])
            {
                if (data.CanDraw(key))
                {
                    canDrawDataBooleans[key] = true;
                    return;
                }
            }
        }

        private static void ResetCanDrawDataDictionary(GameTime _)
        {
            foreach (var key in canDrawDataBooleans.Keys.ToArray())
            {
                canDrawDataBooleans[key] = false;
            }
        }

        private static void ClearRenderTargets(Vector2 _)
        {
            foreach (var key in pixelatedRenderTargets.Keys)
            {
                pixelatedRenderTargets[key] = null;
            }
        }

        private static RenderTarget2D RecreateRenderTarget(GraphicsDevice device, int width, int height) => new(device, width / 2, height / 2);

        // ...

        public delegate bool CanDrawDelegate(DrawKey key);
        public delegate void DrawDelegate(SpriteBatch spriteBatch, DrawKey key);
        public delegate void CustomDrawDelegate(SpriteBatch spriteBatch, DrawLayers layer);

        // ...

        private class DrawData
        {
            public byte Priority;

            public CanDrawDelegate CanDraw { get; }
            public DrawDelegate Draw { get; }

            public DrawData(CanDrawDelegate canDraw, DrawDelegate draw)
            {
                CanDraw = canDraw;
                Draw = draw;
            }
        }
    }
}
