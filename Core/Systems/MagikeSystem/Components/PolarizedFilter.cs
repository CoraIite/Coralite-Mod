using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

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
            if (!entity.ComponentsCache.Any(c => c is IUpgradeable))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.UpgradeableNotFound);
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

        public override void OnRemove(IEntity entity)
        {
            //生成物品
            MagikeTileEntity e = entity as MagikeTileEntity;
            Item.NewItem(new EntitySource_TileEntity(e), Utils.CenteredRectangle(Helper.GetMagikeTileCenter(e.Position), Vector2.One)
                , ItemType);

            base.OnRemove(entity);
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
