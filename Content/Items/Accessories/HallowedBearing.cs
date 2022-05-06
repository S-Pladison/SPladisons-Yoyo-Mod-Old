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
    public class HallowedBearing : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + "HallowedBearing";

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 46;
            Item.height = 50;

            Item.rare = ItemRarityID.Pink;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.longInvince = true;
            player.pStone = true;
            player.GetPladPlayer().hallowedBearingEquipped = true;
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.CrossNecklace);
            recipe.AddIngredient(ItemID.PhilosophersStone);
            recipe.AddIngredient<Bearing>();
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddIngredient(ItemID.PixieDust, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var tooltip = tooltips.Find(i => i.Mod == "Terraria" && i.Name.StartsWith("Tooltip") && i.Text.Contains("{0}"));
            if (tooltip != null)
            {
                Color color = Terraria.ID.Colors.AlphaDarken(ItemRarity.GetColor(Item.rare));
                string value = $"[c/{color.Hex3()}:{HallowedBearing.GetBearingBonus()}%]";
                string text = Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.HallowedBearing", value);
                tooltip.Text = text.Split("\n").ToList().Find(i => i.Contains(value));
            }
        }

        public static int GetBearingBonus() => (int)(22 * (1 + (Main.dayTime ? (float)Math.Sin(Main.time / Main.dayLength * MathHelper.Pi) : -(float)Math.Sin(Main.time / Main.nightLength * MathHelper.Pi)) * 0.33f));
    }
}
