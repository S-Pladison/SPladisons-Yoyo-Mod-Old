using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;

namespace SPladisonsYoyoMod.Common.Primitives.Trails
{
    public interface IPrimitiveTrailUpdate
    {
        int MaxPoints { get; }

        void UpdatePoints(PrimitiveTrail trail);
    }

    public class DefaultTrailUpdate : IPrimitiveTrailUpdate
    {
        protected int maxPoints;

        public DefaultTrailUpdate(int points)
        {
            this.maxPoints = points;
        }

        int IPrimitiveTrailUpdate.MaxPoints => maxPoints;
        void IPrimitiveTrailUpdate.UpdatePoints(PrimitiveTrail trail) => Update(trail);

        protected virtual void Update(PrimitiveTrail trail)
        {
            var positions = trail.Positions;
            var max = (int)(maxPoints * trail.DissolveProgress);

            if (trail.DissolveProgress == 1f)
            {
                positions.Insert(0, trail.Target.Center);
            }

            if (positions.Count > max)
            {
                var diff = positions.Count - max;
                positions.RemoveRange(max, diff);
            }
        }
    }

    public class BoundedTrailUpdate : DefaultTrailUpdate
    {
        protected float maxLength;
        protected PositionDelegate positionMethod;

        public BoundedTrailUpdate(int points, float length, PositionDelegate positionMethod = null) : base(points)
        {
            this.maxLength = length;
            this.positionMethod = positionMethod ?? new PositionDelegate((target) => target.Center);
        }

        protected override void Update(PrimitiveTrail trail)
        {
            var positions = trail.Positions;

            if (trail.DissolveProgress == 1f) positions.Insert(0, positionMethod.Invoke(trail.Target));
            if (positions.Count > maxPoints) positions.Remove(positions.Last());
            if (positions.Count <= 1) return;

            float length = 0;
            float max = maxLength * trail.DissolveProgress;
            int lastIndex = -1;

            for (int i = 1; i < positions.Count; i++)
            {
                float dist = Vector2.Distance(positions[i], positions[i - 1]);
                length += dist;

                if (length > max)
                {
                    lastIndex = i;
                    length -= dist;
                    break;
                }
            }

            if (lastIndex < 0) return;

            var vector = Vector2.Normalize(positions[lastIndex] - positions[lastIndex - 1]) * (max - length);
            positions.RemoveRange(lastIndex, positions.Count - lastIndex);
            positions.Add(positions[lastIndex - 1] + vector);
        }

        public delegate Vector2 PositionDelegate(Entity target);
    }

    public class CustomTrailUpdate : DefaultTrailUpdate
    {
        protected UpdateDelegate update;

        public CustomTrailUpdate(int points, UpdateDelegate update) : base(points)
        {
            this.update = update;
        }

        protected override void Update(PrimitiveTrail trail) => update?.Invoke(trail);

        public delegate float UpdateDelegate(PrimitiveTrail trail);
    }
}
