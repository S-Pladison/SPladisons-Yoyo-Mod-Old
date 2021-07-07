using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Content.Trails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class OneSecondBefore : YoyoItem
    {
        public OneSecondBefore() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "One Second Before", rus: "За секунду до");
            this.SetTooltip(eng: "...",
                            rus: "...");
        }

        public override void YoyoSetDefaults()
        {
            Item.damage = 99;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<OneSecondBeforeProjectile>();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class OneSecondBeforeProjectile : YoyoProjectile
    {
        public OneSecondBeforeProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "One Second Before", rus: "За секунду до");
        }

        public override void OnSpawn()
        {
            NonameTrail trail = new(length: 16 * 6);
            SPladisonsYoyoMod.Primitives.CreateTrail(target: Projectile, trail: trail);
        }

        public override void AI()
        {
            if (Projectile.velocity.Length() >= 1f && Main.rand.Next((int)Projectile.velocity.Length()) > 1)
            {
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center - Vector2.One * 10.5f, 21, 21, ModContent.DustType<Dusts.NonameDust>())];
                dust.velocity = -Vector2.Normalize(Projectile.velocity);
            }
        }

        public override bool PreDrawExtras()
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            var texture = ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_5").Value;
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, 1.3f, SpriteEffects.None, 0);

            return true;
        }
    }
}
