using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Core.Prefabs.Tiles
{
    public abstract class BaseTile : ModTile
    {
        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseTile(string texturePath, bool pathHasName = false)
        {
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public abstract class BaseToiletTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;
        public const int NextStyleHeight = 40;

        public BaseToiletTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.ToiletPrefab(dustType, mapColor);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // 避免能够从远距离触发它
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // 知道这是在玩家和NPC上调用的非常重要的，所以不要使用Main.LocalPlayer，例如使用info.restingEntity。
            Tile tile = Framing.GetTileSafely(i, j);

            //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
            //info.visualOffset = Vector2.Zero; // Defaults to (0,0)

            info.TargetDirection = -1;

            if (tile.TileFrameX != 0)
                info.TargetDirection = 1; // 如果坐在右侧备用位置上，则朝右（之前通过 SetStaticDefaults 中的 addAlternate 添加）

            // 锚点表示椅子最底部的磁贴。这用于对齐实体命中框
            // 由于 i 和 j 可能来自椅子的任何坐标，我们需要基于此调整锚点
            info.AnchorTilePosition.X = i; // 我们的椅子只有1宽，所以没有什么特别的要求
            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % NextStyleHeight == 0)
                info.AnchorTilePosition.Y++; // 在这里，由于我们的椅子只有 2 格高，我们可以检查瓷砖是否是最上面的一块，然后将其向下移动 1 块

            // 在这里，我们为这个物块添加了原版马桶没有的自定义趣味效果。这显示了如何将 restingEntity 类型转换为玩家并使用 visualOffset 。
            //if (info.RestingEntity is Player player && player.HasBuff(BuffID.Stinky))
            //    info.VisualOffset = Main.rand.NextVector2Circular(2, 2);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            { // 避免能够从远距离触发它
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示
            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
                return;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
                player.cursorItemIconReversed = true;
        }
    }

    public abstract class BaseBedTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseBedTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.BedPrefab(dustType, mapColor);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
        {
            // 由于床具有特殊的智能交互功能，因此将左右两侧分成必要的2x2部分
            width = 2; // 默认为 TileObjectData.newTile 定义的宽度
            height = 2; // 默认为 TileObjectData.newTile 定义的高度
                        //extraY = 0; // 取决于如何设置 frameHeight 和 CoordinateHeights 和 CoordinatePaddingFix.Y
        }

        public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // 默认值与常规原版床匹配
            // 如果您的床不是典型的 4x2 瓷砖，您可能需要弄乱此处的信息
            info.VisualOffset.Y += 4f; // 将玩家向下移动一个档次，因为床没有普通床高
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int spawnX = (i - (tile.TileFrameX / 18)) + (tile.TileFrameX >= 72 ? 5 : 2);
            int spawnY = j + 2;

            if (tile.TileFrameY % 38 != 0)
            {
                spawnY--;
            }

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
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

            if (!Player.IsHoveringOverABottomSideOfABed(i, j))
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
                player.cursorItemIconID = ModContent.ItemType<TItem>();
            }
        }

    }

    public abstract class BaseSofaTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;
        public const int NextStyleHeight = 40;

        public BaseSofaTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
            TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            DustType = dustType;
            AdjTiles = new int[] { TileID.Toilets }; // Condider adding TileID.Chairs to AdjTiles to mirror "(regular) Toilet" and "Golden Toilet" behavior for crafting stations

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(mapColor, name);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.addTile(Type);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // 避免能够从远距离触发它
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // 知道这是在玩家和NPC上调用的非常重要的，所以不要使用Main.LocalPlayer，例如使用info.restingEntity。
            Tile tile = Framing.GetTileSafely(i, j);

            //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
            //info.visualOffset = Vector2.Zero; // Defaults to (0,0)

            //info.TargetDirection = -1;

            //if (tile.TileFrameX != 0)
            //    info.TargetDirection = 1; // 如果坐在右侧备用位置上，则朝右（之前通过 SetStaticDefaults 中的 addAlternate 添加）

            // 锚点表示椅子最底部的磁贴。这用于对齐实体命中框
            // 由于 i 和 j 可能来自椅子的任何坐标，我们需要基于此调整锚点
            info.AnchorTilePosition.X = i; // 我们的椅子只有1宽，所以没有什么特别的要求
            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % NextStyleHeight == 0)
                info.AnchorTilePosition.Y++; // 在这里，由于我们的椅子只有 2 格高，我们可以检查瓷砖是否是最上面的一块，然后将其向下移动 1 块

            // 在这里，我们为这个物块添加了原版马桶没有的自定义趣味效果。这显示了如何将 restingEntity 类型转换为玩家并使用 visualOffset 。
            //if (info.RestingEntity is Player player && player.HasBuff(BuffID.Stinky))
            //    info.VisualOffset = Main.rand.NextVector2Circular(2, 2);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            { // 避免能够从远距离触发它
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示
            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
                return;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }
    }

    public abstract class BaseChairTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;
        public const int NextStyleHeight = 40;

        public BaseChairTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.ChairPrefab(dustType, mapColor);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // 避免能够从远距离触发它
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // 知道这是在玩家和NPC上调用的非常重要的，所以不要使用Main.LocalPlayer，例如使用info.restingEntity。
            Tile tile = Framing.GetTileSafely(i, j);

            //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
            //info.visualOffset = Vector2.Zero; // Defaults to (0,0)

            info.TargetDirection = -1;
            if (tile.TileFrameX != 0)
            {
                info.TargetDirection = 1; // 如果坐在右侧备用位置上，则朝右（之前通过 SetStaticDefaults 中的 addAlternate 添加）
            }

            // 锚点表示椅子最底部的磁贴。这用于对齐实体命中框
            // 由于 i 和 j 可能来自椅子的任何坐标，我们需要基于此调整锚点
            info.AnchorTilePosition.X = i; // 我们的椅子只有1宽，所以没有什么特别的要求
            info.AnchorTilePosition.Y = j;

            if (tile.TileFrameY % NextStyleHeight == 0)
            {
                info.AnchorTilePosition.Y++; // 在这里，由于我们的椅子只有 2 格高，我们可以检查瓷砖是否是最上面的一块，然后将其向下移动 1 块
            }
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            { // 避免能够从远距离触发它
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示
            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
                return;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }
    }

    public abstract class BasePianoTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BasePianoTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolidTop[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            DustType = dustType;
            AdjTiles = new int[] { TileID.Tables }; // Condider adding TileID.Chairs to AdjTiles to mirror "(regular) Toilet" and "Golden Toilet" behavior for crafting stations

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(mapColor, name);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.addTile(Type);
        }
    }

    public abstract class BaseTableTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseTableTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.TablePrefab(dustType, mapColor);
        }
    }

    public abstract class BasePlatformTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BasePlatformTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.Platforms[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);

            DustType = dustType;
            AdjTiles = new int[] { TileID.Platforms };

            TileObjectData.newTile.CoordinateHeights = new[] { 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 27;
            TileObjectData.newTile.StyleWrapLimit = 27;
            TileObjectData.newTile.UsesCustomCanPlace = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            AddMapEntry(mapColor);
        }

        public override void PostSetDefaults() => Main.tileNoSunLight[Type] = false;
    }

    public abstract class BaseBookcaseTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseBookcaseTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.BookcasePrefab(dustType, mapColor);
        }
    }

    public abstract class BaseChestTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;
        public const int NextStyleHeight = 40;

        public BaseChestTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.ChestPrefab(dustType, mapColor, MapChestName);
        }

        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            return CreateMapEntryName();
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override bool IsLockedChest(int i, int j) => false;

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public static string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            if (chest < 0)
                return Language.GetTextValue("LegacyChestType.0");

            if (Main.chest[chest].name == "")
                return name;

            return name + ": " + Main.chest[chest].name;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

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

            if (Main.netMode == NetmodeID.MultiplayerClient)
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
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        player.chest = chest;
                        Main.playerInventory = true;
                        Main.recBigList = false;
                        player.chestX = left;
                        player.chestY = top;
                    }

                    Recipe.FindRecipes();
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
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); // This gets the ContainerName text for the currently selected language

            if (chest < 0)
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            else
            {
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
                if (player.cursorItemIconText == defaultName)
                {
                    player.cursorItemIconID = ModContent.ItemType<TItem>();

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
    }

    public abstract class BaseBathtubTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseBathtubTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair); // Beds count as chairs for the purpose of suitable room creation

            DustType = dustType;
            AdjTiles = new int[] { TileID.Bathtubs };

            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); // this style already takes care of direction for us
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(mapColor, name);
        }
    }

    public abstract class BaseClockTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;
        private readonly int height;
        private readonly int[] heights;

        public BaseClockTile(int dustType, Color mapColor, int height, int[] heights, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
            this.height = height;
            this.heights = heights;
        }

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.Clock[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            DustType = dustType;
            AdjTiles = new int[] { TileID.GrandfatherClocks };

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.Origin = new Point16(0, height - 1);
            TileObjectData.newTile.CoordinateHeights = heights;
            TileObjectData.addTile(Type);

            // Etc
            AddMapEntry(mapColor, Language.GetText("ItemName.GrandfatherClock"));
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int x, int y)
        {
            string text = "AM";
            // Get current weird time
            double time = Main.time;
            if (!Main.dayTime)
            {
                // if it's night add this number
                time += 54000.0;
            }

            // Divide by seconds in a day * 24
            time = (time / 86400.0) * 24.0;
            // Dunno why we're taking 19.5. Something about hour formatting
            time = time - 7.5 - 12.0;
            // Format in readable time
            if (time < 0.0)
            {
                time += 24.0;
            }

            if (time >= 12.0)
            {
                text = "PM";
            }

            int intTime = (int)time;
            // Get the decimal points of time.
            double deltaTime = time - intTime;
            // multiply them by 60. Minutes, probably
            deltaTime = (int)(deltaTime * 60.0);
            // This could easily be replaced by deltaTime.ToString()
            string text2 = string.Concat(deltaTime);
            if (deltaTime < 10.0)
            {
                // if deltaTime is eg "1" (which would cause time to display as HH:M instead of HH:MM)
                text2 = "0" + text2;
            }

            if (intTime > 12)
            {
                // This is for AM/PM time rather than 24hour time
                intTime -= 12;
            }

            if (intTime == 0)
            {
                // 0AM = 12AM
                intTime = 12;
            }

            // Whack it all together to get a HH:MM format
            Main.NewText($"Time: {intTime}:{text2} {text}", 255, 240, 20);
            return true;
        }
    }

    public abstract class BaseSinkTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseSinkTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            // Hielo! As you may have noticed, this is a sink --- and as such, it ought to be a water source, right?
            // Well, let's do it one better, shall we?
            TileID.Sets.CountsAsWaterSource[Type] = true;
            // By using these three sets, we've registered our sink as counting as a water, lava, and honey source for crafting purposes! The future is now.
            // Each one works individually and independently of the other two, so feel free to make your sink a source for whatever you'd like it to be!

            // ...modded liquids sold separately.

            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.addTile(Type);

            AddMapEntry(mapColor, Language.GetText("MapObject.Sink"));

            DustType = dustType;
            AdjTiles = new int[] { Type };
        }

    }

    public abstract class BaseDresserTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseDresserTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            // Properties
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.BasicDresser[Type] = true;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            AdjTiles = new int[] { TileID.Dressers };
            DustType = dustType;

            // Names
            AddMapEntry(mapColor, CreateMapEntryName(), MapChestName);

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] {
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            return CreateMapEntryName();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
        {
            width = 3;
            height = 1;
            extraY = 0;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            int left = Main.tile[i, j].TileFrameX / 18;
            left %= 3;
            left = i - left;
            int top = j - Main.tile[i, j].TileFrameY / 18;
            if (Main.tile[i, j].TileFrameY == 0)
            {
                Main.CancelClothesWindow(true);
                Main.mouseRightRelease = false;
                player.CloseSign();
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = "";
                if (Main.editChest)
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.editChest = false;
                    Main.npcChatText = string.Empty;
                }
                if (player.editedChestName)
                {
                    NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                    player.editedChestName = false;
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    if (left == player.chestX && top == player.chestY && player.chest != -1)
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
                    player.piggyBankProjTracker.Clear();
                    player.voidLensChest.Clear();
                    int chestIndex = Chest.FindChest(left, top);
                    if (chestIndex != -1)
                    {
                        Main.stackSplit = 600;
                        if (chestIndex == player.chest)
                        {
                            player.chest = -1;
                            Recipe.FindRecipes();
                            SoundEngine.PlaySound(SoundID.MenuClose);
                        }
                        else if (chestIndex != player.chest && player.chest == -1)
                        {
                            player.OpenChest(left, top, chestIndex);
                            SoundEngine.PlaySound(SoundID.MenuOpen);
                        }
                        else
                        {
                            player.OpenChest(left, top, chestIndex);
                            SoundEngine.PlaySound(SoundID.MenuTick);
                        }
                        Recipe.FindRecipes();
                    }
                }
            }
            else
            {
                Main.playerInventory = false;
                player.chest = -1;
                Recipe.FindRecipes();
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = "";
                Main.interactedDresserTopLeftX = left;
                Main.interactedDresserTopLeftY = top;
                Main.OpenClothesWindow();
            }
            return true;
        }

        // This is not a hook, this is just a normal method used by the MouseOver and MouseOverFar hooks to avoid repeating code.
        public void MouseOverNearAndFarSharedLogic(Player player, int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            left -= tile.TileFrameX % 54 / 18;
            if (tile.TileFrameY % 36 != 0)
            {
                top--;
            }
            int chestIndex = Chest.FindChest(left, top);
            player.cursorItemIconID = -1;
            if (chestIndex < 0)
            {
                player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
            }
            else
            {
                string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY); // This gets the ContainerName text for the currently selected language

                if (Main.chest[chestIndex].name != "")
                {
                    player.cursorItemIconText = Main.chest[chestIndex].name;
                }
                else
                {
                    player.cursorItemIconText = defaultName;
                }
                if (player.cursorItemIconText == defaultName)
                {
                    player.cursorItemIconID = ModContent.ItemType<TItem>();
                    player.cursorItemIconText = "";
                }
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            Player player = Main.LocalPlayer;
            MouseOverNearAndFarSharedLogic(player, i, j);
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = 0;
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            MouseOverNearAndFarSharedLogic(player, i, j);
            if (Main.tile[i, j].TileFrameY > 0)
            {
                player.cursorItemIconID = ItemID.FamiliarShirt;
                player.cursorItemIconText = "";
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }

        public static string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            int chest = Chest.FindChest(left, top);
            if (chest < 0)
            {
                return Language.GetTextValue("LegacyDresserType.0");
            }

            if (Main.chest[chest].name == "")
            {
                return name;
            }

            return name + ": " + Main.chest[chest].name;
        }
    }

    public abstract class BaseWorkBenchTile : BaseTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseWorkBenchTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.WorkBenchPrefab(dustType, mapColor);
        }
    }

    public abstract class BaseDoorOpenTile<TItem, TClosed> : BaseTile where TItem : ModItem where TClosed : ModTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseDoorOpenTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.DoorOpenPrefab(ModContent.TileType<TClosed>(), dustType, mapColor);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[] { new Item(ModContent.ItemType<TItem>()) };
        }
    }

    public abstract class BaseDoorClosedTile<TItem, TOpen> : BaseTile where TItem : ModItem where TOpen : ModTile
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseDoorClosedTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public override void SetStaticDefaults()
        {
            this.DoorClosedPrefab(ModContent.TileType<TOpen>(), dustType, mapColor);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return new Item[] { new Item(ModContent.ItemType<TItem>()) };
        }
    }

    public abstract class BaseBigDroplightTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseBigDroplightTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public abstract void GetLight(ref float r, ref float g, ref float b);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18 * 3)
            {
                GetLight(ref r, ref g, ref b);
                return;
            }
            r = 0f;
            g = 0f;
            b = 0f;
        }

        public override void SetStaticDefaults()
        {
            this.DropLight2Prefab(3, 3, new int[3] { 16, 16, 16 }, dustType, mapColor);
            TileID.Sets.HasOutlines[Type] = true;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18 * 3) / 18;
            int offY = tile.TileFrameY % (18 * 3) / 18;

            for (int k = 0; k < 3; k++)
                for (int m = 0; m < 3; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18 * 3))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18 * 3;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18 * 3;
                }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18 * 3) / 18;
            int offY = tile.TileFrameY % (18 * 3) / 18;

            for (int k = 0; k < 3; k++)
                for (int m = 0; m < 3; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18 * 3))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18 * 3;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18 * 3;
                }
            return true;
        }
    }

    public abstract class BaseDroplightTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseDroplightTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public abstract void GetLight(ref float r, ref float g, ref float b);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                GetLight(ref r, ref g, ref b);
                return;
            }
            r = 0f;
            g = 0f;
            b = 0f;
        }

        public override void SetStaticDefaults()
        {
            this.DropLight2Prefab(1, 2, new int[2] { 16, 16 }, dustType, mapColor);
            TileID.Sets.HasOutlines[Type] = true;
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18) / 18;
            int offY = tile.TileFrameY % (18 * 2) / 18;

            for (int k = 0; k < 1; k++)
                for (int m = 0; m < 2; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18;
                }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18) / 18;
            int offY = tile.TileFrameY % (18 * 2) / 18;

            for (int k = 0; k < 1; k++)
                for (int m = 0; m < 2; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18;
                }
            return true;
        }
    }

    public abstract class BaseCandelabraTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseCandelabraTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public abstract void GetLight(ref float r, ref float g, ref float b);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18 * 2)
            {
                GetLight(ref r, ref g, ref b);
                return;
            }
            r = 0f;
            g = 0f;
            b = 0f;
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            DustType = dustType;
            //tile.ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = itemDrop;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };

            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault(mapName);
            AddMapEntry(mapColor, name);
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18 * 2) / 18;
            int offY = tile.TileFrameY % (18 * 2) / 18;

            for (int k = 0; k < 2; k++)
                for (int m = 0; m < 2; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18 * 2))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18 * 2;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18 * 2;
                }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18 * 2) / 18;
            int offY = tile.TileFrameY % (18 * 2) / 18;

            for (int k = 0; k < 2; k++)
                for (int m = 0; m < 2; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18 * 2))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18 * 2;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18 * 2;
                }
            return true;
        }
    }

    public abstract class BaseCandleTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseCandleTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public abstract void GetLight(ref float r, ref float g, ref float b);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                GetLight(ref r, ref g, ref b);
                return;
            }
            r = 0f;
            g = 0f;
            b = 0f;
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            DustType = dustType;
            //tile.ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = itemDrop;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault(mapName);
            AddMapEntry(mapColor, name);
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18) / 18;
            int offY = tile.TileFrameY % (18) / 18;

            for (int k = 0; k < 1; k++)
                for (int m = 0; m < 1; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18;
                }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18) / 18;
            int offY = tile.TileFrameY % (18) / 18;

            for (int k = 0; k < 1; k++)
                for (int m = 0; m < 1; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18;
                }
            return true;
        }
    }

    public abstract class BaseFloorLampTile<TItem> : BaseTile where TItem : ModItem
    {
        private readonly int dustType;
        private readonly Color mapColor;

        public BaseFloorLampTile(int dustType, Color mapColor, string texturePath, bool pathHasName = false) : base(texturePath, pathHasName)
        {
            this.dustType = dustType;
            this.mapColor = mapColor;
        }

        public abstract void GetLight(ref float r, ref float g, ref float b);
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j].TileFrameX < 18)
            {
                GetLight(ref r, ref g, ref b);
                return;
            }
            r = 0f;
            g = 0f;
            b = 0f;
        }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            DustType = dustType;
            //tile.ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = itemDrop;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };

            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault(mapName);
            AddMapEntry(mapColor, name);
        }

        public override void HitWire(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18) / 18;
            int offY = tile.TileFrameY % (18 * 3) / 18;

            for (int k = 0; k < 1; k++)
                for (int m = 0; m < 3; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18;
                }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<TItem>();
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int offX = tile.TileFrameX % (18) / 18;
            int offY = tile.TileFrameY % (18 * 3) / 18;

            for (int k = 0; k < 1; k++)
                for (int m = 0; m < 3; m++)
                {
                    if (Main.tile[i - offX + k, j - offY + m].TileFrameX >= (18))
                        Main.tile[i - offX + k, j - offY + m].TileFrameX -= 18;
                    else
                        Main.tile[i - offX + k, j - offY + m].TileFrameX += 18;
                }
            return true;
        }
    }
}
