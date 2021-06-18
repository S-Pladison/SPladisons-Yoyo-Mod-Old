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

        public sealed override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Color color = new Color(r, g, b);

            this.PladModifyLight(i, j, ref color);

            r = color.R / 255f; g = color.G / 255f; b = color.B / 255f;
        }

        public void CreateMapEntry(Color color, string eng, string rus = "", Func<string, int, int, string> nameFunc = null)
        {
            this.CreateMapEntry(color, null, eng, rus, nameFunc);
        }

        public void CreateMapEntry(Color color, string key, string eng, string rus = "", Func<string, int, int, string> nameFunc = null)
        {
            ModTranslation name = CreateMapEntryName(key);
            name.SetDefault(eng);
            if (rus != "") name.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);

            if (nameFunc == null) AddMapEntry(color, name);
            else AddMapEntry(color, name, nameFunc);
        }

        public virtual void PladModifyLight(int i, int j, ref Color color) { }
    }
}
