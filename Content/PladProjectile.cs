using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using System.Collections.Generic;
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

        public virtual void OnSpawn() { }

        // ...

        public static void SetSpriteBatch(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, Effect effect = null, bool end = true)
        {
            if (end) Main.spriteBatch.End();
            Main.spriteBatch.Begin(sortMode, blendState ?? BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
