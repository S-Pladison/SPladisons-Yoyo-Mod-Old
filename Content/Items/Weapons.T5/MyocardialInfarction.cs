using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Graphics;
using SPladisonsYoyoMod.Common.Graphics.Primitives;
using SPladisonsYoyoMod.Common.Particles;
using SPladisonsYoyoMod.Content.Particles;
using SPladisonsYoyoMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Content.Items.Weapons
{
    public class MyocardialInfarction : YoyoItem
    {
        public MyocardialInfarction() : base(gamepadExtraRange: 15) { }

        public override void YoyoSetDefaults()
        {
            Item.damage = 43;
            Item.knockBack = 2.5f;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<MyocardialInfarctionProjectile>();

            Item.rare = ItemRarityID.Lime;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 1, silver: 50, copper: 0);
        }
    }

    public class MyocardialInfarctionProjectile : YoyoProjectile, IDrawOnDifferentLayers
    {
        public static readonly Color[] Colors = new Color[] { new(255, 0, 35), new(255, 85, 165), new(255, 40, 0) };

        private const float EFFECT_RADIUS = 16 * 15;
        private const float EFFECT_DRAW_RADIUS = EFFECT_RADIUS + 16 * 2;

        private PrimitiveStrip[] trails;
        private IPrimitiveEffect effect;

        // ...

        public MyocardialInfarctionProjectile() : base(lifeTime: -1f, maxRange: 300f, topSpeed: 13f) { }

        public override bool IsSoloYoyo() => true;

        public override void OnSpawn(IEntitySource source)
        {
            effect = new IPrimitiveEffect.Default(ModAssets.GetExtraTexture(11), true);
            trails = new PrimitiveStrip[2];
            trails[0] = new(p => 8 * (1 - p * 0.75f), p => Colors[0] * (1 - p * p), effect);
            trails[1] = new(p => 5 * (1 - p * 0.9f), p => Colors[1] * (1 - p * p), effect);
        }

        public override void AI()
        {
            var owner = Main.player[Projectile.owner];

            Projectile.localAI[1]++;
            Projectile.rotation -= 0.2f;

            foreach (var target in GetTargets(EFFECT_RADIUS))
            {
                var npc = target.npc;

                if (Main.rand.NextBool(13))
                {
                    var vector = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                    var position = npc.Center + vector * Main.rand.NextFloat(7f, 18f);
                    var velocity = vector * Main.rand.NextFloat(0.5f, 3f);
                    var rotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    var scale = Main.rand.NextFloat(0.85f, 1.25f);
                    var drawType = Main.rand.NextBool(2) ? DrawTypeFlags.Pixelated : DrawTypeFlags.All;
                    Particle.NewParticle<HeartParticle>(DrawLayers.Walls, drawType, position, velocity, Colors[Main.rand.Next(3)], Main.rand.Next(50), rotation, scale);
                }

                owner.lifeRegen++;
            }

            if (Projectile.localAI[1] % (8 - (int)Math.Min(Projectile.velocity.Length() * 0.5f, 7)) == 0)
            {
                var vector = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
                var position = Projectile.Center + vector * Main.rand.NextFloat(7f, 18f);
                var velocity = vector * Main.rand.NextFloat(0.5f, 3f);
                var rotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                var scale = Main.rand.NextFloat(0.85f, 1.25f);
                var drawType = Main.rand.NextBool(2) ? DrawTypeFlags.Pixelated : DrawTypeFlags.All;
                Particle.NewParticle<HeartParticle>(DrawLayers.Walls, drawType, position, velocity, Colors[Main.rand.Next(3)], Main.rand.Next(50), rotation, scale);
            }
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            foreach (var elem in GetTargets(EFFECT_RADIUS))
            {
                var npc = elem.npc;

                if (target.whoAmI == npc.whoAmI) continue;

                npc.StrikeNPC(damage / 2, knockback / 2, MathF.Sign((npc.Center - owner.Center).X), crit);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, drawPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.9f, SpriteEffects.None, 0);

            return false;
        }

        void IDrawOnDifferentLayers.DrawOnDifferentLayers(DrawSystem system)
        {
            // Trails

            var projectilePos = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY;
            var overTilesDrawKey = new DrawSystem.DrawKey(DrawLayers.Tiles, DrawTypeFlags.Pixelated);

            for (int i = 0; i < 2; i++)
            {
                var trail = trails[i];
                var position = projectilePos + new Vector2(6f, 0).RotatedBy(i * MathHelper.Pi + Projectile.rotation);

                trail.UpdatePointsAsSimpleTrail(position, 30, 16 * 25 * ReturningProgress);
                system.AddToLayer(overTilesDrawKey, trail);
            }

            // Targets

            Color color;
            var drawPosition = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModContent.Request<Texture2D>(ModAssets.ParticlesPath + "HeartParticle");
            var scale = Projectile.scale;
            var rect = new Rectangle(0, 0, 200, 200);
            var origin = new Vector2(100);

            foreach (var target in GetTargets(EFFECT_DRAW_RADIUS))
            {
                var progress = 1f - MathF.Pow(target.distance / EFFECT_DRAW_RADIUS, 2.5f);
                var npc = target.npc;
                var npcPos = npc.Center + npc.gfxOffY * Vector2.UnitY;
                var npcDrawPos = npcPos - Main.screenPosition;
                var sin = MathF.Sin(Main.GlobalTimeWrappedHourly * 2f + progress + npc.whoAmI);
                var normal = Vector2.Normalize(Projectile.Center - npc.Center).RotatedBy(MathHelper.PiOver2) * sin * 32f;
                var points = new List<Vector2>() { projectilePos, projectilePos + normal, npcPos - normal, npcPos };
                var strip = new PrimitiveStrip(TargetConnectionWidth, p => TargetConnectionColor(p) * progress, effect);
                strip.Points = BezierCurve.GetPoints(8, points);

                system.AddToLayer(overTilesDrawKey, strip);

                color = Colors[0] * progress;

                system.AddToLayer(overTilesDrawKey, new DefaultDrawData(texture.Value, npcDrawPos, rect, color * 0.33f, -sin, origin, scale * 0.25f, SpriteEffects.None));
                system.AddToLayer(DrawLayers.Dusts, DrawTypeFlags.Pixelated, new DefaultDrawData(texture.Value, npcDrawPos, rect, color, sin, origin, scale * 0.15f, SpriteEffects.None));
            }

            // Projectile effects

            var rotation = MathF.Sin(Main.GlobalTimeWrappedHourly * 3) * 0.8f;
            color = Colors[0] * ReturningProgress * 0.7f;

            system.AddToLayer(overTilesDrawKey, new DefaultDrawData(texture.Value, drawPosition, rect, color, rotation, origin, scale * 0.25f, SpriteEffects.None));
        }

        private List<(NPC npc, float distance)> GetTargets(float radius)
        {
            return NPCUtils.NearestNPCs
            (
                center: Projectile.Center,
                radius: radius * ReturningProgress,
                predicate: npc => npc.CanBeChasedBy(Projectile, false)
            ).Take(YoyoGloveActivated ? 6 : 4).ToList();
        }

        private float TargetConnectionWidth(float progress) => 8f * MathF.Pow((progress - 0.5f) * 2f, 2) + 2f;
        private Color TargetConnectionColor(float progress) => Colors[0] * MathF.Pow((progress - 0.5f) * 2f, 2) * ReturningProgress * 0.4f;
    }
}