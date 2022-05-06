using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SPladisonsYoyoMod.Common.Drawing.Primitives
{
    public class PrimitiveData
    {
        public Effect Effect { get; private set; }
        public PrimitiveType PrimitiveType { get; private set; }

        // ...

        public List<VertexPositionColorTexture> Vertices;
        public int PrimitivesCount;

        // ...

        public delegate void OnUpdateEffectParametersDelegate(Effect effect);
        public OnUpdateEffectParametersDelegate OnUpdateEffectParameters;

        // ...

        public PrimitiveData(PrimitiveType type, int primitivesCount, List<VertexPositionColorTexture> vertices, Effect effect)
        {
            Effect = effect;
            PrimitiveType = type;
            PrimitivesCount = primitivesCount;
            Vertices = vertices ?? new List<VertexPositionColorTexture>();
            Effect = effect;
        }

        public void UpdateEffectParameters(Matrix matrix)
        {
            Effect.Parameters["WorldViewProj"].SetValue(matrix);
            OnUpdateEffectParameters?.Invoke(Effect);
        }

        public void Draw(GraphicsDevice graphics, Matrix matrix)
        {
            RecreateVertices();

            if (PrimitivesCount <= 0) return;

            UpdateEffectParameters(matrix);
            DrawPrimitives(graphics);
        }

        public virtual void RecreateVertices() { }

        public virtual void DrawPrimitives(GraphicsDevice graphics)
        {
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives(PrimitiveType, Vertices.ToArray(), 0, PrimitivesCount);
            }
        }
    }

    public class IndexedPrimitiveData : PrimitiveData
    {

        public List<short> Indeces;

        // ...

        public IndexedPrimitiveData(PrimitiveType type, int primitivesCount, List<VertexPositionColorTexture> vertices, List<short> indeces, Effect effect) : base(type, primitivesCount, vertices, effect)
        {
            Indeces = indeces ?? new List<short>();
        }

        public override void DrawPrimitives(GraphicsDevice graphics)
        {
            foreach (var pass in Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserIndexedPrimitives(PrimitiveType, Vertices.ToArray(), 0, Vertices.Count, Indeces.ToArray(), 0, PrimitivesCount);
            }
        }
    }
}
