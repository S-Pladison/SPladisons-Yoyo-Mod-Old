using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class EmbraceOfRain : YoyoItem
    {
        public EmbraceOfRain() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<EmbraceOfRainProjectile>();

            Item.rare = ItemRarityID.Orange;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class EmbraceOfRainProjectile : YoyoProjectile
    {
        public EmbraceOfRainProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }
    }

    public class EmbraceOfRainEffectSystem : ModSystem
    {

    }
}
