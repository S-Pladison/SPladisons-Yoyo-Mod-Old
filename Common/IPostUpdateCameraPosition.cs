using SPladisonsYoyoMod.Common.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common
{
    public interface IPostUpdateCameraPosition
    {
        /// <summary>
        /// Useful for things like texture rendering, etc.
        /// </summary>
        void PostUpdateCameraPosition();

        // ...

        private class HookLoader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
                On.Terraria.Main.DoDraw_UpdateCameraPosition += (orig) =>
                {
                    orig();

                    if (Main.gameMenu) return;

                    SPladisonsYoyoMod.Events.InvokeOnPostUpdateCameraPosition();
                };
            }

            void ILoadable.Unload() { }
        }
    }
}