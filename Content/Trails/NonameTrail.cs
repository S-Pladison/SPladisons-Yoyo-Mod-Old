using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static SPladisonsYoyoMod.Common.Primitives;

namespace SPladisonsYoyoMod.Content.Trails
{
    public class NonameTrail : SimpleTrail
    {
        public NonameTrail(int length) : base(length, (p) => 21 * (1 - p * 0.25f), (p) => Color.White * (1 - p))
        {
            this.SetEffectTexture(ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_7").Value);
        }

        public override BlendState BlendState => BlendState.AlphaBlend;
    }
}
