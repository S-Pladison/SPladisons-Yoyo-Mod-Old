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
            uint sacrificeCount = 1;

            this.SetStaticDefaults(ref sacrificeCount);

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = (int)sacrificeCount;
        }

        // ...

        /// <summary>
        /// Allows you to modify the properties after initial loading has completed.
        /// </summary>
        /// <param name="sacrificeCount">How many units of that item will be required to complete its research.</param>
        public virtual void SetStaticDefaults(ref uint sacrificeCount) { }
    }
}