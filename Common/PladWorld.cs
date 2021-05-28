using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SPladisonsYoyoMod.Common
{
    public partial class PladWorld : ModSystem
    {
        public override void PostUpdateWorld()
        {
            this.UpdateFlamingFlower();
        }

        public override TagCompound SaveWorldData()
        {
            return new TagCompound
            {
                { "flamingFlowerPosition", flamingFlowerPosition.ToVector2() }
            };
        }

        public override void LoadWorldData(TagCompound tag)
        {
            flamingFlowerPosition = tag.Get<Vector2>("flamingFlowerPosition").ToPoint();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(flamingFlowerPosition.ToVector2());
        }

        public override void NetReceive(BinaryReader reader)
        {
            flamingFlowerPosition = reader.ReadVector2().ToPoint();
        }
    }
}
