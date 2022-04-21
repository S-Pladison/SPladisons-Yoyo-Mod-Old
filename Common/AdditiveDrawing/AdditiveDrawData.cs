using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SPladisonsYoyoMod.Common.AdditiveDrawing
{
    public struct AdditiveDrawData
    {
        public readonly bool UseDestinationRectangle;
        public readonly bool Pixilated;

        public Texture2D texture;
        public Vector2 position;
        public Rectangle destinationRectangle;
        public Rectangle? sourceRect;
        public Color color;
        public float rotation;
        public Vector2 origin;
        public Vector2 scale;
        public SpriteEffects effect;

        public AdditiveDrawData(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, bool pixilated)
        {
            this.UseDestinationRectangle = false;
            this.Pixilated = pixilated;

            this.texture = texture;
            this.position = position;
            this.color = color;
            this.destinationRectangle = default(Rectangle);
            this.sourceRect = sourceRect;
            this.rotation = 0f;
            this.origin = Vector2.Zero;
            this.scale = Vector2.One;
            this.effect = SpriteEffects.None;
        }

        public AdditiveDrawData(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, bool pixilated)
        {
            this.UseDestinationRectangle = false;
            this.Pixilated = pixilated;

            this.texture = texture;
            this.position = position;
            this.sourceRect = sourceRect;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            this.scale = scale;
            this.effect = effect;
            this.destinationRectangle = default;
        }

        public AdditiveDrawData(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, SpriteEffects effect, bool pixilated)
        {
            this.UseDestinationRectangle = true;
            this.Pixilated = pixilated;

            this.texture = texture;
            this.destinationRectangle = destinationRectangle;
            this.sourceRect = sourceRect;
            this.color = color;
            this.rotation = rotation;
            this.origin = origin;
            this.effect = effect;
            this.position = Vector2.Zero;
            this.scale = Vector2.One;
        }

        public void Draw(SpriteBatch sb)
        {
            if (this.UseDestinationRectangle)
            {
                sb.Draw(this.texture, this.destinationRectangle, this.sourceRect, this.color, this.rotation, this.origin, this.effect, 0f);
                return;
            }

            sb.Draw(this.texture, this.position, this.sourceRect, this.color, this.rotation, this.origin, this.scale, this.effect, 0f);
        }
    }
}
