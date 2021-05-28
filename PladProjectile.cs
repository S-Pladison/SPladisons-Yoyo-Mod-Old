using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod
{
    public abstract class PladProjectile : ModProjectile
    {
        private bool _spawn = true;

        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Projectiles/" + this.Name;

        public sealed override bool PreAI()
        {
            if (_spawn)
            {
                this.OnSpawn();
                _spawn = false;
            }
            return this.PladPreAI();
        }

        public void SetDisplayName(string eng, string rus = "")
        {
            DisplayName.SetDefault(eng);
            if (rus != "")
            {
                DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            }
        }

        public virtual bool PladPreAI() { return true; }
        public virtual void OnSpawn() { }
    }
}
