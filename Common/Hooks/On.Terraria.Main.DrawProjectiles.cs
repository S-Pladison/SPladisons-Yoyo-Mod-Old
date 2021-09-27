using SPladisonsYoyoMod.Common.Misc;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HooksLoader
    {
        private static void On_Terraria_Main_DrawProjectiles(On.Terraria.Main.orig_DrawProjectiles orig, Main main)
        {
            PrimitiveTrailSystem.DrawTrails(Main.spriteBatch);
            SoulFilledFlameEffect.Instance?.Draw(Main.spriteBatch);

            orig(main);
        }
    }
}
