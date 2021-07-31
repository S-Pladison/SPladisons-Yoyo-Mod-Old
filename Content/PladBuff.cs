using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladBuff : ModBuff
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Buffs/" + this.Name;
    }
}
