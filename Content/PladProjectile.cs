using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladProjectile : ModProjectile
    {
        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Projectiles/" + this.Name;

        public void SetDisplayName(string eng, string rus = "")
        {
            DisplayName.SetDefault(eng);
            if (rus != "")
            {
                DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            }
        }

        public void SetSpriteBatch(SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, Effect effect = null, bool end = true)
        {
            if (end) spriteBatch.End();
            spriteBatch.Begin(sortMode, blendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);
        }

        public virtual void OnSpawn() { }
    }
}
