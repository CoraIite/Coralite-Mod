﻿using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeFilter : Component
    {
        public sealed override int ID => MagikeComponentID.MagikeFilter;

        public abstract MALevel Level { get; }

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
            entity.AddComponent(this);

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

            SpawnItem(entity);
        }

        /// <summary>
        /// 生成掉落物
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SpawnItem(IEntity entity)
        {
            MagikeTileEntity e = entity as MagikeTileEntity;
            Item.NewItem(new EntitySource_TileEntity(e), Utils.CenteredRectangle(Helper.GetMagikeTileCenter(e.Position), Vector2.One)
                , ItemType);
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

    public class FilterRemoveButton : UIElement
    {
        private IEntity _entity;
        private MagikeFilter _filter;

        private float _scale = 1f;

        public FilterRemoveButton(IEntity entity, MagikeFilter filter)
        {
            Texture2D tex = MagikeSystem.FilterRemoveButton.Value;
            var frameBox = tex.Frame(1, 2);
            Width.Set(frameBox.Width + 8, 0);
            Height.Set(frameBox.Height + 8, 0);

            _entity = entity;
            _filter = filter;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            _entity.RemoveComponent(_filter);
            UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
            base.LeftClick(evt);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeSystem.FilterRemoveButton.Value;

            int frame = 0;

            var position = GetDimensions().Center();

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToRemove));
                frame = 1;
                _scale = Helpers.Helper.Lerp(_scale, 1.1f, 0.2f);
            }
            else
                _scale = Helpers.Helper.Lerp(_scale, 1, 0.2f);

            var frameBox = tex.Frame(1, 2, 0, frame);
            spriteBatch.Draw(tex, position, frameBox, Color.White, 0, frameBox.Size() / 2, _scale, 0, 0);
        }
    }
}
