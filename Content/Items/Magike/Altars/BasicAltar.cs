using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Altars
{
    public class BasicAltar() : MagikeApparatusItem(TileType<BasicAltarTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeAltars)
    {
        public static LocalizedText OnlyItemContainer { get; set; }

        public override void Load()
        {
            OnlyItemContainer = this.GetLocalization(nameof(OnlyItemContainer));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(35)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicAltarTile() : BaseCraftAltarTile
        (5, 5, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeAltarTiles + Name;
        public override int DropItemType => ItemType<BasicAltar>();

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                CrystalLevel.ID,
                GlistentLevel.ID,
                ShadowLevel.ID,
                BrilliantLevel.ID,
                HallowLevel.ID,
                HolyLightLevel.ID,
            ];
        }

        public override Vector2 GetFloatingOffset(float rotation, ushort level)
        {
            if (level == CrystalLevel.ID
                || level == GlistentLevel.ID
                || level == HallowLevel.ID)
                return rotation.ToRotationVector2() * 8;

            if (level == BrilliantLevel.ID
                || level == ShadowLevel.ID
                || level == HallowLevel.ID)
                return rotation.ToRotationVector2() * 12;

            if (level == HolyLightLevel.ID)
                return rotation.ToRotationVector2() * 4;

            return Vector2.Zero;
        }

        public override Vector2 GetRestOffset(float rotation, ushort level)
        {
            if (level == HallowLevel.ID
                || level == HolyLightLevel.ID)
                return rotation.ToRotationVector2() * -6;

            return Vector2.Zero;
        }
    }

    public class BasicAltarTileEntity : BaseCraftAltar<BasicAltarTile>
    {
        public override int ExtendFilterCapacity => 5;

        public override CraftAltar GetStartAltar()
            => new BasicAltarAltar();

        public override MagikeContainer GetStartContainer()
            => new BasicAltarContainer();

        public override GetOnlyItemContainer GetStartGetOnlyItemContainer()
            => new()
            {
                CapacityBase = 4
            };


        public override ItemContainer GetStartItemContainer()
            => new()
            {
                CapacityBase = 1
            };

        public override CheckOnlyLinerSender GetStartSender()
            => new BasicAltarSender();
    }

    public class BasicAltarContainer : UpgradeableContainer<BasicAltarTile>
    {
    }

    public class BasicAltarSender : CheckOnlyLinerSender
    {
        public BasicAltarSender()
        {
            ConnectLengthBase = 16 * 8;
            MaxConnectBase = 12;
        }

        public override bool CanConnect_CheckHasEntity(Point16 receiverPoint, ref string failSource)
        {
            if (!MagikeHelper.TryGetEntityWithComponent(receiverPoint.X, receiverPoint.Y, MagikeComponentID.ItemContainer, out _)
                && !MagikeHelper.TryGetEntityWithComponent(receiverPoint.X, receiverPoint.Y, MagikeComponentID.ItemGetOnlyContainer, out _))
            {
                failSource = BasicAltar.OnlyItemContainer.Value;
                return false;
            }

            return true;
        }

        public override void RecheckConnect()
        {
            if (Entity == null)//不要删掉它！！！！
                return;

            Vector2 selfPos = Helper.GetMagikeTileCenter(Entity.Position);

            for (int i = _relativePoses.Count - 1; i >= 0; i--)
            {
                //移除所有超出长度的，没有TP的，以及TP上没有物品容器的
                if (i + 1 > MaxConnect || !MagikeHelper.TryGetEntityWithTopLeft(Entity.Position + _relativePoses[i], out var magikeTP)
                    || (!magikeTP.HasComponent(MagikeComponentID.ItemContainer) && !magikeTP.HasComponent(MagikeComponentID.ItemContainer)))
                {
                    _relativePoses.RemoveAt(i);
                    continue;
                }

                Vector2 targetPos = Helper.GetMagikeTileCenter(Entity.Position + _relativePoses[i]);
                if (Vector2.Distance(selfPos, targetPos) > ConnectLength)
                    _relativePoses.RemoveAt(i);
            }
        }
    }

    public class BasicAltarAltar : CraftAltar,IUpgradeLoadable
    {
        public int TileType => TileType<BasicAltarTile>();

        public override void InitializeLevel()
        {
            WorkTimeBase = -1;
            CostPercent = 0;
            MinCost = 1;
        }

        public override void Upgrade(ushort incomeLevel)
        {
            string name = this.GetDataPreName();
            WorkTimeBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(WorkTimeBase));
            CostPercent = MagikeSystem.GetLevelDataFloat(incomeLevel, name + nameof(CostPercent));
            MinCost = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(WorkTimeBase));

            Timer = WorkTime;
        }
    }
}
