using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using SPladisonsYoyoMod.Common.Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SPladisonsYoyoMod.Commands
{
    class TestCommand : ModCommand
    {
        public override string Command => "test";

        public override CommandType Type => CommandType.Chat | CommandType.Server | CommandType.Console;

        public override string Description => "| Тестовая команда...";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!int.TryParse(args[0], out int type)) return;

            switch (type)
            {
                case 1:
                    if (PladSystem.FlamingFlowerPosition != Point.Zero) Main.LocalPlayer.Teleport(PladSystem.FlamingFlowerPosition.ToVector2() * 16);
                    break;
                case 2:
                    Main.LocalPlayer.QuickSpawnItem(ItemID.FrozenChest, 99);
                    break;
                case 3:
                    for (int i = 0; i < 20; i++) NPC.NewNPC((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y - 100, NPCID.Harpy);
                    break;
                case 4:
                    foreach (var elem in ModHook.GetHooks)
                    {
                        Main.NewText(elem.Key.GetType().Name + " - " + elem.Value);
                    }
                    break;
            }
        }
    }
}
