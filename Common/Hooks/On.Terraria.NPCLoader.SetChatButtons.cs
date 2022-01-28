using SPladisonsYoyoMod.Content.Items.Misc;
using System.Reflection;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Terraria_NPCLoader_SetChatButtons(orig_SetChatButtons orig, ref string button, ref string button2)
        {
            orig(ref button, ref button2);
            NurseGiftSystem.Instance.SetChatButtons(ref button2);
        }

        private delegate void orig_SetChatButtons(ref string button, ref string button2);
        private delegate void hook_SetChatButtons(orig_SetChatButtons orig, ref string button, ref string button2);

        private static readonly MethodInfo SetChatButtonsMethod = typeof(NPCLoader).GetMethod(nameof(NPCLoader.SetChatButtons));
    }
}