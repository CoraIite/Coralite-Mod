using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.BasicLens
{
    public class DemoniteLens : BaseMagikePlaceableItem
    {
        public DemoniteLens() : base(TileType<DemoniteLensTile>(), Item.sellPrice(0, 0, 50, 0), RarityType<MagicCrystalRarity>(), 50)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystalLens>()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DemoniteLensTile : BaseCostItemLensTile
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
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<DemoniteLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.MediumPurple);
            DustType = DustID.Demonite;
        }
    }

    public class DemoniteLensEntity : MagikeGenerator_FromMagItem
    {
        public const int sendDelay = 540;
        public int sendTimer;
        public DemoniteLensEntity() : base(150, 5 * 16, 540) { }

        public override ushort TileType => (ushort)TileType<DemoniteLensTile>();

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

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.MediumPurple);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.MediumPurple);
        }
    }
}
