using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Terraria_Player_OnHit(On.Terraria.Player.orig_OnHit orig, Player player, float x, float y, Entity victim)
        {
            if (Main.myPlayer != player.whoAmI)
            {
                orig(player, x, y, victim);
                return;
            }

            bool titaniumYoyo = player.HeldItem.type == ModContent.ItemType<TitaniumYoyo>() && (victim is NPC npc) && npc.type != NPCID.TargetDummy;
            bool titaniumArmor = player.onHitTitaniumStorm;

            if (titaniumYoyo)
            {
                if (!titaniumArmor)
                {
                    player.AddBuff(BuffID.TitaniumStorm, 600, true, false);
                    if (player.ownedProjectileCounts[908] < 7)
                    {
                        player.ownedProjectileCounts[908]++;
                        Projectile.NewProjectile(player.GetProjectileSource_SetBonus(4), player.Center, Vector2.Zero, 908, 50, 15f, player.whoAmI, 0f, 0f);
                    }
                }
                else
                {
                    player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] -= 7;
                }
            }

            orig(player, x, y, victim);

            if (titaniumYoyo && titaniumArmor)
            {
                player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] += 7;
            }
        }
    }
}