using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class TitaniumYoyo : YoyoItem
    {
        public TitaniumYoyo() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 200;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<TitaniumYoyoProjectile>();

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TitaniumBar, 13).AddTile(TileID.MythrilAnvil).Register();
        }
    }

    public class TitaniumYoyoProjectile : YoyoProjectile
    {
        public TitaniumYoyoProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override void YoyoSetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
        }

        public override void AI()
        {
            Projectile.rotation -= 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type];
            var drawPos = GetDrawPosition();

            for (int k = 1; k < Projectile.oldPos.Length; k++)
            {
                var progress = ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                var drawPosTrail = Projectile.oldPos[k] - Main.screenPosition + Projectile.Size * 0.5f + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(texture.Value, drawPosTrail, null, lightColor * 0.3f * progress, Projectile.rotation + k * 0.35f, texture.Size() * 0.5f, Projectile.scale * (0.95f - (1 - progress) * 0.2f), SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture.Value, drawPos, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class TitaniumYoyoPlayer : ModPlayer
    {
        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (victim is not NPC npc || Player.HeldItem.type != ModContent.ItemType<TitaniumYoyo>() || npc.type == NPCID.TargetDummy) return;

            int count = Player.ownedProjectileCounts[ProjectileID.TitaniumStormShard];

            if (!Player.onHitTitaniumStorm)
            {
                Player.AddBuff(BuffID.TitaniumStorm, 600, true, false);

                if (count >= 7) return;
                SpawnNewProjectile(victim);
            }

            if (!count.IsBetween(7, 13)) return;
            SpawnNewProjectile(victim);
        }

        private void SpawnNewProjectile(Entity victim)
        {
            Player.ownedProjectileCounts[ProjectileID.TitaniumStormShard]++;
            Projectile.NewProjectile(Player.GetSource_OnHit(victim, "SetBonus_Titanium"), Player.Center, Vector2.Zero, ProjectileID.TitaniumStormShard, 50, 15f, Player.whoAmI, 0f, 0f);
        }
    }

    public class TitaniumYoyoGlobalProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.TitaniumStormShard;

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ai[1] == -13f)
            {
                damage = (int)(damage * 1.3f);
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            var owner = Main.player[projectile.owner];
            if (owner.HeldItem.type == ModContent.ItemType<TitaniumYoyo>() && owner.onHitTitaniumStorm)
            {
                projectile.ai[1] = -13f;
            }
        }
    }
}