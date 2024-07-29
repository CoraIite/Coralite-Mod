using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    /// <summary>
    /// 偏振滤镜，用于升级魔能仪器
    /// </summary>
    public abstract class PolarizedFilter : MagikeFilter
    {
        public abstract MagikeApparatusLevel UpgradeLevel { get; }

        //只有有可升级的组件才能升级
        public override bool CanInsert_SpecialCheck(MagikeTileEntity entity, ref string text)
        {
            //检测是否有可升级的组件
            if (!entity.ComponentsCache.Any(c => c is IUpgradeable))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.UpgradeableNotFound);
                return false;
            }

            //如果没有对应等级的升级那么就返回
            Terraria.Tile targetTile = Framing.GetTileSafely(entity.Position);
            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(targetTile.TileType,out var keyValuePairs))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.CantUpgrade);
                return false;
            }

            if (!keyValuePairs.ContainsKey(UpgradeLevel))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.CantUpgrade);
                return false;
            }

            return true;
        }

        public override bool PostCheckCanInsert(MagikeTileEntity entity, ref string text)
        {
            //找一下有没有其他的升级组件
            if (!(entity as IEntity).HasComponent(MagikeComponentID.MagikeFilter))
                return true;

            PolarizedFilter oldFilter = (PolarizedFilter)entity.Components[MagikeComponentID.MagikeFilter].FirstOrDefault(c => c is PolarizedFilter, null);

            if (oldFilter.UpgradeLevel==UpgradeLevel)
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.CantUpgrade);
                return false;
            }

            //有就弹出这个
            if (oldFilter != null)
            {
                (entity as IEntity).RemoveComponent(MagikeComponentID.MagikeFilter, oldFilter);
                return true;
            }

            if (!entity.CanInsertFilter())//这里不能插入就是真不能插入了
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.FilterFillUp);
                return false;
            }

            return true;
        }

        public override void OnAdd(IEntity entity)
        {
            ChangeTileFrame(UpgradeLevel, entity);
            base.OnAdd(entity);
        }

        public override void OnRemove(IEntity entity)
        {
            ChangeTileFrame(MagikeApparatusLevel.None, entity);
            //生成物品
            MagikeTileEntity e = entity as MagikeTileEntity;
            Item.NewItem(new EntitySource_TileEntity(e), Utils.CenteredRectangle(Helper.GetMagikeTileCenter(e.Position), Vector2.One)
                , ItemType);

            base.OnRemove(entity);
        }

        public static void ChangeTileFrame(MagikeApparatusLevel level,IEntity entity)
        {
            Point16 topLeft = (entity as MagikeTileEntity).Position;
            Terraria.Tile tile = Framing.GetTileSafely(topLeft);

            if (!tile.HasTile)
                return;

            MagikeHelper.GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData data, out _);

            int frameX = MagikeSystem.MagikeApparatusLevels[tile.TileType][level] * data.CoordinateFullWidth;

            for (int i = 0; i < data.Width; i++)
                for (int j = 0; j < data.Height; j++)
                {
                    Terraria.Tile t = Framing.GetTileSafely(topLeft + new Point16(i, j));

                    t.TileFrameX = (short)(frameX + i * (data.CoordinateWidth + data.CoordinatePadding));
                }
        }

        public override void ChangeComponentValues(Component component)
        {
            if (component is IUpgradeable upgrade)
                upgrade.Upgrade(UpgradeLevel);
        }

        public override void RestoreComponentValues(Component component)
        {
            if (component is IUpgradeable upgrade)
                upgrade.Upgrade(MagikeApparatusLevel.None);
        }
    }
}
