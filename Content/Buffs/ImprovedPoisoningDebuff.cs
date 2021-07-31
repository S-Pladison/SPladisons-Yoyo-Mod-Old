using SPladisonsYoyoMod.Common.Globals;
using Terraria;

namespace SPladisonsYoyoMod.Content.Buffs
{
    public class ImprovedPoisoningDebuff : PladBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<PladGlobalNPC>().improvedPoisoningDebuff = true;
        }
    }
}
