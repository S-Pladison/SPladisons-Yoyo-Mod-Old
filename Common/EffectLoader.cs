using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SPladisonsYoyoMod.Common
{
    [Autoload(Side = ModSide.Client)]
    public class EffectLoader : ILoadable
    {
        private static readonly Dictionary<string, Asset<Effect>> effectsByName = new();

        // ...

        void ILoadable.Load(Mod mod)
        {
            var methodInfo = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            var modFile = (TmodFile)methodInfo.Invoke(mod, null);
            var entries = modFile.Where(i => i.Name.StartsWith("Assets/Effects/") && i.Name.EndsWith(".xnb"));

            foreach (var entry in entries)
            {
                var path = entry.Name.Replace(".xnb", "");
                var name = path.Replace("Assets/Effects/", "");

                if (effectsByName.ContainsKey(name)) continue;

                if (ModContent.RequestIfExists<Effect>($"{mod.Name}/{path}", out Asset<Effect> asset, AssetRequestMode.ImmediateLoad))
                {
                    effectsByName.Add(name, asset);
                    SetEffectInitParameters(name, asset.Value.Parameters);
                }
            }
        }

        void ILoadable.Unload()
        {
            effectsByName.Clear();
        }

        // ...

        public static Asset<Effect> GetEffect(string name)
            => effectsByName[name];

        public static void CreateSceneFilter(string effect, EffectPriority priority)
            => Filters.Scene[$"{SPladisonsYoyoMod.Instance.Name}:{effect}"] = new(new ScreenShaderData(new Ref<Effect>(GetEffect(effect).Value), effect), priority);

        private static void SetEffectInitParameters(string name, EffectParameterCollection parameters)
        {
            static Texture2D GetExtraTexture(int index)
                => ModAssets.GetExtraTexture(index, AssetRequestMode.ImmediateLoad).Value;

            switch (name)
            {
                case "AdamantiteYoyoTrail":
                    parameters["Texture0"].SetValue(GetExtraTexture(11));
                    break;
                case "BlackholeBackground":
                    parameters["Texture1"].SetValue(GetExtraTexture(28));
                    parameters["Texture2"].SetValue(GetExtraTexture(29));
                    parameters["Color0"].SetValue(new Color(8, 9, 15).ToVector4());
                    parameters["Color1"].SetValue(new Color(198, 50, 189).ToVector4());
                    parameters["Color2"].SetValue(new Color(25, 25, 76).ToVector4());
                    break;
                case "BlackholeTrail":
                    parameters["Texture0"].SetValue(GetExtraTexture(11));
                    break;
                case "IgnisTrail":
                    parameters["Texture0"].SetValue(GetExtraTexture(17));
                    break;
                case "MetaBlastFilter":
                    parameters["Contrast"].SetValue(1.15f);
                    break;
                case "MyocardialInfarctionStrip":
                    parameters["Texture0"].SetValue(GetExtraTexture(11));
                    break;
                case "ResidualLightTrail":
                    parameters["Texture0"].SetValue(GetExtraTexture(36));
                    parameters["Texture1"].SetValue(GetExtraTexture(27));
                    parameters["Texture2"].SetValue(GetExtraTexture(38));
                    parameters["Texture3"].SetValue(GetExtraTexture(37));
                    parameters["Texture4"].SetValue(GetExtraTexture(39));
                    break;
                case "TheStellarThrowParticleTrail":
                    parameters["Texture0"].SetValue(GetExtraTexture(35));
                    break;
                case "TheStellarThrowTrail":
                    parameters["Texture0"].SetValue(GetExtraTexture(7));
                    break;
            }
        }
    }
}