using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Content.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class OutburstingGust : YoyoItem
    {
        public OutburstingGust() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<OutburstingGustProjectile>();

            Item.rare = ItemRarityID.Green;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class OutburstingGustProjectile : YoyoProjectile
    {
        public OutburstingGustProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }
    }
}
