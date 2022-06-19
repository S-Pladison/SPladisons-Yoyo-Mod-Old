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
            var spriteBatchInfo = new SpriteBatchInfo(spriteBatch);

            spriteBatch.End();
            {
                DrawingManager.DrawLayer(DrawLayers.OverWalls);
            }
            spriteBatchInfo.Begin(spriteBatch);
        }
    }
}