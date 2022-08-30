using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Global
{
    public class PladGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            switch (item.type)
            {
                case ItemID.YoYoGlove:
                    item.rare = ItemRarityID.Orange; // Light Red
                    item.value = Terraria.Item.buyPrice(gold: 35); // Gold: 50
                    break;
                case ItemID.YoyoBag:
                    item.rare = ItemRarityID.Orange; // Light Red
                    break;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            switch (item.type)
            {
                case ItemID.YoYoGlove:
                    var line = GetTooltipsLast(tooltips);
                    if (line != null)
                    {
                        line.Text = Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.YoyoGlove");
                    }
                    break;
                case int type when type >= ItemID.RedString && type <= ItemID.BlackString:
                    int index = GetTooltipsLastIndex(tooltips);
                    if (index >= 0)
                    {
                        tooltips.RemoveAt(index);
                        var text = Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.YoyoString").Split("\n");
                        var n = 0;

                        foreach (var elem in text)
                        {
                            tooltips.Insert(index++, new TooltipLine(Mod, "ModTooltip" + n++, elem));
                        }
                    }
                    break;
            }
        }

        // ...

        private static TooltipLine GetTooltipsLast(List<TooltipLine> tooltips)
            => tooltips.FindLast(i => i.Mod == "Terraria" && i.Name.StartsWith("Tooltip"));

        private static int GetTooltipsLastIndex(List<TooltipLine> tooltips)
            => tooltips.FindLastIndex(i => i.Mod == "Terraria" && i.Name.StartsWith("Tooltip"));
    }
}