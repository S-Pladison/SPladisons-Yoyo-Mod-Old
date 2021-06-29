using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace SPladisonsYoyoMod
{
    public class SPladisonsYoyoMod : Mod
    {
        public static Mod Instance { get; private set; }
        public static Primitives Primitives { get; private set; }
        public static IReadOnlyList<int> GetYoyos { get { return _yoyos; } }

        public SPladisonsYoyoMod()
        {
            Instance = this;
        }

        public override void Load()
        {
            Primitives = new Primitives(Main.graphics.GraphicsDevice);

            _yoyos = new List<int>();
        }

        public override void PostSetupContent()
        {
            LoadYoyos();
        }

        public override void Unload()
        {
            _yoyos = null;

            Primitives = null;
            Instance = null;
        }

        private static void LoadYoyos()
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

        private static List<int> _yoyos;
    }
}