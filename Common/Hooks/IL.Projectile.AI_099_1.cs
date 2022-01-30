using MonoMod.Cil;
using SPladisonsYoyoMod.Content.Items;
using System;
using Terraria;
using static Mono.Cecil.Cil.OpCodes;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        // I'm not a professional and I'm not very good at this... If you want, you can rewrite this
        private static void IL_Projectile_AI_099_1(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // if (Main.projectile[i].active && Main.projectile[i].owner == this.owner && Main.projectile[i].aiStyle == 99 && (Main.projectile[i].type < 556 || Main.projectile[i].type > 561))
            // {
            //     flag2 = true;
            // }

            // IL_0078: ldsfld    class Terraria.Projectile[] Terraria.Main::projectile
            // IL_007D: ldloc.s i
            // IL_007F: ldelem.ref
            // IL_0080: ldfld int32 Terraria.Projectile::aiStyle
            // IL_0085: ldc.i4.s  99
            // IL_0087: bne.un.s IL_00B4

            int iIndex = -1;
            ILLabel label = null;

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld<Main>("projectile"),
                i => i.MatchLdloc(out iIndex),
                i => i.MatchLdelemRef(),
                i => i.MatchLdfld<Projectile>("aiStyle"),
                i => i.MatchLdcI4((sbyte)99),
                i => i.MatchBneUn(out label))) return;

            c.Emit(Ldloc, iIndex);
            c.EmitDelegate<Func<int, bool>>(i => Main.projectile[i].ModProjectile is not CounterweightProjectile);
            c.Emit(Brfalse_S, label);
        }
    }
}
