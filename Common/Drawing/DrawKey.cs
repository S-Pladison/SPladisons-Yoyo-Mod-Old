namespace SPladisonsYoyoMod.Common.Drawing
{
    public struct DrawKey
    {
        public DrawLayers Layer;
        public DrawTypeFlags Flags;

        public bool Additive => Flags.HasFlag(DrawTypeFlags.Additive);
        public bool Pixelated => Flags.HasFlag(DrawTypeFlags.Pixelated);

        public DrawKey(DrawLayers layer, DrawTypeFlags flags)
        {
            Layer = layer;
            Flags = flags;
        }

        public override string ToString() => "Layer{" + Layer + "} Flags{" + Flags + "}";
    }
}