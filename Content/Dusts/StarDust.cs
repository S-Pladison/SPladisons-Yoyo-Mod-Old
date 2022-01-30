using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Dusts
{
    public class StarDust : ModDust
    {
        public override string Texture => ModAssets.DustsPath + "StarDust";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 180;
            dust.rotation = 0;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 4) * 10, 10, 10);
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += Math.Sign(Main.rand.Next(-1, 0)) * 0.01f;
            dust.color = Color.White;
            return base.Update(dust);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(dust.color.R, dust.color.G, dust.color.B, dust.alpha);
    }
}
