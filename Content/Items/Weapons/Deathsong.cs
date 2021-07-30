using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class Deathsong : YoyoItem
    {
        public Deathsong() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<DeathsongProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class DeathsongProjectile : YoyoProjectile
    {
        public DeathsongProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Deathsong", rus: "Песнь смерти");
        }

        public override void OnSpawn()
        {
            RetrowaveTrail trail = new RetrowaveTrail(length: 16 * 11, width: (p) => 21 * (1 - p), color: (p) => Color.Lerp(new Color(32, 244, 250), new Color(32, 160, 250), p))
            {
                MaxPoints = 30
            };
            trail.SetEffectTexture(SPladisonsYoyoMod.ExtraTextures[13].Value);
            SPladisonsYoyoMod.Primitives?.CreateTrail(target: Projectile, trail: trail);
        }
    }
}
