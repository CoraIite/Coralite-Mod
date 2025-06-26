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
            FightFairyPanel.BackgroundColor = Color.OrangeRed * 0.5f;
            FightFairyPanel.BorderColor = Color.LightCoral * 0.75f;

            ContainFairyPanel = new UIPanel(ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground"),
                ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBorder"));
            ContainFairyPanel.BackgroundColor = Color.Cyan * 0.5f;
            ContainFairyPanel.BorderColor = Color.LightCyan * 0.75f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory)
            {
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

            bottleHang ??= new FairyBottleHang();
            bottleHang?.SetCenter(new Vector2(610 + OffsetX, -80));

            Append(bottleHang);

            Player p = Main.LocalPlayer;

            if (ShowContains&& p.TryGetModPlayer(out FairyCatcherPlayer fcp)&&!fcp.BottleItem.IsAir
                &&fcp.BottleItem.ModItem is BaseFairyBottle bottle)
            {
                FightFairyPanel?.SetCenter(new Vector2(bottleHang.Left.Pixels + bottleHang.Width.Pixels + 10, 30));

                int count = bottle.FightCapacity;
                int maxwidth = count > 10 ? 10 : 10;
                Append(FightFairyPanel);
            }
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
        public static int VineType=0;

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
                Top.Set(Helper.Lerp(Top.Pixels,-80,0.15f), 0);
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
                return;
            }

            //取出
            if (p.inventory[58].IsAir && !fcp.BottleItem.IsAir)
            {
                p.inventory[58] = fcp.BottleItem.Clone();
                Main.mouseItem = fcp.BottleItem.Clone();

                fcp.BottleItem.TurnToAir();
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);

                return;
            }

            //交换
            if (p.selectedItem == 58 && !p.HeldItem.IsAir && !fcp.BottleItem.IsAir && p.HeldItem.ModItem is BaseFairyBottle)
            {
                Item i = fcp.BottleItem;
                fcp.BottleItem = p.HeldItem;
                p.inventory[58] = i;
                Main.mouseItem = i.Clone();
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0);

                return;
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

            UILoader.GetUIState<FairyBottleUI>().ShowContains = true;
            UILoader.GetUIState<FairyBottleUI>().Recalculate();
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

            spriteBatch.Draw(vineTex, pos+new Vector2(0,-65),null, Color.White,0,new Vector2(vineTex.Width/2,0),1,0,0);

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
        private bool fight;
        private readonly int _index;
        private float _scale = 1f;

        public FairyBottleSlot(bool fight, int index)
        {
            this.fight = fight;
            _index = index;
            this.SetSize(54, 54);
        }

        public bool TryGetItem(out Item item)
        {
            Player p = Main.LocalPlayer;

            item = null;
            if (p.TryGetModPlayer(out FairyCatcherPlayer fcp)&&!fcp.BottleItem.IsAir
                &&fcp.BottleItem.ModItem is BaseFairyBottle bottle)
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
                Player p = Main.LocalPlayer;

                if (p.TryGetModPlayer(out FairyCatcherPlayer fcp) && !fcp.BottleItem.IsAir
                    && fcp.BottleItem.ModItem is BaseFairyBottle bottle)
                {
                    if (fight)
                    {
                        if (bottle.FightFairies.IndexInRange(_index))
                        {
                            Item inv = bottle.FightFairies[_index];
                            Main.LocalPlayer.mouseInterface = true;
                            ItemSlot.OverrideHover(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.LeftClick(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.RightClick(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.MouseHover(ref inv, ItemSlot.Context.VoidItem);
                            bottle.FightFairies[_index] = inv;
                        }
                    }
                    else
                    {
                        if (bottle.ContainFairies.IndexInRange(_index))
                        {
                            Item inv = bottle.ContainFairies[_index];
                            Main.LocalPlayer.mouseInterface = true;
                            ItemSlot.OverrideHover(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.LeftClick(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.RightClick(ref inv, ItemSlot.Context.VoidItem);
                            ItemSlot.MouseHover(ref inv, ItemSlot.Context.VoidItem);
                            bottle.ContainFairies[_index] = inv;
                        }
                    }
                }

                _scale = Helper.Lerp(_scale, 1.05f, 0.2f);

                //if ((Main.mouseRightRelease && Main.mouseRight) || (Main.mouseLeftRelease && Main.mouseLeft))
                //{
                //    SendData();
                //}
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!TryGetItem(out Item inv))
                return;

            HandleItemSlotLogic();

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref inv, ItemSlot.Context.VoidItem, position, Color.White);

            Main.inventoryScale = scale;
        }
    }

}
