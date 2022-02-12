using Microsoft.Xna.Framework;
using Terraria;

namespace SPladisonsYoyoMod
{
    public static class ProjectileUtils
    {
        public static void MoveTowards(this Projectile projectile, Vector2 target, float resistance, float speed, float minSpeed = 0f)
        {
            var move = target - projectile.Center;
            var length = move.Length();

            if (length > speed) move *= speed / length;

            move = (projectile.velocity * resistance + move) / (resistance + 1f);
            move = Vector2.Normalize(move) * MathHelper.Max(move.Length(), minSpeed);
            length = move.Length();

            if (length > speed) move *= speed / length;
            projectile.velocity = move;
        }
    }
}