using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items
{

    /*
     
     | Pre-Hardmode
     -----------------------------------------------------------------------------------------------
     • Wooden Yoyo        | gamepadExtraRange: 4  | lifeTime: 3f  | maxRange: 130f | topSpeed: 9f
     • Rally              | gamepadExtraRange: 6  | lifeTime: 5f  | maxRange: 170f | topSpeed: 11f
     • Malaise            | gamepadExtraRange: 8  | lifeTime: 7f  | maxRange: 195f | topSpeed: 12.5f
     • Artery             | gamepadExtraRange: 8  | lifeTime: 6f  | maxRange: 207f | topSpeed: 12f
     • Amazon             | gamepadExtraRange: 9  | lifeTime: 8f  | maxRange: 215f | topSpeed: 13f
     • Code 1             | gamepadExtraRange: 10 | lifeTime: 9f  | maxRange: 220f | topSpeed: 13f
     • Valor              | gamepadExtraRange: 10 | lifeTime: 11f | maxRange: 225f | topSpeed: 14f
     • Cascade            | gamepadExtraRange: 10 | lifeTime: 13f | maxRange: 235f | topSpeed: 14f

     | Hardmode
     -----------------------------------------------------------------------------------------------
     • Chik               | gamepadExtraRange: 12 | lifeTime: 16f | maxRange: 275f | topSpeed: 17f
     • Format:C           | gamepadExtraRange: 10 | lifeTime: 8f  | maxRange: 235f | topSpeed: 15f
     • Hel-Fire           | gamepadExtraRange: 13 | lifeTime: 12f | maxRange: 275f | topSpeed: 15f
     • Amarok             | gamepadExtraRange: 11 | lifeTime: 15f | maxRange: 270f | topSpeed: 14f
     • Gradient           | gamepadExtraRange: 11 | lifeTime: 10f | maxRange: 250f | topSpeed: 12f
     • Code 2             | gamepadExtraRange: 13 | lifeTime: -   | maxRange: 280f | topSpeed: 17f
     • Yelets             | gamepadExtraRange: 13 | lifeTime: 14f | maxRange: 290f | topSpeed: 16f
     • Red's Throw        | gamepadExtraRange: 18 | lifeTime: -   | maxRange: 370f | topSpeed: 16f
     • Valkyrie Yoyo      | gamepadExtraRange: 18 | lifeTime: -   | maxRange: 370f | topSpeed: 16f
     • Kraken             | gamepadExtraRange: 17 | lifeTime: -   | maxRange: 340f | topSpeed: 16f
     • The Eye of Cthulhu | gamepadExtraRange: 18 | lifeTime: -   | maxRange: 360f | topSpeed: 16.5f
     • Terrarian          | gamepadExtraRange: 21 | lifeTime: -   | maxRange: 400f | topSpeed: 17.5f

    */

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

    public abstract class YoyoProjectile : ModProjectile
    {
        public bool YoyoGloveActivated { get; private set; }
        public bool IsReturning { get => Projectile.ai[0] == -1; }
        public float ReturningProgress { get; private set; } = 1f; // 1 -> 0

        private readonly float lifeTime;
        private readonly float maxRange;
        private readonly float topSpeed;

        private Vector2? positionBeforeReturning;

        public YoyoProjectile(float lifeTime, float maxRange, float topSpeed)
        {
            this.lifeTime = lifeTime;
            this.maxRange = maxRange;
            this.topSpeed = topSpeed;
        }

        public override string Texture => ModAssets.ProjectilesPath + (ModContent.RequestIfExists<Texture2D>(ModAssets.ProjectilesPath + Name, out _) ? Name : "UnknownYoyoProjectile");

        // ...

        public sealed override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = lifeTime;
            ProjectileID.Sets.YoyosMaximumRange[Type] = maxRange;
            ProjectileID.Sets.YoyosTopSpeed[Type] = topSpeed;

            SPladisonsYoyoMod.Sets.IsSoloYoyoProjectile[Type] = IsSoloYoyo();

            YoyoSetStaticDefaults();
        }

        public sealed override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;

            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.aiStyle = Common.Globals.YoyoGlobalProjectile.YoyoAIStyle;
            Projectile.friendly = true;
            Projectile.penetrate = -1;

            YoyoSetDefaults();
        }

        public sealed override bool PreAI()
        {
            var owner = Main.player[Projectile.owner];

            if (!YoyoPreAI(owner)) return false;

            if (IsReturning)
            {
                if (!positionBeforeReturning.HasValue)
                {
                    positionBeforeReturning = Projectile.Center;
                }

                var progress = Vector2.DistanceSquared(owner.Center, Projectile.Center) / Vector2.DistanceSquared(owner.Center, positionBeforeReturning.Value);
                ReturningProgress = MathHelper.Clamp(progress, 0f, 1f);
            }

            return true;
        }

        public sealed override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            var owner = Main.player[Projectile.owner];

            if (owner.yoyoGlove && !YoyoGloveActivated)
            {
                YoyoGloveActivated = true;
                OnActivateYoyoGlove();
            }

            YoyoOnHitNPC(owner, target, damage, knockback, crit);
        }

        public sealed override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(YoyoGloveActivated);
            YoyoSendExtraAI(writer);
        }

        public sealed override void ReceiveExtraAI(BinaryReader reader)
        {
            YoyoGloveActivated = reader.ReadBoolean();
            YoyoReceiveExtraAI(reader);
        }

        // ...

        public virtual bool YoyoPreAI(Player owner) => true;
        public virtual void YoyoSetStaticDefaults() { }
        public virtual void YoyoSetDefaults() { }
        public virtual void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit) { }
        public virtual void YoyoSendExtraAI(BinaryWriter writer) { }
        public virtual void YoyoReceiveExtraAI(BinaryReader reader) { }

        public virtual bool IsSoloYoyo() => false;
        public virtual void OnActivateYoyoGlove() { }
        public virtual void ModifyYoyoLifeTime(ref float lifeTime) { }
        public virtual void ModifyYoyoMaximumRange(ref float maxRange) { }
    }

    public abstract class CounterweightProjectile : ModProjectile
    {
        private static readonly MethodInfo AI_099_1_MethodInfo = typeof(Projectile).GetMethod("AI_099_1", BindingFlags.NonPublic | BindingFlags.Instance);

        private bool Try_AI_099_1()
        {
            if (AI_099_1_MethodInfo != null)
            {
                try
                {
                    AI_099_1_MethodInfo.Invoke(Projectile, Array.Empty<object>());
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return false;
        }

        // ...

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 0;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 99;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.scale = 1f;
            Projectile.counterweight = true;
        }

        public sealed override bool PreAI()
        {
            if (!Try_AI_099_1())
            {
                Projectile.Kill();
                return false;
            }

            return false;
        }

        public sealed override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool flag = false;

            if (Projectile.velocity.X != oldVelocity.X)
            {
                flag = true;
                Projectile.velocity.X = oldVelocity.X * -1f;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                flag = true;
                Projectile.velocity.Y = oldVelocity.Y * -1f;
            }

            if (flag)
            {
                Vector2 vector = Main.player[Projectile.owner].Center - Projectile.Center;
                vector.Normalize();
                vector *= Projectile.velocity.Length();
                vector *= 0.25f;

                Projectile.velocity *= 0.75f;
                Projectile.velocity += vector;

                if (Projectile.velocity.Length() > 6f)
                {
                    Projectile.velocity *= 0.5f;
                }
            }

            return CounterweightOnTileCollide(oldVelocity);
        }

        public virtual bool CounterweightOnTileCollide(Vector2 oldVelocity) { return true; }
    }
}
