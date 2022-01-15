using MonoMod.Cil;
using SPladisonsYoyoMod.Common.Globals;
using Terraria;
using Terraria.ID;
using static Mono.Cecil.Cil.OpCodes;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        // I'm not a professional and I'm not very good at this... If you want, you can rewrite this
        private static void IL_Terraria_Projectile_AI_099_2(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // float num2 = ProjectileID.Sets.YoyosLifeTimeMultiplier[this.type];

            // IL_00EE: ldsfld    float32[] Terraria.ID.ProjectileID/Sets::YoyosLifeTimeMultiplier
            // IL_00F3: ldarg.0
            // IL_00F4: ldfld int32 Terraria.Projectile::'type'
            // IL_00F9: ldelem.r4
            // IL_00FA: stloc.s num2

            int num2Index = -1;
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(ProjectileID.Sets).GetField("YoyosLifeTimeMultiplier")),
                i => i.MatchLdarg(0),
                i => i.MatchLdfld<Projectile>("type"),
                i => i.MatchLdelemR4(),
                i => i.MatchStloc(out num2Index))) return;

            c.Emit(Ldarg_0);
            c.Emit(Ldloca, num2Index);
            c.EmitDelegate<ModifyYoyoDelegate>(YoyoGlobalProjectile.ModifyYoyoLifeTime);

            // float num7 = ProjectileID.Sets.YoyosMaximumRange[this.type];

            // IL_0522: ldsfld float32[] Terraria.ID.ProjectileID / Sets::YoyosMaximumRange
            // IL_0527: ldarg.0
            // IL_0528: ldfld int32 Terraria.Projectile::'type'
            // IL_052D: ldelem.r4
            // IL_052E: stloc.s num10

            int num10Index = -1;
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(ProjectileID.Sets).GetField("YoyosMaximumRange")),
                i => i.MatchLdarg(0),
                i => i.MatchLdfld<Projectile>("type"),
                i => i.MatchLdelemR4(),
                i => i.MatchStloc(out num10Index))) return;

            c.Emit(Ldarg_0);
            c.Emit(Ldloca, num10Index);
            c.EmitDelegate<ModifyYoyoDelegate>(YoyoGlobalProjectile.ModifyYoyoMaximumRange);

            // if (yoyoString)
            // {
            //     num7 = num7 * 1.25f + 30f;
            // }

            // IL_063D: ldloc.s   num10
            // IL_063F: ldc.r4    1.25
            // IL_0644: mul
            // IL_0645: ldc.r4    30
            // IL_064A: add
            // IL_064B: stloc.s num10

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(num10Index),
                i => i.MatchLdcR4(1.25f),
                i => i.MatchMul(),
                i => i.MatchLdcR4(30f),
                i => i.MatchAdd(),
                i => i.MatchStloc(num10Index))) return;

            c.Emit(Ldarg_0);
            c.Emit(Ldloca, num10Index);
            c.EmitDelegate<ModifyYoyoDelegate>(RedoYoyoString);
        }

        private static void RedoYoyoString(Projectile proj, ref float length)
        {
            length = (length - 30) / 1.25f;
            length += 64;
        }

        private delegate void ModifyYoyoDelegate(Projectile proj, ref float value);
    }
}
