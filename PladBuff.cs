using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public abstract class PladBuff : ModBuff
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Buffs/" + this.Name;

        public void SetDisplayName(string eng, string rus = "")
        {
            DisplayName.SetDefault(eng);
            if (rus != "")
            {
                DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            }
        }

        public void SetDescription(string eng, string rus = "")
        {
            Description.SetDefault(eng);
            if (rus != "")
            {
                Description.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            }
        }
    }
}
