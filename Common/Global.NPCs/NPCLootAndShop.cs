using SPladisonsYoyoMod.Content.Items.Misc;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Global
{
    public partial class PladGlobalNPC : GlobalNPC
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<SpaceKey>(), 2500, 1, 1, new SpaceKeyCondition(), 1));
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            switch (type)
            {
                case NPCID.SkeletonMerchant:
                    {
                        int yoyoGloveIndex = shop.item.ToList().FindIndex(i => i.type == ItemID.YoYoGlove);
                        if (yoyoGloveIndex == -1 && NPC.downedBoss3 && Main.rand.NextBool(2))
                        {
                            shop.item[nextSlot].SetDefaults(ItemID.YoYoGlove);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}