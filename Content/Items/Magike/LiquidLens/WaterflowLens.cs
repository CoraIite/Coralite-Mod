using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.LiquidLens
{
    public class WaterflowLens : BaseMagikePlaceableItem
    {
        public WaterflowLens() : base(TileType<WaterflowLensTile>(), Item.sellPrice(0, 0, 10, 0), RarityType<MagikeCrystalRarity>(), 50)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient(ItemID.BottledWater, 5)
                .AddIngredient(ItemID.CrimtaneBar, 5)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient(ItemID.BottledWater, 5)
                .AddIngredient(ItemID.DemoniteBar, 5)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }

    public class WaterflowLensTile : BaseCostItemLensTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
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
            //TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.OnlyInFullLiquid;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<WaterflowLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Aqua);
            DustType = DustID.BubbleBurst_Blue;
        }
    }

    public class WaterflowLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 10 * 60;
        public int sendTimer;
        public WaterflowLensEntity() : base(40, 5 * 16, 11 * 60) { }

        public override ushort TileType => (ushort)TileType<WaterflowLensTile>();

        public override int HowManyPerSend => 2;
        public override int HowManyToGenerate => 1;

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

        public override void OnGenerate(int howMany)
        {
            Generate(howMany);
        }

        public override bool CanGenerate()
        {
            Tile tile = Framing.GetTileSafely(Position);
            return tile.LiquidType == LiquidID.Water && tile.LiquidAmount > 128;
        }

        public override void SendVisualEffect(MagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Aqua);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Aqua, DustID.FireworksRGB);
        }
    }

}
