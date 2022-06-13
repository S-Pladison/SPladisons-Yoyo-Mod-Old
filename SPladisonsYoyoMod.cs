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
            Events.Unload();
            Sets.Unload();

            Instance = null;
        }
    }
}