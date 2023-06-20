using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike
{
    public class CrimtaneLens : BaseMagikePlaceableItem
    {
        public CrimtaneLens() : base(TileType<CrimtaneLensTile>(), Item.sellPrice(0, 0, 50, 0), RarityType<MagikeCrystalRarity>(), 50)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystalLens>()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrimtaneLensTile : BaseLensTile
    {
        public override void SetStaticDefaults()
        {
            //Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[3] {
                16,
                16,
                18
            };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrimtaneLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Red);
            DustType = DustID.Crimson;
        }
    }

    public class CrimtaneLensEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 570;   //血腥相比腐化的要稍微弱那么一点点
        public int sendTimer;
        public CrimtaneLensEntity() : base(150, 5 * 16, 570) { }

        public override ushort TileType => (ushort)TileType<CrimtaneLensTile>();

        public override int HowManyPerSend => 15;

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

        public override void SendVisualEffect(MagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Red);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Red);
        }
    }
}
