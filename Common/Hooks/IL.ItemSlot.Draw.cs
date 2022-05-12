using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using static Mono.Cecil.Cil.OpCodes;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        // I'm not a professional and I'm not very good at this... If you want, you can rewrite this
        // And don't say it's pointless editing
        private static void IL_ItemSlot_Draw(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // float num8 = 1f;
            // if (rectangle2.Width > 32 || rectangle2.Height > 32)
            // {
            //     num8 = ((rectangle2.Width <= rectangle2.Height) ? (32f / (float)rectangle2.Height) : (32f / (float)rectangle2.Width));
            // }

            int num8Index = -1;
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdcR4(32f),
                i => i.MatchLdloc(out _),
                i => i.MatchLdfld<Rectangle>("Height"),
                i => i.MatchConvR4(),
                i => i.MatchDiv(),
                i => i.MatchStloc(out num8Index))) return;

            c.Emit(Ldarg, 1);
            c.Emit(Ldarg, 2);
            c.Emit(Ldarg, 3);
            c.Emit(Ldloca, num8Index);
            c.EmitDelegate<ModifyNum8>(ResetScale);
        }

        private static void ResetScale(Item[] inv, int context, int slot, ref float num8)
        {
            var type = inv[slot].type;
            var scale = SPladisonsYoyoMod.Sets.ItemCustomInventoryScale[type];

            if (scale.HasValue)
            {
                num8 = scale.Value;
            }
        }

        private delegate void ModifyNum8(Item[] inv, int context, int slot, ref float num8);
    }
}
