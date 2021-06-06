using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class Blackhole : YoyoItem
    {
        public Blackhole() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Blackhole", rus: "Черная дыра");
            this.SetTooltip(
                eng: "...",
                rus: "..."
                );
        }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<FogOfDesiresProjectile>();

            Item.rare = ItemRarityID.Cyan;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BlackholeProjectile : YoyoProjectile
    {
        public BlackholeProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Blackhole", rus: "Черная дыра");
        }
    }
}
