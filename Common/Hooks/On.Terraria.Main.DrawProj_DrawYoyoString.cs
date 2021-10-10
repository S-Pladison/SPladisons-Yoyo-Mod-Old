using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour.HookGen;
using SPladisonsYoyoMod.Common.Interfaces;
using System.Reflection;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Terraria_Main_DrawProj_DrawYoyoString(OrigDrawYoyoString orig, Main main, Projectile projectile, Vector2 mountedCenter)
        {
            /*if (!projectile.counterweight)
            {
                mountedCenter += new Vector2(0, -100);
            }*/

            if (projectile.ModProjectile is IDrawCustomString yoyo)
            {
                yoyo.DrawCustomString(mountedCenter);
                return;
            }

            orig(main, projectile, mountedCenter);
        }

        // ...

        private static readonly MethodInfo DrawYoyoStringInfo = typeof(Main).GetMethod("DrawProj_DrawYoyoString", BindingFlags.Instance | BindingFlags.NonPublic);

        private delegate void OrigDrawYoyoString(Main main, Projectile projectile, Vector2 mountedCenter);
        private delegate void HookDrawYoyoString(OrigDrawYoyoString orig, Main main, Projectile projectile, Vector2 mountedCenter);

        private static event HookDrawYoyoString OnDrawYoyoString
        {
            add => HookEndpointManager.Add(DrawYoyoStringInfo, value);
            remove => HookEndpointManager.Remove(DrawYoyoStringInfo, value);
        }
    }
}
