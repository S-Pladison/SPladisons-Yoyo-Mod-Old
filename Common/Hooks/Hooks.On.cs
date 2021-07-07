using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class Hooks
    {
        public static void LoadOn()
        {
            // Percentages will not work correctly if someone does exactly the same :(
            On.Terraria.Projectile.AI_099_2 += (orig, projectile) =>
            {
                void SetYoyoStats(float lt, float mr, float ts)
                {
                    ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = lt;
                    ProjectileID.Sets.YoyosMaximumRange[projectile.type] = mr;
                    ProjectileID.Sets.YoyosTopSpeed[projectile.type] = ts;
                }

                float oldLifeTime = ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type],
                      oldMaxRange = ProjectileID.Sets.YoyosMaximumRange[projectile.type],
                      oldTopSpeed = ProjectileID.Sets.YoyosTopSpeed[projectile.type];

                float lifeTime = oldLifeTime, maxRange = oldMaxRange, topSpeed = oldTopSpeed;

                YoyoGlobalProjectile.ModifyYoyo(projectile, ref lifeTime, ref maxRange, ref topSpeed);

                if (oldLifeTime == -1f) lifeTime = -1f;

                SetYoyoStats(lifeTime, maxRange, topSpeed);
                orig(projectile);
                SetYoyoStats(oldLifeTime, oldMaxRange, oldTopSpeed);
            };

            On.Terraria.Main.DrawMiscMapIcons += (On.Terraria.Main.orig_DrawMiscMapIcons orig, Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString) =>
            {
                Content.Items.Accessories.FlamingFlowerTile.DrawMapIcon(spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
                orig(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
            };

            On.Terraria.Projectile.NewProjectile_IProjectileSource_float_float_float_float_int_int_float_int_float_float += (orig, spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1) =>
            {
                var index = orig(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1);
                var proj = Main.projectile[index];

                if (proj.ModProjectile is Content.PladProjectile pladProj) pladProj.OnSpawn();

                return index;
            };

            On.Terraria.Main.DrawProjectiles += (orig, self) =>
            {
                SPladisonsYoyoMod.Primitives?.DrawTrails(Main.spriteBatch);
                orig(self);
            };
        }
    }
}
