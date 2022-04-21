using SPladisonsYoyoMod.Common.AdditiveDrawing;
using SPladisonsYoyoMod.Common.Primitives;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DrawDust(On.Terraria.Main.orig_DrawDust orig, Main main)
        {
            orig(main);

            PrimitiveSystem.Instance.DrawPrimitives(PrimitiveDrawLayer.Dusts);
            AdditiveDrawSystem.DrawToScreen(Main.spriteBatch);
        }
    }
}