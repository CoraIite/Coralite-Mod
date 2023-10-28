using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Gels
{
    public class GelThrone : BaseRelicItem
    {
        public GelThrone() : base(ModContent.TileType<GelThroneTile>(), AssetDirectory.GelItems) { }
    }

    public class GelThroneTile : ModTile
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.CanBeSatOnForNPCs[Type] = true; // Facilitates calling ModifySittingTargetInfo for NPCs
            TileID.Sets.CanBeSatOnForPlayers[Type] = true; // Facilitates calling ModifySittingTargetInfo for Players
            TileID.Sets.DisableSmartCursor[Type] = true;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            DustType = DustID.Gold;
            AdjTiles = new int[] { TileID.Chairs };

            AddMapEntry(Color.Gold, Language.GetText("MapObject.Chair"));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.CoordinateHeights = new int[4] { 16, 16, 16, 18 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            // 知道这是在玩家和NPC上调用的非常重要的，所以不要使用Main.LocalPlayer，例如使用info.restingEntity。
            Tile tile = Framing.GetTileSafely(i, j);

            //info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
            //info.visualOffset = Vector2.Zero; // Defaults to (0,0)

            info.TargetDirection = info.RestingEntity.direction;
            //if (tile.TileFrameX != 0)
            //{
            //    info.TargetDirection = 1; // 如果坐在右侧备用位置上，则朝右（之前通过 SetStaticDefaults 中的 addAlternate 添加）
            //}

            // 锚点表示椅子最底部的磁贴。这用于对齐实体命中框
            // 由于 i 和 j 可能来自椅子的任何坐标，我们需要基于此调整锚点
            info.AnchorTilePosition.X = i; // 我们的椅子只有1宽，所以没有什么特别的要求
            info.AnchorTilePosition.Y = j;
            info.DirectionOffset = -2;
            int whoAmI_X = tile.TileFrameX / 18;
            if (whoAmI_X == 0)
                info.AnchorTilePosition.X++;
            else if (whoAmI_X == 2)
                info.AnchorTilePosition.X--;

            int whoAmI_Y = tile.TileFrameY / 18;
            if (whoAmI_Y == 0)
                info.AnchorTilePosition.Y += 3;
            else if (whoAmI_Y == 1)
                info.AnchorTilePosition.Y += 2;
            else if (whoAmI_Y == 2)
                info.AnchorTilePosition.Y += 1;
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
            player.cursorItemIconID = ModContent.ItemType<GelThrone>();
        }
    }
}
