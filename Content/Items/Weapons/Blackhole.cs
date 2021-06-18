using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class Blackhole : YoyoItem
    {
        public Blackhole() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Blackhole", rus: "Черная дыра");
            this.SetTooltip(eng: "...",
                            rus: "...");
        }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<BlackholeProjectile>();

            Item.rare = ItemRarityID.Cyan;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class BlackholeProjectile : YoyoProjectile
    {
        public static Effect Effect { get; private set; }

        public readonly float radius = 16 * 9;
        public float radiusProgress = 0;
        public float pulse = 0;

        public BlackholeProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void Load()
        {
            Effect = Mod.GetEffect("Assets/Effects/Blackhole").Value;
            Effect.Parameters["perlinTexture"].SetValue(ModContent.GetTexture("Terraria/Images/Misc/Perlin").Value);
        }
        public override void Unload() => Effect = null;

        public override void YoyoSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Blackhole", rus: "Черная дыра");
        }

        public override void AI()
        {
            this.UpdateRadius(flag: IsReturning);

            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC target = Main.npc[i];

                if (target == null || !target.active) continue;
                if (target.friendly || target.lifeMax <= 5 || target.boss || target.dontTakeDamage || target.immortal) continue;
                if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height)) continue;

                float numX = Projectile.position.X - target.position.X - (float)(target.width / 2);
                float numY = Projectile.position.Y - target.position.Y - (float)(target.height / 2);
                float distance = (float)Math.Sqrt((double)(numX * numX + numY * numY));

                if (distance < (radius * radiusProgress))
                {
                    distance = 2f / distance;
                    numX *= distance * 3;
                    numY *= distance * 3;

                    target.velocity.X = numX;
                    target.velocity.Y = numY;
                    target.netUpdate = true;
                }
            }

            Lighting.AddLight(Projectile.Center, new Vector3(171 / 255f, 97 / 255f, 255 / 255f) * 0.45f * radiusProgress);

            pulse = MathHelper.SmoothStep(-0.05f, 0.05f, (float)Math.Abs(Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f)));
        }

        public void UpdateRadius(bool flag)
        {
            radiusProgress += !flag ? 0.05f : -0.1f;

            if (radiusProgress > 1) radiusProgress = 1;
            if (radiusProgress < 0) radiusProgress = 0;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            var texture = ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_2").Value;

            Effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            Effect.Parameters["width"].SetValue(texture.Width / 4);

            this.SetSpriteBatch(spriteBatch, SpriteSortMode.Immediate, BlendState.Additive);

            Effect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(texture, drawPosition, null, new Color(95, 65, 255) * 0.75f, 0f, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), radiusProgress * 0.5f - pulse, SpriteEffects.None, 0);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            this.SetSpriteBatch(spriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Effect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(texture, drawPosition, null, new Color(35, 0, 100) * 0.5f, 0f, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), radiusProgress * 0.35f - pulse, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPosition, null, Color.Black, 0f, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), radiusProgress * 0.2f - pulse, SpriteEffects.None, 0);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            this.SetSpriteBatch(spriteBatch);

            this.PostDraw(spriteBatch, lightColor); // Временно
            return false; // Временно false
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            this.SetSpriteBatch(spriteBatch, SpriteSortMode.Deferred, BlendState.Additive);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            var texture = ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_3").Value;
            spriteBatch.Draw(texture, drawPosition, null, Color.White, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.5f), new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), radiusProgress - pulse * 5f, SpriteEffects.None, 0);

            texture = ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_4").Value;
            spriteBatch.Draw(texture, drawPosition, null, Color.White, Projectile.rotation * 0.33f, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), radiusProgress * 0.35f, SpriteEffects.None, 0);

            this.SetSpriteBatch(spriteBatch);

            texture = ModContent.GetTexture("SPladisonsYoyoMod/Assets/Textures/Misc/Extra_5").Value;
            spriteBatch.Draw(texture, drawPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, texture.Height * 0.5f), Projectile.scale * radiusProgress, SpriteEffects.None, 0);
        }
    }
}
