using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class PrototypeF34 : YoyoItem
    {
        public PrototypeF34() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<PrototypeF34Projectile>();

            Item.rare = ItemRarityID.Green;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class PrototypeF34Projectile : YoyoProjectile
    {
        public PrototypeF34Projectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public static Effect Effect { get; private set; }

        public override void YoyoSetStaticDefaults()
        {
            if (Main.dedServ) return;

            Effect = ModAssets.GetEffect("PrototypeF34", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void YoyoSetDefaults()
        {
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Отрисовываем йо-йо
            // Далее поверх него типа свет. маску с шейдером,
            // в который передаются две текстуры Main.instance.tilemap и еще одна
            // Вродь как можно матрицу для отельной текстуры перекинуть прям в шейдер,
            // поэтому мы не рендерим текстуру, и пихаем ее, а юзаем с матрицами сразу эти две

            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;

            Vector2 vector = new Vector2((float)Main.offScreenRange, (float)Main.offScreenRange);
            if (Main.drawToScreen)
            {
                vector = Vector2.Zero;
            }

            //Main.spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition + new Vector2(0, -16 * 10), Color.White);

            DrawUtils.BeginProjectileSpriteBatch(sortMode: SpriteSortMode.Immediate, blendState: BlendState.AlphaBlend, effect: Effect);
            {
                var texture = ModAssets.GetExtraTexture(1);
                var scale = Projectile.scale * 3f;
                var tileTarget = Main.instance.tileTarget;
                var sizeDiff = tileTarget.Size() / (texture.Size() * scale);
                var tileMatrix = Matrix.CreateScale(sizeDiff.X, sizeDiff.Y, 1f);

                Effect.Parameters["TileTarget"].SetValue(tileTarget);
                Effect.Parameters["TileMatrix"].SetValue(tileMatrix);

                Main.EntitySpriteDraw(texture.Value, Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition, null, Color.White, 0f, texture.Size() * 0.5f, scale, SpriteEffects.None, 0);
            }
            DrawUtils.BeginProjectileSpriteBatch();

            return true;
        }
    }
}