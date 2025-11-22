using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MTBStructure;
using Coralite.Helpers;
using System.Linq;
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
            CoraliteContent.GetMultiblock<LASERMultiblock>().CheckStructure(new Point16(i, j));

            return true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            int type = CoraliteContent.MultiblockType<LASERMultiblock>();
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ProjectileType<PreviewMultiblockPeoj>() && p.ai[0] == type))
                p.Kill();
        }
    }

    public class LASERMultiblock : PreviewMultiblock
    {
        public override void SetStaticDefaults()
        {
            int CrystalBlock = TileType<MagicCrystalBlockTile>();
            int HardBasalt = TileType<HardBasaltTile>();
            int Basalt = TileType<BasaltTile>();
            int CopperPlating = TileID.CopperPlating;
            int Core = TileType<LASERCoreTile>();

            //{h, h,  -1,  h, h },
            //{b, -1, cb, -1, b },
            //{b, -1, cb, -1, b },
            //{cp,cp,core,cp,cp },
            //{h, h,  h,  h,  h },

            AddTiles((-2, -3, HardBasalt), (-1, -3, HardBasalt), (1, -3, HardBasalt), (2, -3, HardBasalt));
            AddTiles((-2, -2, Basalt), (0, -2, CrystalBlock), (2, -2, Basalt));
            AddTiles((-2, -1, Basalt), (0, -1, CrystalBlock), (2, -1, Basalt));
            AddTiles((-2, 0, CopperPlating), (-1, 0, CopperPlating),(0, 0, Core), (1, 0, CopperPlating), (2, 0, CopperPlating));
            AddTiles((-2, 1, HardBasalt), (-1, 1, HardBasalt),(0, 1, HardBasalt), (1, 1, HardBasalt), (2, 1, HardBasalt));
        }

        public override void OnSuccess(Point16 origin)
        {
            base.OnSuccess(origin);

            KillAll(origin);

            for (int i = 0; i < 3; i++)
            {
                Tile t = Framing.GetTileSafely(origin + new Point16(i - 1, 2));
                if (!Helper.HasReallySolidTile(t))//如果没有实心物块就掉落出来
                    goto dropItem;
            }

            int tileType = TileType<LASERTile>();
            WorldGen.PlaceTile(origin.X, origin.Y + 1, tileType);

            Tile t2 = Framing.GetTileSafely(origin);
            if (t2.TileType != tileType)//放置失败，生成物品
                goto dropItem;

            return;
        dropItem:
            Helper.SpawnItemTileBreakNet<LASER>(origin);
        }

    }
}
