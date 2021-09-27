using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Content.Items.Accessories;
using Terraria;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HooksLoader
    {
        private static void On_Terraria_Main_DrawMiscMapIcons(On.Terraria.Main.orig_DrawMiscMapIcons orig, Main main, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
        {
            FlamingFlowerTile.DrawMapIcon(spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);

            orig(main, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
        }
    }
}
