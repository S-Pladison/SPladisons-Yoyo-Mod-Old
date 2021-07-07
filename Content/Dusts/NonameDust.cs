using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Dusts
{
    public class NonameDust : PladDust
    {
        private float progress;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.alpha = 180;
            dust.rotation = 0;
            dust.frame = new Rectangle(0, Main.rand.Next(0, 4) * 10, 10, 10);
            progress = dust.frame.Y * 0.166f;
        }

        public override bool Update(Dust dust)
        {
            dust.rotation += Math.Sign(Main.rand.Next(-1, 0)) * 0.01f;
            progress += 0.005f;
            if (progress >= 2f) progress = 0;

            /*switch (dust.frame.Y)
            {
                case 0:
                    dust.color = Color.Lerp(new Color(255, 95, 244), new Color(250, 100, 100), progress > 1 ? 2 - progress : progress);
                    break;
                case 10:
                    dust.color = Color.Lerp(new Color(250, 100, 100), new Color(95, 255, 151), progress > 1 ? 2 - progress : progress);
                    break;
                case 20:
                    dust.color = Color.Lerp(new Color(95, 255, 151), new Color(95, 175, 255), progress > 1 ? 2 - progress : progress);
                    break;
                case 30:
                    dust.color = Color.Lerp(new Color(95, 175, 255), new Color(255, 95, 244), progress > 1 ? 2 - progress : progress);
                    break;
                default:
                    dust.color = Color.Lerp(new Color(95, 175, 255), new Color(255, 95, 244), progress > 1 ? 2 - progress : progress);
                    break;
            }*/

            dust.color = Color.White;

            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), dust.color.R / 255f * 0.4f, dust.color.G / 255f * 0.4f, dust.color.B / 255f * 0.4f);

            return base.Update(dust);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(dust.color.R, dust.color.G, dust.color.B, dust.alpha);
    }
}
