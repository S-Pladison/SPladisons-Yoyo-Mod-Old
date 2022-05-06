using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
            Item.width = 32;
            Item.height = 28;

            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<WakeOfEarthProjectile>();

            Item.rare = ItemRarityID.Blue;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class WakeOfEarthProjectile : YoyoProjectile
    {
        public WakeOfEarthProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void AI()
        {
            Projectile.rotation += 0.2f;
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life > 0) return;

            var type = ModContent.ProjectileType<WakeOfEarthSpawnerProjectile>();
            var source = Projectile.GetSource_FromThis();
            var position = target.position + new Vector2(target.Hitbox.Width * 0.5f, -20);

            Projectile.NewProjectile(source, position + new Vector2(16, 0), Vector2.Zero, type, 0, 0f, Projectile.owner, 5);
            Projectile.NewProjectile(source, position - new Vector2(16, 0), Vector2.Zero, type, 0, 0f, Projectile.owner, -5);
        }
    }

    public class WakeOfEarthSpawnerProjectile : PladProjectile
    {
        public int SpawnCounter
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override string Texture => ModAssets.InvisiblePath;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 25;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.width = 16;
            Projectile.height = 16;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            Point point = (Projectile.Center / 16f).ToPoint();
            int maxY = point.Y + 10;

            while (point.Y < maxY &&
                Main.tile[point.X, point.Y] != null && !WorldGen.SolidTile2(point.X, point.Y) &&
                Main.tile[point.X - 1, point.Y] != null && !WorldGen.SolidTile2(point.X - 1, point.Y) &&
                Main.tile[point.X + 1, point.Y] != null && !WorldGen.SolidTile2(point.X + 1, point.Y)) point.Y++;

            if (point.Y <= maxY)
            {
                var position = (point + new Point(0, -1)).ToVector2() * 16 + new Vector2(8, 8);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, Vector2.Zero, ModContent.ProjectileType<WakeOfEarthShakingTileProjectile>(), 0, 0f, Projectile.owner);
            }

            if (SpawnCounter == 0) return;

            if (SpawnCounter > 0) SpawnCounter--;
            else SpawnCounter++;
        }

        public override void AI()
        {
            if (SpawnCounter == 0 || Projectile.timeLeft != 18) return;

            var type = ModContent.ProjectileType<WakeOfEarthSpawnerProjectile>();
            var source = Projectile.GetSource_FromThis();

            Projectile.NewProjectile(source, Projectile.Center + new Vector2(16 * (Math.Sign(SpawnCounter) >= 0 ? 1 : -1), 0), Vector2.Zero, type, 0, 0f, Projectile.owner, SpawnCounter);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class WakeOfEarthShakingTileProjectile : PladProjectile
    {
        public override string Texture => ModAssets.InvisiblePath;

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

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            //ScreenShakeSystem.NewScreenShake(position: Projectile.Center, power: 0.65f, range: 16 * 25, time: 50);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Point coord = (Projectile.Center / 16).ToPoint() + new Point(0, 1);
            Tile tile = Framing.GetTileSafely(coord);

            if (tile == null || !tile.HasTile || !Main.tileSolid[tile.TileType]) return false;

            Vector2 position = GetDrawPosition(coord.ToVector2() * 16) - new Vector2(0, (float)Math.Sin((1 - Projectile.timeLeft / 25.0f) * MathHelper.Pi)) * 12f;
            Color color = lightColor * 0.7f;
            color.A = lightColor.A;

            Main.spriteBatch.Draw(TextureAssets.Tile[tile.TileType].Value, position, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), color);

            return false;
        }
    }
}
