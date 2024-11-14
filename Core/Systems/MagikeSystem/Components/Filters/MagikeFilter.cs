using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using InnoVault;
using InnoVault.TileProcessors;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeFilter : MagikeComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeFilter;

        public abstract MALevel Level { get; }

        public int whoAmI = -1;

        /// <summary>
        /// 对应的物品类型，在替换时弹出以及在物块破坏时弹出
        /// </summary>
        public abstract int ItemType { get; }

        public override void Update() { }

        public bool CanInsert(MagikeTP entity, out string text)
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
        public virtual bool CanInsert_SpecialCheck(MagikeTP entity, ref string text)
        {
            return true;
        }

        /// <summary>
        /// 在这里检测其他内容，默认检测物块实体内滤镜的是否已满
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public virtual bool PostCheckCanInsert(MagikeTP entity, ref string text)
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
        public virtual void Insert(MagikeTP entity)
        {
            entity.AddComponent(this);

            if (entity == MagikeApparatusPanel.CurrentEntity)
            {
                var ui = UILoader.GetUIState<MagikeApparatusPanel>();

                if (ui.visible)
                    ui.Recalculate();
            }
        }

        public override void OnAdd(MagikeTP entity)
        {
            for (int i = 0; i < entity.ComponentsCache.Count; i++)
                ChangeComponentValues(entity.ComponentsCache[i]);
        }

        public override void OnRemove(MagikeTP entity)
        {
            for (int i = 0; i < entity.ComponentsCache.Count; i++)
                RestoreComponentValues(entity.ComponentsCache[i]);

            SpawnItem(entity);
        }

        /// <summary>
        /// 生成掉落物
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SpawnItem(MagikeTP entity)
        {
            if (!VaultUtils.isClient)
            {
                IEntitySource source = new EntitySource_WorldGen($"MagikeTP:{entity.ID}");
                int type = Item.NewItem(source, Utils.CenteredRectangle(Helper.GetMagikeTileCenter(entity.Position), Vector2.One), ItemType);
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, type, 0f, 0f, 0f, 0, 0, 0);
            }
        }

        /// <summary>
        /// 改变组件的属性
        /// </summary>
        /// <param name="component"></param>
        public virtual void ChangeComponentValues(MagikeComponent component) { }

        /// <summary>
        /// 还原组件的属性
        /// </summary>
        /// <param name="component"></param>
        public virtual void RestoreComponentValues(MagikeComponent component) { }
    }

    public class FilterRemoveButton : UIElement
    {
        private MagikeTP _entity;
        private MagikeFilter _filter;

        private float _scale = 1f;

        public FilterRemoveButton(MagikeTP entity, MagikeFilter filter)
        {
            Texture2D tex = MagikeSystem.FilterRemoveButton.Value;
            var frameBox = tex.Frame(1, 2);
            Width.Set(frameBox.Width + 8, 0);
            Height.Set(frameBox.Height + 8, 0);

            _entity = entity;
            _filter = filter;
        }

        internal void Send_LeftClick_Data(MagikeTP tP)
        {
            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CLNetWorkEnum.FilterRemoveButton_LeftClick);
            modPacket.Write(tP.ID);
            modPacket.Write(tP.Position.X);
            modPacket.Write(tP.Position.Y);
            modPacket.Write(_filter.whoAmI);
            modPacket.Send();
        }

        internal static void Hander_LeftClick_Data(BinaryReader reader, int whoAmI)
        {
            int id = reader.ReadInt32();
            short posX = reader.ReadInt16();
            short posY = reader.ReadInt16();
            int filterWhoAmI = reader.ReadInt32();
            TileProcessor tp = TileProcessorLoader.FindModulePreciseSearch(id, posX, posY);
            if (tp != null && tp is MagikeTP magikeTP)
            {
                MagikeFilter _filter = null;
                foreach (var components in magikeTP.ComponentsCache)
                {
                    if (components is MagikeFilter filter && filter.whoAmI == filterWhoAmI)
                    {
                        _filter = filter;
                    }
                }
                if (_filter != null)
                {
                    magikeTP.RemoveComponent(_filter);
                    if (Main.dedServ)
                    {
                        ModPacket modPacket = Coralite.Instance.GetPacket();
                        modPacket.Write((byte)CLNetWorkEnum.FilterRemoveButton_LeftClick);
                        modPacket.Write(id);
                        modPacket.Write(posX);
                        modPacket.Write(posY);
                        modPacket.Write(filterWhoAmI);
                        modPacket.Send(-1, whoAmI);
                    }
                }
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            _entity.RemoveComponent(_filter);
            UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
            if (VaultUtils.isClient )
            {
                Send_LeftClick_Data(_entity);
            }
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
