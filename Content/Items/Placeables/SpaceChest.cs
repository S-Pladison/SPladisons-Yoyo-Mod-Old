using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SPladisonsYoyoMod.Content.Items.Placeables
{
    public class SpaceChest : PladItem
    {
        public override void PladSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Space Chest", rus: "Космический сундук");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 0, silver: 5, copper: 0);

            Item.consumable = true;
            Item.createTile = ModContent.TileType<SpaceChestTile>();
            Item.placeStyle = 1;
        }
    }

    public class SpaceKey : PladItem
    {
        public override void PladSetStaticDefaults()
        {
            this.SetDisplayName(eng: "Space Key", rus: "Космический ключ");
            this.SetTooltip(eng: "Unlocks a Space Chest in the dungeon",
                            rus: "Открывает космический сундук в Темнице");
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 26;

            Item.rare = ItemRarityID.Yellow;
            Item.value = Terraria.Item.sellPrice(platinum: 0, gold: 2, silver: 0, copper: 0);
            Item.maxStack = 99;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var tooltip = tooltips.Find(i => i.mod == "Terraria" && i.Name.StartsWith("Tooltip"));

            if (tooltip != null && !NPC.downedPlantBoss) tooltip.text = Language.GetTextValue("LegacyTooltip.59");
        }
    }

    public class SpaceChestTile : PladTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileOreFinderPriority[Type] = 500;

            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.BasicChest[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            this.DustType = ModContent.DustType<Dusts.SpaceChestDust>();
            this.AdjTiles = new int[] { TileID.Containers };
            this.ChestDrop = ModContent.ItemType<SpaceChest>();

            this.ContainerName.SetDefault("Space Chest");
            this.ContainerName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Russian), "Космический сундук");

            Color mapColor = new Color(174, 129, 92);

            this.CreateMapEntry(color: mapColor, eng: "Space Chest", rus: "Космический сундук", nameFunc: MapChestName);
            this.CreateMapEntry(color: mapColor, key: this.Name + "_Locked", eng: "Locked Space Chest", rus: "Запертый космический сундук", nameFunc: MapChestName);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f * 0.3f;
            g = 0.17f * 0.3f;
            b = 0.23f * 0.3f;
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].frameX / 36);
        public override bool HasSmartInteract() => true;
        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].frameX / 36 == 1;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            if (!NPC.downedPlantBoss) return false;

            dustType = this.DustType;
            AchievementsHelper.NotifyProgressionEvent(AchievementHelperID.Events.UnlockedBiomeChest);
            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 32, 32, ChestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;

            if (tile.frameX % 36 != 0) left--;
            if (tile.frameY != 0) top--;
            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }
            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }
            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }

            bool isLocked = IsLockedChest(left, top);
            if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                if (isLocked)
                {
                    int key = ModContent.ItemType<SpaceKey>();
                    if (player.ConsumeItem(key) && Chest.Unlock(left, top))
                    {
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, left, top);
                        }
                    }
                }
                else
                {
                    int chest = Chest.FindChest(left, top);
                    if (chest >= 0)
                    {
                        Main.stackSplit = 600;
                        if (chest == player.chest)
                        {
                            player.chest = -1;
                            SoundEngine.PlaySound(SoundID.MenuClose);
                        }
                        else
                        {
                            player.chest = chest;
                            Main.playerInventory = true;
                            Main.recBigList = false;
                            player.chestX = left;
                            player.chestY = top;
                            SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        }
                        Recipe.FindRecipes();
                    }
                }
            }
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;

            if (tile.frameX % 36 != 0) left--;
            if (tile.frameY != 0) top--;

            int chest = Chest.FindChest(left, top);
            if (chest < 0) player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            else
            {
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "Space Chest";
                if (player.cursorItemIconText == "Space Chest")
                {
                    player.cursorItemIconID = ModContent.ItemType<SpaceChest>();
                    if (Main.tile[left, top].frameX / 36 == 1) player.cursorItemIconID = ModContent.ItemType<SpaceKey>();
                    player.cursorItemIconText = "";
                }
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);
            Player player = Main.LocalPlayer;

            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            var player = Main.LocalPlayer;

            if (player != null && tile != null && (tile.frameX == 18 || tile.frameX == 18 * 3) && tile.frameY == 18)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                int chest = FindSpaceChest(i, j);
                if (chest > 0)
                {
                    Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
                    if (Main.drawToScreen) zero = Vector2.Zero;

                    var texture = ModContent.Request<Texture2D>(this.Texture + "_Effect").Value;
                    var position = new Vector2(i * 32 + player.Center.X * 0.2f, j * 32 + player.Center.Y * 0.2f) + zero;
                    var rectangle = new Rectangle((tile.frameX / 18 - 1) * 16, Main.chest[chest].frame * 34, 32, 32);

                    var shader = GameShaders.Armor.GetShaderFromItemId(ItemID.TwilightDye);
                    shader.Apply(null, new DrawData(texture, position, rectangle, Color.White));

                    position = new Vector2(i * 16 - 16 - (int)Main.screenPosition.X, j * 16 - 16 - (int)Main.screenPosition.Y) + zero;
                    spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }

                spriteBatch.End();
                spriteBatch.Begin();
            }
        }

        private static int FindSpaceChest(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;

            if (tile.frameX % 36 != 0) left--;
            if (tile.frameY != 0) top--;

            return Chest.FindChest(left, top);
        }

        public static string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];

            if (tile.frameX % 36 != 0) left--;
            if (tile.frameY != 0) top--;

            int chest = Chest.FindChest(left, top);

            if (chest < 0) return Language.GetTextValue("LegacyChestType.0");
            if (Main.chest[chest].name == "") return name;

            return name + ": " + Main.chest[chest].name;
        }
    }
}
