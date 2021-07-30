using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using SPladisonsYoyoMod.Test;
using Terraria.UI;
using SPladisonsYoyoMod.Common.Misc;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SPladisonsYoyoMod
{
    public class SPladisonsYoyoMod : Mod
    {
        public static SPladisonsYoyoMod Instance { get; private set; }
        public static Primitives Primitives { get; private set; }
        public static Asset<Texture2D>[] ExtraTextures { get; private set; }
        public static IReadOnlyList<int> GetYoyos { get { return _yoyos; } }

        public TesterUI testerUI;
        public UserInterface testerUserInterface;

        public SPladisonsYoyoMod() => Instance = this;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Primitives = new Primitives();

                testerUI = new TesterUI();
                testerUI.Activate();
                testerUserInterface = new UserInterface();
                testerUserInterface.SetState(testerUI);
            }

            ExtraTextures = new Asset<Texture2D>[21];
            for (int i = 0; i < ExtraTextures.Length; i++) ExtraTextures[i] = this.Assets.Request<Texture2D>("Assets/Textures/Misc/Extra_" + i);

            _yoyos = new List<int>();

            Main.OnPreDraw += DrawTargets;

            var _effect = Assets.Request<Effect>("Assets/Effects/Primitive");
            if (_effect == null) this.Logger.Info(":(");
            if (_effect.Value == null) this.Logger.Info(":((");


            //_effect.Value.Parameters["texture0"].SetValue(SPladisonsYoyoMod.ExtraTextures[6].Value);
        }

        public override void Unload()
        {
            Main.OnPreDraw -= DrawTargets;

            _yoyos = null;

            ExtraTextures = null;
            Primitives = null;
            Instance = null;
        }

        public override void PostSetupContent()
        {
            static void TryAddYoyo(int type)
            {
                Item item = new Item();
                item.SetDefaults(type, true);

                if (ItemID.Sets.Yoyo[item.type])
                {
                    _yoyos.Add(type);
                    return;
                }

                if (item.shoot <= ProjectileID.None) return;

                var proj = new Projectile();
                proj.SetDefaults(item.shoot);

                if (proj.IsYoyo()) _yoyos.Add(type);
            }

            for (int i = 0; i < ItemLoader.ItemCount; i++) TryAddYoyo(i);
        }

        private static void DrawTargets(GameTime gameTime)
        {
            SoulFilledFlameEffect.Instance?.Render(Main.spriteBatch);
        }

        private static List<int> _yoyos;
    }
}