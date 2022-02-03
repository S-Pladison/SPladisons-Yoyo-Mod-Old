using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Content.Items.Weapons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class HookLoader
    {
        private static void On_Player_OnHit(On.Terraria.Player.orig_OnHit orig, Player player, float x, float y, Entity victim)
        {
            if (Main.myPlayer != player.whoAmI || player.HeldItem.type != ModContent.ItemType<TitaniumYoyo>() || victim is not NPC npc || npc.type == NPCID.TargetDummy)
            {
                orig(player, x, y, victim);
                return;
            }

            if (!player.onHitTitaniumStorm)
            {
                player.AddBuff(BuffID.TitaniumStorm, 600, true, false);

                if (player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] < 7)
                {
                    player.ownedProjectileCounts[ProjectileID.TitaniumStormShard]++;
                    Projectile.NewProjectile(player.GetProjectileSource_SetBonus(4), player.Center, Vector2.Zero, ProjectileID.TitaniumStormShard, 50, 15f, player.whoAmI, 0f, 0f);
                }

                orig(player, x, y, victim);
                return;
            }

            player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] -= 7;
            orig(player, x, y, victim);
            player.ownedProjectileCounts[ProjectileID.TitaniumStormShard] += 7;
        }
    }
}