using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace SPladisonsYoyoMod.Common
{
    public partial class PladSystem : ModSystem
    {
        public override void PostUpdateWorld()
        {
            this.UpdateFlamingFlower();
        }

        public override void PostUpdateEverything()
        {
            SPladisonsYoyoMod.Primitives?.UpdateTrails();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int index = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (index >= 0)
            {
                tasks.Insert(index + 1, new PassLegacy(Language.GetTextValue("Mods.SPladisonsYoyoMod.WorldGen.SpaceChest"), this.GenerateSpaceChest));
            }
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
