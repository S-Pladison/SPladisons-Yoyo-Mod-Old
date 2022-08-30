using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Graphics.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Graphics
{
    [Autoload(Side = ModSide.Client)]
    public partial class DrawSystem : ILoadable
    {
        public delegate void PostDrawLayerDelegate(SpriteBatch spriteBatch);

        public static Matrix TransformMatrix { get; private set; }
        public static Matrix PixelatedTransformMatrix { get; private set; }
        public static Matrix ActiveTransformMatrix { get; private set; }

        private Dictionary<DrawLayers, PostDrawLayerDelegate> postDrawMethodsDict;
        private Dictionary<DrawKey, DrawSublayer> sublayersDict;

        // ...

        void ILoadable.Load(Mod mod)
        {
            postDrawMethodsDict = new();
            sublayersDict = new();

            foreach (DrawLayers layer in Enum.GetValues(typeof(DrawLayers)))
            {
                if (!postDrawMethodsDict.ContainsKey(layer))
                {
                    postDrawMethodsDict.Add(layer, (s) => { });
                }

                foreach (DrawTypeFlags flags in Enum.GetValues(typeof(DrawTypeFlags)))
                {
                    var key = new DrawKey(layer, flags);

                    if (!sublayersDict.ContainsKey(key))
                    {
                        BlendState blendState = flags.HasFlag(DrawTypeFlags.Additive) ? BlendState.Additive : BlendState.AlphaBlend;
                        DrawSublayer sublayer = flags.HasFlag(DrawTypeFlags.Pixelated) ? new DrawSublayer.Pixelated(blendState) : new DrawSublayer.Default(blendState);
                        sublayersDict.Add(key, sublayer);
                    }
                }
            }

            SPladisonsYoyoMod.Events.OnPostDraw += ClearDrawData;
            SPladisonsYoyoMod.Events.OnResolutionChanged += RecreateRenderTargets;
            SPladisonsYoyoMod.Events.OnPostUpdateCameraPosition += OnPostUpdateCameraPosition;

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

            sublayersDict.Clear();
            sublayersDict = null;
        }

        private void DrawLayer(DrawLayers layer, SpriteBatch spriteBatch)
        {
            void DrawSublayer(DrawTypeFlags drawType)
            {
                var key = new DrawKey(layer, drawType);
                sublayersDict[key].DrawOnScreen(spriteBatch);
            }

            DrawSublayer(DrawTypeFlags.None);
            DrawSublayer(DrawTypeFlags.Pixelated);
            DrawSublayer(DrawTypeFlags.Additive);
            DrawSublayer(DrawTypeFlags.All);

            postDrawMethodsDict[layer].Invoke(spriteBatch);
        }

        private void OnPostUpdateCameraPosition()
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

            // ...

            var spriteBatch = Main.spriteBatch;
            var device = spriteBatch.GraphicsDevice;

            foreach (var (_, sublayer) in sublayersDict)
            {
                sublayer.DrawOnTarget(spriteBatch, device);
            }
        }

        private void ClearDrawData(GameTime _ = null)
        {
            foreach (var (_, sublayer) in sublayersDict)
            {
                sublayer.ClearData();
            }
        }

        private void RecreateRenderTargets(Vector2 size)
        {
            foreach (var (_, sublayer) in sublayersDict)
            {
                sublayer.RecreateTarget(size);
            }
        }

        private void UpdateTransformMatrix()
        {
            var matrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);

            TransformMatrix = PixelatedTransformMatrix = matrix;
            TransformMatrix *= Main.GameViewMatrix.EffectMatrix;

            matrix = Matrix.CreateTranslation(Main.screenWidth / 2, Main.screenHeight / -2, 0);
            matrix *= Matrix.CreateRotationZ(MathHelper.Pi);

            PixelatedTransformMatrix *= matrix;
            TransformMatrix *= matrix;
            TransformMatrix *= Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1f);

            matrix = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 1000);

            TransformMatrix *= matrix;
            PixelatedTransformMatrix *= matrix;
            ActiveTransformMatrix = TransformMatrix;
        }

        public void AddToLayer(DrawLayers layer, DrawTypeFlags drawType, IDrawData data)
            => AddToLayer(new DrawKey(layer, drawType), data);

        public void AddToLayer(DrawKey drawKey, IDrawData data)
            => sublayersDict[drawKey].AddData(data);

        // ...

        public static void AddPostDrawLayerMethod(DrawLayers layer, PostDrawLayerDelegate drawMethod)
        {
            if (drawMethod is not null)
            {
                ModContent.GetInstance<DrawSystem>().postDrawMethodsDict[layer] += drawMethod;
            }
        }
    }
}