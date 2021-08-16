using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class WakeOfEarth : YoyoItem
    {
        public WakeOfEarth() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<WakeOfEarthProjectile>();

            Item.rare = ItemRarityID.Green;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class WakeOfEarthProjectile : YoyoProjectile
    {
        private bool _hitTile = false;

        public WakeOfEarthProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void PostDraw(Color lightColor)
        {
            /*Point coord = (Projectile.Center / 16).ToPoint() + new Point(0, 1);
            Tile tile = Framing.GetTileSafely(coord);

            if (tile == null || !tile.IsActive || !Main.tileSolid[tile.type]) return;

            Main.spriteBatch.Draw(TextureAssets.Tile[tile.type].Value, GetDrawPosition(coord.ToVector2() * 16), new Rectangle(tile.frameX, tile.frameY, 16, 16), lightColor);*/
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!_hitTile)
            {
                var type = ModContent.ProjectileType<WakeOfEarthShakingTileProjectile>();
                var source = Projectile.GetProjectileSource_FromThis();

                Projectile.NewProjectile(source, Projectile.Center + new Vector2(16, 0), Vector2.Zero, type, 0, 0f, Projectile.owner, 5, 1);
                Projectile.NewProjectile(source, Projectile.Center - new Vector2(16, 0), Vector2.Zero, type, 0, 0f, Projectile.owner, 5, -1);

                _hitTile = true;
            }
            return base.OnTileCollide(oldVelocity);
        }
    }

    public class WakeOfEarthShakingTileProjectile : PladProjectile
    {
        public int SpawnCounter
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Misc/Extra_0";

        public override void SetDefaults()
        {
            Projectile.timeLeft = 25;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = true;

            Projectile.width = 16;
            Projectile.height = 16;
        }

        public override void AI()
        {
            if (Projectile.timeLeft != 18) return;
            if (SpawnCounter <= 0) return;

            var type = ModContent.ProjectileType<WakeOfEarthShakingTileProjectile>();
            var source = Projectile.GetProjectileSource_FromThis();

            Projectile.NewProjectile(source, Projectile.Center + new Vector2(16 * Projectile.ai[1], 0), Vector2.Zero, type, 0, 0f, Projectile.owner, --SpawnCounter, Projectile.ai[1]);
            ScreenShakeSystem.NewScreenShake(position: Projectile.Center, power: 0.5f, range: 16 * 20, time: 50);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Point coord = (Projectile.Center / 16).ToPoint() + new Point(0, 1);
            Tile tile = Framing.GetTileSafely(coord);

            if (tile == null || !tile.IsActive || !Main.tileSolid[tile.type]) return false;

            Vector2 position = GetDrawPosition(coord.ToVector2() * 16) - new Vector2(0, (float)Math.Sin((1 - Projectile.timeLeft / 25.0f) * MathHelper.Pi)) * 12f;
            Color color = lightColor * 0.7f;
            color.A = lightColor.A;

            Main.spriteBatch.Draw(TextureAssets.Tile[tile.type].Value, position, new Rectangle(tile.frameX, tile.frameY, 16, 16), color);

            return false;
        }
    }
}
