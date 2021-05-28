using Microsoft.Xna.Framework;
using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladTile : ModTile
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Tiles/" + this.Name;

        public void CreateMapEntry(Color color, string eng, string rus = "")
        {
            ModTranslation name = CreateMapEntryName();
            name.SetDefault(eng);
            if (rus != "") name.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            AddMapEntry(color, name);
        }
    }
}
