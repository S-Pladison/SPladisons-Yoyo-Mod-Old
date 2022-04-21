using MonoMod.RuntimeDetour.HookGen;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader : ILoadable
    {
        public void Load(Mod mod)
        {
            On.Terraria.Main.CheckMonoliths += On_Main_CheckMonoliths;
            On.Terraria.Main.DoDraw_Tiles_Solid += On_Main_DoDraw_Tiles_Solid;
            On.Terraria.Main.DoDraw_WallsAndBlacks += On_Main_DoDraw_WallsAndBlacks;
            On.Terraria.Main.DrawDust += On_Main_DrawDust;
            On.Terraria.Main.DrawProj_DrawYoyoString += On_Main_DrawProj_DrawYoyoString;

            On.Terraria.Player.OnHit += On_Player_OnHit;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float += On_Projectile_NewProjectile;

            HookEndpointManager.Add<hook_SetChatButtons>(SetChatButtonsMethod, On_NPCLoader_SetChatButtons);

            IL.Terraria.Player.Counterweight += IL_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_1 += IL_Projectile_AI_099_1;
            IL.Terraria.Projectile.AI_099_2 += IL_Projectile_AI_099_2;
        }

        public void Unload()
        {
            On.Terraria.Main.CheckMonoliths -= On_Main_CheckMonoliths;
            On.Terraria.Main.DoDraw_Tiles_Solid -= On_Main_DoDraw_Tiles_Solid;
            On.Terraria.Main.DoDraw_WallsAndBlacks -= On_Main_DoDraw_WallsAndBlacks;
            On.Terraria.Main.DrawDust -= On_Main_DrawDust;
            On.Terraria.Main.DrawProj_DrawYoyoString -= On_Main_DrawProj_DrawYoyoString;

            On.Terraria.Player.OnHit -= On_Player_OnHit;
            On.Terraria.Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float -= On_Projectile_NewProjectile;

            HookEndpointManager.Remove<hook_SetChatButtons>(SetChatButtonsMethod, On_NPCLoader_SetChatButtons);

            IL.Terraria.Player.Counterweight -= IL_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_1 -= IL_Projectile_AI_099_1;
            IL.Terraria.Projectile.AI_099_2 -= IL_Projectile_AI_099_2;
        }
    }
}
