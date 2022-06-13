using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace SPladisonsYoyoMod
{
    public partial class SPladisonsYoyoMod
    {
        public static class Events
        {
            public static void Unload()
            {
                // Mod

                OnPostUpdateCameraPosition = null;

                // Vanilla

                Main.OnPostDraw -= OnPostDraw_Unload;
                OnPostDraw_Unload = null;

                Main.OnResolutionChanged -= OnResolutionChanged_Unload;
                OnResolutionChanged_Unload = null;
            }

            // Mod

            public static event Action OnPostUpdateCameraPosition;
            public static void InvokeOnPostUpdateCameraPosition()
                => OnPostUpdateCameraPosition?.Invoke();

            // Vanilla

            private static event Action<GameTime> OnPostDraw_Unload;
            public static event Action<GameTime> OnPostDraw
            {
                add
                {
                    Main.OnPostDraw += value;
                    OnPostDraw_Unload += value;
                }
                remove
                {
                    Main.OnPostDraw -= value;
                    OnPostDraw_Unload -= value;
                }
            }

            private static event Action<Vector2> OnResolutionChanged_Unload;
            public static event Action<Vector2> OnResolutionChanged
            {
                add
                {
                    Main.OnResolutionChanged += value;
                    OnResolutionChanged_Unload += value;
                }
                remove
                {
                    Main.OnResolutionChanged -= value;
                    OnResolutionChanged_Unload -= value;
                }
            }
        }
    }
}