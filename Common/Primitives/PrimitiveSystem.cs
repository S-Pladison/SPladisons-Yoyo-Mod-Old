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

        public static void DrawPrimitives(PrimitiveDrawData drawData)
        {
            Instance.drawDatas.Add(drawData);
        }

        // ...

        private readonly List<PrimitiveDrawData> drawDatas = new();
        private readonly List<PrimitiveTrail> trails = new();
        private Matrix transformMatrix;

        public override void Load()
        {
            Main.OnPreDraw += PreDraw;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= PreDraw;
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            transformMatrix = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up);
            transformMatrix *= Main.GameViewMatrix.EffectMatrix;
            transformMatrix *= Matrix.CreateTranslation(Main.screenWidth / 2, -Main.screenHeight / 2, 0);
            transformMatrix *= Matrix.CreateRotationZ(MathHelper.Pi);
            transformMatrix *= Matrix.CreateScale(Main.GameViewMatrix.Zoom.X, Main.GameViewMatrix.Zoom.Y, 1);
            transformMatrix *= Matrix.CreateOrthographic(Main.screenWidth, Main.screenHeight, 0, 1000);
        }

        public override void OnWorldUnload()
        {
            foreach (var trail in trails.ToArray())
            {
                trail.Kill();
            }

            trails.Clear();
            drawDatas.Clear();
        }

        public void PreDraw(GameTime time)
        {
            foreach (var trail in trails.ToArray())
            {
                trail.Update();
                trail.Draw();
            }
        }

        public void DrawPrimitives(PrimitiveDrawLayer drawLayer)
        {
            foreach (var data in drawDatas.FindAll(i => i.Layer == drawLayer))
            {
                foreach (var param in data.Effect.Value.Parameters)
                {
                    switch (param.Name)
                    {
                        case "time":
                            param.SetValue(Main.GlobalTimeWrappedHourly);
                            break;
                        case "transformMatrix":
                            param.SetValue(transformMatrix);
                            break;
                        default:
                            break;
                    }
                }

                data.Draw(Main.spriteBatch);
                drawDatas.Remove(data);
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