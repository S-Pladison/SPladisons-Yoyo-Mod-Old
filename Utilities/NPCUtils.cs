using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace SPladisonsYoyoMod.Utilities
{
    public static class NPCUtils
    {
        public static (NPC npc, float distance) NearestNPC(Vector2 center, float? radius = null, Predicate<NPC> predicate = null)
        {
            var targets = NearestNPCs(center, radius, predicate);
            return targets.Any() ? targets.First() : (null, 0);
        }

        public static List<(NPC npc, float distance)> NearestNPCs(Vector2 center, float? radius = null, Predicate<NPC> predicate = null)
        {
            List<(NPC npc, float distance)> result = new();

            foreach (var npc in Main.npc)
            {
                if (!predicate?.Invoke(npc) ?? false) continue;

                var distance = Vector2.Distance(center, npc.Center);

                if (!radius.HasValue || distance <= radius)
                {
                    result.Add((npc, distance));
                }
            }

            result.Sort((x, y) => x.distance.CompareTo(y.distance));
            return result;
        }
    }
}
