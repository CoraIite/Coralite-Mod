using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace Coralite.Content.UI
{
    class YujianHuluBackpack : BetterUIState
    {
        public static bool visible = false;
        public static BaseHulu huluItem = null;
        public static HuluBackpackSlot[] huluBackpackSlots;
        public static HuluBackpackPanel panel = new();

        public static Vector2 basePos = new((Main.screenWidth / 2) - 159, (Main.screenHeight / 2) - 200 - 47);

        public YujianHuluBackpack()
        {
            huluBackpackSlots = new HuluBackpackSlot[Coralite.YujianHuluContainsMax];
            for (int i = 0; i < Coralite.YujianHuluContainsMax; i++)
            {
                huluBackpackSlots[i] = new HuluBackpackSlot(i);
            }
        }

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override bool Visible => visible;

        public override void OnInitialize()
        {
            Elements.Clear();

            panel.Top.Set(basePos.Y, 0f);
            panel.Left.Set(basePos.X, 0f);
            Append(panel);

            for (int i = 0; i < Coralite.YujianHuluContainsMax; i++)
            {
                huluBackpackSlots[i].Top.Set(basePos.Y + 12, 0f);
                //(- 318 / 2) + 10 = -149 就是最左边一个的位置
                huluBackpackSlots[i].Left.Set(basePos.X + 10 + 30 * i, 0f);
                Append(huluBackpackSlots[i]);
            }

            base.OnInitialize();
        }

        public override void Recalculate()
        {

            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (!Main.playerInventory)
                visible = false;

            base.Update(gameTime);
        }
    }

    public class HuluBackpackSlot : UIElement
    {
        public readonly int slotIndex;

        public HuluBackpackSlot(int slotIndex)
        {
            this.slotIndex = slotIndex;
        }

        public override void OnInitialize()
        {
            Width.Set(28, 0f);
            Height.Set(72, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (!YujianHuluBackpack.huluItem.CanUseSlot(slotIndex))
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0f);
                return;
            }

            Item Item = YujianHuluBackpack.huluItem.GetItem(slotIndex);

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

                return;
            }

            if (Main.mouseItem.IsAir && !Item.IsAir) //鼠标物品为空且UI内有物品，直接取出
            {
                Main.mouseItem = Item.Clone();
                Item.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            if (!Main.mouseItem.IsAir && Item.IsAir && Main.mouseItem.ModItem is BaseYujian) //鼠标有物品并且UI内为空，放入
            {
                YujianHuluBackpack.huluItem.SaveItem(slotIndex, Main.mouseItem.Clone());
                Main.mouseItem.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            if (!Main.mouseItem.IsAir && !Item.IsAir && Main.mouseItem.ModItem is BaseYujian) //都有物品，进行交换
            {
                var temp = Item;
                YujianHuluBackpack.huluItem.SaveItem(slotIndex, Main.mouseItem.Clone());
                Main.mouseItem = temp;
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            if (!YujianHuluBackpack.huluItem.CanUseSlot(slotIndex))
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0f);
                return;
            }

            SoundEngine.PlaySound(SoundID.Grab);
            //YujianHuluBackpack.huluItem.SetCanChannel(slotIndex);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = GetDimensions().Position();
            Vector2 center = GetDimensions().Center();
            //如果能放置就进行对应的绘制，不行的话就绘制个小锁头
            if (YujianHuluBackpack.huluItem.CanUseSlot(slotIndex))
            {
                Texture2D canChannelAnim = ModContent.Request<Texture2D>(AssetDirectory.UI + "CanChannelAnim").Value;

                int frame = (int)(Main.timeForVisualEffects % 36) / 3;
                Rectangle source = new(frame * 30, 0, 30, 72);        //贴图长宽这里就直接写了

                spriteBatch.Draw(canChannelAnim, position, source, Color.White);
                //if (YujianHuluBackpack.huluItem.CanChannel[slotIndex])      //绘制特效
                //{

                //}

                Texture2D mainTex;
                if (YujianHuluBackpack.huluItem.Yujians[slotIndex].IsAir)
                {
                    mainTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "HuluEmpty").Value;
                    spriteBatch.Draw(mainTex, center, mainTex.Frame(), Color.White, 0f, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 1f, SpriteEffects.None, 0f);
                }
                else
                {
                    Item Item = YujianHuluBackpack.huluItem.Yujians[slotIndex];
                    mainTex = TextureAssets.Item[Item.type].Value;

                    Rectangle rectangle2;
                    if (Main.itemAnimations[Item.type] != null)
                        rectangle2 = Main.itemAnimations[Item.type].GetFrame(mainTex, -1);
                    else
                        rectangle2 = mainTex.Frame();

                    float itemScale = 1f;
                    float pixelWidth = 20;      //同样的魔法数字，是物品栏的长和宽（去除了边框的）
                    float pixelHeight = 64;
                    if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelHeight)
                    {
                        if (rectangle2.Width > mainTex.Height)
                            itemScale = pixelWidth / rectangle2.Width;
                        else
                            itemScale = pixelHeight / rectangle2.Height;
                    }

                    position.X += 14 - rectangle2.Width * itemScale / 2f;
                    position.Y += 36 - rectangle2.Height * itemScale / 2f;      //魔法数字，是物品栏宽和高的一半

                    spriteBatch.Draw(mainTex, position, new Rectangle?(rectangle2), Item.GetAlpha(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);
                    if (Item.color != default)
                        spriteBatch.Draw(mainTex, position, new Rectangle?(rectangle2), Item.GetColor(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);

                    if (IsMouseHovering)
                    {
                        Main.HoverItem = Item.Clone();
                        Main.hoverItemName = "Yujian";
                    }
                }
            }
            else
            {
                Texture2D lockTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "HuluLock").Value;
                spriteBatch.Draw(lockTex, center, lockTex.Frame(), Color.White, 0f, new Vector2(lockTex.Width / 2, lockTex.Height / 2), 1f, SpriteEffects.None, 0f);
            }
        }
    }

    public class HuluBackpackPanel : UIElement
    {
        public override void OnInitialize()
        {
            Width.Set(108, 0f);
            Height.Set(94, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "YujianHuluBackpack1").Value;
            spriteBatch.Draw(mainTex, GetDimensions().Position(), Color.White);
        }
    }
}
