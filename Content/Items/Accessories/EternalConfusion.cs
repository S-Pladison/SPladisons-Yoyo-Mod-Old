using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Accessories
{
    public class EternalConfusion : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + "EternalConfusion";

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
            Item.width = 40;
            Item.height = 36;

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.GetPladPlayer().eternalConfusionDye != 0)
            {
                var index = tooltips.FindIndex(i => i.Mod == "Terraria" && (i.Name.StartsWith("Social") || i.Name.StartsWith("Vanity")));

                if (index != -1) tooltips.Insert(index + 1, new TooltipLine(Mod, "EternalConfusionDye", Language.GetTextValue("Mods.SPladisonsYoyoMod.ItemTooltip.EternalConfusionDye")));
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var modPlayer = player.GetPladPlayer();

            modPlayer.eternalConfusionEquipped = true;
            modPlayer.eternalConfusionVisible = !hideVisual;

            if (!hideVisual && modPlayer.eternalConfusionDye == 0)
            {
                Lighting.AddLight(player.position + player.headPosition + player.headFrame.Size() * 0.5f, new Color(231, 209, 51).ToVector3() * 0.1f);
            }
        }

        public override void UpdateVanity(Player player)
        {
            var modPlayer = player.GetPladPlayer();
            modPlayer.eternalConfusionVisible = true;

            if (modPlayer.eternalConfusionDye == 0)
            {
                Lighting.AddLight(player.position + player.headPosition + player.headFrame.Size() * 0.5f, new Color(231, 209, 51).ToVector3() * 0.1f);
            }
        }
    }
}
