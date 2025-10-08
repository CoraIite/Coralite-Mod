using Coralite.Content.UI;
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
        /// 是否显示需要插入偏振滤镜
        /// </summary>
        public virtual bool ShowPolarizedTip { get; } = true;

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
            Item i = ContentSamples.ItemsByType[TileLoader.GetItemDropFromTypeAndStyle(Framing.GetTileSafely(Entity.Position).TileType)];
            UIElement title = new ComponentUIElementText<ApparatusInformation>(c =>
                 i.Name, this, parent, new Vector2(1.3f));
            parent.Append(title);

            //当前等级
            UIElement text = this.NewTextBar(c =>
            {
                if (ShowPolarizedTip && c.CurrentLevel == MALevel.None)
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

            if (Entity.ExtendFilterCapacity > 0)
                AddFilterController(parent, slot.Top.Pixels + slot.Height.Pixels);
        }

        /// <summary>
        /// 添加滤镜控制器
        /// </summary>
        /// <param name="parent"></param>
        public void AddFilterController(UIElement parent, float height)
        {
            UIElement title = new ComponentUIElementText(() => MagikeSystem.GetUIText(MagikeSystem.UITextID.FilterController), parent, new Vector2(1.3f));
            title.SetTopLeft(height + 10, 0);

            parent.Append(title);
            UIElement Text1 = new ComponentUIElementText(() => MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToRemove), parent);
            Text1.SetTopLeft(title.Top.Pixels + title.Height.Pixels + 8, 0);
            parent.Append(Text1);

            FixedUIGrid grid = new FixedUIGrid();
            for (int i = 0; i < Entity.ExtendFilterCapacity; i++)
                grid.Add(new FilterButton(i));

            grid.SetSize(0, 0, 1, 1);
            grid.SetTopLeft(Text1.Top.Pixels + title.Height.Pixels + 8, 0);

            grid.QuickInvisibleScrollbar();

            parent.Append(grid);
        }
        #endregion

        #region 网络同步

        public override void SendData(ModPacket data)
        {
            data.Write((byte)CurrentLevel);
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            CurrentLevel = (MALevel)reader.ReadByte();
        }

        #endregion

        #region 数据存储

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(CurrentLevel), (int)CurrentLevel);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            CurrentLevel = (MALevel)tag.GetInt(preName + nameof(CurrentLevel));
        }

        #endregion
    }

    public class ApparatusInformation_NoPolar : ApparatusInformation
    {
        public override bool ShowPolarizedTip => false;
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
            Tile tile = Framing.GetTileSafely(_Component.Entity.Position);
            if (!tile.HasTile)
                return;

            Item i = ContentSamples.ItemsByType[TileLoader.GetItemDropFromTypeAndStyle(tile.TileType)].Clone();

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
