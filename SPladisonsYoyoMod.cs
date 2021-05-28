using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public class SPladisonsYoyoMod : Mod
    {
        public static Mod Instance { get; private set; }

        public static PrimitiveSystem Primitives { get; private set; }

        public SPladisonsYoyoMod()
        {
            Instance = this;
        }

        public override void Load()
        {
            Primitives = new PrimitiveSystem(Main.graphics.GraphicsDevice);

            Main.OnPreDraw += RenderSpecial;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= RenderSpecial;

            Primitives = null;
            Instance = null;
        }

        private void RenderSpecial(GameTime obj)
        {
            Primitives?.Render();
        }
    }
}