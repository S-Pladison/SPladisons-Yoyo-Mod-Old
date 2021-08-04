﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class NonameVanity : PladItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.width = 40;
            Item.height = 36;

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual) player.GetPladPlayer().starsOverHeadVisible = true;
        }

        public override void UpdateVanity(Player player)
        {
            player.GetPladPlayer().starsOverHeadVisible = true;
        }
    }
}