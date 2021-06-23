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
        public static PrimitiveSystem Primitives { get; private set; }
        public static IReadOnlyList<int> GetYoyos => _yoyos;

        public SPladisonsYoyoMod()
        {
            Instance = this;
        }

        public override void Load()
        {
            Primitives = new PrimitiveSystem(Main.graphics.GraphicsDevice);

            _yoyos = new List<int>();
        }

        public override void PostSetupContent()
        {
            this.LoadYoyos();
        }

        public override void Unload()
        {
            Primitives = null;
            Instance = null;

            _yoyos = null;
        }

        private static List<int> _yoyos;

        private void LoadYoyos()
        {
            void TryAddYoyo(int type)
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
    }
}