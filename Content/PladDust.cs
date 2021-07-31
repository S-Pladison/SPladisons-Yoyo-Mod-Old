using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladDust : ModDust
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Dusts/" + this.Name;
    }
}
