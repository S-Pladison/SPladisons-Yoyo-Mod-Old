using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Content.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class ResidualLight : YoyoItem
    {
        public ResidualLight() : base(gamepadExtraRange: 13) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;

            Item.shoot = ModContent.ProjectileType<ResidualLightProjectile>();
            Item.autoReuse = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.2f * Item.scale);
        }

        public override void AddRecipes()
        {
            var recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Valor);
            recipe.AddIngredient(ItemID.LightShard, 2);
            recipe.AddIngredient(ItemID.SoulofLight, 16);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    public class ResidualLightProjectile : YoyoProjectile
    {
        public static Asset<Effect> ResidualLightEffect { get; private set; }
        private static readonly float _radius = 16 * 12;

        // ...

        public ResidualLightProjectile() : base(lifeTime: 12f, maxRange: 275f, topSpeed: 15f) { }

        public override bool IsSoloYoyo() => true;
        public override void Unload() => ResidualLightEffect = null;

        public override void YoyoSetStaticDefaults()
        {
            if (Main.dedServ) return;

            ResidualLightEffect = ModContent.Request<Effect>("SPladisonsYoyoMod/Assets/Effects/ResidualLight", AssetRequestMode.ImmediateLoad);
            ResidualLightEffect.Value.Parameters["texture1"].SetValue(SPladisonsYoyoMod.GetExtraTextures[20].Value);
        }

        public override void OnSpawn()
        {
            SimpleTrail trail = new RoundedTrail
            (
                target: Projectile,
                length: 16 * 3,
                width: (p) => 25 * Math.Min(Vector2.Distance(Projectile.position, Projectile.oldPosition) * 0.075f, 1) * (1 - p * 0.5f),
                color: (p) => new Color(255, 115, 250) * (1 - p),
                additive: true,
                smoothness: 20
            );
            trail.SetEffectTexture(SPladisonsYoyoMod.GetExtraTextures[11].Value);
            trail.SetDissolveSpeed(0.35f);
            trail.SetMaxPoints(10);
            PrimitiveTrailSystem.NewTrail(trail);

            trail = new TriangularTrail
            (
                target: Projectile,
                length: 16 * 10,
                width: (p) => 20,
                color: (p) => new Color(230, 230, 230, 230) * (1 - p),
                effect: ResidualLightEffect,
                additive: true,
                tipLength: 3
            );
            trail.SetDissolveSpeed(0.35f);
            trail.SetMaxPoints(20);
            PrimitiveTrailSystem.NewTrail(trail);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.2f * Projectile.scale);

            Projectile.rotation -= 0.2f;

            if (Projectile.velocity.Length() >= 1f && Main.rand.Next((int)(Projectile.velocity.Length() * 0.5f)) > 1)
            {
                var dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0, 0, 220, default, Main.rand.NextFloat(1f, 1.5f))];
                dust.noLight = true;
                dust.noGravity = true;
            }
        }

        public override void ModifyYoyoMaximumRange(ref float maxRange)
        {
            if (!Main.dayTime) maxRange += 0.2f;
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            var projType = ModContent.ProjectileType<ResidualLightHitProjectile>();
            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.Zero, projType, 0, 0, Projectile.owner, 1f, 20);

            for (int i = 0; i < 7; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, 205, Vector2.Normalize(-Projectile.velocity).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(2, 4), 220, default, Main.rand.NextFloat(0.2f, 0.5f));
                dust.noLight = true;
            }

            if (!this.YoyoGloveActivated) return;

            List<int> list = new List<int>();
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc == null || !npc.active) continue;
                if (npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage || npc.immortal) continue;
                if (i == target.whoAmI) continue;

                float distance = Vector2.Distance(Projectile.Center, npc.Center);
                if (distance > _radius || distance < 16 * 3) continue;

                list.Add(i);
            }

            float min = Math.Min(list.Count, 3);
            for (int i = 0; i < min; i++)
            {
                var index = list[Main.rand.Next(list.Count)];
                list.Remove(index);

                var npc = Main.npc[index];
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), npc.Center, Vector2.Zero, projType, (int)(Projectile.damage * 0.3f), 0f, Projectile.owner, 0.6f, 15);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    public class ResidualLightHitProjectile : PladProjectile
    {
        private readonly Color[] _colors = new Color[] { new Color(252, 222, 252), new Color(202, 243, 248), new Color(155, 255, 225) };
        private Color _color;

        public override string Texture => "SPladisonsYoyoMod/Assets/Textures/Misc/Extra_0";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;

            Projectile.friendly = true;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn()
        {
            Projectile.rotation += Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.timeLeft = (int)Projectile.ai[1];

            _color = _colors[Main.rand.Next(_colors.Length)];

            if (Projectile.ai[0] < 0.9f)
            {
                for (int i = 0; i < 7; i++)
                {
                    var dust = Dust.NewDustPerfect(Projectile.Center, 205, Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(0.5f, 2.5f), 220, default, Main.rand.NextFloat(0.25f, 0.5f));
                    dust.noLight = true;
                }
            }
        }

        public override void AI()
        {
            Projectile.friendly = Projectile.timeLeft == 10;

            Projectile.rotation += 0.05f;
            Projectile.scale = ModUtils.GradientValue<float>(MathHelper.Lerp, 1 - Projectile.timeLeft / Projectile.ai[1], new float[] { 1f, 1.2f, 0.6f, 0f }) * Projectile.ai[0];

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.3f * Projectile.scale);
        }

        public override void DrawAdditive()
        {
            var position = GetDrawPosition();
            var texture = SPladisonsYoyoMod.GetExtraTextures[21];
            Main.spriteBatch.Draw(texture.Value, position, null, _color * Projectile.ai[0], Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.6f, SpriteEffects.None, 0);

            texture = SPladisonsYoyoMod.GetExtraTextures[23];
            Main.spriteBatch.Draw(texture.Value, position, null, _color * 0.25f * Projectile.scale * Projectile.ai[0], Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.4f, SpriteEffects.None, 0);

            texture = SPladisonsYoyoMod.GetExtraTextures[3];
            Main.spriteBatch.Draw(texture.Value, position, null, _color * Projectile.ai[0], Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.3f, SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
