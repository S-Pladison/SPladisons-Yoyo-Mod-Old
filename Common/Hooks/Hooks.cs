using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    // TODO: Unload this shit...
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
