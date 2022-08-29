using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common.Graphics;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public class ModAssets
    {
        public const string Path = "SPladisonsYoyoMod/Assets/";

        public const string EffectsPath = Path + "Effects/";
        public const string StructuresPath = Path + "Structures/";
        public const string TexturesPath = Path + "Textures/";
        public const string SoundsPath = Path + "Sounds/";

        public const string ItemsPath = TexturesPath + "Items/";
        public const string ProjectilesPath = TexturesPath + "Projectiles/";
        public const string DustsPath = TexturesPath + "Dusts/";
        public const string ParticlesPath = TexturesPath + "Particles/";
        public const string TilesPath = TexturesPath + "Tiles/";
        public const string MiscPath = TexturesPath + "Misc/";
        public const string InvisiblePath = TexturesPath + "Misc/Extra_0";

        // ...

        public static Asset<Texture2D> GetExtraTexture(int type, AssetRequestMode mode = AssetRequestMode.AsyncLoad) => ModContent.Request<Texture2D>(MiscPath + "Extra_" + type, mode);
        public static Asset<Effect> GetEffect(string name) => EffectLoader.GetEffect(name);
    }
}