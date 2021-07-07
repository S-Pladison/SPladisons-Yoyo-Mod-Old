using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Content.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class FogOfDesires : YoyoItem
    {
        public FogOfDesires() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Fog of Desires", rus: "Туман желаний");
            this.SetTooltip(eng: "...",
                            rus: "...");
        }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<FogOfDesiresProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class FogOfDesiresProjectile : YoyoProjectile
    {
        public FogOfDesiresProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Fog of Desires", rus: "Туман желаний");
        }

        public override void OnSpawn()
        {
            SimpleTrail trail = new (length: 16 * 10, width: (progress) => 20, color: (progress) => Color.White);
            trail.SetEffectTexture(ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_8").Value);
            SPladisonsYoyoMod.Primitives.CreateTrail(target: Projectile, trail: trail);
        }
    }
}
