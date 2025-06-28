using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class FairyBottleUI : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override bool Visible => true;

        public static float OffsetX = 0;

        public UIPanel FightFairyPanel;
        public UIPanel ContainFairyPanel;
        public UIInformationIcon FightIcon;
        public UIInformationIcon ContainIcon;

        public SortButton sortButton;

        public FairyBottleHang bottleHang;

        /// <summary>
        /// 是否显示容纳的仙灵
        /// </summary>
        public bool ShowContains;

        private int time;
        private bool init;

        public override void OnInitialize()
        {
            FightFairyPanel = new UIPanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
                ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"));
            FightFairyPanel.BackgroundColor = (Color.Coral * 0.25f) with { A = 75 };
            FightFairyPanel.BorderColor = new Color(255, 200, 200) * 0.75f;

            ContainFairyPanel = new UIPanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
                ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"));
            ContainFairyPanel.BackgroundColor = (Color.DarkBlue * 0.25f) with { A = 75 };
            ContainFairyPanel.BorderColor = Color.SkyBlue * 0.75f;

            FightIcon = new UIInformationIcon(ModContent.Request<Texture2D>(AssetDirectory.UI + "FightFairyIcon"));
            ContainIcon = new UIInformationIcon(ModContent.Request<Texture2D>(AssetDirectory.UI + "ContainFairyIcon"));
            sortButton = new SortButton(); 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory)
            {
                if (ShowContains)
                {
                    ShowContains = false;
                    Recalculate();
                }
                //if (time > 0)
                //{
                //    time++;
                //    Recalculate();
                //}
            }
            else
            {
                //init = false;

                if (!init)
                {
                    init = true;
                    Recalculate();
                }
            }
        }

        public override void Recalculate()
        {
            base.Recalculate();

            RemoveAllChildren();

            if (bottleHang == null)
            {
                bottleHang = new FairyBottleHang();
                bottleHang.SetCenter(new Vector2(610 + OffsetX, -80));
            }
            else
                bottleHang.SetCenter(new Vector2(610 + OffsetX, bottleHang.Top.Pixels + bottleHang.Height.Pixels / 2 - 5));

            Append(bottleHang);

            Player p = Main.LocalPlayer;

            if (ShowContains && p.TryGetModPlayer(out FairyCatcherPlayer fcp) && !fcp.BottleItem.IsAir
                && fcp.BottleItem.ModItem is BaseFairyBottle bottle)
            {
                AddFightPanel(bottle);
                AddContainPanel(bottle);
            }
        }

        /// <summary>
        /// 在此加入战斗仙灵的面板
        /// </summary>
        /// <param name="bottle"></param>
        private void AddFightPanel(BaseFairyBottle bottle)
        {
            FightFairyPanel?.SetTopLeft(30, bottleHang.Left.Pixels + bottleHang.Width.Pixels + 15);
            FightFairyPanel?.RemoveAllChildren();

            int count = bottle.FightCapacity;
            //面板宽度，固定
            int width = 10 * 46;
            //面板高度，动态变化
            int height = (count / 10 + (count % 10 > 0 ? 1 : 0)) * 50 + 18;
            FightFairyPanel?.SetSize(width, height);
            var grid = new FixedUIGrid();
            for (int i = 0; i < count; i++)
                grid.Add(new FairyBottleSlot(true, i));

            grid.SetSize(0, 0, 1, 1);
            FightFairyPanel?.Append(grid);

            Append(FightFairyPanel);

            FightIcon.SetSize();
            FightIcon.SetCenter(new Vector2(FightFairyPanel.Left.Pixels, FightFairyPanel.Top.Pixels));
            FightIcon.SetText(FairySystem.FightFairy);
            Append(FightIcon);
        }

        private void AddContainPanel(BaseFairyBottle bottle)
        {
            ContainFairyPanel?.SetTopLeft(10 + FightFairyPanel.Top.Pixels + FightFairyPanel.Height.Pixels, bottleHang.Left.Pixels + bottleHang.Width.Pixels + 15);
            ContainFairyPanel?.RemoveAllChildren();

            int count = bottle.ContainCapacity;
            //面板宽度，固定
            int width = 10 * 46;
            //面板高度，动态变化
            int height = (count / 10 + (count % 10 > 0 ? 1 : 0)) * 50 + 18;
            ContainFairyPanel?.SetSize(width, height);

            var grid = new FixedUIGrid();
            for (int i = 0; i < count; i++)
                grid.Add(new FairyBottleSlot(false, i));

            grid.SetSize(0, 0, 1, 1);
            ContainFairyPanel?.Append(grid);

            Append(ContainFairyPanel);

            ContainIcon.SetSize();
            ContainIcon.SetCenter(new Vector2(ContainFairyPanel.Left.Pixels, ContainFairyPanel.Top.Pixels));
            ContainIcon.SetText(FairySystem.ContainsFairy);
            Append(ContainIcon);

            sortButton.SetSize();
            sortButton.SetCenter(new Vector2(ContainFairyPanel.Left.Pixels
                , ContainFairyPanel.Top.Pixels + ContainIcon.Height.Pixels / 2 + 10 + sortButton.Height.Pixels / 2));
            Append(sortButton);
        }

        public void ShowUI()
        {
            Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0);

            Recalculate();
        }

        public void CloseBottlePanel()
        {
            ShowContains = false;
            Recalculate();
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }
    }

    /// <summary>
    /// 吊着仙灵瓶的藤蔓
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.UI)]
    public class FairyBottleHang : UIElement
    {
        public static int VineType = 0;

        public static ATex Vine { get; set; }
        public static ATex Vine2 { get; set; }

        public FairyBottleHang()
        {
            this.SetSize(52, 60);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory && GetDimensions().Y > -80)
            {
                Top.Set(Helper.Lerp(Top.Pixels, -80, 0.15f), 0);
                Recalculate();
            }
            else if (Main.playerInventory && GetDimensions().Y < 62)
            {
                Top.Set(Helper.Lerp(Top.Pixels, 62, 0.15f), 0);
                Recalculate();
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            Helper.PlayPitched("Fairy/ButtonTick", 0.4f, 0);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Player p = Main.LocalPlayer;

            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp) || !p.ItemTimeIsZero)
                return;

            //放入
            if (p.selectedItem == 58 && fcp.BottleItem.IsAir && !p.HeldItem.IsAir && p.HeldItem.ModItem is BaseFairyBottle)
            {
                fcp.BottleItem = p.HeldItem.Clone();
                p.HeldItem.TurnToAir();
                Main.mouseItem.TurnToAir();

                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);
                goto over;
            }

            //取出
            if (p.inventory[58].IsAir && !fcp.BottleItem.IsAir)
            {
                p.inventory[58] = fcp.BottleItem.Clone();
                Main.mouseItem = fcp.BottleItem.Clone();

                fcp.BottleItem.TurnToAir();
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);

                goto over;
            }

            //交换
            if (p.selectedItem == 58 && !p.HeldItem.IsAir && !fcp.BottleItem.IsAir && p.HeldItem.ModItem is BaseFairyBottle)
            {
                Item i = fcp.BottleItem;
                fcp.BottleItem = p.HeldItem;
                p.inventory[58] = i;
                Main.mouseItem = i.Clone();
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);

                goto over;
            }

        over:
            if (fcp.BottleItem.IsAir)
            {
                FairyBottleUI fairyBottleUI = UILoader.GetUIState<FairyBottleUI>();
                fairyBottleUI.ShowContains = false;
                fairyBottleUI.Recalculate();
            }
            else
            {
                FairyBottleUI fairyBottleUI = UILoader.GetUIState<FairyBottleUI>();
                fairyBottleUI.ShowContains = true;
                fairyBottleUI.Recalculate();
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            Player p = Main.LocalPlayer;

            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return;

            if (fcp.BottleItem.IsAir)
                return;

            FairyBottleUI fairyBottleUI = UILoader.GetUIState<FairyBottleUI>();
            fairyBottleUI.ShowContains = !fairyBottleUI.ShowContains;
            fairyBottleUI.Recalculate();
            Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            //绘制藤蔓线
            Texture2D vineTex = VineType switch
            {
                0 => Vine.Value,
                _ => Vine2.Value,
            };

            var d = GetDimensions();
            Vector2 pos = d.Center() + new Vector2(0, -d.Height / 2);

            spriteBatch.Draw(vineTex, pos + new Vector2(0, -65), null, Color.White, 0, new Vector2(vineTex.Width / 2, 0), 1, 0, 0);

            Player p = Main.LocalPlayer;

            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return;

            //绘制仙灵瓶物品
            if (!fcp.BottleItem.IsAir)
            {
                Helper.GetItemTexAndFrame(fcp.BottleItem.type, out Texture2D tex, out Rectangle frameBox);

                spriteBatch.Draw(tex, d.Center(), frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);

                if (IsMouseHovering)
                {
                    Main.HoverItem = fcp.BottleItem.Clone();
                    Main.hoverItemName = "a";
                }
            }

            //CoraliteNotePanel.DrawDebugFrame(this, spriteBatch);
        }
    }

    public class FairyBottleSlot : UIElement
    {
        /// <summary>
        /// 判断是否是战斗仙灵，是的话就用战斗仙灵的
        /// </summary>
        private readonly bool fight;
        private readonly int _index;
        private float _scale = 1f;

        public FairyBottleSlot(bool fight, int index)
        {
            this.fight = fight;
            _index = index;
            this.SetSize(44, 44);
        }

        public override int CompareTo(object obj)
        {
            if (obj is FairyBottleSlot slot)
                return _index.CompareTo(slot._index);

            return 0;
        }

        public bool TryGetItem(out Item item)
        {
            Player p = Main.LocalPlayer;

            item = null;
            if (p.TryGetModPlayer(out FairyCatcherPlayer fcp) && !fcp.BottleItem.IsAir
                && fcp.BottleItem.ModItem is BaseFairyBottle bottle)
            {
                if (fight)
                {
                    if (bottle.FightFairies.IndexInRange(_index))
                    {
                        item = bottle.FightFairies[_index];
                        if (item == null)
                        {
                            UILoader.GetUIState<FairyBottleUI>().Recalculate();
                            return false;
                        }
                    }
                }
                else
                {
                    if (bottle.ContainFairies.IndexInRange(_index))
                    {
                        item = bottle.ContainFairies[_index];
                        if (item == null)
                        {
                            UILoader.GetUIState<FairyBottleUI>().Recalculate();
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        private void HandleItemSlotLogic()
        {
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Player p = Main.LocalPlayer;

                if (p.TryGetModPlayer(out FairyCatcherPlayer fcp) && !fcp.BottleItem.IsAir
                    && fcp.BottleItem.ModItem is BaseFairyBottle bottle)
                {
                    Main.LocalPlayer.mouseInterface = true;

                    if (fight)
                    {
                        Item inv = bottle.FightFairies[_index];

                        if (bottle.FightFairies.IndexInRange(_index))
                        {
                            ItemSlot.OverrideHover(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.MouseHover(ref inv, ItemSlot.Context.VoidItem);

                            if (Main.mouseItem.IsAir || Main.mouseItem.ModItem is BaseFairyItem)
                            {
                                ItemSlot.LeftClick(ref inv, ItemSlot.Context.VoidItem);
                                ItemSlot.RightClick(ref inv, ItemSlot.Context.VoidItem);
                            }
                            bottle.FightFairies[_index] = inv;
                        }
                    }
                    else
                    {
                        Item inv = bottle.ContainFairies[_index];

                        if (bottle.ContainFairies.IndexInRange(_index))
                        {
                            ItemSlot.OverrideHover(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.MouseHover(ref inv, ItemSlot.Context.VoidItem);

                            if (Main.mouseItem.IsAir || Main.mouseItem.ModItem is BaseFairyItem)
                            {
                                ItemSlot.LeftClick(ref inv, ItemSlot.Context.VoidItem);
                                ItemSlot.RightClick(ref inv, ItemSlot.Context.VoidItem);
                            }

                            bottle.ContainFairies[_index] = inv;
                        }
                    }
                }

                _scale = Helper.Lerp(_scale, 0.8f, 0.2f);

                //if ((Main.mouseRightRelease && Main.mouseRight) || (Main.mouseLeftRelease && Main.mouseLeft))
                //{
                //    SendData();
                //}
            }
            else
                _scale = Helper.Lerp(_scale, 0.75f, 0.2f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!TryGetItem(out Item inv))
                return;

            HandleItemSlotLogic();

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            if (inv != null)
                ItemSlot.Draw(spriteBatch, ref inv, fight ? ItemSlot.Context.VoidItem : ItemSlot.Context.InventoryCoin, position, Color.White);

            Main.inventoryScale = scale;
        }
    }

    public class SortButton : UIElement
    {
        private readonly ATex _tex;
        private static BaseFairyBottle.SortStyle _sortStyle;

        public SortButton()
        {
            _tex = ModContent.Request<Texture2D>(AssetDirectory.UI + "FairySortButton");
        }

        public void SetSize()
        {
            this.SetSize(_tex.Width(), _tex.Height() / 7);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Player p = Main.LocalPlayer;

            if (p.TryGetModPlayer(out FairyCatcherPlayer fcp) && !fcp.BottleItem.IsAir
                && fcp.BottleItem.ModItem is BaseFairyBottle bottle)
            {
                bottle.Sort(_sortStyle);
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            _sortStyle++;
            if (_sortStyle>BaseFairyBottle.SortStyle.ByStaminaLevel)
                _sortStyle = BaseFairyBottle.SortStyle.ByRarity;

            Player p = Main.LocalPlayer;

            if (p.TryGetModPlayer(out FairyCatcherPlayer fcp) && !fcp.BottleItem.IsAir
                && fcp.BottleItem.ModItem is BaseFairyBottle bottle)
            {
                bottle.Sort(_sortStyle);
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            _tex.Value.QuickCenteredDraw(spriteBatch, new Rectangle(0,(int)_sortStyle,1,7),GetDimensions().Center(), Color.White, scale: IsMouseHovering ? 0.85f : 0.8f);
            
            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                string text = _sortStyle switch
                {
                    BaseFairyBottle.SortStyle.ByLifeMaxLevel => FairySystem.SortByLifeMax.Value,
                    BaseFairyBottle.SortStyle.ByDamageLevel => FairySystem.SortByDamage.Value,
                    BaseFairyBottle.SortStyle.ByDefenceLevel => FairySystem.SortByDefence.Value,
                    BaseFairyBottle.SortStyle.BySpeedLevel => FairySystem.SortBySpeed.Value,
                    BaseFairyBottle.SortStyle.BySkillLevelLevel => FairySystem.SortBySkillLevel.Value,
                    BaseFairyBottle.SortStyle.ByStaminaLevel => FairySystem.SortByStamina.Value,
                    _ => FairySystem.SortByRarity.Value
                };

                text += "\n" + FairySystem.HowToSort.Value;
                UICommon.TooltipMouseText(text);
            }
        }
    }
}
