using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Drawing;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DoDraw_WallsAndBlacks(On.Terraria.Main.orig_DoDraw_WallsAndBlacks orig, Main main)
        {
            orig(main);

            var spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            {
                DrawingManager.DrawLayer(DrawLayers.OverWalls);
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
}