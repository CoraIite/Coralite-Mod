using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.ObjectData;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class Charger : MagikeFactory, IUIShowable
    {
        /// <summary>
        /// 为顶部的物品充能
        /// </summary>
        public bool ChargeItemsOnUp { get; set; } = true;

        /// <summary>
        /// 为顶部的玩家身上的物品充能
        /// </summary>
        public bool ChargePlayerItemsOnUp { get; set; } = true;

        /// <summary>
        /// 单个物品每次能充能多少
        /// </summary>
        public int MagikePerCharge { get; set; }

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";

            var items = GetChargeItems();

            if (items.Count < 1)
            {
                text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerItemNotFound);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取所有的充能物品
        /// </summary>
        /// <returns></returns>
        public List<MagikeItem> GetChargeItems()
        {
            List<MagikeItem> items = new List<MagikeItem>();

            if (Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                foreach (Item item in container.Items)
                {
                    if (item.IsAir || !item.IsMagikeChargable())
                        continue;

                    items.Add(item.GetMagikeItem());
                }
            }

            if (ChargeItemsOnUp)//遍历物品，检测是否在上方
            {
                MagikeHelper.GetMagikeAlternateData(Entity.Position.X, Entity.Position.Y, out TileObjectData data, out _);

                Rectangle selfRect = new Rectangle(Entity.Position.X * 16, (Entity.Position.Y - 2) * 16, data.Width * 16, 16 * 2);

                for (int i = 0; i < Main.maxItems; i++)
                {
                    Item item = Main.item[i];
                    if (item == null || item.IsAir || !item.IsMagikeChargable())
                        continue;

                    if (item.getRect().Intersects(selfRect))
                        items.Add(item.GetMagikeItem());
                }
            }

            if (ChargePlayerItemsOnUp)//遍历玩家，检测是否在上方
            {
                MagikeHelper.GetMagikeAlternateData(Entity.Position.X, Entity.Position.Y, out TileObjectData data, out _);

                Rectangle selfRect = new Rectangle(Entity.Position.X * 16, (Entity.Position.Y - 2) * 16, data.Width * 16, 16 * 2);

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];

                    if (p == null || !p.Alives())
                        continue;

                    if (p.getRect().Intersects(selfRect))
                    {
                        foreach (var item in p.inventory)
                            if (!item.IsAir && item.IsMagikeChargable())
                                items.Add(item.GetMagikeItem());

                        if (p.useVoidBag())
                            foreach (var item in p.bank4.item)
                                if (!item.IsAir && item.IsMagikeChargable())
                                    items.Add(item.GetMagikeItem());
                    }
                }
            }

            return items;
        }

        public override bool DuringWork()
        {
            OnWorking();

            //每隔固定时间充能一次
            if (UpdateTime())
            {
                if (ChargeAll())
                    return false;
                else
                {
                    Timer = WorkTime;
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 充能所有的物品，并检测所有的物品是否充满了，返回<see langword="true"/>就是全部充满了
        /// </summary>
        /// <returns></returns>
        public bool ChargeAll()
        {
            var items = GetChargeItems();

            if (items.Count < 1)
                return true;

            var container = Entity.GetMagikeContainer();

            int currentMagike = container.Magike;
            int cosumeMagike = 0;
            bool hasItemNotFillUp = false;

            foreach (MagikeItem item in items)
            {
                if (item.FillUp)
                    continue;

                int charge = MagikePerCharge;
                int magikeLeast = currentMagike - cosumeMagike;//自身还剩多少魔能
                int magikeCanCharge = item.MagikeMax - item.Magike;//物品还能容纳多少魔能

                //检测当前的魔能是否足够去充能，不够就充当前的
                if (charge > magikeLeast)
                    charge = magikeLeast;

                if (charge > magikeCanCharge)
                    charge = magikeCanCharge;

                item.Charge(charge);//充能物品，并记录是否有充满
                if (!item.FillUp)
                    hasItemNotFillUp = true;

                if (magikeLeast < 1)//自身莫得了那就跳出循环
                    break;

                cosumeMagike += charge;
            }

            container.ReduceMagike(cosumeMagike);

            if (hasItemNotFillUp)
                return false;

            return true;
        }

        public override void OnWorking()
        {
            if (Timer % 2 == 0)
            {
                MagikeHelper.GetMagikeAlternateData(Entity.Position.X, Entity.Position.Y, out TileObjectData data, out _);

                Rectangle selfRect = new Rectangle(Entity.Position.X * 16, (Entity.Position.Y - 2) * 16, data.Width * 16, 16 * 2);

                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(selfRect), DustID.FireworksRGB, new Vector2(0, -Main.rand.NextFloat(2, 4))
                    , newColor: Coralite.MagicCrystalPink, Scale: Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = true;
            }
        }

        #region UI

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.Charger, parent);

            var text = this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerDescription), parent);
            text.SetTopLeft(title.Height.Pixels + 5, 0);

            parent.Append(text);

            var text2 = this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerPerCharge) + MagikePerCharge, parent);
            text2.SetTopLeft(text.Height.Pixels + text.Top.Pixels + 5, 0);

            parent.Append(text2);

            ChargerItemButton button1 = new ChargerItemButton(this);
            ChargerPlayerButton button2 = new ChargerPlayerButton(this);

            button1.SetTopLeft(text2.Top.Pixels + text2.Height.Pixels, 0);
            button2.SetTopLeft(button1.Top.Pixels, button1.Width.Pixels + 5);

            parent.Append(button1);
            parent.Append(button2);

            if (Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                UIGrid grid = new()
                {
                    OverflowHidden = false
                };

                for (int i = 0; i < container.Items.Length; i++)
                {
                    ItemContainerSlot slot = new(container, i);
                    grid.Add(slot);
                }

                grid.SetSize(0, -button1.Top.Pixels - button1.Height.Pixels, 1, 1);
                grid.SetTopLeft(button1.Top.Pixels + button1.Height.Pixels + 6, 0);

                parent.Append(grid);
            }
        }

        #endregion

        #region 同步

        #endregion

        #region 存储

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);

            if (ChargeItemsOnUp)
                tag.Add(nameof(ChargeItemsOnUp), true);
            if (ChargePlayerItemsOnUp)
                tag.Add(nameof(ChargePlayerItemsOnUp), true);

            tag.Add(nameof(MagikePerCharge), MagikePerCharge);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);

            if (tag.ContainsKey(nameof(ChargeItemsOnUp)))
                ChargeItemsOnUp = true;
            if (tag.ContainsKey(nameof(ChargePlayerItemsOnUp)))
                ChargePlayerItemsOnUp = true;

            MagikePerCharge = tag.GetInt(nameof(MagikePerCharge));
        }

        #endregion
    }

    public class ChargerItemButton : UIElement
    {
        private float _scale = 1f;
        private Charger _charger;

        public ChargerItemButton(Charger charger)
        {
            Texture2D mainTex = MagikeSystem.ChargerItemButton.Value;

            var frameBox = mainTex.Frame(2, 1);
            this.SetSize(frameBox.Width + 6, frameBox.Height + 6);

            _charger = charger;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            _charger.ChargeItemsOnUp = !_charger.ChargeItemsOnUp;
            Helper.PlayPitched("UI/Tick", 0.4f, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.ChargerItemButton.Value;
            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);

                string text;
                if (_charger.ChargeItemsOnUp)
                    text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerChargeItem);
                else
                    text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerNotChargeItem);

                UICommon.TooltipMouseText(text);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(2, 1, _charger.ChargeItemsOnUp ? 0 : 1);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2, _scale, 0, 0);
        }
    }

    public class ChargerPlayerButton : UIElement
    {
        private float _scale = 1f;
        private Charger _charger;

        public ChargerPlayerButton(Charger charger)
        {
            Texture2D mainTex = MagikeSystem.ChargerPlayerButton.Value;

            var frameBox = mainTex.Frame(2, 1);
            this.SetSize(frameBox.Width + 6, frameBox.Height + 6);

            _charger = charger;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            _charger.ChargePlayerItemsOnUp = !_charger.ChargePlayerItemsOnUp;
            Helper.PlayPitched("UI/Tick", 0.4f, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeSystem.ChargerPlayerButton.Value;
            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);

                string text;
                if (_charger.ChargePlayerItemsOnUp)
                    text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerChargePlayer);
                else
                    text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerNotChargePlayer);

                UICommon.TooltipMouseText(text);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(2, 1, _charger.ChargePlayerItemsOnUp ? 0 : 1);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2, _scale, 0, 0);
        }
    }
}
