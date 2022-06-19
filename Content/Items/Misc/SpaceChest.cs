using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SPladisonsYoyoMod.Common.Drawing;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SPladisonsYoyoMod.Content.Items.Misc
{
    public class SpaceChest : ModItem
    {
        public override string Texture => ModAssets.ItemsPath + "SpaceChest";

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
            Item.placeStyle = 0;
        }
    }

    public class SpaceChestTile : ModTile
    {
        public override string Texture => ModAssets.TilesPath + nameof(SpaceChestTile);

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

            DustType = ModContent.DustType<Dusts.SpaceChestDust>();
            AdjTiles = new int[] { TileID.Containers };
            ChestDrop = ModContent.ItemType<SpaceChest>();

            var color = new Color(174, 129, 92);
            AddMapEntry(color, CreateMapEntryName(), MapChestName);
            AddMapEntry(color, CreateMapEntryName(Name + "_Locked"), MapChestName);

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

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = 1;

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            if (!NPC.downedPlantBoss) return false;

            dustType = DustType;
            AchievementsHelper.NotifyProgressionEvent(AchievementHelperID.Events.UnlockedBiomeChest);

            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ChestDrop);
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            var player = Main.LocalPlayer;
            var tile = Main.tile[i, j];
            var left = i;
            var top = j;

            Main.mouseRightRelease = false;

            if (tile.TileFrameX % 36 != 0) left--;
            if (tile.TileFrameY != 0) top--;

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
            var player = Main.LocalPlayer;
            var tile = Main.tile[i, j];
            var left = i;
            var top = j;

            if (tile.TileFrameX % 36 != 0) left--;
            if (tile.TileFrameY != 0) top--;

            int chest = Chest.FindChest(left, top);

            if (chest < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            }
            else
            {
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "Space Chest";

                if (player.cursorItemIconText == "Space Chest")
                {
                    player.cursorItemIconText = "";

                    if (Main.tile[left, top].TileFrameX / 36 == 1)
                    {
                        player.cursorItemIconID = ModContent.ItemType<SpaceKey>();
                    }
                    else
                    {
                        player.cursorItemIconID = ModContent.ItemType<SpaceChest>();
                    }
                }
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);

            var player = Main.LocalPlayer;

            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var tile = Main.tile[i, j];
            var player = Main.LocalPlayer;

            if (player != null && tile != null && (tile.TileFrameX == 18 || tile.TileFrameX == 18 * 3) && tile.TileFrameY == 18)
            {
                int chest = FindSpaceChest(i, j);

                if (chest > 0)
                {

                    var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

                    if (Main.drawToScreen) zero = Vector2.Zero;

                    var texture = ModContent.Request<Texture2D>(this.Texture + "_Effect").Value;
                    var position = new Vector2(i * 32 + player.Center.X * 0.2f, j * 32 + player.Center.Y * 0.2f) + zero;
                    var rectangle = new Rectangle((tile.TileFrameX / 18 - 1) * 16, Main.chest[chest].frame * 34, 32, 32);
                    var shader = GameShaders.Armor.GetShaderFromItemId(ItemID.TwilightDye);
                    var spriteBatchInfo = new SpriteBatchInfo(spriteBatch);

                    spriteBatch.End();
                    spriteBatchInfo.Begin(spriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend, null);
                    {
                        shader.Apply(null, new DrawData(texture, position, rectangle, Color.White));
                        position = new Vector2(i * 16 - 16 - (int)Main.screenPosition.X, j * 16 - 16 - (int)Main.screenPosition.Y) + zero;
                        spriteBatch.Draw(texture, position, rectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    spriteBatch.End();
                    spriteBatchInfo.Begin(spriteBatch);
                }
            }
        }

        private static int FindSpaceChest(int i, int j)
        {
            var tile = Main.tile[i, j];
            var left = i;
            var top = j;

            if (tile.TileFrameX % 36 != 0) left--;
            if (tile.TileFrameY != 0) top--;

            return Chest.FindChest(left, top);
        }

        public static string MapChestName(string name, int i, int j)
        {
            var left = i;
            var top = j;
            var tile = Main.tile[i, j];

            if (tile.TileFrameX % 36 != 0) left--;
            if (tile.TileFrameY != 0) top--;

            var chest = Chest.FindChest(left, top);

            if (chest < 0) return Language.GetTextValue("LegacyChestType.0");
            if (Main.chest[chest].name == "") return name;

            return name + ": " + Main.chest[chest].name;
        }
    }
}
