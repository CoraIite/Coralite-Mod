using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.RedJades
{
    public class RedJadeToilet : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + Name;
        public const int NextStyleHeight = 40;

        public override void SetStaticDefaults()
        {
            this.ToiletPrefab(DustID.GemRuby, Coralite.Instance.RedJadeRed);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
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
            player.cursorItemIconID = ModContent.ItemType<Items.RedJades.RedJadeToilet>();

            if (Main.tile[i, j].TileFrameX / 18 < 1)
                player.cursorItemIconReversed = true;
        }
    }
}
