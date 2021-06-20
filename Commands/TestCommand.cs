using Microsoft.Xna.Framework;
using SPladisonsYoyoMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
			if (PladSystem.flamingFlowerPosition != Point.Zero) Main.LocalPlayer.Teleport(PladSystem.flamingFlowerPosition.ToVector2() * 16);
		}
	}
}
