using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Particles
{
    [Autoload(Side = ModSide.Client)]
    public class ParticleSystem : ModSystem
    {
        internal static readonly List<Particle> particles = new();
        internal static readonly Dictionary<int, Particle> particleInstances = new();

        // ...

        public override void Unload()
        {
            ClearParticles();

            particleInstances.Clear();
        }

        public override void OnWorldUnload()
        {
            ClearParticles();
        }

        public override void PostUpdateEverything()
        {
            foreach (var particle in particles.ToArray())
            {
                particle.Update();
            }
        }

        // ...

        public static void ClearParticles(GameTime _ = null)
        {
            foreach (var particle in particles.ToArray())
            {
                particle.Kill();
            }

            particles.Clear();
        }

        public static int ParticleType<T>() where T : Particle
        {
            T t = ModContent.GetInstance<T>();
            if (t == null) return -1;
            return t.Type;
        }

        public static Particle GetParticleInstance(int type)
        {
            if (!particleInstances.ContainsKey(type)) return null;
            return particleInstances[type];
        }
    }
}