﻿using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
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
        public static LocalizedText OnlyItemContainer {  get; set; }

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

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Glistent,
                MALevel.Shadow,
                MALevel.CrystallineMagike,
                MALevel.Hallow,
                MALevel.HolyLight,
            ];
        }

        public override Vector2 GetFloatingOffset(float rotation, MALevel level)
        {
            return level switch
            {
                MALevel.MagicCrystal
                or MALevel.Glistent
                or MALevel.Hallow
                => rotation.ToRotationVector2() * 8,
                MALevel.CrystallineMagike
                or MALevel.Shadow
                => rotation.ToRotationVector2() * 12,
                MALevel.HolyLight => rotation.ToRotationVector2() * 4,
                _ => Vector2.Zero
            };
        }

        public override Vector2 GetRestOffset(float rotation, MALevel level)
        {
            return level switch
            {
                MALevel.MagicCrystal
                or MALevel.Glistent
                or MALevel.Shadow
                or MALevel.CrystallineMagike
                => Vector2.Zero,
                MALevel.Hallow => -6 * rotation.ToRotationVector2(),
                MALevel.HolyLight => -6 * rotation.ToRotationVector2(),
                _ => Vector2.Zero
            };
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

    public class BasicAltarContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 8, 60 * 2),
                MALevel.Glistent => MagikeHelper.CalculateMagikeCost(MALevel.Glistent, 8, 60 * 2),
                MALevel.Shadow => MagikeHelper.CalculateMagikeCost(MALevel.Shadow, 8, 60 * 2),
                MALevel.CrystallineMagike => MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 8, 60 * 2),
                MALevel.Hallow => MagikeHelper.CalculateMagikeCost(MALevel.Hallow, 8, 60 * 2),
                MALevel.HolyLight => MagikeHelper.CalculateMagikeCost(MALevel.HolyLight, 8, 60 * 2),
                MALevel.SplendorMagicore => MagikeHelper.CalculateMagikeCost(MALevel.SplendorMagicore, 8, 60 * 2),
                _ => 0,
            };

            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase * 2;
            //LimitAntiMagikeAmount();
        }
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

            for (int i = _receivers.Count - 1; i >= 0; i--)
            {
                //移除所有超出长度的，没有TP的，以及TP上没有物品容器的
                if (i + 1 > MaxConnect || !MagikeHelper.TryGetEntity(_receivers[i].X, _receivers[i].Y, out var magikeTP)
                    || (!magikeTP.HasComponent(MagikeComponentID.ItemContainer) && !magikeTP.HasComponent(MagikeComponentID.ItemContainer)))
                {
                    _receivers.RemoveAt(i);
                    continue;
                }

                Vector2 targetPos = Helper.GetMagikeTileCenter(_receivers[i]);
                if (Vector2.Distance(selfPos, targetPos) > ConnectLength)
                    _receivers.RemoveAt(i);
            }
        }
    }

    public class BasicAltarAltar : CraftAltar
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            float second = incomeLevel switch
            {
                MALevel.MagicCrystal => 0.5f,
                MALevel.Glistent => 0.45f,
                MALevel.Shadow => 0.4f,
                MALevel.CrystallineMagike => 0.35f,
                MALevel.Hallow => 0.3f,
                MALevel.HolyLight => 0.25f,
                _ => -1,
            };

            CostPercent = incomeLevel switch
            {
                MALevel.MagicCrystal => 0.05f,
                MALevel.Glistent => 0.05f,
                MALevel.Shadow => 0.07f,
                MALevel.CrystallineMagike => 0.1f,
                MALevel.Hallow => 0.1f,
                MALevel.HolyLight => 0.13f,
                _ => 0,
            };

            MinCost = incomeLevel switch
            {
                MALevel.MagicCrystal => 1,
                MALevel.Glistent => 3,
                MALevel.Shadow => 8,
                MALevel.CrystallineMagike => 15,
                MALevel.Hallow => 30,
                MALevel.HolyLight => 50,
                _ => 1,
            };

            WorkTimeBase = (int)(second * 60);
        }
    }
}
