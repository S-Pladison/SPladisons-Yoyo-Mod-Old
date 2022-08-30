namespace SPladisonsYoyoMod.Common.Graphics
{
    public partial class DrawSystem
    {
        public struct DrawKey
        {
            public DrawLayers Layer;
            public DrawTypeFlags DrawType;

            public DrawKey(DrawLayers layer, DrawTypeFlags drawType)
            {
                Layer = layer;
                DrawType = drawType;
            }
        }
    }
}