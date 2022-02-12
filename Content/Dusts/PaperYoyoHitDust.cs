using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Dusts
{
    public class PaperYoyoHitDust : ModDust
    {
        public float ColorProgress(Dust dust) => MathUtils.MultipleLerp(MathHelper.Lerp, dust.fadeIn / 60f, new[] { 0f, 1f, 1f, 1f, 1f, 0f });

        public override string Texture => ModAssets.DustsPath + "PaperYoyoHitDust";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = 0;
            dust.frame = new Rectangle(0, 0, 50, 38);
            dust.fadeIn = 0;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => Color.White * ColorProgress(dust);

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;

            dust.velocity *= 0.92f;
            dust.velocity.X *= 0.95f;
            dust.rotation *= 0.95f;
            dust.position += dust.velocity;

            if (dust.fadeIn >= 60)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
