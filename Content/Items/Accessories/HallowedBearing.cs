using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class HallowedBearing : PladItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 32;
            Item.height = 34;

            Item.rare = ItemRarityID.Pink;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetPladPlayer().hallowedBearingEquipped = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<Bearing>().AddIngredient(ItemID.HallowedBar, 5).AddIngredient(ItemID.PixieDust, 10).AddTile(TileID.MythrilAnvil).Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var tooltip = tooltips.Find(i => i.mod == "Terraria" && i.Name.StartsWith("Tooltip") && i.text.Contains("{0}"));
            if (tooltip != null)
            {
                Color color = Terraria.ID.Colors.AlphaDarken(ItemRarity.GetColor(Item.rare));
                string value = $"[c/{color.Hex3()}:{HallowedBearing.GetBearingBonus()}%]";
                string text = Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.HallowedBearing", value);
                tooltip.text = text.Split("\n").ToList().Find(i => i.Contains(value));
            }
        }

        public static int GetBearingBonus() => (int)(22 * (1 + (Main.dayTime ? (float)Math.Sin(Main.time / Main.dayLength * MathHelper.Pi) : -(float)Math.Sin(Main.time / Main.nightLength * MathHelper.Pi)) * 0.33f));
    }
}
