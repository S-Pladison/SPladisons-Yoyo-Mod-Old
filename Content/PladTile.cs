namespace SPladisonsYoyoMod.Content
{
    /*public abstract class PladTile : ModTile
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Tiles/" + this.Name;

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
    }*/
}
