namespace SPladisonsYoyoMod.Common.Primitives.Trails
{
    public interface IPrimitiveTrailWidth
    {
        float GetWidth(float progress, float dissolveProgress);
    }

    public class DefaultTrailWidth : IPrimitiveTrailWidth
    {
        protected float width;
        protected bool disappearOverTime;

        public DefaultTrailWidth(float width, bool disappearOverTime = false)
        {
            this.width = width;
            this.disappearOverTime = disappearOverTime;
        }

        float IPrimitiveTrailWidth.GetWidth(float progress, float dissolveProgress) => (disappearOverTime ? (width * (1 - progress)) : width) * dissolveProgress;
    }

    public class CustomTrailWidth : IPrimitiveTrailWidth
    {
        protected WidthDelegate width;

        public CustomTrailWidth(WidthDelegate width)
        {
            this.width = width;
        }

        float IPrimitiveTrailWidth.GetWidth(float progress, float dissolveProgress) => (width?.Invoke(progress) ?? 0) * dissolveProgress;

        public delegate float WidthDelegate(float progress);
    }
}
