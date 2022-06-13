using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
{
    public class YoyoGlobalItem : GlobalItem
    {
        public float lifeTimeMult = 1;
        public float maxRangeMult = 1;
        public float topSpeedMult = 1;

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

        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            if (player.GetPladPlayer().flamingFlowerEquipped) crit += 12;
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Should fix ridiculous 1 tick player direction
            position += Utils.SafeNormalize(velocity, Vector2.Zero);
        }

        public override void UseStyle(Item item, Player player, Rectangle heldItemFrame)
        {
            if (!ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle) return;

            float rotation = player.itemRotation * player.gravDir - 1.57079637f * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, rotation);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            RemoveOneDropLogo(tooltips);

            int index = tooltips.FindLastIndex(i => i.Name.StartsWith("Tooltip")) + 1;
            if (index > 0)
            {
                if (lifeTimeMult != 1)
                {
                    string text = (lifeTimeMult < 1 ? "-" : "+") + ((Math.Abs(lifeTimeMult - 1)) * 100) + Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.PrefixYoyoLifeTime");
                    tooltips.Insert(index, new TooltipLine(Mod, "PrefixYoyoLifeTime", text)
                    {
                        IsModifier = true,
                        IsModifierBad = lifeTimeMult < 1
                    });
                }

                if (maxRangeMult != 1)
                {
                    string text = (maxRangeMult < 1 ? "-" : "+") + ((Math.Abs(maxRangeMult - 1)) * 100) + Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.PrefixYoyoMaxRange");
                    tooltips.Insert(index, new TooltipLine(Mod, "PrefixYoyoMaxRange", text)
                    {
                        IsModifier = true,
                        IsModifierBad = maxRangeMult < 1
                    });
                }

                if (topSpeedMult != 1)
                {

                }
            }

            InsertYoyoGloveTooltips(item, tooltips);
        }

        // ...

        private void RemoveOneDropLogo(List<TooltipLine> tooltips)
        {
            var oneDropLogoTooltip = tooltips.Find(i => i.Name == "OneDropLogo");
            if (oneDropLogoTooltip != null)
            {
                tooltips.Remove(oneDropLogoTooltip);
            }
        }

        private void InsertYoyoGloveTooltips(Item item, List<TooltipLine> tooltips)
        {
            var yoyoGloveIsEquipped = Main.LocalPlayer.yoyoGlove;
            var index = -1;
            var counter = 0;
            var yoyoGloveTooltips = tooltips.FindAll(i => i.Text.StartsWith("[YG]"));
            var rarityColor = ItemRarity.GetColor(item.rare);
            var hexRarityColor = Colors.AlphaDarken(rarityColor).Hex3();
            var infoText = Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.YoyoGloveInfo");

            if (yoyoGloveTooltips.Any())
            {
                index = tooltips.IndexOf(yoyoGloveTooltips.First());
                if (index == -1) return;

                tooltips.RemoveAll(i => yoyoGloveTooltips.Contains(i));
                if (!yoyoGloveIsEquipped) return;

                tooltips.Insert(index++, new TooltipLine(Mod, "YoyoGloveInfo", infoText) { OverrideColor = rarityColor });

                foreach (var line in yoyoGloveTooltips)
                {
                    var text = line.Text.Replace("[YG]", $"[c/{hexRarityColor}:‣ ]");

                    tooltips.Insert(index++, new TooltipLine(Mod, "YoyoGloveDescription" + counter++, text));
                }
            }
            else
            {
                if (!yoyoGloveIsEquipped || !ModContent.GetInstance<PladConfig>().ShowYoyoGloveStandardDescription) return;

                index = GetTooltipsLastIndex(tooltips) + 1;
                if (index == -1) return;

                var text = $"[c/{hexRarityColor}:‣] " + Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.YoyoGloveVanillaDescription");
                tooltips.Insert(index++, new TooltipLine(Mod, "YoyoGloveInfo", infoText) { OverrideColor = rarityColor });
                tooltips.Insert(index++, new TooltipLine(Mod, "YoyoGloveDescription0", text));
            }
        }

        // ...

        private static int GetTooltipsLastIndex(List<TooltipLine> tooltips)
            => tooltips.FindLastIndex(tt => tt.Name.Equals("Speed") || tt.Name.Equals("Knockback") || tt.Name.Equals("Material") || tt.Name.StartsWith("Tooltip"));
    }
}