using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Nightmare
{
    public class NightmareBed : BaseBedItem
    {
        public NightmareBed() : base(Item.sellPrice(0, 1), ItemRarityID.Master, ModContent.TileType<NightmareBedTile>(), AssetDirectory.NightmareItems) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.master = true;
        }
    }

    public class NightmareBedTile : ModTile
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSleptIn[Type] = true; // Facilitates calling ModifySleepingTargetInfo
            TileID.Sets.InteractibleByNPCs[Type] = true; // Town NPCs will palm their hand at this tile
            TileID.Sets.IsValidSpawnPoint[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation

            DustType = DustID.VilePowder;
            AdjTiles = new int[] { TileID.Beds };

            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); // this style already takes care of direction for us
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(2, 2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault(mapName);
            AddMapEntry(NightmarePlantera.nightmareRed, name);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
        {
            // 由于床具有特殊的智能交互功能，因此将左右两侧分成必要的2x2部分
            width = 5; // 默认为 TileObjectData.newTile 定义的宽度
            //frameWidth = 18 * 2 + 9;
            height = 3; // 默认为 TileObjectData.newTile 定义的高度
                        //extraY = 0; // 取决于如何设置 frameHeight 和 CoordinateHeights 和 CoordinatePaddingFix.Y
        }

        public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // 默认值与常规原版床匹配
            // 如果您的床不是典型的 4x2 瓷砖，您可能需要弄乱此处的信息
            Tile tile = Framing.GetTileSafely(i, j);

            info.TargetDirection = tile.TileFrameX >= 18 * 5 ? 1 : -1;

            info.AnchorTilePosition.X = i; // 我们的椅子只有1宽，所以没有什么特别的要求
            info.AnchorTilePosition.Y = j;
            int leftOrRight = tile.TileFrameX / 90;
            int whoAmI_X = tile.TileFrameX / 18;
            if (leftOrRight == 0)
            {
                if (whoAmI_X == 2)
                    info.AnchorTilePosition.X--;
                if (whoAmI_X == 3)
                    info.AnchorTilePosition.X -= 2;
                else if (whoAmI_X == 4)
                    info.AnchorTilePosition.X -= 3;
            }
            else
            {
                if (whoAmI_X == 5)
                    info.AnchorTilePosition.X += 3;
                if (whoAmI_X == 6)
                    info.AnchorTilePosition.X += 2;
            }

            int whoAmI_Y = tile.TileFrameY / 18;
            //if (whoAmI_Y == 1)
            //   info.AnchorTilePosition.Y -= 1;
            if (whoAmI_Y == 2)
                info.AnchorTilePosition.Y -= 2;

            info.VisualOffset.Y -= 16f; // 将玩家向下移动一个档次，因为床没有普通床高
                                        //info.VisualOffset.X += info.TargetDirection * 32f;
            Point faa = info.AnchorTilePosition;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int spawnX = (i - (tile.TileFrameX / 18)) + (tile.TileFrameX >= 72 ? 5 : 2);
            int spawnY = j + 2;

            if (tile.TileFrameY % 56 != 0)
            {
                spawnY--;
            }

            if (!IsHoveringOverABottomSideOfABed(i, j))
            { // 这假设您的床是 4x2 和 2x2 部分。否则你必须在这里编写自己的代码
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                {
                    player.GamepadEnableGrappleCooldown();
                    player.sleeping.StartSleeping(player, i, j);
                }
            }
            else
            {
                player.FindSpawn();

                if (player.SpawnX == spawnX && player.SpawnY == spawnY)
                {
                    player.RemoveSpawn();
                    Main.NewText(Language.GetTextValue("Game.SpawnPointRemoved"), byte.MaxValue, 240, 20);
                }
                else if (Player.CheckSpawn(spawnX, spawnY))
                {
                    player.ChangeSpawn(spawnX, spawnY);
                    Main.NewText(Language.GetTextValue("Game.SpawnPointSet"), byte.MaxValue, 240, 20);
                }
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!IsHoveringOverABottomSideOfABed(i, j))
            {
                if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
                { // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示
                    player.noThrow = 2;
                    player.cursorItemIconEnabled = true;
                    player.cursorItemIconID = ItemID.SleepingIcon;
                }
            }
            else
            {
                player.noThrow = 2;
                player.cursorItemIconEnabled = true;
                player.cursorItemIconID = ModContent.ItemType<NightmareBed>();
            }
        }

        public static bool IsHoveringOverABottomSideOfABed(int myX, int myY)
        {
            short frameX = Main.tile[myX, myY].TileFrameX;
            bool flag = frameX / 90 == 1;
            bool flag2 = frameX % 90 < 36;
            if (flag)
                flag2 = !flag2;

            return flag2;
        }

    }

}
