using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class YoyoItem : PladItem
    {
        private readonly int _gamepadExtraRange;

        public YoyoItem(int gamepadExtraRange)
        {
            _gamepadExtraRange = gamepadExtraRange;
        }

        public sealed override void PladSetStaticDefaults()
        {
            ItemID.Sets.Yoyo[Type] = true;
            ItemID.Sets.GamepadExtraRange[Type] = _gamepadExtraRange;
            ItemID.Sets.GamepadSmartQuickReach[Type] = true;

            this.YoyoSetStaticDefaults();
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

            this.YoyoSetDefaults();
        }

        public virtual void YoyoSetStaticDefaults() { }
        public virtual void YoyoSetDefaults() { }
    }

    public abstract class YoyoProjectile : PladProjectile
    {
        private readonly float _lifeTime;
        private readonly float _maxRange;
        private readonly float _topSpeed;

        public YoyoProjectile(float lifeTime, float maxRange, float topSpeed)
        {
            _lifeTime = lifeTime;
            _maxRange = maxRange;
            _topSpeed = topSpeed;
        }

        public sealed override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = _lifeTime;
            ProjectileID.Sets.YoyosMaximumRange[Type] = _maxRange;
            ProjectileID.Sets.YoyosTopSpeed[Type] = _topSpeed;

            this.YoyoSetStaticDefaults();
        }

        public sealed override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;

            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.aiStyle = Global.PladGlobalProjectile.YoyoAIStyle;
            Projectile.friendly = true;
            Projectile.penetrate = -1;

            this.YoyoSetDefaults();
        }

        public virtual void YoyoSetStaticDefaults() { }
        public virtual void YoyoSetDefaults() { }
        public virtual void ModifyYoyo(ref float lifeTime, ref float maxRange, ref float topSpeed) { }
    }
}
