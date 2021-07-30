using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SPladisonsYoyoMod.Content.Dusts
{
    /*public class SoulFilledFlameDust : PladDust
    {
        public override string Texture => SPladisonsYoyoMod/Assets/Textures/Misc/Extra_0";

        public static int GetCustomData(Dust dust) => (dust.customData as int?) ?? 0;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.rotation = 0;
            
            SoulFilledFlameEffect.Instance?.AddDust(dust.dustIndex);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.X *= 0.9f;
            dust.position += dust.velocity;
            dust.scale *= 0.96f;

            int time = GetCustomData(dust) - 1;
            dust.customData = time;

            if (time <= 0 || dust.scale <= 0)
            {
                SoulFilledFlameEffect.Instance?.RemoveDust(dust.dustIndex);
                dust.active = false;
            }
            return false;
        }
    }*/
}
