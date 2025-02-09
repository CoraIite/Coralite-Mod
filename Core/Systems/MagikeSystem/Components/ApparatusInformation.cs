using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class ApparatusInformation : MagikeComponent, IUpgradeable, IUIShowable
    {
        public override int ID => MagikeComponentID.ApparatusInformation;

        /// <summary>
        /// 当前仪器的等级，用于显示名字
        /// </summary>
        public MALevel CurrentLevel { get; private set; }
        /// <summary>
        /// 自身物品类型
        /// </summary>
        public Item SelfItem { get; set; }

        public sealed override void Update() { }

        #region 升级

        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual void Upgrade(MALevel incomeLevel)
        {
            CurrentLevel = incomeLevel;
        }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        #endregion

        #region UI

        public void ShowInUI(UIElement parent)
        {
            //标题
            Item i = SelfItem;
            UIElement title = new ComponentUIElementText<ApparatusInformation>(c =>
                 i.Name, this, parent, new Vector2(1.3f));
            parent.Append(title);

            //当前等级
            UIElement text = this.NewTextBar(c =>
            {
                if (c.CurrentLevel == MALevel.None)
                    return MagikeSystem.GetUIText(MagikeSystem.UITextID.NeedPolarizedFilter);
                else
                    return MagikeSystem.GetUIText(MagikeSystem.UITextID.CurrentLevel) + MagikeSystem.GetMALevelText(c.CurrentLevel);
            }, parent);

            text.SetTopLeft(title.Height.Pixels + 8, 0);
            parent.Append(text);

            //物品格
            ShowOnlySlot slot = new ShowOnlySlot(this);
            slot.SetTopLeft(text.Top.Pixels + text.Height.Pixels + 8, 0);
            parent.Append(slot);
        }

        #endregion

        #region 网络同步

        public override void SendData(ModPacket data)
        {
            data.Write((int)CurrentLevel);
            ItemIO.Send(SelfItem, data);
            data.Write(SelfItem.type);
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            CurrentLevel = (MALevel)reader.ReadInt32();
            SelfItem = ItemIO.Receive(reader);
        }

        #endregion

        #region 数据存储

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(CurrentLevel), (int)CurrentLevel);
            tag.Add(preName + nameof(SelfItem), SelfItem);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            CurrentLevel = (MALevel)tag.GetInt(preName + nameof(CurrentLevel));
            SelfItem = tag.Get<Item>(preName + nameof(SelfItem));
        }

        #endregion
    }

    public class ShowOnlySlot : UIElement
    {
        private readonly ApparatusInformation _Component;
        private float _scale = 1f;

        public ShowOnlySlot(ApparatusInformation container)
        {
            _Component = container;
            this.SetSize(54, 54);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Item i = _Component.SelfItem.Clone();

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                ItemSlot.OverrideHover(ref i, ItemSlot.Context.ShopItem);
                ItemSlot.MouseHover(ref i, ItemSlot.Context.ShopItem);
                _scale = Helper.Lerp(_scale, 1.1f, 0.2f);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref i, ItemSlot.Context.ShopItem, position, Color.White);

            Main.inventoryScale = scale;
        }
    }
}
