using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SPladisonsYoyoMod.Common.Primitives
{
    public interface IDrawPrimitives
    {
        PrimitiveDrawLayer PrimitiveDrawLayer { get; }

        void DrawPrimitives(SpriteBatch spriteBatch, Matrix matrix);
    }
}
