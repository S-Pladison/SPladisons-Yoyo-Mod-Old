using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Common
{
    public class ScreenShakeSystem : ModSystem
    {
        private static readonly List<ScreenShake> _screenShakes = new();

        public override void ModifyScreenPosition()
        {
            if (Main.gameMenu || Main.gamePaused) return;

            float power = 0;
            Vector2 screenCenter = new(Main.screenPosition.X + Main.screenWidth * 0.5f, Main.screenPosition.Y + Main.screenHeight * 0.5f);

            _screenShakes.ForEach((elem) =>
            {
                float progress = (float)elem.timeLeft / (float)elem.Time;
                float currentPowerMult = 1 - Math.Min(Vector2.Distance(screenCenter, elem.Position) / elem.Range, 1);

                power += elem.Power * currentPowerMult * progress;
            });

            Main.screenPosition += new Vector2(Main.rand.NextFloat(-power, power), Main.rand.NextFloat(-power, power)).Floor();
        }

        public override void PostUpdateEverything()
        {
            if (Main.gamePaused) return;

            for (int i = 0; i < _screenShakes.Count; i++)
            {
                var screenShake = _screenShakes[i];
                screenShake.timeLeft--;

                if (screenShake.timeLeft <= 0)
                {
                    _screenShakes.RemoveAt(i);
                }
            }
        }

        public static void NewScreenShake(Vector2 position, float power, float range, int time)
        {
            _screenShakes.Add(new ScreenShake(position, power, range, time));
        }

        // ...

        private class ScreenShake
        {
            public readonly Vector2 Position;
            public readonly float Range;
            public readonly float Power;
            public readonly int Time;

            public int timeLeft;

            public ScreenShake(Vector2 position, float power, float range, int time)
            {
                Position = position;
                Power = power;
                Range = range;

                this.timeLeft = Time = time;
            }
        }
    }
}
