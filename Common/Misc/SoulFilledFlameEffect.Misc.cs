using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SPladisonsYoyoMod.Common.Misc
{
    public interface ISoulFilledFlame
    {
        public void DrawSoulFilledFlame(SpriteBatch spriteBatch);
    }

    public class Particle
    {
        public bool active = true;

        public Vector2 position;
        public Vector2 velocity;

        public float rotation;
        public float scale;
        public float timeLeft;

        public Color color;
        public float layerDepth;

        public Action<Particle> updateAction;

        public Particle(Vector2 pos, float scale, float timeLeft, Color color, Action<Particle> updateAction = null, float layerDepth = 0)
        {
            this.position = pos;
            this.scale = scale;
            this.timeLeft = timeLeft;
            this.color = color;
            this.updateAction = updateAction;
            this.layerDepth = layerDepth;

            rotation = 0f;
            velocity = Vector2.Zero;
        }

        public void Update()
        {
            if (!active)
            {
                this.Kill();
                return;
            };

            if (updateAction != null)
            {
                updateAction.Invoke(this);
            }
            else
            {
                timeLeft--;
                velocity.X *= 0.9f;
                position += velocity;
                scale *= 0.96f;
            }

            if (timeLeft <= 0 || scale <= 0f) active = false;
        }

        public void Kill()
        {
            active = false;
            SoulFilledFlameEffect.Instance?.RemoveParticle(this);
        }
    }
}
