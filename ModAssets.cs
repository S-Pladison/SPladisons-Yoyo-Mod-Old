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
        public static Asset<Texture2D> PerlinNoiseTexture { get; private set; }
        public static IReadOnlyList<Asset<Texture2D>> ExtraTextures => _extraTexturesList;

        // Effects
        public static Asset<Effect> BasicPrimitiveEffect { get; private set; }
        public static Asset<Effect> BellowingThunderEffect { get; private set; }
        public static Asset<Effect> BlackholeEffect { get; private set; }
        public static Asset<Effect> ResidualLightEffect { get; private set; }

        public static void Load(Mod mod)
        {
            PerlinNoiseTexture = ModContent.Request<Texture2D>("Terraria/Images/Misc/Perlin");

            int index = 0;
            while (mod.RequestAssetIfExists("Assets/Textures/Misc/Extra_" + index++, out Asset<Texture2D> texture)) _extraTexturesList.Add(texture);

            if (Main.dedServ) return;

            BasicPrimitiveEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/Primitive");
            BellowingThunderEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/BellowingThunder");
            BlackholeEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/Blackhole");
            ResidualLightEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/ResidualLight");
        }

        public static void Unload()
        {
            PerlinNoiseTexture = null;

            _extraTexturesList.Clear();

            BasicPrimitiveEffect = null;
            BellowingThunderEffect = null;
            BlackholeEffect = null;
            ResidualLightEffect = null;
        }

        public static void SetEffectsParameters()
        {
            if (Main.dedServ) return;

            BellowingThunderEffect.Value.Parameters["texture1"].SetValue(ModAssets.ExtraTextures[24].Value);

            BlackholeEffect.Value.Parameters["texture1"].SetValue(ModAssets.PerlinNoiseTexture.Value);
            BlackholeEffect.Value.Parameters["width"].SetValue(ModAssets.ExtraTextures[2].Width() / 4);

            ResidualLightEffect.Value.Parameters["texture1"].SetValue(ModAssets.ExtraTextures[20].Value);
        }

        private static readonly List<Asset<Texture2D>> _extraTexturesList = new List<Asset<Texture2D>>();
    }
}
