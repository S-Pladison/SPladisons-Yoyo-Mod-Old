using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common
{
    public class PladPlayer : ModPlayer
    {
        public bool ZoneFlamingFlower { get; set; }

        public bool bearingEquipped;
        public bool candyCaneEquipped;
        public bool desecratedDiceEquipped;
        public bool flamingFlowerEquipped;

        public int starsOverHeadDye;
        public bool starsOverHeadVisible;

        public override void ResetEffects()
        {
            bearingEquipped = false;
            candyCaneEquipped = false;
            desecratedDiceEquipped = false;
            flamingFlowerEquipped = false;

            starsOverHeadVisible = false;

            this.UpdateDyes();
        }

        public override void PostUpdate()
        {
            this.UpdateFlamingFlowerZoneEffect();
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            /*if (Player.direction == 1 && Main.MouseWorld.X > Player.position.X)
            {
                Player.headRotation = Utils.Clamp((Main.MouseWorld - Player.Center).ToRotation(), -0.5f, 0.5f);
            }
            if (Player.direction == -1 && Main.MouseWorld.X < Player.position.X)
            {
                Player.headRotation = Utils.Clamp((Player.Center - Main.MouseWorld).ToRotation(), -0.5f, 0.5f);
            }*/
        }

        public override void UpdateEquips()
        {
            /*if (Player.counterWeight > 0)
            {
                this.counterWeight = 556 + Main.rand.Next(6);
                this.yoyoGlove = true;
                this.yoyoString = true;
            }

            if (currentItem.type >= 3309 && currentItem.type <= 3314)
            {
                this.counterWeight = 556 + currentItem.type - 3309;
            }*/
        }

        private void UpdateFlamingFlowerZoneEffect()
        {
            bool flag = Main.UseHeatDistortion && this.ZoneFlamingFlower;
            if (flag)
            {
                Player.ManageSpecialBiomeVisuals("HeatDistortion", flag);
                if (!Player.ZoneDesert && !Player.ZoneUndergroundDesert && !Player.ZoneUnderworldHeight)
                {
                    Terraria.Graphics.Effects.Filters.Scene["HeatDistortion"].GetShader().UseIntensity(0.85f);
                }
            }
        }

        private void UpdateDyes()
        {
            starsOverHeadDye = 0;

            for (int i = 0; i < 20; i++)
            {
                if (!Player.IsAValidEquipmentSlotForIteration(i)) continue;

                int num = i % 10;
                this.UpdateItemDye(i < 10, Player.hideVisibleAccessory[num], Player.armor[i], Player.dye[num]);
            }
        }

        private void UpdateItemDye(bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
        {
            if (armorItem.IsAir) return;

            bool flag = armorItem.wingSlot > 0 || armorItem.type == ItemID.FlyingCarpet || armorItem.type == 4341 || armorItem.type == 4563 || armorItem.type == ItemID.GingerBeard || armorItem.type == ItemID.AngelHalo;
            bool flag2 = isNotInVanitySlot && isSetToHidden;

            if (!flag && flag2) return;

            if (armorItem.type == ModContent.ItemType<Content.Items.Accessories.NonameVanity>())
            {
                starsOverHeadDye = dyeItem.dye;
            }
        }
    }
}
