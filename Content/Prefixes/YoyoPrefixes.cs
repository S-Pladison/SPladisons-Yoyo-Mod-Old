using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Prefixes
{
    public class YoyoPrefixes : ILoadable
    {
        private static List<byte> _prefixes;

        public static IReadOnlyList<byte> GetPrefixes() => _prefixes;

        public void Load(Mod mod)
        {
            _prefixes = new List<byte>();

            this.AddPrefix(mod, name: "Test1", eng: "Test #1", rus: "Тестовый #1", lifeTime: 0f, maxRange: 0.1f, topSpeed: 0f);
            this.AddPrefix(mod, name: "Test2", eng: "Test #2", rus: "Тестовый #2", lifeTime: 0.1f, maxRange: 0f, topSpeed: 0f);
        }

        public void Unload()
        {
            _prefixes?.Clear();
            _prefixes = null;
        }

        private void AddPrefix(Mod mod, string name, string eng, string rus, float lifeTime, float maxRange, float topSpeed)
        {
            mod.AddContent(new YoyoPrefix(name, eng, rus, lifeTime, maxRange, topSpeed));
            if (mod.TryFind(name, out ModPrefix prefix)) _prefixes.Add(prefix.Type);
        }
    }

    [Autoload(false)]
    public class YoyoPrefix : ModPrefix
    {
        private readonly string name;
        private readonly string engName;
        private readonly string rusName;

        public readonly float lifeTimeMult;
        public readonly float maxRangeMult;
        public readonly float topSpeedMult;

        public override string Name => name;

        public YoyoPrefix() { }
        public YoyoPrefix(string name, string eng, string rus, float lifeTime, float maxRange, float topSpeed)
        {
            this.name = name;
            this.engName = eng;
            this.rusName = rus;

            this.lifeTimeMult = lifeTime;
            this.maxRangeMult = maxRange;
            this.topSpeedMult = topSpeed;
        }

        public override void SetDefaults()
        {
            DisplayName.SetDefault(engName);
            if (rusName != string.Empty) DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rusName);
        }

        public override bool CanRoll(Item item) => item.IsYoyo();
        public override PrefixCategory Category => PrefixCategory.Custom;

        public override void Apply(Item item)
        {
            var yoyo = item.GetYoyoGlobalItem();

            if (ProjectileID.Sets.YoyosLifeTimeMultiplier[item.shoot] != -1f) yoyo.lifeTimeMult += lifeTimeMult;

            yoyo.maxRangeMult += maxRangeMult;
            yoyo.topSpeedMult += topSpeedMult;
        }

        public override void SetStats(ref float damageMult, ref float knockbackMult, ref float useTimeMult, ref float scaleMult, ref float shootSpeedMult, ref float manaMult, ref int critBonus)
        {
            //damageMult += 100;
        }

        public override void ModifyValue(ref float valueMult)
        {
            //valueMult *= (1 + _fishingPower / 100f + _hookTime / 115f + _baitConsume / 115f + _crateChance / 115f);
        }
    }
}
