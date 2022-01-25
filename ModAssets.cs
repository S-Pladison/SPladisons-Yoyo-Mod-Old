using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public class ModAssets
    {
        public const string Path = "SPladisonsYoyoMod/Assets/";

        public const string ItemsPath = "SPladisonsYoyoMod/Assets/Textures/Items/";
        public const string ProjectilesPath = "SPladisonsYoyoMod/Assets/Textures/Projectiles/";

        // ...

        public static Dictionary<string, Asset<Effect>> Effects = new();

        public static void Load(Mod mod)
        {
            if (Main.dedServ) return;

            // ...
        }

        public static void Unload()
        {
            Effects.Clear();
            Effects = null;
        }
    }
}
