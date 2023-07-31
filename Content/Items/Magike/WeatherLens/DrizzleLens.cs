using Coralite.Content.Raritys;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.WeatherLens
{
    public class DrizzleLens : BaseMagikePlaceableItem
    {
        public DrizzleLens() : base(TileType<DrizzleLensTile>(), Item.sellPrice(0, 0, 10, 0), RarityType<MagicCrystalRarity>(), 50)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient(ItemID.TissueSample, 8)
                .AddIngredient(ItemID.RainCloud,8)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient(ItemID.ShadowScale, 8)
                .AddIngredient(ItemID.RainCloud, 8)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DrizzleLensTile : BaseCostItemLensTile
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
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.LavaPlacement = Terraria.Enums.LiquidPlacement.Allowed;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<DrizzleLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.DeepSkyBlue);
            DustType = DustID.Water;
        }
    }

    public class DrizzleLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 10 * 60;
        public int sendTimer;
        public DrizzleLensEntity() : base(50, 5 * 16, 10 * 60) { }

        public override ushort TileType => (ushort)TileType<DrizzleLensTile>();

        public override int HowManyPerSend => 2;
        public override int HowManyToGenerate => 2;

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
            GenerateAndChargeSelf(howMany);
        }

        public override bool CanGenerate() => Main.raining;

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.DeepSkyBlue, DustID.Water);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.DeepSkyBlue,DustID.Water);
        }
    }
}
