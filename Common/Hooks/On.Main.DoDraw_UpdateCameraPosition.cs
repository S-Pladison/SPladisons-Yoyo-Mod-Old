using SPladisonsYoyoMod.Common.Graphics;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DoDraw_UpdateCameraPosition(On.Terraria.Main.orig_DoDraw_UpdateCameraPosition orig)
        {
            orig();

            if (Main.gameMenu) return;

            DrawSystem.GetDrawData();
            SPladisonsYoyoMod.Events.InvokeOnPostUpdateCameraPosition();
        }
    }
}