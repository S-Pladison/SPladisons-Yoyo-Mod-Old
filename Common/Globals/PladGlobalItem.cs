using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
{
    public class PladGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            switch (item.type)
            {
                case ItemID.YoYoGlove:
                    int index = GetTooltipsLastIndex(tooltips);
                    if (index >= 0)
                    {
                        string text = "Some yoyos have different effect";
                        tooltips.Insert(index + 1, new TooltipLine(Mod, "ModTooltip", text));
                    }
                    break;
                case ItemID.YoyoBag:
                    index = GetTooltipsLastIndex(tooltips);
                    if (index >= 0)
                    {
                        string text = "Text";
                        tooltips.Insert(index + 1, new TooltipLine(Mod, "ModTooltip", text));
                    }
                    break;
            }
        }

        private static int GetTooltipsLastIndex(List<TooltipLine> tooltips) => tooltips.FindLastIndex(i => i.mod == "Terraria" && i.Name.StartsWith("Tooltip"));
    }
}
