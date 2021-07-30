using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.ItemDropRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
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
