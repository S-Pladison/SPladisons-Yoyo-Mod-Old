using MonoMod.RuntimeDetour.HookGen;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Misc
{
    public class StrangeDrink : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + "StrangeDrink";

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Quest;
            Item.width = 22;
            Item.height = 46;
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
        private bool buttonWasPressed = false;

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

                button2 = Language.GetTextValue($"Mods.SPladisonsYoyoMod.GameUI.NurseButton_{(buttonWasPressed ? 2 : 1)}");
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
                Main.npcChatText = Language.GetTextValue($"Mods.SPladisonsYoyoMod.GameUI.NurseText_1");
            }
            else
            {
                var player = Main.player[Main.myPlayer];
                var item = player.FindItem(strangeDrinkType);

                if (item != -1)
                {
                    Main.npcChatText = Language.GetTextValue($"Mods.SPladisonsYoyoMod.GameUI.NurseText_2");

                    player.inventory[item] = new Item();
                    player.QuickSpawnItem(player.GetSource_GiftOrReward(), GiftType);
                }

                Main.npcChatCornerItem = 0;
            }

            buttonWasPressed = !buttonWasPressed;
        }

        public override void OnWorldLoad()
        {
            buttonWasPressed = false;
        }

        public override void Load()
            => HookEndpointManager.Add<hook_SetChatButtons>(SetChatButtonsMethod, On_NPCLoader_SetChatButtons);

        public override void Unload()
            => HookEndpointManager.Remove<hook_SetChatButtons>(SetChatButtonsMethod, On_NPCLoader_SetChatButtons);

        // ...

        private static void On_NPCLoader_SetChatButtons(orig_SetChatButtons orig, ref string button, ref string button2)
        {
            orig(ref button, ref button2);

            Instance.SetChatButtons(ref button2);
        }

        private delegate void orig_SetChatButtons(ref string button, ref string button2);
        private delegate void hook_SetChatButtons(orig_SetChatButtons orig, ref string button, ref string button2);

        private static readonly MethodInfo SetChatButtonsMethod = typeof(NPCLoader).GetMethod(nameof(NPCLoader.SetChatButtons));
    }
}
