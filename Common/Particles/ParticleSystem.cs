using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Particles
{
    public class ParticleSystem : ModSystem
    {
        public static ParticleSystem Instance { get => ModContent.GetInstance<ParticleSystem>(); }
        public static readonly int MaxParticles = 2000;

        internal static List<Particle> particles = new List<Particle>();
        internal static IDictionary<int, Particle> particleInstances = new Dictionary<int, Particle>();

        public override void Load()
        {

        }

        public override void Unload()
        {
            particleInstances.Clear();
            particleInstances = null;

            particles.Clear();
            particles = null;
        }

        public override void PostUpdateEverything()
        {
            foreach (var particle in particles.ToArray())
            {
                particle.Update();
            }
        }

        public override void OnWorldUnload()
        {
            particles.Clear();
        }

        // ...

        public static int ActiveParticles => particles.Count();

        public static Particle GetParticleInstance(int type)
        {
            if (!particleInstances.ContainsKey(type)) return null;
            return particleInstances[type];
        }

        public static int ParticleType<T>() where T : Particle
        {
            T t = ModContent.GetInstance<T>();
            if (t == null) return -1;
            return t.Type;
        }

        public static void DrawParticles()
        {
            foreach (var particle in particles)
            {
                particle.Draw();
            }
        }

        internal static void AddParticle(Particle particle)
        {
            if (!particles.Contains(particle))
            {
                particles.Add(particle);
            }
        }

        internal static void RemoveParticle(Particle particle)
        {
            particles.Remove(particle);
        }
    }
}