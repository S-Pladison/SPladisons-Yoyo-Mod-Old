using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Prefixes
{
    internal class ModPrefixLoader : ILoadable
    {
        public static IReadOnlyList<byte> GetYoyoPrefixes() => _yoyoPrefixes;

        public void Load(Mod mod)
        {
            _yoyoPrefixes = new List<byte>();

            AddPrefix(mod, name: "Test1", eng: "Test #1", rus: "Тестовый #1", lifeTime: 0f, maxRange: 0.1f, topSpeed: 0f);
            AddPrefix(mod, name: "Test2", eng: "Test #2", rus: "Тестовый #2", lifeTime: 0.1f, maxRange: 0f, topSpeed: 0f);
        }

        public void Unload()
        {
            _yoyoPrefixes?.Clear();
            _yoyoPrefixes = null;
        }

        private static void AddPrefix(Mod mod, string name, string eng, string rus, float lifeTime, float maxRange, float topSpeed)
        {
            mod.AddContent(new YoyoPrefix(name, eng, rus, lifeTime, maxRange, topSpeed));
            if (mod.TryFind(name, out ModPrefix prefix)) _yoyoPrefixes.Add(prefix.Type);
        }

        private static List<byte> _yoyoPrefixes;
    }
}
