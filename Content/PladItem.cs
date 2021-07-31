using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladItem : ModItem
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Items/" + this.Name;

        public sealed override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = SacrificeCount;

            this.PladSetStaticDefaults();
        }

        public virtual int SacrificeCount => 1;
        public virtual void PladSetStaticDefaults() { }
    }
}
