using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader : ILoadable
    {
        public void Load(Mod mod)
        {
            On.Terraria.Main.DrawMiscMapIcons += On_Terraria_Main_DrawMiscMapIcons;
            OnDrawYoyoString += On_Terraria_Main_DrawProj_DrawYoyoString;
            On.Terraria.Main.DrawProjectiles += On_Terraria_Main_DrawProjectiles;
            On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float += On_Terraria_Projectile_NewProjectile;

            IL.Terraria.Player.Counterweight += IL_Terraria_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_2 += IL_Terraria_Projectile_AI_099_2;
        }

        public void Unload()
        {
            On.Terraria.Main.DrawMiscMapIcons -= On_Terraria_Main_DrawMiscMapIcons;
            OnDrawYoyoString -= On_Terraria_Main_DrawProj_DrawYoyoString;
            On.Terraria.Main.DrawProjectiles -= On_Terraria_Main_DrawProjectiles;
            On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float -= On_Terraria_Projectile_NewProjectile;

            IL.Terraria.Player.Counterweight -= IL_Terraria_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_2 -= IL_Terraria_Projectile_AI_099_2;
        }
    }
}
