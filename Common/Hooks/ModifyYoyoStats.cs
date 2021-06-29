using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public class ModifyYoyoStats : ModHook
    {
        public override void OnLoad() => IL.Terraria.Projectile.AI_099_2 += ModifyYoyoStatsMethod;
        public override void OnUnload() => IL.Terraria.Projectile.AI_099_2 -= ModifyYoyoStatsMethod;

        public static void ModifyYoyoStatsMethod(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // float num2 = ProjectileID.Sets.YoyosLifeTimeMultiplier[this.type];
            // bool flag8 = num2 != -1f && num > num2;

            int num2Index = -1; // lifeTime
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld(typeof(ProjectileID.Sets), "YoyosLifeTimeMultiplier"),
                i => i.MatchLdarg(0),
                i => i.MatchLdfld<Projectile>("type"),
                i => i.MatchLdelemR4(),
                i => i.MatchStloc(out num2Index))) return;

            // ...
        }

        private delegate void ModifyYoyoLifeTimeDelegate();
    }
}
