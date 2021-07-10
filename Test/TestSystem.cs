using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SPladisonsYoyoMod.Test
{
    public class TestSystem : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            if (SPladisonsYoyoMod.Instance.testerUI.Visible)
            {
                SPladisonsYoyoMod.Instance.testerUserInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "SPladisonsYoyoMod: Tester UI",
                    delegate {
                        if (SPladisonsYoyoMod.Instance.testerUI.Visible) SPladisonsYoyoMod.Instance.testerUI.Draw(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
