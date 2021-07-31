using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.ItemDropRules;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
{
    public class PladGlobalNPC : GlobalNPC
    {
        public bool improvedPoisoningDebuff;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            improvedPoisoningDebuff = false;
        }

        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Content.Items.Placeables.SpaceKey>(), 2500, 1, 1, new SpaceKeyCondition(), 1));
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.SkeletonMerchant)
            {
                int yoyoGloveIndex = shop.item.ToList().FindIndex(i => i.type == ItemID.YoYoGlove);
                if (yoyoGloveIndex == -1 && NPC.downedBoss3 && Main.rand.NextBool(2))
                {
                    shop.item[nextSlot].SetDefaults(ItemID.YoYoGlove);
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (improvedPoisoningDebuff && Main.rand.Next(26) == 0)
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 46, 0f, 0f, 120, default, 0.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.9f;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (improvedPoisoningDebuff)
            {
                if (npc.lifeRegen > 0) npc.lifeRegen = 0;
                npc.lifeRegen -= 8;
                if (damage < 3) damage = 3;
            }
        }
    }
}
