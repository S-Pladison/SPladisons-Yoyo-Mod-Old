using SPladisonsYoyoMod.Common.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static int On_Projectile_NewProjectile(On.Terraria.Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float orig,
        IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1)
        {
            var index = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
            var proj = Main.projectile[index];

            if (proj != null)
            {
                if (proj.ModProjectile is Content.PladProjectile pladProj) pladProj.OnSpawn();
                ModContent.GetInstance<PladGlobalProjectile>().OnSpawn(proj);
            }

            return index;
        }
    }
}
