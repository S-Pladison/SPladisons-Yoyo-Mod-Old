using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class Hooks : ILoadable
    {
        public void Load(Mod mod)
        {
            LoadOn();
            LoadIL();
        }

        public void Unload()
        {
            UnloadIL();
        }
    }
}
