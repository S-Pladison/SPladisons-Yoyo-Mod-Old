using SPladisonsYoyoMod.Common.Drawing;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DrawDust(On.Terraria.Main.orig_DrawDust orig, Main main)
        {
            orig(main);

            DrawingManager.DrawLayer(DrawLayers.OverDusts);
        }
    }
}