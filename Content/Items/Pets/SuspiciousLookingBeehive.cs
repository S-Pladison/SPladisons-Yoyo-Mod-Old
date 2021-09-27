using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Pets
{
    public class SuspiciousLookingBeehive : PladItem
    {
        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<SquareBeeProjectile>();
            Item.buffType = ModContent.BuffType<SquareBeeBuff>();
            Item.rare = ItemRarityID.Cyan;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item2;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 5, 0, 0);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }

    public class SquareBeeBuff : PladBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<SquareBeeProjectile>();
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }

    public class SquareBeeProjectile : PladProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 38;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= Main.projFrames[Projectile.type] + 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];
            if (!player.dead && player.HasBuff(ModContent.BuffType<SquareBeeBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            float num33 = 0.4f;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num31 = player.position.X + (float)(player.width / 2) - vector3.X;
            float num30 = player.position.Y + (float)(player.height / 2) - vector3.Y;
            float num24 = 14f;

            num30 += (float)Main.rand.Next(-10, 21);
            num31 += (float)Main.rand.Next(-10, 21);
            num31 += 60f * (0f - (float)player.direction);
            num30 -= 60f;

            float num25 = (float)Math.Sqrt((double)(num31 * num31 + num30 * num30));

            if (num25 < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.99f;
                }
                num33 = 0.01f;
            }
            else
            {
                if (num25 < 100f) num33 = 0.1f;
                if (num25 > 300f) num33 = 0.6f;

                num25 = num24 / num25;
                num31 *= num25;
                num30 *= num25;
            }

            if (Projectile.velocity.X < num31)
            {
                Projectile.velocity.X = Projectile.velocity.X + num33;
                if (num33 > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num33;
                }
            }

            if (Projectile.velocity.X > num31)
            {
                Projectile.velocity.X = Projectile.velocity.X - num33;
                if (num33 > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num33;
                }
            }

            if (Projectile.velocity.Y < num30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num33;
                if (num33 > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num33 * 2f;
                }
            }

            if (Projectile.velocity.Y > num30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num33;
                if (num33 > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num33 * 2f;
                }
            }

            if ((double)Projectile.velocity.X > 0) Projectile.direction = -1;
            else if ((double)Projectile.velocity.X <= 0) Projectile.direction = 1;

            if (Math.Abs(Projectile.velocity.X) < 0.15f) Projectile.velocity.X *= 1.05f;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }
    }
}
