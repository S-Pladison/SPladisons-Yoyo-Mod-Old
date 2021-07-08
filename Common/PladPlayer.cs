using SPladisonsYoyoMod.Content.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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

        public override void ResetEffects()
        {
            bearingEquipped = false;
            candyCaneEquipped = false;
            desecratedDiceEquipped = false;
            flamingFlowerEquipped = false;
        }

        public override void PostUpdate()
        {
            this.UpdateFlamingFlowerZoneEffect();
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

        public void UpdateFlamingFlowerZoneEffect()
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
    }
}
