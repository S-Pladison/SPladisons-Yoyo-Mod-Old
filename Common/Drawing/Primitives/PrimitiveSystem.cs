using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Drawing.Primitives
{
    [Autoload(Side = ModSide.Client)]
    public class PrimitiveSystem : ModSystem
    {
        private static readonly Dictionary<DrawKey, List<PrimitiveData>> dataCache = new();

        private static Matrix transformMatrix;
        private static Matrix pixelatedTransformMatrix;

        // ...

        public override void Load()
        {
            Main.OnPostDraw += ClearDataCache;
        }

        public override void PostSetupContent()
        {
            foreach (var key in DrawingManager.GetAllPossibleKeys())
            {
                dataCache.Add(key, new List<PrimitiveData>());
            }

            DrawingManager.AddToEachExistingVariant(Any, Draw, 50);
        }

        public override void Unload()
        {
            Main.OnPostDraw -= ClearDataCache;

            ClearDataCache();
            dataCache.Clear();
        }

        public override void OnWorldUnload()
        {
            ClearDataCache();
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix _)
        {
            var matrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
            matrix *= Main.GameViewMatrix.EffectMatrix;
            matrix *= Matrix.CreateTranslation(Main.screenWidth / 2, Main.screenHeight / -2, 0);
            matrix *= Matrix.CreateRotationZ(MathHelper.Pi);

            transformMatrix = pixelatedTransformMatrix = matrix;
            transformMatrix *= Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1);

            matrix = Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 1000);

            transformMatrix *= matrix;
            pixelatedTransformMatrix *= matrix;
        }

        // ...

        public static bool Any(DrawKey key) => dataCache[key].Any();

        public static void AddToDataCache(DrawLayers layer, DrawTypeFlags flags, PrimitiveData data)
        {
            dataCache[new DrawKey(layer, flags)].Add(data);
        }

        public static void ClearDataCache(GameTime _ = null)
        {
            foreach (var list in dataCache.Values)
            {
                list.Clear();
            }
        }

        public static void Draw(SpriteBatch spriteBatch, DrawKey key)
        {
            var device = spriteBatch.GraphicsDevice;
            var matrix = GetTransformMatrix(key.Flags);
            var cullMode = device.RasterizerState.CullMode;
            var blendState = device.BlendState;

            // ... I don't like it at all
            device.RasterizerState.CullMode = Main.Rasterizer.CullMode;
            device.BlendState = key.Additive ? BlendState.Additive : BlendState.AlphaBlend;

            foreach (var data in dataCache[key])
            {
                data.Draw(device, matrix);
            }

            device.BlendState = blendState;
            device.RasterizerState.CullMode = cullMode;
        }

        public static Matrix GetTransformMatrix(DrawTypeFlags flags) => GetTransformMatrix(flags.HasFlag(DrawTypeFlags.Pixelated));
        public static Matrix GetTransformMatrix(bool pixelated) => pixelated ? pixelatedTransformMatrix : transformMatrix;
    }
}