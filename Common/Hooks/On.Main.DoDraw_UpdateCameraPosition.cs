using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Main_DoDraw_UpdateCameraPosition(On.Terraria.Main.orig_DoDraw_UpdateCameraPosition orig)
        {
            orig();

            if (Main.gameMenu) return;

            foreach (var proj in Main.projectile)
            {
                if (proj.active && proj.ModProjectile is IPostUpdateCameraPosition modProj)
                {
                    modProj.PostUpdateCameraPosition();
                }
            }

            SPladisonsYoyoMod.PostUpdateCameraPosition();
        }
    }
}