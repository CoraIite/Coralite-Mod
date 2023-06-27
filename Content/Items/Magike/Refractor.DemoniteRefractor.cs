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

namespace Coralite.Content.Items.Magike
{
    public class DemoniteRefractor: BaseMagikePlaceableItem
    {
        public DemoniteRefractor() : base(TileType<DemoniteRefractorTile>(), Item.sellPrice(0, 0, 25, 0), RarityType<MagikeCrystalRarity>(), 25)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystalRefractor>()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.ShadowScale, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DemoniteRefractorTile : BaseRefractorTile
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
                16
            };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<DemoniteRefractorEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.MediumPurple);
            DustType = DustID.Demonite;
        }
    }

    public class DemoniteRefractorEntity : MagikeSender_Line
    {
        public const int sendDelay = 4 * 60+30;
        public int sendTimer;

        public DemoniteRefractorEntity() : base(50, 25 * 16) { }

        public override int HowManyPerSend => 15;

        public override ushort TileType => (ushort)TileType<DemoniteRefractorTile>();

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
            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Color.MediumPurple);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Color.MediumPurple);
        }
    }
}
