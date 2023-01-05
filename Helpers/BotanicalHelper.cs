using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.BotanicalSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Helpers
{
    public static partial class BotanicalHelper
    {
        public static BotanicalItem GetBotanicalItem(this Item item)
        {
            return item.GetGlobalItem<BotanicalItem>();
        }

        public static CrossBreedCatalystItem GetCatalystItem(this Item item)
        {
            return item.GetGlobalItem<CrossBreedCatalystItem>();
        }

        /// <summary>
        /// 获取当前植物的生长状态
        /// </summary>
        /// <param name="i">X坐标</param>
        /// <param name="j">Y坐标</param>
        /// <param name="frameWidth">该物块帧图宽度</param>
        /// <param name="frameWidthMax">该物块有多少帧</param>
        /// <returns></returns>
        public static PlantStage GetPlantStage(int i, int j, int frameWidth, int frameWidthMax)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            int frame = tile.TileFrameX / frameWidth;
            if (frame == (frameWidthMax - 1))
                return PlantStage.Grown;
            else
                return (PlantStage)frame;
        }

        /// <summary>
        /// 获取多块物块的左上角
        /// </summary>
        /// <param name="i">The tile X-coordinate</param>
        /// <param name="j">The tile Y-coordinate</param>
        public static Point16 GetTileOrigin(int i, int j)
        {
            //Framing.GetTileSafely ensures that the returned Tile instance is not null
            //Do note that neither this method nor Framing.GetTileSafely check if the wanted coordiates are in the world!
            Tile tile = Framing.GetTileSafely(i, j);

            Point16 coord = new Point16(i, j);
            Point16 frame = new Point16(tile.TileFrameX / 18, tile.TileFrameY / 18);

            return coord - frame;
        }

        /// <summary>
        /// 使用 <seealso cref="GetTileOrigin(int, int)"/> 去获取这个多块物块的entity (<paramref name="i"/>, <paramref name="j"/>).
        /// </summary>
        /// <typeparam name="T">尝试获取的entity的type</typeparam>
        /// <param name="i">物块X坐标</param>
        /// <param name="j">物块Y坐标</param>
        /// <param name="entity">能找到的<typeparamref name="T"/> 的实例, 如果只有1个的话</param>
        /// <returns>返回 <see langword="true"/> 如果找到 <typeparamref name="T"/> 实例, 返回 <see langword="false"/> 如果没有entity存在 或是 这个entity不是 <typeparamref name="T"/> 类型的实例.</returns>
        public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : ModTileEntity
        {
            int index = ModContent.GetInstance<T>().Find(i, j);

            if (index == -1)
            {
                entity = null;
                return false;
            }

            entity=(T)TileEntity.ByID[index];
            return true;
        }

        public static bool TryGetTileEntityForMultTile<T>(ushort tileType,int i, int j, out T entity) where T : ModTileEntity
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileType!=tileType)
            {
                entity=null;
                return false;
            }

            TileObjectData data = TileObjectData.GetTileData(tile);
            int index = -1;
            //遍历！哈哈！
            for (int m = 0; m < data.Width; m++)
            {
                for (int n = 0; n < data.Height; n++)
                {
                    index = ModContent.GetInstance<T>().Find(i - m, j - n);
                    if (index != -1)
                        break;
                }
            }

            if (index == -1)
            {
                entity = null;
                return false;
            }

            entity = (T)TileEntity.ByID[index];
            return true;
        }

        /// <summary>
        /// 用于随机更新物块时使用
        /// </summary>
        /// <typeparam name="T">你的TileEntity类型</typeparam>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="frameWidth"></param>
        /// <param name="frameWidthMax"></param>
        public static void UpdatePlant<T>(int i, int j, short frameWidth, int frameWidthMax) where T : BasePlantTileEntity
        {
            Tile tile = Framing.GetTileSafely(i, j);
            PlantStage stage = GetPlantStage(i, j, frameWidth, frameWidthMax);

            if (stage == PlantStage.Grown)
                return;

            if (TryGetTileEntityAs(i, j, out T plantEntity))
            {
                plantEntity.growTime++;
                if (plantEntity.growTime >= plantEntity.DominantGrowTime)
                {
                    plantEntity.growTime = 0;
                    tile.TileFrameX += frameWidth;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendTileSquare(-1, i, j, 1);
                }
            }

        }

        /// <summary>
        /// 放置植物事使用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="item"></param>
        public static void PlacePlant<T>(int i, int j, Item item) where T : BasePlantTileEntity
        {
            BotanicalItem botanicalItem = item.GetBotanicalItem();

            if (TryGetTileEntityAs(i, j, out BasePlantTileEntity plantEntity))
            {
                plantEntity.DominantGrowTime = botanicalItem.DominantGrowTime;
                plantEntity.RecessiveGrowTime = botanicalItem.RecessiveGrowTime;
            }
        }

        /// <summary>
        /// 用于Kill指定位置的Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public static void KillPlant<T>(int i, int j) where T : ModTileEntity
        {
            Point16 origin = BotanicalHelper.GetTileOrigin(i, j);
            ModContent.GetInstance<T>().Kill(origin.X, origin.Y);
        }
    }

    public enum PlantStage : byte
    {
        Seed,
        Growing1,
        Growing2,
        Growing3,
        Growing4,
        Grown,
    }
}
