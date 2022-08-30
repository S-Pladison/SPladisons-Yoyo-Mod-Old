using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Utilities;
using System;
using Terraria;

namespace SPladisonsYoyoMod.Common.Graphics.Primitives
{
    public class PrimitiveSquare : IndexedPrimitiveData
    {
        public Vector2 Position;
        public Vector2 Size;
        public float Rotation;
        public Color[] Colors;
        public SpriteEffects SpriteEffect;

        public PrimitiveSquare(Vector2 position, Vector2 size, float rotation, Color[] colors, SpriteEffects spriteEffect, IPrimitiveEffect effect) : base(PrimitiveType.TriangleStrip, 0, null, null, effect)
        {
            if (colors is null) throw new Exception("Ok...");
            if (colors.Length != 4) throw new Exception("Are you stupid? The square has 4 corners...");

            Position = position;
            Size = size;
            Rotation = rotation;
            Colors = colors;
            SpriteEffect = spriteEffect;

            Indeces.AddRange(new short[] { 0, 1, 2, 3 });
            PrimitivesCount = Indeces.Count - 2;
        }

        public PrimitiveSquare(Vector2 position, Vector2 size, float rotation, Color color, SpriteEffects spriteEffect, IPrimitiveEffect effect) : base(PrimitiveType.TriangleStrip, 0, null, null, effect)
        {
            Position = position;
            Size = size;
            Rotation = rotation;
            Colors = new Color[] { color, color, color, color };
            SpriteEffect = spriteEffect;

            Indeces.AddRange(new short[] { 0, 1, 2, 3 });
            PrimitivesCount = Indeces.Count - 2;
        }

        public override void RecreateVertices()
        {
            Vertices.Clear();

            int colorIndex = 0;

            void AddVertex(Vector2 offset)
            {
                var texCoord = new Vector2((Convert.ToBoolean(offset.X) ^ SpriteEffect.HasFlag(SpriteEffects.FlipHorizontally)).ToInt(), (Convert.ToBoolean(offset.Y) ^ SpriteEffect.HasFlag(SpriteEffects.FlipVertically)).ToInt());
                Vertices.AddVertex(Position - Main.screenPosition + (Vector2.Subtract(offset, new Vector2(0.5f)) * Size).RotatedBy(Rotation), Colors[colorIndex++], texCoord);
            }

            AddVertex(new(0, 1));
            AddVertex(new(0, 0));
            AddVertex(new(1, 1));
            AddVertex(new(1, 0));
        }
    }
}