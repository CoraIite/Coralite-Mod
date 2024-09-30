using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ObjectData;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    /// <summary>
    /// 偏振滤镜，用于升级魔能仪器
    /// </summary>
    public abstract class PolarizedFilter : MagikeFilter, IUIShowable
    {
        #region 插入与取出部分

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
            Tile targetTile = Framing.GetTileSafely(entity.Position);
            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(targetTile.TileType, out var keyValuePairs))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.CantUpgrade);
                return false;
            }

            if (!keyValuePairs.ContainsKey(Level))
            {
                text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.CantUpgrade);
                return false;
            }

            return true;
        }

        public override bool PostCheckCanInsert(MagikeTileEntity entity, ref string text)
        {
            //没有其他任何滤镜时
            if (!entity.HasComponent(MagikeComponentID.MagikeFilter))
                return true;

            PolarizedFilter oldFilter = (PolarizedFilter)(entity.Components[MagikeComponentID.MagikeFilter] as List<Component>)
                .FirstOrDefault(c => c is PolarizedFilter, null);

            if (oldFilter != null)
            {
                if (oldFilter.Level == Level)
                {
                    text = MagikeSystem.GetFilterText(MagikeSystem.FilterID.CantUpgrade);
                    return false;
                }

                //有就弹出这个
                if (oldFilter != null)
                {
                    entity.RemoveComponentWithoutOnRemove(oldFilter);
                    oldFilter.SpawnItem(entity);
                    return true;
                }
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
            ChangeTileFrame(Level, entity);
            base.OnAdd(entity);
        }

        public override void OnRemove(IEntity entity)
        {
            ChangeTileFrame(MALevel.None, entity);
            SpawnItem(entity);
            base.OnRemove(entity);
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

        public static void ChangeTileFrame(MALevel level, IEntity entity)
        {
            Point16 topLeft = (entity as MagikeTileEntity).Position;
            Tile tile = Framing.GetTileSafely(topLeft);

            if (!tile.HasTile)
                return;

            MagikeHelper.GetMagikeAlternateData(topLeft.X, topLeft.Y, out TileObjectData data, out _);

            int frameX = MagikeSystem.MagikeApparatusLevels[tile.TileType][level] * data.CoordinateFullWidth;

            for (int i = 0; i < data.Width; i++)
                for (int j = 0; j < data.Height; j++)
                {
                    Tile t = Framing.GetTileSafely(topLeft + new Point16(i, j));

                    t.TileFrameX = (short)(frameX + (i * (data.CoordinateWidth + data.CoordinatePadding)));
                }
        }

        public override void ChangeComponentValues(Component component)
        {
            if (component is IUpgradeable upgrade)
                upgrade.Upgrade(Level);
        }

        public override void RestoreComponentValues(Component component)
        {
            if (component is IUpgradeable upgrade)
                upgrade.Upgrade(MALevel.None);
        }

        #endregion

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.MagikePolarizedFilterName, parent);

            UIList list =
            [
                //等级
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.PolarizedFilterLevel), parent),
                new PolarizedFilterText(c =>
                    $"  ▶ [i:{c.ItemType}]",this,parent),
                //取出按钮
                new FilterRemoveButton(Entity, this)
            ];

            list.SetSize(0, 0, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        #endregion
    }

    public class PolarizedFilterText(Func<PolarizedFilter, string> text, PolarizedFilter component, UIElement parent, Vector2? scale = null) : ComponentUIElementText<PolarizedFilter>(text, component, parent, scale)
    {
        private Item item = new(component.ItemType);

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering)
            {
                Main.HoverItem = item.Clone();
                Main.hoverItemName = "Coralite: PolarizedFilter";
            }
        }
    }
}
