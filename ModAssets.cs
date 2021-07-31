using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public static class ModAssets
    {
        // Textures
        public static IReadOnlyList<Asset<Texture2D>> ExtraTextures => _extraTexturesList;

        // Effects
        public static Asset<Effect> BasicPrimitiveEffect { get; private set; }

        public static void Load(Mod mod)
        {
            int index = 0;
            while (mod.RequestAssetIfExists("Assets/Textures/Misc/Extra_" + index++, out Asset<Texture2D> texture)) _extraTexturesList.Add(texture);

            if (Main.dedServ) return;

            BasicPrimitiveEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/Primitive");
        }

        public static void Unload()
        {
            _extraTexturesList.Clear();

            BasicPrimitiveEffect = null;
        }

        private static readonly List<Asset<Texture2D>> _extraTexturesList = new List<Asset<Texture2D>>();
    }
}
