using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common.Primitives.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common.Primitives
{
    public class PrimitiveSystem : ModSystem
    {
        public static PrimitiveSystem Instance { get => ModContent.GetInstance<PrimitiveSystem>(); }

        private List<PrimitiveTrail> trails = new();
        private Matrix transformMatrix;

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            transformMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
            transformMatrix *= Main.GameViewMatrix.EffectMatrix;
            transformMatrix *= Matrix.CreateTranslation(Main.screenWidth / 2, -Main.screenHeight / 2, 0);
            transformMatrix *= Matrix.CreateRotationZ(MathHelper.Pi);
            transformMatrix *= Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1);
            transformMatrix *= Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 1000);
        }

        public override void PostUpdateEverything()
        {
            foreach (var trail in trails.ToArray())
            {
                trail.Update();
            }
        }

        public void DrawPrimitives(PrimitiveDrawLayer drawLayer)
        {
            foreach (var trail in trails.FindAll(i => i.PrimitiveDrawLayer == drawLayer))
            {
                trail.DrawPrimitives(Main.spriteBatch, transformMatrix);
            }

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var proj = Main.projectile[i];
                if (!proj.active || proj.ModProjectile is not IDrawPrimitives modProj || modProj.PrimitiveDrawLayer != drawLayer) continue;

                modProj.DrawPrimitives(Main.spriteBatch, transformMatrix);
            }
        }

        // ...

        internal void AddTrail(PrimitiveTrail trail)
        {
            if (!trails.Contains(trail))
            {
                trails.Add(trail);
            }
        }

        internal void RemoveTrail(PrimitiveTrail trail)
        {
            trails.Remove(trail);
        }
    }
}