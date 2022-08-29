using System;

namespace SPladisonsYoyoMod.Common.Graphics
{
    public enum DrawLayers
    {
        Walls,
        Tiles,
        Dusts
    }

    [Flags]
    public enum DrawTypeFlags : byte
    {
        None = 0,

        Additive = 1 << 0,
        Pixelated = 1 << 1,

        All = Additive | Pixelated
    }
}