using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public partial class SPladisonsYoyoMod : Mod
    {
        public static SPladisonsYoyoMod Instance { get; private set; }

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
    }
}