using Microsoft.Xna.Framework;
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
        public bool improvedPoisoningDebuff;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            improvedPoisoningDebuff = false;
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            switch (npc.type)
            {
                case NPCID.Nurse:
                    NurseGiftSystem.Instance.GetChat();
                    break;
                default:
                    break;
            }
        }

        public override void OnChatButtonClicked(NPC npc, bool firstButton)
        {
            switch (npc.type)
            {
                case NPCID.Nurse:
                    NurseGiftSystem.Instance.OnChatButtonClicked(firstButton);
                    break;
                default:
                    break;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (improvedPoisoningDebuff && Main.rand.NextBool(26))
            {
                Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Poisoned, 0f, 0f, 120, default, 0.2f);
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
