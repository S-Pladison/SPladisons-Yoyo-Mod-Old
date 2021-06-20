using SPladisonsYoyoMod.Common.ItemDropRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Global
{
    public class PladGlobalNPC : GlobalNPC
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Content.Items.Placeables.SpaceKey>(), 2500, 1, 1, new SpaceKeyCondition(), 1));
        }
    }
}
