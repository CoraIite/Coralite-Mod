using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeFilter : Component
    {
        public sealed override int ID => MagikeComponentID.MagikeFilter;

        public abstract MagikeApparatusLevel Level { get; }

        /// <summary>
        /// 对应的物品类型，在替换时弹出以及在物块破坏时弹出
        /// </summary>
        public abstract int ItemType { get; }

        public override void Update(IEntity entity) { }

        public bool CanInsert(MagikeTileEntity entity, out string text)
        {
            text = "";

            //特殊检测例如：偏振滤镜检测是否能升级
            //这个特殊检测如果没有满足那么将直接返回
            if (!CanInsert_SpecialCheck(entity, ref text))
                return false;

            //检测其他内容，默认检测物块实体内滤镜的是否已满
            if (!PostCheckCanInsert(entity, ref text))
                return false;

            text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.InsertSuccess);
            return true;
        }

        /// <summary>
        /// 特殊检测
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool CanInsert_SpecialCheck(MagikeTileEntity entity, ref string text)
        {
            return true;
        }

        /// <summary>
        /// 在这里检测其他内容，默认检测物块实体内滤镜的是否已满
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool PostCheckCanInsert(MagikeTileEntity entity, ref string text)
        {
            if (!entity.CanInsertFilter())
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.FilterFillUp);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 直接插入滤镜，如果有特殊需要请在这里检测
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(MagikeTileEntity entity)
        {
            entity.AddComponentDirectly(this);

            if (entity == MagikeApparatusPanel.CurrentEntity)
            {
                var ui = UILoader.GetUIState<MagikeApparatusPanel>();

                if (ui.visible)
                    ui.Recalculate();
            }
        }

        public override void OnAdd(IEntity entity)
        {
            for (int i = 0; i < entity.ComponentsCache.Count; i++)
                ChangeComponentValues(entity.ComponentsCache[i]);
        }

        public override void OnRemove(IEntity entity)
        {
            for (int i = 0; i < entity.ComponentsCache.Count; i++)
                RestoreComponentValues(entity.ComponentsCache[i]);
        }

        /// <summary>
        /// 改变组件的属性
        /// </summary>
        /// <param name="component"></param>
        public virtual void ChangeComponentValues(Component component) { }

        /// <summary>
        /// 还原组件的属性
        /// </summary>
        /// <param name="component"></param>
        public virtual void RestoreComponentValues(Component component) { }
    }
}
