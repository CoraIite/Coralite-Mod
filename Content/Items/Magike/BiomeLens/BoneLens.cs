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

namespace Coralite.Content.Items.Magike.BiomeLens
{
    public class BoneLens : BaseMagikePlaceableItem
    {
        public BoneLens() : base(TileType<BoneLensTile>(), Item.sellPrice(0, 0, 50, 0), RarityType<MagicCrystalRarity>(), 50)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(5)
                .AddIngredient(ItemID.Bone, 20)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDungeon;
        }
    }

    public class BoneLensTile : BaseCostItemLensTile
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
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.Allowed;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BoneLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Gray);
            DustType = DustID.Bone;
        }
    }

    public class BoneLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 10 * 60;
        public int sendTimer;
        public BoneLensEntity() : base(100, 5 * 16, 5 * 60) { }

        public override ushort TileType => (ushort)TileType<BoneLensTile>();

        public override int HowManyPerSend => 4;
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

        public override bool CanGenerate() => true;

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Gray, DustID.Bone);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Gray, DustID.Bone);
        }
    }
}
