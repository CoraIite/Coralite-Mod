using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike
{
    public class CrystalLense : BaseLense
    {
        public CrystalLense() : base(TileType<CrystalLenseTile>(), Item.sellPrice(0, 0, 10, 0), GetInstance<MagikeCrystalRarity>().Type, 50)
        { }

        public override void AddRecipes()
        {
            
        }
    }

    public class CrystalLenseTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true;//不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;

            TileObjectData.addTile(Type);
        }

        public override void MouseOver(int i, int j)
        {
            MagikeHelper.ShowMagikeNumber(i, j);
        }
    }

    public class CrystalLenseEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 600;
        public int sendTimer;
        public CrystalLenseEntity() : base(50, 5 * 16,600) { }

        public override ushort TileType => (ushort)TileType<CrystalLenseTile>();

        public override int HowManyPerSend => 10;

        public override bool CanSend()
        {
            sendTimer++;
            if (sendTimer > sendDelay)
            {
                sendTimer = 0;
                return true;
            }

            return false;
        }
    }
}
