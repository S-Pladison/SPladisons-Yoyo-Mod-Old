using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Misc
{
    public class StrangeDrink : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + "StrangeDrink";

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Quest;
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LifeFruit, 3);
            recipe.AddIngredient(ItemID.Milkshake);
            recipe.AddIngredient(ItemID.GrapeJuice);
            recipe.AddIngredient(ItemID.PrismaticPunch);
            recipe.AddIngredient(ItemID.PinaColada);
            recipe.AddIngredient(ItemID.TropicalSmoothie);
            recipe.AddIngredient(ItemID.SmoothieofDarkness);
            recipe.AddIngredient(ItemID.AppleJuice);
            recipe.AddIngredient(ItemID.BananaDaiquiri);
            recipe.AddIngredient(ItemID.Lemonade);
            recipe.AddIngredient(ItemID.PeachSangria);
            recipe.AddTile(TileID.CookingPots);
            recipe.Register();

            recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.LifeFruit, 3);
            recipe.AddIngredient(ItemID.Milkshake);
            recipe.AddIngredient(ItemID.GrapeJuice);
            recipe.AddIngredient(ItemID.PrismaticPunch);
            recipe.AddIngredient(ItemID.PinaColada);
            recipe.AddIngredient(ItemID.TropicalSmoothie);
            recipe.AddIngredient(ItemID.BloodyMoscato);
            recipe.AddIngredient(ItemID.AppleJuice);
            recipe.AddIngredient(ItemID.BananaDaiquiri);
            recipe.AddIngredient(ItemID.Lemonade);
            recipe.AddIngredient(ItemID.PeachSangria);
            recipe.AddTile(TileID.CookingPots);
            recipe.Register();
        }
    }

    public class NurseGiftSystem : ModSystem
    {
        private static bool buttonWasPressed = false;

        // ...

        public static NurseGiftSystem Instance { get => ModContent.GetInstance<NurseGiftSystem>(); }
        public static int GiftType { get => ModContent.ItemType<Weapons.MyocardialInfarction>(); }

        // ...

        public void GetChat()
        {
            buttonWasPressed = false;
        }

        public void SetChatButtons(ref string button2)
        {
            var player = Main.player[Main.myPlayer];

            if (player.talkNPC >= 0 && Main.npc[player.talkNPC].type == NPCID.Nurse && player.HasItem(ModContent.ItemType<StrangeDrink>()))
            {
                if (Main.npcChatFocus4 && Main.mouseLeft && !Main.mouseLeftRelease)
                {
                    buttonWasPressed = false;
                }

                if (!buttonWasPressed) button2 = "Подарок";
                else button2 = "Подарить";
            }
        }

        public void OnChatButtonClicked(bool firstButton)
        {
            if (firstButton)
            {
                buttonWasPressed = false;
                return;
            }

            int strangeDrinkType = ModContent.ItemType<StrangeDrink>();

            if (!buttonWasPressed)
            {
                Main.npcChatCornerItem = strangeDrinkType;
                Main.npcChatText = "Чем могу помочь?\n\n" +
                                   "(Если хотите, можете подарить данный предмет)";
            }
            else
            {
                var player = Main.player[Main.myPlayer];
                var item = player.FindItem(strangeDrinkType);

                if (item != -1)
                {
                    Main.npcChatText = "Ах, не стоило... Спасибо. Взамен возьми вот это. Думаю, тебе он пригодится больше, чем мне.\n\n" +
                                       "(Откуда у нее йо-йо?..)";

                    player.inventory[item] = new Item();
                    player.QuickSpawnItem(GiftType);
                }

                Main.npcChatCornerItem = 0;
            }

            buttonWasPressed = !buttonWasPressed;
        }

        public override void OnWorldLoad()
        {
            buttonWasPressed = false;
        }
    }
}
