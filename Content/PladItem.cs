using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content
{
    public abstract class PladItem : ModItem
    {
        public virtual int SacrificeCount => 1;

        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Items/" + this.Name;

        public sealed override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = SacrificeCount;

            this.PladSetStaticDefaults();
        }

        public void SetDisplayName(string eng, string rus = "")
        {
            DisplayName.SetDefault(eng);
            if (rus != "")
            {
                DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            }
        }

        public void SetTooltip(string eng, string rus = "")
        {
            Tooltip.SetDefault(eng);
            if (rus != "")
            {
                Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), rus);
            }
        }

        public virtual void PladSetStaticDefaults() { }
    }
}
