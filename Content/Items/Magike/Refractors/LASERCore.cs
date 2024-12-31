using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MTBStructure;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class LASERCore() : BasePlaceableItem(Item.sellPrice(copper: 50), RarityType<MagicCrystalRarity>()
        , TileType<LASERCoreTile>(), AssetDirectory.MagikeRefractors)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BasicRefractor>()
                .AddIngredient<MagicCrystal>(12)
                .AddIngredient(ItemID.CopperPlating, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LASERCoreTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Gray);

            DustType = DustID.CrystalSerpent_Pink;
        }

        public override bool RightClick(int i, int j)
        {
            CoraliteContent.GetMTBS<LASERMBS>().CheckStructure(new Point(i, j - 1));

            return true;
        }
    }

    public class LASERMBS : Multiblock
    {
        /// <summary>
        /// 魔力水晶块
        /// </summary>
        private int cb => TileType<MagicCrystalBlockTile>();
        /// <summary>
        /// 硬化玄武岩
        /// </summary>
        private int h => TileType<HardBasaltTile>();
        /// <summary>
        /// 玄武岩
        /// </summary>
        private int b => TileType<BasaltTile>();
        /// <summary>
        /// 铜
        /// </summary>
        private int cp => TileID.CopperPlating;
        /// <summary>
        /// 核心
        /// </summary>
        private int core => TileType<LASERCoreTile>();

        public override int[,] StructureTile =>
            new int[5, 5]
            {
                {h, h,  -1,  h, h },
                {b, -1, cb, -1, b },
                {b, -1, cb, -1, b },
                {cp,cp,core,cp,cp },
                {h, h,  h,  h,  h },
            };

        public override void OnSuccess(Point origin)
        {
            KillAll(origin);

            Item.NewItem(new EntitySource_TileBreak(origin.X, origin.Y), origin.ToWorldCoordinates()
                , ItemType<LASER>());
        }
    }
}
