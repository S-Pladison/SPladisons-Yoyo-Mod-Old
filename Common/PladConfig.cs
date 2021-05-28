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
        public override void OnLoaded()
        {
            Mod.AddModTranslation(key: "Config.YoyoCustomUseStyle",
                                  eng: "Custom style of using yoyo",
                                  rus: "Пользовательский стиль использования йо-йо");
        }

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [Label("$Mods.SPladisonsYoyoMod.Config.YoyoCustomUseStyle")]
        public bool YoyoCustomUseStyle { get; set; }
    }
}
