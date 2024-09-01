using Coralite.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Gels
{
    public class RoyalGelCannon : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RoyalGelCannonTile>());
            Item.width = 30;
            Item.height = 40;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(0, 1, 0, 0);
        }
    }

    public class RoyalGelCannonTile : ModTile
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = DustID.GoldCoin;

            AddMapEntry(new Color(78, 136, 255, 80));

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.newTile.LavaDeath = true;
            // The following 3 lines are needed if you decide to add more styles and stack them vertically
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1); // Facing right will use the second texture style
            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, 16 * 4); // Avoid being able to trigger it from long range
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, 16 * 4))
            { // 避免能够从远距离触发它
                player.GamepadEnableGrappleCooldown();
                Shoot(i, j, 25);
            }

            return true;
        }

        public override void HitWire(int i, int j)
        {
            Shoot(i, j, 10);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            // 右键单击中的匹配条件。仅当单击交互执行某些操作时，交互才应显示
            if (!player.IsWithinSnappngRangeToTile(i, j, 16 * 4))
                return;

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<RoyalGelCannon>();

            if (Main.tile[i, j].TileFrameX / (18 * 3) > 0)
            {
                player.cursorItemIconReversed = true;
            }
        }

        public static void Shoot(int i, int j, int damage)
        {
            Tile tile = Main.tile[i, j];

            int dir = tile.TileFrameX / (18 * 3);
            Vector2 vel = dir == 0 ? new Vector2(18, 0) : new Vector2(-18, 0);
            int Xoffset = tile.TileFrameX % (18 * 3) / 18;
            int Yoffset = tile.TileFrameY % (18 * 3) / 18;

            Vector2 pos = (new Vector2(i - Xoffset, j - Yoffset) * 16) + new Vector2(16 * 3 / 2, 8);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(new EntitySource_TileUpdate(i, j), pos, vel.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), ModContent.ProjectileType<SlimeEruptionBall>(), damage, 2, Main.myPlayer);

            SoundEngine.PlaySound(CoraliteSoundID.BubbleGun_Item85, pos);
        }
    }
}
