using Coralite.Content.Items.Botanical.Plants;
using Coralite.Content.Items.Botanical.Seeds;
using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.BotanicalSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Tiles.Plants
{
    public class AgropyronFrozen : BaseBottomSolidSeedPlantTile<NormalPlantTileEntity>
    {
        public AgropyronFrozen() : base(AssetDirectory.PlantTiles, 24, 3, ItemType<AgropyronSeed>(), ItemType<AgropyronFreezer>()) { }

        public override void SetStaticDefaults()
        {
            (this).SoildBottomPlantPrefab<NormalPlantTileEntity>(new int[] { TileID.IceBlock, TileID.SnowBlock }, new int[] { TileID.ClayPot, TileID.PlanterBox }, new int[] { 22 }, -6, SoundID.Grass, DustID.Grass, Color.Green, 24, "寒霜冰草", 1, 0);
        }

        public override void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            plantItemStack = Main.rand.Next(2);
            seedItemStack = Main.rand.Next(3);
        }

        public override void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack)
        {
            if (stage == PlantStage.Grown)
                plantItemStack = Main.rand.Next(4);

            seedItemStack = Main.rand.Next(3);
        }

        public override bool Drop(int i, int j)
        {
            PlantStage stage = BotanicalHelper.GetPlantStage(i, j, FrameWidth, FrameCount);

            if (stage == PlantStage.Seed)//种子状态啥也不掉
                return false;

            Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
            Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

            int plantStack = 0;
            int rarePlantStack = 0;
            int seedStack = 0;

            if (nearestPlayer.active && nearestPlayer.HeldItem.type == ItemID.StaffofRegrowth)
                DropItemWithStaffOfRegrowth(stage, ref rarePlantStack, ref plantStack, ref seedStack);
            else if (stage == PlantStage.Grown)
                DropItemNormally(ref rarePlantStack, ref plantStack, ref seedStack);

            //生成稀有植物掉落物，注意，稀有植物物品掉落会使正常植物和种子不会掉落
            if (RarePlantType > 0 && rarePlantStack > 0)
            {
                int id = Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, RarePlantType, rarePlantStack);
                if (BotanicalHelper.TryGetTileEntityAs(i, j, out NormalPlantTileEntity entity))
                {
                    Item plantItem = Main.item[id];
                    BotanicalItem botanicalItem = plantItem.GetBotanicalItem();
                    if (botanicalItem.botanicalItem)
                    {
                        botanicalItem.DominantGrowTime = entity.DominantGrowTime;
                        botanicalItem.RecessiveGrowTime = entity.RecessiveGrowTime;
                        botanicalItem.DominantLevel = entity.DominantLevel;
                        botanicalItem.RecessiveLevel = entity.RecessiveLevel;
                    }
                }
                return false;
            }

            //生成植物掉落物
            if (PlantType > 0 && plantStack > 0)
            {
                int id = Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, PlantType, plantStack);
                if (id > ItemID.Count)
                    if (BotanicalHelper.TryGetTileEntityAs(i, j, out NormalPlantTileEntity entity))
                    {
                        Item seedItem = Main.item[id];
                        BotanicalItem botanicalItem = seedItem.GetBotanicalItem();
                        if (botanicalItem.botanicalItem)
                        {
                            botanicalItem.DominantGrowTime = entity.DominantGrowTime;
                            botanicalItem.RecessiveGrowTime = entity.RecessiveGrowTime;
                            botanicalItem.DominantLevel = entity.DominantLevel;
                            botanicalItem.RecessiveLevel = entity.RecessiveLevel;
                        }
                    }
            }
            //生成种子掉落物
            if (SeedType > 0 && seedStack > 0)
            {
                int id = Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, SeedType, seedStack);
                if (BotanicalHelper.TryGetTileEntityAs(i, j, out NormalPlantTileEntity entity))
                {
                    Item plantItem = Main.item[id];
                    BotanicalItem botanicalItem = plantItem.GetBotanicalItem();
                    if (botanicalItem.botanicalItem)
                    {
                        botanicalItem.DominantGrowTime = entity.DominantGrowTime;
                        botanicalItem.RecessiveGrowTime = entity.RecessiveGrowTime;
                        botanicalItem.DominantLevel = entity.DominantLevel;
                        botanicalItem.RecessiveLevel = entity.RecessiveLevel;
                    }
                }
            }

            Item.NewItem(new EntitySource_TileBreak(i, j), worldPosition, ItemType<IceyDust>(), Main.rand.Next(3));
            return false;
        }
    }
}
