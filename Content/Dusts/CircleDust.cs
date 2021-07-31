using Microsoft.Xna.Framework;
using Terraria;

namespace SPladisonsYoyoMod.Content.Dusts
{
    public class CircleDust : PladDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = 0;
            dust.frame = new Rectangle(0, 0, 16, 16);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(dust.color.R, dust.color.G, dust.color.B, dust.alpha);
    }
}
