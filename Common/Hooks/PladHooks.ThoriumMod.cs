using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    // Why not use Vanilla AI...
    public partial class PladHooks : ILoadable
    {
        public static bool ModifiedThorium { get; private set; }

        public static MethodInfo ThoriumYoyoAIMethod { get; private set; }
        public static MethodInfo ThoriumDrawStringMethod { get; private set; }

        public static void LoadThorium(Mod thoriumMod)
        {
            if (thoriumMod == null) return;

            foreach (Type type in thoriumMod.GetType().Assembly.GetTypes())
            {
                if (type.Name == "ProjectileExtras")
                {
                    ThoriumYoyoAIMethod = type.GetMethod("YoyoAI", BindingFlags.Public | BindingFlags.Static);
                    ThoriumDrawStringMethod = type.GetMethod("DrawString", BindingFlags.Public | BindingFlags.Static);
                }
            }

            try
            {
                ModifiedThorium = false;

                ModifyThoriumYoyoAIMethod += (orig, projIndex, lifeTime, maxRange, topSpeed, rotateSpeed, _unknownAction, _unknownAction2) =>
                {
                    if (PladHooks.ModifiedThorium)
                    {
                        var proj = Main.projectile[projIndex];

                        float oldLifeTime = lifeTime;
                        float oldMaxRange = maxRange;
                        float oldTopSpeed = topSpeed;

                        var globalProjectile = proj.GetYoyoGlobalProjectile();
                        globalProjectile.ModifyYoyo(proj, ref lifeTime, ref maxRange, ref topSpeed);

                        orig(projIndex, lifeTime, maxRange, topSpeed, rotateSpeed, _unknownAction, _unknownAction2);

                        lifeTime = oldLifeTime;
                        maxRange = oldMaxRange;
                        topSpeed = oldTopSpeed;
                    }
                    else orig(projIndex, lifeTime, maxRange, topSpeed, rotateSpeed, _unknownAction, _unknownAction2);
                };

                ModifyThoriumDrawStringMethod += (orig, projIndex, start) =>
                {
                    if (PladHooks.ModifiedThorium && ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle && Main.projectile[projIndex].IsYoyo() && start == default(Vector2)) return;

                    orig(projIndex, start);
                };

                ModifiedThorium = true;
            }
            catch
            {
                SPladisonsYoyoMod.Instance.Logger.Error("Failed to modify Thorium Mod");
            }
        }

        public static void UnloadThorium()
        {
            ThoriumYoyoAIMethod = null;
            ThoriumDrawStringMethod = null;

            ModifiedThorium = false;
        }

        // YoyoAI(int, float, float, float, float, ThoriumMod.Projectiles.ExtraAction, ThoriumMod.Projectiles.ExtraAction)
        private delegate void OrigThoriumYoyoAIMethod(int projIndex, float lifeTime, float maxRange, float topSpeed, float rotateSpeed, Delegate _unknownAction, Delegate _unknownAction2);
        private delegate void HookThoriumYoyoAIMethod(OrigThoriumYoyoAIMethod orig, int projIndex, float lifeTime, float maxRange, float topSpeed, float rotateSpeed, Delegate _unknownAction, Delegate _unknownAction2);
        private static event HookThoriumYoyoAIMethod ModifyThoriumYoyoAIMethod
        {
            add { HookEndpointManager.Add(ThoriumYoyoAIMethod, value); }
            remove { HookEndpointManager.Remove(ThoriumYoyoAIMethod, value); }
        }

        // DrawString(int, Vector2)
        private delegate void OrigThoriumDrawStringMethod(int projIndex, Vector2 start);
        private delegate void HookThoriumDrawStringMethod(OrigThoriumDrawStringMethod orig, int projIndex, Vector2 start);
        private static event HookThoriumDrawStringMethod ModifyThoriumDrawStringMethod
        {
            add { HookEndpointManager.Add(ThoriumDrawStringMethod, value); }
            remove { HookEndpointManager.Remove(ThoriumDrawStringMethod, value); }
        }
    }
}
