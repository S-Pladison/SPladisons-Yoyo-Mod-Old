using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
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
        public static Primitives Primitives { get; private set; }

        public static IReadOnlyList<int> GetYoyos => _yoyos;

        public SPladisonsYoyoMod() => Instance = this;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Primitives = new Primitives();
            }

            _yoyos = new List<int>();

            ModAssets.Load(this);
            Main.OnPreDraw += DrawTargets;
        }

        public override void Unload()
        {
            ModAssets.Unload();
            Main.OnPreDraw -= DrawTargets;

            _yoyos = null;

            Primitives = null;
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

        private static void DrawTargets(GameTime gameTime)
        {
            SoulFilledFlameEffect.Instance?.Render(Main.spriteBatch);
        }

        private static List<int> _yoyos;
    }
}