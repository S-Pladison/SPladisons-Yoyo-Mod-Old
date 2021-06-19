using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Prefixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace SPladisonsYoyoMod.Common.Global
{
    public class YoyoGlobalItem : GlobalItem
    {
        public float lifeTimeMult = 1;
        public float maxRangeMult = 1;
        public float topSpeedMult = 1;

        public override void Load()
        {
            Mod.AddModTranslation(key: "Tooltip.PrefixYoyoLifeTime", eng: "% life time", rus: "% время полета");
            Mod.AddModTranslation(key: "Tooltip.PrefixYoyoMaxRange", eng: "% maximum range", rus: "% максимальная дальность");
        }

        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.IsYoyo();
        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            YoyoGlobalItem myClone = (YoyoGlobalItem)base.Clone(item, itemClone);
            myClone.lifeTimeMult = lifeTimeMult;
            myClone.maxRangeMult = maxRangeMult;
            myClone.topSpeedMult = topSpeedMult;
            return myClone;
        }

        public override void ModifyWeaponCrit(Item item, Player player, ref int crit)
        {
            if (player.GetPladPlayer().flamingFlowerEquipped) crit += 12;
        }

        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            if (ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle)
            {
                float rotation = player.itemRotation * player.gravDir - 1.57079637f * player.direction;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, rotation);
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int index = tooltips.FindLastIndex(i => i.Name == "OneDropLogo" || i.Name.StartsWith("Tooltip")) + 1;
            if (index > 0)
            {
                if (lifeTimeMult != 1)
                {
                    string text = (lifeTimeMult < 1 ? "-" : "+") + ((Math.Abs(lifeTimeMult - 1)) * 100) + Language.GetTextValue("Mods.SPladisonsYoyoMod.Tooltip.PrefixYoyoLifeTime");
                    tooltips.Insert(index, new TooltipLine(Mod, "PrefixYoyoLifeTime", text)
                    {
                        isModifier = true,
                        isModifierBad = lifeTimeMult < 1
                    });
                }

                if (maxRangeMult != 1)
                {
                    string text = (maxRangeMult < 1 ? "-" : "+") + ((Math.Abs(maxRangeMult - 1)) * 100) + Language.GetTextValue("Mods.SPladisonsYoyoMod.Tooltip.PrefixYoyoMaxRange");
                    tooltips.Insert(index, new TooltipLine(Mod, "PrefixYoyoMaxRange", text)
                    {
                        isModifier = true,
                        isModifierBad = maxRangeMult < 1
                    });
                }

                if (topSpeedMult != 1)
                {

                }
            }

            var tooltip = tooltips.Find(i => i.mod == "Terraria" && i.text.Contains("|?|"));
            if (tooltip != null)
            {
                tooltip.text = tooltip.text.Replace("|?| ", "");
                tooltip.overrideColor = ItemRarity.GetColor(item.rare);
            }
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand) => rand.Next(YoyoPrefixes.GetPrefixes().ToList());
    }
}
