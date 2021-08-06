using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class Stable : YoyoItem
    {
        public Stable() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<StableProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class StableProjectile : YoyoProjectile
    {
        public StableProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Stable", rus: "Стабильный");
        }

        public override void OnSpawn()
        {
            static Color StripColors(float progressOnStrip)
            {
                Color value = Main.hslToRgb((progressOnStrip * 1.6f - Main.GlobalTimeWrappedHourly) % 1f, 1f, 0.5f, byte.MaxValue);
                Color result = Color.Lerp(Color.White, value, Utils.GetLerpValue(-0.2f, 0.5f, progressOnStrip, true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip, false));
                result.A = 0;
                return result;
            }

            var trail = new SimpleTrail(target: Projectile, length: 16 * 8, width: (p) => 16 * (1 - p), color: StripColors);
            //trail.SetEffectTexture(ModAssets.ExtraTextures[17].Value);
            trail.SetDissolveSpeed(0.15f);
            SPladisonsYoyoMod.Primitives.NewTrail(trail);
        }
    }
}
