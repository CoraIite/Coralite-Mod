using Coralite.Core.Systems.BotanicalSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Prefabs.Tiles
{
    public abstract class BaseTopSoildSeedlingPlantTile<T> : BasePlantTile where T : BasePlantTileEntity
    {
        private readonly string _Texture;
        private readonly bool PathHasName;
        private readonly int SeedType;
        private readonly int PlantType;
        private readonly int RarePlantType;

        public BaseTopSoildSeedlingPlantTile(string texture, short frameWidth, int frameCount, int seedType = 0, int plantType = 0, int rarePlantType = 0, bool pathHasName = false) : base((short)(frameWidth + 2), frameCount)
        {
            _Texture = texture;
            PathHasName = pathHasName;
            SeedType = seedType;
            PlantType = plantType;
            RarePlantType = rarePlantType;
        }

        public override string Texture => _Texture + (PathHasName ? "" : Name);

        public override bool CanPlace(int i, int j)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile topTile = Framing.GetTileSafely(i, j - 1);
            //没有物块直接放
            if (!tile.HasTile)
                return true;
            if (!topTile.HasTile&&WorldGen.SolidTile(topTile))//顶部必须有方块
                return false;

            //有物块的时候，这个物块和自己是不是一样的物块，并且如果这个物块状态是成熟那就直接替换
            int tileType = tile.TileType;
            if (tileType == Type)
            {
                PlantStage stage = BotanicalHelper.GetPlantStage(i, j, FrameWidth, FrameCount);
                return stage == PlantStage.Grown;
            }

            //如果这个地方是流动的水、岩浆之类乱七八糟的话放不了
            if (!(Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip))
                return false;

            //不满足以下条件放不了
            bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
            bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
            bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);
            if (!(foliageGrass || moddedFoliage || harvestableVanillaHerb))
                return false;

            //满足上述条件后Kill掉原本的物块并放上这个
            WorldGen.KillTile(i, j);
            if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);

            return true;
        }

        #region 收获植物

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
                if (BotanicalHelper.TryGetTileEntityAs(i, j, out T entity))
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
                    if (BotanicalHelper.TryGetTileEntityAs(i, j, out T entity))
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
                if (BotanicalHelper.TryGetTileEntityAs(i, j, out T entity))
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

            return false;
        }

        /// <summary>
        /// 设置玩家使用再生法杖时掉落的物品
        /// <para>请根据状态来自行判断掉落数量</para>
        /// </summary>
        /// <param name="rarePlantStack">稀有物品数量</param>
        /// <param name="plantItemStack">植物物品数量</param>
        /// <param name="seedItemStack">种子物品数量</param>
        public abstract void DropItemWithStaffOfRegrowth(PlantStage stage, ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack);

        /// <summary>
        /// 正常情况下掉落物品，仅成熟时候
        /// </summary>
        /// <param name="rarePlantStack">稀有物品数量</param>
        /// <param name="plantItemStack">植物物品数量</param>
        /// <param name="seedItemStack">种子物品数量</param>
        public abstract void DropItemNormally(ref int rarePlantStack, ref int plantItemStack, ref int seedItemStack);

        #endregion

        public override bool IsTileSpelunkable(int i, int j)
        {
            PlantStage stage = BotanicalHelper.GetPlantStage(i, j, FrameWidth, FrameCount);
            return stage == PlantStage.Grown;
        }

        public override void RandomUpdate(int i, int j)
        {
            BotanicalHelper.UpdatePlant<T>(i, j, FrameWidth, FrameCount);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            BotanicalHelper.KillPlantEntity<T>(i, j);
        }

    }
}
