using SPladisonsYoyoMod.Common.Globals;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Buffs
{
    public class ImprovedPoisoningDebuff : ModBuff
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Buffs/" + this.Name;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // АЛОООО: А нужна ли вообще эта переменная, если мы итак накладывает дебафф...
            npc.GetGlobalNPC<PladGlobalNPC>().improvedPoisoningDebuff = true;
        }
    }
}
