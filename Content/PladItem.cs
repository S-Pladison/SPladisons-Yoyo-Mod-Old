using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladItem : ModItem
    {
        public override string Texture
        {
            get
            {
                string path = "SPladisonsYoyoMod/Assets/Textures/Items/" + this.Name;
                return ModContent.RequestIfExists<Texture2D>(path, out _) ? path : "ModLoader/UnloadedItem";
            }
        }

        public sealed override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = SacrificeCount;

            this.PladSetStaticDefaults();
        }

        public virtual int SacrificeCount => 1;
        public virtual void PladSetStaticDefaults() { }
    }
}