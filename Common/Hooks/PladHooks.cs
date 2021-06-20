using static Mono.Cecil.Cil.OpCodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Reflection;
using MonoMod.RuntimeDetour.HookGen;
using SPladisonsYoyoMod.Content.Items;

namespace SPladisonsYoyoMod.Common.Hooks
{
    public partial class PladHooks : ILoadable
    {
        public void Load(Mod mod)
        {
            IL.Terraria.Main.DrawProj += ModifyYoyoStringPosition;
            IL.Terraria.Player.Counterweight += ModifyCounterweight;

            On.Terraria.Projectile.AI_099_2 += (orig, projectile) =>
            {
                // Percentages will not work correctly if someone does exactly the same

                void SetYoyoData(float lt, float mr, float ts)
                {
                    ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type] = lt;
                    ProjectileID.Sets.YoyosMaximumRange[projectile.type] = mr;
                    ProjectileID.Sets.YoyosTopSpeed[projectile.type] = ts;
                }

                float oldLifeTime = ProjectileID.Sets.YoyosLifeTimeMultiplier[projectile.type];
                float oldMaxRange = ProjectileID.Sets.YoyosMaximumRange[projectile.type];
                float oldTopSpeed = ProjectileID.Sets.YoyosTopSpeed[projectile.type];

                float lifeTime = oldLifeTime, maxRange = oldMaxRange, topSpeed = oldTopSpeed;

                var globalProjectile = projectile.GetYoyoGlobalProjectile();
                globalProjectile.ModifyYoyo(projectile, ref lifeTime, ref maxRange, ref topSpeed);

                SetYoyoData(lifeTime, maxRange, topSpeed);
                orig(projectile);
                SetYoyoData(oldLifeTime, oldMaxRange, oldTopSpeed);
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

            // LoadThorium(ModLoader.GetMod("ThoriumMod"));
        }

        public void Unload()
        {
            IL.Terraria.Main.DrawProj -= ModifyYoyoStringPosition;
            IL.Terraria.Player.Counterweight -= ModifyCounterweight;

            // UnloadThorium();
        }

        private void ModifyYoyoStringPosition(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // IL_012E: ldloc.1
            // IL_012F: ldfld     int32 Terraria.Projectile::aiStyle
            // IL_0134: ldc.i4.s  99
            // IL_0136: bne.un    IL_0634

            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdloc(1),
                i => i.MatchLdfld<Projectile>("aiStyle"),
                i => i.MatchLdcI4(99),
                i => i.MatchBneUn(out _))) return;

            // IL_013F: ldloca.s  'vector' // 14
            // IL_0141: ldflda    float32 [Microsoft.Xna.Framework]Microsoft.Xna.Framework.Vector2::Y
            // IL_0146: dup
            // IL_0147: ldind.r4
            // IL_0148: ldsfld    class Terraria.Player[] Terraria.Main::player
            // IL_014D: ldloc.1
            // IL_014E: ldfld     int32 Terraria.Projectile::owner
            // IL_0153: ldelem.ref
            // IL_0154: ldfld     float32 Terraria.Player::gfxOffY
            // IL_0159: add
            // IL_015A: stind.r4

            int vectorIndex = -1;
            if (!c.TryGotoNext(MoveType.Before,
                i => i.MatchLdloca(out vectorIndex),
                i => i.MatchLdflda<Vector2>("Y"),
                i => i.MatchDup(),
                i => i.MatchLdindR4(),
                i => i.MatchLdsfld<Main>("player"),
                i => i.MatchLdloc(1),
                i => i.MatchLdfld<Projectile>("owner"),
                i => i.MatchLdelemRef(),
                i => i.MatchLdfld<Player>("gfxOffY"),
                i => i.MatchAdd(),
                i => i.MatchStindR4())) return;

            c.Emit(Ldarg_1); // index
            c.EmitDelegate<Func<int, Vector2>>((i) =>
            {
                var player = Main.player[Main.projectile[i].owner];

                // I need to get a better handle on this
                if (!ModContent.GetInstance<PladConfig>().YoyoCustomUseStyle) return player.MountedCenter;
                else
                {
                    Vector2 offset = (player.gravDir >= 0 ? new Vector2(0, -8) : new Vector2(0, -4));
                    offset += new Vector2(-3, 0) * player.direction;
                    return player.MountedCenter + offset; // Нужно пофиксить ****** нити
                }
            });
            c.Emit(Stloc, vectorIndex);
        }

        private void ModifyCounterweight(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            // IL_0052: ldsfld    class Terraria.Projectile[] Terraria.Main::projectile
            // IL_0057: ldloc.3
            // IL_0058: ldelem.ref
            // IL_0059: ldfld int32 Terraria.Projectile::aiStyle
            // IL_005E: ldc.i4.s  99
            // IL_0060: bne.un.s IL_0068

            int index = -1;
            if (!c.TryGotoNext(MoveType.After,
                i => i.MatchLdsfld<Main>("projectile"),
                i => i.MatchLdloc(out index),
                i => i.MatchLdelemRef(),
                i => i.MatchLdfld<Projectile>("aiStyle"),
                i => i.MatchLdcI4(99),
                i => i.MatchBneUn(out _))) return;

            // IL_0068: ldloc.3
            // IL_0069: ldc.i4.1
            // IL_006A: add
            // IL_006B: stloc.3

            if (!c.TryGotoNext(MoveType.Before,
                i => i.MatchLdloc(index),
                i => i.MatchLdcI4(1),
                i => i.MatchAdd(),
                i => i.MatchStloc(out _))) return;

            c.Emit(Ldloc, index);
            c.EmitDelegate<Func<int, int>>((i) => (Main.projectile[i].ModProjectile is YoyoProjectile yoyo && yoyo.IsSoloYoyo()).ToInt());
            c.Emit(Ldloc, 1);
            c.Emit(Add);
            c.Emit(Stloc, 1);
        }
    }
}
