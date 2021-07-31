using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
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
                    int index = GetTooltipsLastIndex(tooltips);
                    if (index >= 0)
                    {
                        tooltips.Insert(index + 1, new TooltipLine(Mod, "ModTooltip", Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.YoYoGlove")));
                    }
                    break;
            }
        }

        private static int GetTooltipsLastIndex(List<TooltipLine> tooltips) => tooltips.FindLastIndex(i => i.mod == "Terraria" && i.Name.StartsWith("Tooltip"));
    }
}
