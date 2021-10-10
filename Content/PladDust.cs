using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladDust : ModDust
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Dusts/" + this.Name;
    }
}
