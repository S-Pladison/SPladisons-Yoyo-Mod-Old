using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Terraria_Main_SetDisplayMode(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            if (width != Main.screenWidth || height != Main.screenHeight)
            {
                var metaball = BlackholeSpaceSystem.Instance;
                metaball?.RecreateRenderTarget(width, height);
            }

            orig(width, height, fullscreen);
        }
    }
}