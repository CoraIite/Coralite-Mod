using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class MagikeItemSlotPanel : BetterUIState
    {
        public static float scale = 1f;
        public static bool visible = false;
        public static IMagikeSingleItemContainer tileEntity = null;
        public static SingleItemSlot slot = new SingleItemSlot();
        public static CloseButton closeButton = new CloseButton();

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        private Vector2 basePos = Vector2.One;
        public override bool Visible => visible;

        public override void OnInitialize()
        {
            Elements.Clear();

            closeButton.Top.Set(-44, 0f);
            closeButton.Left.Set(-44, 0f);
            closeButton.OnLeftClick += CloseButton_OnLeftClick;
            Append(closeButton);

            slot.Top.Set(-26, 0f);
            slot.Left.Set(-26, 0f);

            Append(slot);
        }

        private void CloseButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            visible = false;
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (tileEntity is null)
            {
                visible = false;
                return;
            }

            Vector2 worldPos = tileEntity.GetWorldPosition().ToScreenPosition();
            if (!Helper.OnScreen(worldPos))
            {
                visible = false;
                return;
            }

            if (basePos != worldPos)
            {
                basePos = worldPos;
                Top.Set((int)basePos.Y, 0f);
                Left.Set((int)basePos.X, 0f);
                Recalculate();
            }
        }

        public override void Recalculate()
        {
            scale = ModContent.GetInstance<MagikeUIConfig>().UIScale;
            slot.SetContainer(tileEntity);
            base.Recalculate();
        }
    }

    public class SingleItemSlot : UIElement
    {
        public ISingleItemContainer container;

        public SingleItemSlot()
        {
            Width.Set(52 * MagikeItemSlotPanel.scale, 0f);
            Height.Set(52 * MagikeItemSlotPanel.scale, 0f);
        }

        public override void OnInitialize()
        {
            Width.Set(52 * MagikeItemSlotPanel.scale, 0f);
            Height.Set(52 * MagikeItemSlotPanel.scale, 0f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (container is null)
                return;
            Item Item = container.GetItem();
            bool canInsert = true;

            if (!Main.mouseItem.IsAir)
                canInsert = container.CanInsertItem(Main.mouseItem);
            if (Item is null)
                return;

            if (!container.CanGetItem())
                return;

            //快捷取回到玩家背包中
            if (PlayerInput.Triggers.Current.SmartSelect)
            {
                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

                if (!Item.IsAir && invSlot != -1)
                {
                    Main.LocalPlayer.GetItem(Main.myPlayer, Item.Clone(), GetItemSettings.InventoryUIToInventorySettings);
                    Item.TurnToAir();
                    SoundEngine.PlaySound(SoundID.Grab);
                }

                goto baseClick;
            }

            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
            {
                Main.mouseItem = Item.Clone();
                Item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                goto baseClick;
            }

            if (!Main.mouseItem.IsAir && Item.IsAir && canInsert) //鼠标有物品并且UI内为空，放入
            {
                container.InsertItem(Main.mouseItem.Clone());
                Main.mouseItem.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                goto baseClick;
            }

            if (!Main.mouseItem.IsAir && !Item.IsAir && canInsert) //都有物品，进行交换
            {
                var temp = Item;
                container.InsertItem(Main.mouseItem.Clone());
                Main.mouseItem = temp;
                SoundEngine.PlaySound(SoundID.Grab);
                goto baseClick;
            }

        baseClick:
            base.LeftClick(evt);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            if (container is null)
                return;

            Item Item = container.GetItem();
            if (Item is null)
                return;

            if (!container.CanGetItem())
                return;

            if (!Main.mouseItem.IsAir && !Item.IsAir) //都有物品，且种类相同,一个个取出
            {
                Item heldItem = Main.mouseItem;
                if (heldItem.type != Item.type)
                    return;

                heldItem.stack++;
                if (Item.stack > 1)
                    Item.stack--;
                else
                    Item.TurnToAir();

                //SoundEngine.PlaySound(SoundID.Grab);
                goto baseClick;
            }

            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出1个
            {
                if (Item.stack > 1)
                {
                    Main.mouseItem = Item.Clone();
                    Main.mouseItem.stack = 1;
                    Item.stack--;
                    //SoundEngine.PlaySound(SoundID.Grab);
                }
                else
                {
                    Main.mouseItem = Item.Clone();
                    Item.TurnToAir();
                    //SoundEngine.PlaySound(SoundID.Grab);
                }
            }

        baseClick:
            base.RightClick(evt);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (container is null)
                return;

            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            Vector2 position = GetDimensions().Position();
            Vector2 center = GetDimensions().Center();

            Texture2D backTex = TextureAssets.InventoryBack.Value;
            spriteBatch.Draw(backTex, center, null, Color.White, 0, backTex.Size() / 2, MagikeItemSlotPanel.scale, SpriteEffects.None, 0);

            Item Item = container.GetItem();
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

                spriteBatch.Draw(mainTex, center, new Rectangle?(rectangle2), Item.GetAlpha(Color.White), 0f, rectangle2.Size()/2, itemScale, 0, 0f);
                if (Item.color != default(Color))
                    spriteBatch.Draw(mainTex, center, new Rectangle?(rectangle2), Item.GetColor(Color.White), 0f, rectangle2.Size() / 2, itemScale, 0, 0f);

                if (Item.stack > 1)
                    Utils.DrawBorderString(spriteBatch, Item.stack.ToString(), center + new Vector2(12, 16), Color.White, MagikeItemSlotPanel.scale, 1, 0.5f);
                if (IsMouseHovering)
                {
                    Main.HoverItem = Item.Clone();
                    Main.hoverItemName = "Coralite: MagikeGenerator_ContainItem";
                }
            }
        }

        public void SetContainer(ISingleItemContainer container)
        {
            this.container = container;
        }
    }

    public class CloseButton : UIImageButton
    {
        public CloseButton() : base(ModContent.Request<Texture2D>(AssetDirectory.UI + "CloseButton", ReLogic.Content.AssetRequestMode.ImmediateLoad)) { }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            SoundEngine.PlaySound(CoraliteSoundID.MenuClose);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
            base.DrawSelf(spriteBatch);
        }
    }
}
