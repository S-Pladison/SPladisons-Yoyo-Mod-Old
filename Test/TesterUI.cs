using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace SPladisonsYoyoMod.Test
{
    public class TesterUI : UIState
    {
        public bool Visible { get; set; } = true;

        public UIPanel panel;

        public override void OnInitialize()
        {
            panel = new UIPanel();
            this.UpdatePanel();

            this.Append(panel);
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdatePanel();
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.mapStyle != 0 || Main.playerInventory) return;

            string[] array = new string[]
            {
                "Players: " + Main.player?.Count(i => i.active),
                "Projectiles: " + Main.projectile?.Count(i => i.active),
                "NPCs: " + Main.npc?.Count(i => i.active),
                "Items: " + Main.item?.Count(i => i.active),
                "Dusts: " + Main.dust?.Count(i => i.active),
                "Gores: " + Main.gore?.Count(i => i.active),
                "Trails: " + SPladisonsYoyoMod.Primitives?.TrailCount
            };

            base.Draw(spriteBatch);

            for (int i = 0; i < array.Length; i++)
            {
                Utils.DrawBorderString(spriteBatch, array[i], new Vector2(panel.Left.Pixels + 10 + panel.Width.Pixels / 2 * (int)(i / 4), panel.Top.Pixels + 10 + 20 * (i % 4)), Color.White, 0.8f);
            }
        }

        public void UpdatePanel()
        {
            panel.Left.Set(Main.miniMapX, 0f);
            panel.Top.Set(Main.miniMapY, 0f);
            panel.Width.Set(Main.miniMapWidth, 0f);
            panel.Height.Set(20 + 20 * 4, 0f);
        }
    }
}
