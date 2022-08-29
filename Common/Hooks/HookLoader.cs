using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader : ILoadable
    {
        public void Load(Mod mod)
        {
            IL.Terraria.Player.Counterweight += IL_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_1 += IL_Projectile_AI_099_1;
            IL.Terraria.Projectile.AI_099_2 += IL_Projectile_AI_099_2;

            if (Main.dedServ) return;

            On.Terraria.Main.DrawProj_DrawYoyoString += On_Main_DrawProj_DrawYoyoString;
            On.Terraria.Main.DoDraw_UpdateCameraPosition += On_Main_DoDraw_UpdateCameraPosition;

            HookEndpointManager.Add<hook_SetChatButtons>(SetChatButtonsMethod, On_NPCLoader_SetChatButtons);

            IL.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += IL_ItemSlot_Draw;
        }

        public void Unload()
        {
            IL.Terraria.Player.Counterweight -= IL_Player_Counterweight;
            IL.Terraria.Projectile.AI_099_1 -= IL_Projectile_AI_099_1;
            IL.Terraria.Projectile.AI_099_2 -= IL_Projectile_AI_099_2;

            if (Main.dedServ) return;

            On.Terraria.Main.DrawProj_DrawYoyoString -= On_Main_DrawProj_DrawYoyoString;
            On.Terraria.Main.DoDraw_UpdateCameraPosition -= On_Main_DoDraw_UpdateCameraPosition;

            HookEndpointManager.Remove<hook_SetChatButtons>(SetChatButtonsMethod, On_NPCLoader_SetChatButtons);

            IL.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color -= IL_ItemSlot_Draw;
        }
    }
}
