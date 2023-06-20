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
    public class CrimtaneRefractor: BaseMagikePlaceableItem
    {
        public CrimtaneRefractor() : base(TileType<CrimtaneRefractorTile>(), Item.sellPrice(0, 0, 50, 0), RarityType<MagikeCrystalRarity>(), 25)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystalRefractor>()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }

    public class CrimtaneRefractorTile : BaseRefractorTile
    {
        public override void SetStaticDefaults()
        {
            //Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                18
            };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrimtaneRefractorEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Red);
            DustType = DustID.Crimson;
        }
    }

    public class CrimtaneRefractorEntity : MagikeSender
    {
        public const int sendDelay = 4 * 60 + 30;
        public int sendTimer;

        public CrimtaneRefractorEntity() : base(50, 25 * 16) { }

        public override int HowManyPerSend => 15;

        public override ushort TileType => (ushort)TileType<CrimtaneRefractorTile>();

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
            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Color.Red);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Color.Red);
        }
    }
}
