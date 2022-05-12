using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public partial class SPladisonsYoyoMod : Mod
    {
        public static SPladisonsYoyoMod Instance { get; private set; }

        public static IReadOnlyList<int> GetYoyos => _yoyos;

        // ...

        public SPladisonsYoyoMod()
        {
            Instance = this;
        }

        public override void Unload()
        {
            Sets.Unload();
            UnloadEvents();
            Instance = null;
        }

        public override void PostSetupContent()
        {
            static void TryAddYoyo(int type)
            {
                Item item = new();
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

        private static readonly List<int> _yoyos = new();
    }
}