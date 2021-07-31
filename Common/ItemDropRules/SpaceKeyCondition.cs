using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace SPladisonsYoyoMod.Common.ItemDropRules
{
    public class SpaceKeyCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => info.npc.value > 0f && Main.hardMode && !info.IsInSimulation && info.player.ZoneSkyHeight;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => Language.GetTextValue("SPladisonsYoyoMod.Bestiary.ItemDropConditions.SpaceKeyCondition");
    }
}
