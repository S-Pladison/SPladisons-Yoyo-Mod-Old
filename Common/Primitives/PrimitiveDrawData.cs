using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace SPladisonsYoyoMod.Common.Primitives
{
    public struct PrimitiveDrawData
    {
        public Asset<Effect> Effect { get; set; }
        public PrimitiveDrawLayer Layer { get; set; }
        public int PrimitivesCount { get; set; }
        public PrimitiveType PrimitiveType { get; set; }
        public VertexPositionColorTexture[] Vertices { get; set; }

        public PrimitiveDrawData(PrimitiveDrawLayer layer, PrimitiveType type, int primitivesCount, VertexPositionColorTexture[] vertices, Asset<Effect> effect = null)
        {
            Layer = layer;
            PrimitiveType = type;
            PrimitivesCount = primitivesCount;
            Vertices = vertices;

            Effect = effect ?? ModAssets.GetEffect("Primitive", AssetRequestMode.ImmediateLoad);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (PrimitivesCount <= 0) return;

            var graphics = spriteBatch.GraphicsDevice;
            foreach (var pass in Effect.Value.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives(PrimitiveType, Vertices, 0, PrimitivesCount);
            }
        }
    }
}
