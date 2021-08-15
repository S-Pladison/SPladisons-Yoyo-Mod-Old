using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace SPladisonsYoyoMod.Common
{
    public partial class PladSystem : ModSystem
    {
        public override void ResetNearbyTileEffects()
        {
            Main.LocalPlayer.GetPladPlayer().ZoneFlamingFlower = false;
        }

        public override void PostUpdateWorld()
        {
            UpdateFlamingFlower();
        }

        public override void PostUpdateEverything()
        {
            SoulFilledFlameEffect.Instance?.UpdateParticles();
        }

        public override void PostWorldGen()
        {
            ChangeLootInChests();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (index >= 0)
            {
                tasks.Insert(index + 1, new PassLegacy(Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.SpaceChest"), GenerateSpaceChest));
            }
        }

        public override TagCompound SaveWorldData()
        {
            return new TagCompound
            {
                { "flamingFlowerPosition", FlamingFlowerPosition.ToVector2() }
            };
        }

        public override void LoadWorldData(TagCompound tag)
        {
            FlamingFlowerPosition = tag.Get<Vector2>("flamingFlowerPosition").ToPoint();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(FlamingFlowerPosition.ToVector2());
        }

        public override void NetReceive(BinaryReader reader)
        {
            FlamingFlowerPosition = reader.ReadVector2().ToPoint();
        }

        // ...

        private static void ChangeLootInChests()
        {
            for (int chestIndex = 0; chestIndex < Main.chest.Length; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest == null || Main.tile[chest.x, chest.y].type != TileID.Containers) continue;

                switch (Main.tile[chest.x, chest.y].frameX / 36)
                {
                    case 12: // Living Wood Chest
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                AddItemToFirstSlotOfChest(chest, ModContent.ItemType<Content.Items.Weapons.BloomingDeath>());
                            }
                        }
                        break;
                }
            }
        }

        private static void AddItemToFirstSlotOfChest(Chest chest, int itemType, int quantity = 1)
        {
            if (chest.item.Count(i => i.type > ItemID.None) >= Chest.maxItems) return;

            var items = chest.item.ToList();

            Item item = new Item();
            item.SetDefaults(itemType);
            item.stack = quantity;

            items.Insert(0, item);
            items.Remove(items.Last());

            chest.item = items.ToArray();
        }
    }
}
