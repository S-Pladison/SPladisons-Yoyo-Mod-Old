using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Globals
{
    public class PladGlobalProjectile : GlobalProjectile
    {
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            switch (projectile.type)
            {
                case ProjectileID.TitaniumStormShard:
                    {
                        if (projectile.ai[1] == -13f)
                        {
                            damage = (int)(damage * 1.3f);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void OnSpawn(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.TitaniumStormShard:
                    {
                        var owner = Main.player[projectile.owner];
                        if (owner.HeldItem.type == ModContent.ItemType<TitaniumYoyo>() && owner.onHitTitaniumStorm)
                        {
                            projectile.ai[1] = -13f;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
