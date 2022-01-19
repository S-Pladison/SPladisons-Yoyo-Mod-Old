using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader : ILoadable
    {
        public void Load(Mod mod)
        {
            On.Terraria.Main.CheckMonoliths += On_Terraria_Main_CheckMonoliths;
            On.Terraria.Main.DoDraw_WallsAndBlacks += On_Terraria_Main_DoDraw_WallsAndBlacks;
            On.Terraria.Main.DrawDust += On_Terraria_Main_DrawDust;
            On.Terraria.Main.DrawMiscMapIcons += On_Terraria_Main_DrawMiscMapIcons;
            On.Terraria.Main.DrawPlayers_BehindNPCs += On_Terraria_Main_DrawPlayers_BehindNPCs;
            On.Terraria.Main.DrawProj_DrawYoyoString += On_Terraria_Main_DrawProj_DrawYoyoString;
            On.Terraria.Main.SetDisplayMode += On_Terraria_Main_SetDisplayMode;
            On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float += On_Terraria_Projectile_NewProjectile;

            IL.Terraria.Player.Counterweight += IL_Terraria_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_2 += IL_Terraria_Projectile_AI_099_2;
        }

        public void Unload()
        {
            On.Terraria.Main.CheckMonoliths -= On_Terraria_Main_CheckMonoliths;
            On.Terraria.Main.DoDraw_WallsAndBlacks -= On_Terraria_Main_DoDraw_WallsAndBlacks;
            On.Terraria.Main.DrawDust -= On_Terraria_Main_DrawDust;
            On.Terraria.Main.DrawMiscMapIcons -= On_Terraria_Main_DrawMiscMapIcons;
            On.Terraria.Main.DrawPlayers_BehindNPCs -= On_Terraria_Main_DrawPlayers_BehindNPCs;
            On.Terraria.Main.DrawProj_DrawYoyoString -= On_Terraria_Main_DrawProj_DrawYoyoString;
            On.Terraria.Main.SetDisplayMode -= On_Terraria_Main_SetDisplayMode;
            On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float -= On_Terraria_Projectile_NewProjectile;

            IL.Terraria.Player.Counterweight -= IL_Terraria_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_2 -= IL_Terraria_Projectile_AI_099_2;
        }
    }
}
