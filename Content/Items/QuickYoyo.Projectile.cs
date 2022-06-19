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
