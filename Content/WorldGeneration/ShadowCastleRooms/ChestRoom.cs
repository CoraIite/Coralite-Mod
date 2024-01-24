using Coralite.Content.Items.Shadow;
using Coralite.Content.Tiles.ShadowCastle;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.WorldGeneration.ShadowCastleRooms
{
    public class ChestRoom : ShadowCastleRoom
    {
        public int tresureItemType;

        public override int RandomTypeCount => 3;

        public override Point[] UpCorridor => new Point[]
        {
            new Point(32,19),
            new Point(31,12),
            new Point(32,12),
        };
        public override Point[] DownCorridor => new Point[]
        {
            new Point(31,44),
            new Point(31,52),
            new Point(32,51),
        };
        public override Point[] LeftCorridor => new Point[]
        {
            new Point(19,32),
            new Point(12,32),
            new Point(12,32),
        };
        public override Point[] RightCorridor => new Point[]
        {
            new Point(45,32),
            new Point(52,32),
            new Point(51,32),
        };

        public static Point[] ChestPoint => new Point[]
        {
            new Point(31,36),
            new Point(31,38),
            new Point(31,18),
        };

        public ChestRoom(Point center, int tresureItemType) : base(center, RoomType.SingleChest)
        {
            this.tresureItemType = tresureItemType;
        }

        public override void PostGenerateSelf()
        {
            GenerateObject();
            //生成宝箱
            int index = WorldGen.PlaceChest(roomRect.X + ChestPoint[randomType].X, roomRect.Y + ChestPoint[randomType].Y, (ushort)ModContent.TileType<MercuryChestTile>());
            if (index >= 0)
            {
                Chest chest = Main.chest[index];

                RandChestItem(chest, tresureItemType);

                //放影水晶
                if (WorldGen.genRand.NextBool(1, 2))
                    RandChestItem(chest, ModContent.ItemType<ShadowCrystal>()
                        , WorldGen.genRand.Next(1, 5));

                //放第一批药水
                for (int i = 0; i < 3; i++)
                    if (WorldGen.genRand.NextBool(2, 3))
                        RandChestItem(chest, WorldGen.genRand.NextFromList(
                            ItemID.SpelunkerPotion, ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.ArcheryPotion, ItemID.GravitationPotion,
                            ItemID.ObsidianSkinPotion, ItemID.RegenerationPotion, ItemID.SwiftnessPotion, ItemID.IronskinPotion), WorldGen.genRand.Next(1, 3));
                //放第二批药水
                for (int i = 0; i < 2; i++)
                    if (WorldGen.genRand.NextBool(1, 3))
                        RandChestItem(chest, WorldGen.genRand.NextFromList(
                            ItemID.ThornsPotion, ItemID.HunterPotion, ItemID.WaterWalkingPotion, ItemID.TeleportationPotion)
                            , WorldGen.genRand.Next(1, 3));

                //放回城药水
                if (WorldGen.genRand.NextBool(1, 2))
                    RandChestItem(chest, ItemID.RecallPotion
                        , WorldGen.genRand.Next(1, 3));

                //放恢复药水
                if (WorldGen.genRand.NextBool(1, 2))
                    RandChestItem(chest, ItemID.HealingPotion
                        , WorldGen.genRand.Next(3, 6));

                //放火把
                if (WorldGen.genRand.NextBool(1, 2))
                    RandChestItem(chest, WorldGen.genRand.NextFromList(ItemID.Torch, ItemID.Glowstick)
                        , WorldGen.genRand.Next(15, 29));

                //放马内，放一大堆的Money
                if (WorldGen.genRand.NextBool(1, 2))
                    RandChestItem(chest, ItemID.GoldCoin
                        , WorldGen.genRand.Next(1, 2));
                for (int i = 0; i < 5; i++)
                    if (WorldGen.genRand.NextBool(1, 3))
                        RandChestItem(chest, ItemID.SilverCoin
                            , WorldGen.genRand.Next(1, 20));
                for (int i = 0; i < 8; i++)
                    if (WorldGen.genRand.NextBool(1, 3))
                        RandChestItem(chest, ItemID.SilverCoin
                            , WorldGen.genRand.Next(1, 90));
                Chest.Lock(chest.x, chest.y);
            }

        }

        public static void RandChestItem(Chest chest, int itemtype, int stack = 1)
        {
            int itemIndex = WorldGen.genRand.Next(0, chest.item.Length);
            int limit = 0;
            while (!chest.item[itemIndex].IsAir && limit < chest.item.Length * 2)
            {
                limit++;
                itemIndex = WorldGen.genRand.Next(0, chest.item.Length);
            }

            chest.item[itemIndex].SetDefaults(itemtype);
            chest.item[itemIndex].stack = stack;
        }
    }
}
