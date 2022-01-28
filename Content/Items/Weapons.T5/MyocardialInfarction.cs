using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class MyocardialInfarction : YoyoItem
    {
        public MyocardialInfarction() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<MyocardialInfarctionProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class MyocardialInfarctionProjectile : YoyoProjectile
    {
        public MyocardialInfarctionProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }
    }
}