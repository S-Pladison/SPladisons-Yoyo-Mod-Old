using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_SetDisplayMode(On.Terraria.Main.orig_SetDisplayMode orig, int width, int height, bool fullscreen)
        {
            var screen = new Point(Main.screenWidth, Main.screenHeight);

            orig(width, height, fullscreen);

            if (Main.screenWidth == screen.X && Main.screenHeight == screen.Y)
            {
                return;
            }

            BlackholeSpaceSystem.Instance?.RecreateRenderTarget(Main.screenWidth, Main.screenHeight);
            PaperEffectSystem.Instance?.RecreateRenderTarget(Main.screenWidth, Main.screenHeight);
        }
    }
}