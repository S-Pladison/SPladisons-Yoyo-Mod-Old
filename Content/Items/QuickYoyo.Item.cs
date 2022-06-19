using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items
{
    public abstract class YoyoItem : ModItem
    {
        private readonly int gamepadExtraRange;

        public YoyoItem(int gamepadExtraRange)
        {
            this.gamepadExtraRange = gamepadExtraRange;
        }

        public override string Texture => ModAssets.ItemsPath + (ModContent.RequestIfExists<Texture2D>(ModAssets.ItemsPath + Name, out _) ? Name : "UnknownYoyo");

        public sealed override void SetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = gamepadExtraRange;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;

            SacrificeTotal = 1;

            YoyoSetStaticDefaults();
        }

        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.width = 30;
            Item.height = 26;
            Item.shootSpeed = 16f;

            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 25;
            Item.useTime = 25;

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            YoyoSetDefaults();
        }

        public virtual void YoyoSetStaticDefaults() { }
        public virtual void YoyoSetDefaults() { }
    }
}