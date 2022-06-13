using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Drawing;
using SPladisonsYoyoMod.Common.Drawing.AdditionalDrawing;
using SPladisonsYoyoMod.Common.Drawing.Primitives;
using SPladisonsYoyoMod.Utilities;
using Terraria;
using Terraria.DataStructures;
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

        public override void HoldItem(Player player)
        {
            // Generator.GenerateStructure("Assets/Structures/Test", Main.MouseWorld.ToTileCoordinates16(), Mod);
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

    public class ResidualLightProjectile : YoyoProjectile, IPostUpdateCameraPosition
    {
        public static float TrailTextureWidth { get; private set; }

        // ...

        private PrimitiveStrip trail;
        private int timer;

        // ...

        public ResidualLightProjectile() : base(lifeTime: 12f, maxRange: 275f, topSpeed: 15f) { }

        public override string Texture => ModAssets.ProjectilesPath + nameof(ResidualLightProjectile);
        public override bool IsSoloYoyo() => true;

        public override void Load()
        {
            if (Main.dedServ) return;

            TrailTextureWidth = ModAssets.GetExtraTexture(36, AssetRequestMode.ImmediateLoad).Width();

            /*var texture = ModAssets.GetExtraTexture(11, AssetRequestMode.ImmediateLoad);
            TrailEffect = ModAssets.GetEffect("ResidualLightTrail", AssetRequestMode.ImmediateLoad).Value;
            TrailEffect.Parameters["Texture0"].SetValue(texture.Value);

            texture = ModAssets.GetExtraTexture(20, AssetRequestMode.ImmediateLoad);
            TrailEffect.Parameters["Texture1"].SetValue(texture.Value);
            TrailTextureWidth = texture.Width();

            texture = ModAssets.GetExtraTexture(7, AssetRequestMode.ImmediateLoad);
            TrailEffect.Parameters["Texture2"].SetValue(texture.Value);*/
        }

        public override void OnSpawn(IEntitySource source)
        {
            trail = new PrimitiveStrip(GetTrailWidth, GetTrailColor, ModAssets.GetEffect("ResidualLightTrail").Value);
            trail.OnUpdateEffectParameters += UpdateTrailEffect;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.2f * Projectile.scale);

            timer++;

            Projectile.rotation -= 0.2f;

            /*if (Projectile.velocity.Length() >= 1f && Main.rand.Next((int)(Projectile.velocity.Length() * 0.5f)) > 1)
            {
                var dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0, 0, 220, default, Main.rand.NextFloat(1f, 1.5f))];
                dust.noLight = true;
                dust.noGravity = true;
            }*/
        }

        public override void ModifyYoyoMaximumRange(ref float maxRange)
        {
            //if (!Main.dayTime) maxRange += 0.2f;
        }

        public override void YoyoOnHitNPC(Player owner, NPC target, int damage, float knockback, bool crit)
        {
            /*var projType = ModContent.ProjectileType<ResidualLightHitProjectile>();
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projType, 0, 0, Projectile.owner, 1f, 20);

            for (int i = 0; i < 7; i++)
            {
                var dust = Dust.NewDustPerfect(Projectile.Center, 205, Vector2.Normalize(-Projectile.velocity).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(2, 4), 220, default, Main.rand.NextFloat(0.2f, 0.5f));
                dust.noLight = true;
            }

            if (!this.YoyoGloveActivated) return;

            List<int> list = new();
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];

                if (npc == null || !npc.active) continue;
                if (npc.friendly || npc.lifeMax <= 5 || npc.dontTakeDamage || npc.immortal) continue;
                if (i == target.whoAmI) continue;

                float distance = Vector2.Distance(Projectile.Center, npc.Center);
                if (distance > _radius || distance < 16 * 2) continue;

                list.Add(i);
            }

            float min = Math.Min(list.Count, 3);
            for (int i = 0; i < min; i++)
            {
                var index = list[Main.rand.Next(list.Count)];
                list.Remove(index);

                var npc = Main.npc[index];
                var colorType = Main.rand.Next(ResidualLightHitProjectile.Colors.Length);
                var color = ResidualLightHitProjectile.Colors[colorType];

                var proj = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, projType, (int)(Projectile.damage * 0.3f), 0f, Projectile.owner, 0.6f, 15)];
                proj.localAI[0] = colorType;
                proj = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * Main.rand.NextFloat(15, 30), ModContent.ProjectileType<ResidualLightEffectProjectile>(), 0, 0f, Projectile.owner, npc.Center.X, npc.Center.Y)];

                /*PrimitiveTrail.Create(proj, t =>
                {
                    t.SetColor(new DefaultTrailColor(color: color));
                    t.SetTip(new TriangularTrailTip());
                    t.SetWidth(new DefaultTrailWidth(width: 10));
                    t.SetUpdate(new BoundedTrailUpdate(points: 10, length: 16 * 15));
                    t.SetEffectTexture(ModAssets.GetExtraTexture(9).Value);
                });
            }*/
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        private void UpdateTrailEffect(Effect effect)
        {
            effect.Parameters["Time"].SetValue(-timer * 0.05f);
            effect.Parameters["Width"].SetValue(trail.Points.TotalDistance() / TrailTextureWidth * 3f);
        }

        private float GetTrailWidth(float progress) => 25f * (1 - progress * 0.8f);
        private Color GetTrailColor(float progress) => new Color(230, 230, 230, 230) * (1 - progress) * ReturningProgress;

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            trail.UpdatePointsAsSimpleTrail(Projectile.Center, 20, 16 * 10);
            PrimitiveSystem.AddToDataCache(DrawLayers.OverTiles, DrawTypeFlags.None, trail);
        }
    }

    public class ResidualLightHitProjectile : ModProjectile, IPostUpdateCameraPosition
    {
        public static readonly Color[] Colors = new Color[] { new Color(252, 222, 252), new Color(202, 243, 248), new Color(155, 255, 225) };
        private Color _color;

        public override string Texture => ModAssets.InvisiblePath;

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

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            Projectile.rotation += Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.timeLeft = (int)Projectile.ai[1];

            _color = Colors[(int)Projectile.localAI[0]];

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
            Projectile.scale = MathUtils.MultipleLerp<float>(MathHelper.Lerp, 1 - Projectile.timeLeft / Projectile.ai[1], new float[] { 1f, 1.2f, 0.6f, 0f }) * Projectile.ai[0];

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.3f * Projectile.scale);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        void IPostUpdateCameraPosition.PostUpdateCameraPosition()
        {
            var position = Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition;
            var texture = ModAssets.GetExtraTexture(21);
            var scale = Projectile.scale;
            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.All, new(texture.Value, position, null, _color * Projectile.ai[0], Projectile.rotation, texture.Size() * 0.5f, scale * 0.6f, SpriteEffects.None, 0));

            texture = ModAssets.GetExtraTexture(23);
            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.All, new(texture.Value, position, null, _color * 0.25f * Projectile.scale * Projectile.ai[0], Projectile.rotation, texture.Size() * 0.5f, scale * 0.4f, SpriteEffects.None, 0));

            texture = ModAssets.GetExtraTexture(3);
            AdditionalDrawingSystem.AddToDataCache(DrawLayers.OverDusts, DrawTypeFlags.All, new(texture.Value, position, null, _color * Projectile.ai[0], Projectile.rotation, texture.Size() * 0.5f, scale * 1.3f, SpriteEffects.None, 0));
        }
    }

    public class ResidualLightEffectProjectile : ModProjectile
    {
        public Vector2 TargetPos { get => new(Projectile.ai[0], Projectile.ai[1]); }

        public override string Texture => ModAssets.InvisiblePath;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;

            Projectile.timeLeft = 5000;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Vector2 targer = this.TargetPos;
            Vector2 vectorTo = targer - Projectile.Center;
            float distance = vectorTo.Length();

            distance = 8f / distance;
            Projectile.velocity = vectorTo * distance;

            if (Projectile.Hitbox.Contains((int)targer.X, (int)targer.Y))
            {
                Projectile.Kill();
            }
        }
    }
}