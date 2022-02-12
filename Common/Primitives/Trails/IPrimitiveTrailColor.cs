using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPladisonsYoyoMod.Common.Primitives.Trails
{
    public interface IPrimitiveTrailColor
    {
        Color GetColor(float progress);
    }

    public class DefaultTrailColor : IPrimitiveTrailColor
    {
        protected Color color;
        protected bool disappearOverTime;

        public DefaultTrailColor(Color color, bool disappearOverTime = false)
        {
            this.color = color;
            this.disappearOverTime = disappearOverTime;
        }

        Color IPrimitiveTrailColor.GetColor(float progress) => disappearOverTime ? (color * (1 - progress)) : color;
    }

    public class GradientTrailColor : IPrimitiveTrailColor
    {
        protected List<Color> colors;
        protected bool disappearOverTime;

        public GradientTrailColor(IEnumerable<Color> colors, bool disappearOverTime = false)
        {
            this.colors = colors?.ToList() ?? new List<Color>();

            if (this.colors.Count == 0)
            {
                this.colors.Add(Color.White);
            }

            this.disappearOverTime = disappearOverTime;
        }

        Color IPrimitiveTrailColor.GetColor(float progress)
        {
            Color color = ModUtils.GradientValue(Color.Lerp, progress, colors.ToArray());
            return disappearOverTime ? (color * (1 - progress)) : color;
        }
    }

    public class CustomTrailColor : IPrimitiveTrailColor
    {
        protected ColorDelegate color;

        public CustomTrailColor(ColorDelegate color)
        {
            this.color = color;
        }

        Color IPrimitiveTrailColor.GetColor(float progress) => color?.Invoke(progress) ?? Color.White;

        public delegate Color ColorDelegate(float progress);
    }
}
