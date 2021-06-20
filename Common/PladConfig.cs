using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace SPladisonsYoyoMod.Common
{
    public class PladConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [Label("$Mods.SPladisonsYoyoMod.Config.YoyoCustomUseStyle")]
        public bool YoyoCustomUseStyle { get; set; }
    }
}
