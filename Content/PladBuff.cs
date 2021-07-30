using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladBuff : ModBuff
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Buffs/" + this.Name;
    }
}
