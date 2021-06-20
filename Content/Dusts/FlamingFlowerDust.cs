using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SPladisonsYoyoMod.Content.Dusts
{
    public class FlamingFlowerDust : PladDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 180;
            dust.rotation = 0;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 2) * 5, 5, 5);
            dust.color = new Color(255, 240, 70);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.985f;
            dust.position += dust.velocity * 0.96f + new Vector2(Main.rand.NextFloat(-0.35f, 0.35f), 0);
            dust.scale *= 0.945f;

            if (dust.scale <= 0.1) dust.active = false;
            dust.color = Color.Lerp(new Color(255, 240, 70), new Color(140, 50, 50), 1 - dust.scale);
            dust.alpha = (int)MathHelper.Lerp(180, 0, 1 - dust.scale);

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(dust.color.R, dust.color.G, dust.color.B, dust.alpha);
    }
}
