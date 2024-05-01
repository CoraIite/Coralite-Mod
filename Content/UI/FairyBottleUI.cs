using Coralite.Content.UI.FairyEncyclopedia;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class FairyBottleUI : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public bool visible;
        public override bool Visible => visible;

        public static UIDragablePanel panel;
        public static UIGrid itemSlotGrid;
        public static IFairyBottle bottle;

        public State state;

        public enum State
        {
            FadeIn,
            Normal,
            FadeOut
        }

        public override void OnInitialize()
        {
            panel = new UIDragablePanel(true,true,true);

            panel.SetPadding(6);
            //panel.Left.Set(-310f, 0f);
            panel.Top.Set(90f, 0f);
            panel.Width.Set(305, 0f);
            panel.MinWidth.Set(120, 0f);
            panel.MaxWidth.Set(600f, 0f);
            panel.Height.Set(350, 0f);
            panel.MinHeight.Set(120, 0f);
            panel.MaxHeight.Set(550, 0f);

            panel.Left.Set(1920 / 2, 0f);
            panel.Top.Set(1080 / 2, 0f);

            Append(panel);

            itemSlotGrid = new UIGrid();
            itemSlotGrid.Top.Set(40, 0);
            panel.Append(itemSlotGrid);

            panel.OnResizing += Panel_OnResizing;
        }

        private void Panel_OnResizing()
        {
            itemSlotGrid.Width.Set(
                MathHelper.Clamp(panel.Width.Pixels - 18, panel.MinWidth.Pixels, panel.MaxWidth.Pixels)
                , 0);
            itemSlotGrid.Height.Set(
                MathHelper.Clamp(panel.Height.Pixels - 70, panel.MinHeight.Pixels-66, panel.MaxHeight.Pixels - 80)
                , 0);
        }

        public override void Update(GameTime gameTime)
        {
            //panel.MinWidth.Set(120, 0f);
            //panel.MinHeight.Set(120, 0f);

            switch (state)
            {
                case State.FadeIn:
                    {
                        panel.Top.Set((int)Helper.Lerp(panel.Top.Pixels, Main.screenHeight / 4, 0.07f), 0);
                        panel.Left.Set((int)Helper.Lerp(panel.Left.Pixels, Main.screenWidth / 2, 0.07f), 0);
                        panel.Width.Set(Helper.Lerp(panel.Width.Pixels, 305, 0.2f), 0f);
                        panel.Height.Set(Helper.Lerp(panel.Height.Pixels, 350, 0.2f), 0f);
                        panel.BackgroundColor = Color.Lerp(panel.BackgroundColor, new Color(63, 82, 151) * 0.7f, 0.15f);

                        if (Vector2.Distance(
                            new Vector2(Main.screenWidth / 2, Main.screenHeight / 4),
                            new Vector2(panel.Left.Pixels, panel.Top.Pixels)) < 100)
                        {
                            TurnToNormal();
                        }

                        itemSlotGrid.Width.Set(
                            MathHelper.Clamp(panel.Width.Pixels - 18, panel.MinWidth.Pixels, panel.MaxWidth.Pixels)
                            , 0);
                        itemSlotGrid.Height.Set(
                            MathHelper.Clamp(panel.Height.Pixels - 70, panel.MinHeight.Pixels, panel.MaxHeight.Pixels - 70)
                            , 0);

                        base.Recalculate();

                        if (!Main.playerInventory)
                            TurnToFadeOut();
                    }
                    break;
                case State.Normal:
                    {
                        if (!Main.playerInventory)
                            TurnToFadeOut();
                    }
                    break;
                default:
                case State.FadeOut:
                    {
                        panel.Top.Set(Helper.Lerp(panel.Top.Pixels, Main.screenHeight + 200, 0.09f), 0);
                        base.Recalculate();
                        if (panel.Top.Pixels > Main.screenHeight)
                            visible = false;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public override void Recalculate()
        {
            itemSlotGrid?.Clear();
            if (bottle != null)
            {
                for (int i = 0; i < bottle.Capacity; i++)
                {
                    FairyItemSlot slot = new FairyItemSlot(i);
                    itemSlotGrid.Add(slot);
                }
            }

            base.Recalculate();
        }

        public void ShowUI(IFairyBottle bottle)
        {
            visible = true;
            state = State.FadeIn;
            FairyBottleUI.bottle = bottle;

            Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0);

            panel.BackgroundColor = Color.Transparent;
            panel.Top.Set((int)Main.MouseScreen.Y, 0);
            panel.Left.Set((int)Main.MouseScreen.X, 0);
            panel.Width.Set(20f, 0f);
            panel.Height.Set(30, 0f);
            itemSlotGrid.Width.Set(panel.Width.Pixels - 18, 0);
            itemSlotGrid.Height.Set(panel.Height.Pixels - 70, 0);

            Recalculate();
        }

        public void TurnToNormal()
        {
            state = State.Normal;
            panel.BackgroundColor = new Color(63, 82, 151) * 0.7f;
            panel.Width.Set(305, 0f);
            panel.Height.Set( 350, 0f);
            //panel.Recalculate();
        }

        public void TurnToFadeOut()
        {
            state = State.FadeOut;
            base.Recalculate();
        }

        public void SetState(State state)
        {
            this.state = state;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            //if (state == State.Normal)
                base.DrawChildren(spriteBatch);
        }
    }

    public class FairyItemSlot : UIElement
    {
        public readonly int index;

        public FairyItemSlot(int index)
        {
            Width.Set(52, 0f);
            Height.Set(52, 0f);
            this.index = index;
        }

        public override void OnInitialize()
        {
            Width.Set(52, 0f);
            Height.Set(52, 0f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (FairyBottleUI.bottle is null)
                return;

            Item[] items = FairyBottleUI.bottle.Fairies;

            Item Item = items[index];
            bool canInsert = true;

            if (!Main.mouseItem.IsAir)
                canInsert = Main.mouseItem.ModItem is IFairyItem;
            if (Item is null)
                return;

            //快捷取回到玩家背包中
            if (PlayerInput.Triggers.Current.SmartSelect)
            {
                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

                if (!Item.IsAir && invSlot != -1)
                {
                    Main.LocalPlayer.GetItem(Main.myPlayer, Item, GetItemSettings.InventoryUIToInventorySettings);
                    items[index]=new Item();
                    Helper.PlayPitched("Fairy/FairyBottleClick", 0.4f, 0);
                }

                goto baseClick;
            }

            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
            {
                Main.mouseItem = Item;
                items[index] = new Item();
                Helper.PlayPitched("Fairy/FairyBottleClick", 0.4f, 0);
                goto baseClick;
            }

            if (!Main.mouseItem.IsAir && Item.IsAir && canInsert) //鼠标有物品并且UI内为空，放入
            {
                FairyBottleUI.bottle.Fairies[index] = Main.mouseItem;
                Main.mouseItem = new Item();
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 1f);
                goto baseClick;
            }

            if (!Main.mouseItem.IsAir && !Item.IsAir && canInsert) //都有物品，进行交换
            {
                var temp = Item;
                items[index] = Main.mouseItem;
                Main.mouseItem = temp;
                Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 1f);
                goto baseClick;
            }

        baseClick:
            base.LeftClick(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (FairyBottleUI.bottle is null)
                return;

            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            Vector2 position = GetDimensions().Position();
            Vector2 center = GetDimensions().Center();

            Item Item = FairyBottleUI.bottle.Fairies[index];

            Texture2D backTex = TextureAssets.InventoryBack15.Value;
            if (Main.LocalPlayer.HeldItem.ModItem is BaseFairyCatcher catcher
                && catcher.currentFairyIndex == index)
            {
                backTex = TextureAssets.InventoryBack14.Value;
            }
            else if(Item.ModItem is IFairyItem fairyItem&&fairyItem.IsDead)
            {
                backTex = TextureAssets.InventoryBack11.Value;
            }

            spriteBatch.Draw(backTex, center, null, Color.White, 0, backTex.Size() / 2, MagikeItemSlotPanel.scale, SpriteEffects.None, 0);

            if (Item is not null && !Item.IsAir)
            {
                Main.instance.LoadItem(Item.type);
                Texture2D mainTex = TextureAssets.Item[Item.type].Value;
                Rectangle rectangle2;

                if (Main.itemAnimations[Item.type] != null)
                    rectangle2 = Main.itemAnimations[Item.type].GetFrame(mainTex, -1);
                else
                    rectangle2 = mainTex.Frame();

                float itemScale = 1f;
                float pixelWidth = 40 * MagikeItemSlotPanel.scale;      //同样的魔法数字，是物品栏的长和宽（去除了边框的）
                float pixelHeight = pixelWidth;
                if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelHeight)
                {
                    if (rectangle2.Width > mainTex.Height)
                        itemScale = pixelWidth / rectangle2.Width;
                    else
                        itemScale = pixelHeight / rectangle2.Height;
                }

                //position.X += 26 * MagikeItemSlotPanel.scale - rectangle2.Width * itemScale / 2f;
                //position.Y += 26 * MagikeItemSlotPanel.scale - rectangle2.Height * itemScale / 2f;      //魔法数字，是物品栏宽和高

                spriteBatch.Draw(mainTex, center, new Rectangle?(rectangle2), Item.GetAlpha(Color.White), 0f, rectangle2.Size() / 2, itemScale, 0, 0f);
                if (Item.color != default(Color))
                    spriteBatch.Draw(mainTex, center, new Rectangle?(rectangle2), Item.GetColor(Color.White), 0f, rectangle2.Size() / 2, itemScale, 0, 0f);

                if (Item.stack > 1)
                    Utils.DrawBorderString(spriteBatch, Item.stack.ToString(), center + new Vector2(12, 16), Color.White, MagikeItemSlotPanel.scale, 1, 0.5f);
                if (IsMouseHovering)
                {
                    Main.HoverItem = Item.Clone();
                    Main.hoverItemName = "Coralite: FairyBottle";
                }
            }
        }
    }
}
