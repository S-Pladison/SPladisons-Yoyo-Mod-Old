using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SPladisonsYoyoMod.Common.Drawing.AdditionalDrawing
{
    public struct AdditionalDrawingData
    {
        public readonly bool UseDestinationRectangle;

        public Texture2D Texture;
        public Vector2 Position;
        public Rectangle DestinationRectangle;
        public Rectangle? SourceRect;
        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effect;

        public AdditionalDrawingData(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color)
        {
            UseDestinationRectangle = false;
            Texture = texture;
            Position = position;
            Color = color;
            DestinationRectangle = default(Rectangle);
            SourceRect = sourceRect;
            Rotation = 0f;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Effect = SpriteEffects.None;
        }

        public AdditionalDrawingData(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect)
        {
            UseDestinationRectangle = false;
            Texture = texture;
            Position = position;
            SourceRect = sourceRect;
            Color = color;
            Rotation = rotation;
            Origin = origin;
            Scale = scale;
            Effect = effect;
            DestinationRectangle = default;
        }

        public AdditionalDrawingData(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, SpriteEffects effect)
        {
            UseDestinationRectangle = true;
            Texture = texture;
            DestinationRectangle = destinationRectangle;
            SourceRect = sourceRect;
            Color = color;
            Rotation = rotation;
            Origin = origin;
            Effect = effect;
            Position = Vector2.Zero;
            Scale = Vector2.One;
        }

        public void Draw(SpriteBatch sb)
        {
            if (UseDestinationRectangle)
            {
                sb.Draw(Texture, DestinationRectangle, SourceRect, Color, Rotation, Origin, Effect, 0f);
                return;
            }

            sb.Draw(Texture, Position, SourceRect, Color, Rotation, Origin, Scale, Effect, 0f);
        }
    }
}
