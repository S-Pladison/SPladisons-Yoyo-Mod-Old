using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace SPladisonsYoyoMod.Common.Graphics
{
    public partial class DrawSystem
    {
        private abstract class DrawSublayer
        {
            private readonly List<IDrawData> drawDataList;
            private readonly BlendState blendState;

            public DrawSublayer(BlendState blendState)
            {
                this.drawDataList = new();
                this.blendState = blendState;
            }

            public abstract void DrawOnScreen(SpriteBatch spriteBatch);
            public virtual void DrawOnTarget(SpriteBatch spriteBatch, GraphicsDevice device) { }
            public virtual void RecreateTarget(Vector2 size) { }
            public void ClearData() => drawDataList.Clear();
            public void AddData(IDrawData data) => drawDataList.Add(data);

            // ...

            public class Default : DrawSublayer
            {
                public Default(BlendState blendState) : base(blendState) { }

                public override void DrawOnScreen(SpriteBatch spriteBatch)
                {
                    if (!drawDataList.Any()) return;

                    spriteBatch.Begin(SpriteSortMode.Deferred, blendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    foreach (var data in drawDataList) data.Draw(spriteBatch);
                    spriteBatch.End();
                }
            }

            public class Pixelated : DrawSublayer
            {
                private readonly Matrix matrix;
                private RenderTarget2D target;

                public Pixelated(BlendState blendState) : base(blendState)
                {
                    matrix = Matrix.CreateScale(0.5f);

                    Main.QueueMainThreadAction(() => RecreateTarget(new Vector2(Main.screenWidth, Main.screenHeight)));
                }

                public override void DrawOnScreen(SpriteBatch spriteBatch)
                {
                    if (!drawDataList.Any()) return;

                    spriteBatch.Begin(SpriteSortMode.Deferred, blendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    spriteBatch.Draw(target, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
                    spriteBatch.End();
                }

                public override void DrawOnTarget(SpriteBatch spriteBatch, GraphicsDevice device)
                {
                    if (!drawDataList.Any()) return;

                    ActiveTransformMatrix = PixelatedTransformMatrix;

                    device.SetRenderTarget(target);
                    device.Clear(Color.Transparent);

                    spriteBatch.Begin(SpriteSortMode.Deferred, blendState, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, matrix);
                    foreach (var data in drawDataList) data.Draw(spriteBatch);
                    spriteBatch.End();

                    device.SetRenderTarget(null);

                    ActiveTransformMatrix = TransformMatrix;
                }

                public override void RecreateTarget(Vector2 size)
                {
                    target = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)size.X / 2, (int)size.Y / 2);
                }
            }
        }
    }
}