using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public class ModAssets
    {
        public const string Path = "SPladisonsYoyoMod/Assets/";

        public const string ItemsPath = "SPladisonsYoyoMod/Assets/Textures/Items/";
        public const string ProjectilesPath = "SPladisonsYoyoMod/Assets/Textures/Projectiles/";
        public const string DustsPath = "SPladisonsYoyoMod/Assets/Textures/Dusts/";
        public const string MiscPath = "SPladisonsYoyoMod/Assets/Textures/Misc/";
        public const string InvisiblePath = "SPladisonsYoyoMod/Assets/Textures/Misc/Extra_0";

        public const string EffectsPath = "SPladisonsYoyoMod/Assets/Effects/";

        // ...
        public static Asset<Texture2D> GetExtraTexture(int type, AssetRequestMode mode = AssetRequestMode.AsyncLoad) => ModContent.Request<Texture2D>(MiscPath + "Extra_" + type, mode);
        public static Asset<Effect> GetEffect(string name, AssetRequestMode mode = AssetRequestMode.AsyncLoad) => ModContent.Request<Effect>(EffectsPath + name, mode);
    }
}