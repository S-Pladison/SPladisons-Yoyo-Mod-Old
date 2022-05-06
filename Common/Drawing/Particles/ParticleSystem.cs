using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Drawing.Particles
{
    [Autoload(Side = ModSide.Client)]
    public class ParticleSystem : ModSystem
    {
        internal static readonly Dictionary<DrawKey, List<Particle>> particles = new();
        internal static readonly Dictionary<int, Particle> particleInstances = new();

        // ...

        public override void PostSetupContent()
        {
            foreach (var key in DrawingManager.GetAllPossibleKeys())
            {
                particles.Add(key, new List<Particle>());
            }

            DrawingManager.AddToEachExistingVariant(Any, Draw, 200);
        }

        public override void Unload()
        {
            ClearParticles();

            particles.Clear();
            particleInstances.Clear();
        }

        public override void OnWorldUnload()
        {
            ClearParticles();
        }

        public override void PostUpdateEverything()
        {
            foreach (var key in particles)
            {
                foreach (var particle in key.Value.ToArray())
                {
                    particle.Update();
                }
            }
        }

        // ...

        public static bool Any(DrawKey key) => particles[key].Any();

        public static void ClearParticles(GameTime _ = null)
        {
            foreach (var list in particles.Values)
            {
                foreach (var particle in list.ToArray())
                {
                    particle.Kill();
                }

                list.Clear();
            }
        }

        public static void Draw(SpriteBatch spriteBatch, DrawKey key)
        {
            foreach (var particle in particles[key])
            {
                particle.Draw(spriteBatch);
            }
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