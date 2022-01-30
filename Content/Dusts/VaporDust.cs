using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Dusts
{
    public class VaporDust : ModDust
    {
        public override string Texture => ModAssets.DustsPath + "VaporDust";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 225;
            dust.rotation = 0;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 3) * 10, 10, 10);
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += Math.Sign(Main.rand.Next(-1, 0)) * 0.01f;
            return base.Update(dust);
        }
    }
}
