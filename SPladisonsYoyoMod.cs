using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public class SPladisonsYoyoMod : Mod
    {
        public static SPladisonsYoyoMod Instance { get; private set; }
        public static IReadOnlyList<int> GetYoyos => _yoyos;
        public static IReadOnlyList<Asset<Texture2D>> GetExtraTextures => _extraTextures;

        public SPladisonsYoyoMod() => Instance = this;

        public override void Load()
        {
            SPladisonsYoyoMod.LoadExtraTextures(this);

            Main.OnPreDraw += DrawTargets;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= DrawTargets;

            Instance = null;
        }

        public override void PostSetupContent()
        {
            static void TryAddYoyo(int type)
            {
                Item item = new Item();
                item.SetDefaults(type, true);

                if (ItemID.Sets.Yoyo[item.type])
                {
                    _yoyos.Add(type);
                    return;
                }

                if (item.shoot <= ProjectileID.None) return;

                var proj = new Projectile();
                proj.SetDefaults(item.shoot);

                if (proj.IsYoyo()) _yoyos.Add(type);
            }

            for (int i = 0; i < ItemLoader.ItemCount; i++) TryAddYoyo(i);
        }

        // ...

        private static void DrawTargets(GameTime gameTime)
        {
            SoulFilledFlameEffect.Instance?.Render(Main.spriteBatch);
        }

        private static void LoadExtraTextures(Mod mod)
        {
            int index = 0;
            while (mod.RequestAssetIfExists("Assets/Textures/Misc/Extra_" + index++, out Asset<Texture2D> texture)) _extraTextures.Add(texture);
        }

        private static readonly List<int> _yoyos = new List<int>();
        private static readonly List<Asset<Texture2D>> _extraTextures = new List<Asset<Texture2D>>();
    }
}