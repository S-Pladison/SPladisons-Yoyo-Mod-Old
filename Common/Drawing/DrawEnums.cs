using System;

namespace SPladisonsYoyoMod.Common.Drawing
{
    public enum DrawLayers
    {
        OverWalls,
        OverTiles,
        OverDusts
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