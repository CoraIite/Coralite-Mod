using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class MagikeRemodelUI : BetterUIState
    {
        public static Asset<Texture2D> okTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "OKButton", AssetRequestMode.ImmediateLoad);
        public static Asset<Texture2D> notOKTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "CloseButton", AssetRequestMode.ImmediateLoad);
        public static Asset<Texture2D> arrowTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "ButtonPlay", AssetRequestMode.ImmediateLoad);

        public static bool visible = false;
        public static Vector2 basePos = Vector2.Zero;
        public static Item currentChooseItem;

        public static MagikeFactory_RemodelPool remodelPool = null;
        public static CloseButton closeButton = new CloseButton();
        public static SingleItemSlot selfSlot = new SingleItemSlot();
        public static CraftImage image = new CraftImage();
        public static UIList list = new UIList();

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public override bool Visible => visible;

        public override void OnInitialize()
        {
            Elements.Clear();

            closeButton.Top.Set(-48, 0f);
            closeButton.Left.Set(-48, 0f);
            closeButton.OnLeftClick += CloseButton_OnLeftClick;
            Append(closeButton);

            selfSlot.Top.Set(-26, 0f);
            selfSlot.Left.Set(-26, 0f);
            selfSlot.OnLeftClick += SelfSlot_OnLeftClick;
            Append(selfSlot);

            image.Top.Set(-30, 0f);
            image.Left.Set(42, 0f);
            Append(image);

            list.OverflowHidden = true;
            list.ListPadding = 4;
            list.Top.Set(-44, 0f);
            list.Left.Set(100, 0);
            list.Width.Set(52, 0f);
            list.Height.Set(200, 0f);

            Append(list);

            UIScrollbar scrollbar = new UIScrollbar();
            scrollbar.SetView(100f, 1000f);
            scrollbar.Top.Pixels = 2_0000;
            scrollbar.Height.Set(-42f - 8, 1f);
            scrollbar.HAlign = 1f;
            Append(scrollbar);
            list.SetScrollbar(scrollbar);
        }

        private void SelfSlot_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Recalculate();
        }

        private void CloseButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            visible = false;
            Recalculate();
        }

        public override void Recalculate()
        {
            list.Width.Set(52 * 2 + 4, 0f);

            selfSlot.SetContainer(remodelPool);
            //寻找当前物品的合成表
            if (remodelPool is not null)
            {
                if (remodelPool.containsItem is null || remodelPool.containsItem.IsAir || remodelPool.chooseRecipe == null)
                {
                    image.showItem = null;
                    remodelPool.chooseRecipe = null;
                }
                else if (remodelPool.chooseRecipe != null)
                {
                    image.showItem = remodelPool.chooseRecipe.resultItem;
                }

                list.Clear();
                Item item = remodelPool.GetItem();
                if (item is not null && !item.IsAir)
                {
                    int type = item.type;
                    if (MagikeSystem.TryGetRemodelRecipes(type, out List<RemodelRecipe> recipeList))
                    {
                        foreach (var recipe in recipeList)
                        {
                            RemodelItemButton shower = new RemodelItemButton(recipe);
                            list.Add(shower);
                        }
                    }
                }
            }

            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (remodelPool is null)
            {
                visible = false;
                return;
            }

            Vector2 worldPos = remodelPool.GetWorldPosition().ToScreenPosition();
            if (!Helpers.Helper.OnScreen(worldPos))
            {
                visible = false;
                return;
            }

            if (basePos != worldPos)
            {
                basePos = worldPos;
                Top.Set((int)basePos.Y - 32, 0f);
                Left.Set((int)basePos.X - 32, 0f);
                base.Recalculate();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = arrowTex.Value;
            Vector2 center = GetDimensions().Position() + new Vector2(20, -16);

            spriteBatch.Draw(mainTex, center, Color.White);
        }
    }

    /// <summary>
    /// 显示图片用的，没有任何交互按键
    /// </summary>
    public class CraftImage : UIElement
    {
        public Item showItem;

        public CraftImage()
        {
            Width.Set(52, 0f);
            Height.Set(52, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;

            Vector2 position = GetDimensions().Position();
            Vector2 center = GetDimensions().Center();

            Texture2D backTex = TextureAssets.InventoryBack.Value;
            spriteBatch.Draw(backTex, center, null, Color.White, 0, backTex.Size() / 2, 1, SpriteEffects.None, 0);

            if (showItem != null && !showItem.IsAir)
            {
                Main.instance.LoadItem(showItem.type);
                Texture2D mainTex = TextureAssets.Item[showItem.type].Value; ;
                Rectangle rectangle2;

                if (Main.itemAnimations[showItem.type] != null)
                    rectangle2 = Main.itemAnimations[showItem.type].GetFrame(mainTex, -1);
                else
                    rectangle2 = mainTex.Frame();

                float itemScale = 1f;
                float pixelWidth = 40;      //同样的魔法数字，是物品栏的长和宽（去除了边框的）
                float pixelHeight = pixelWidth;
                if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelHeight)
                {
                    if (rectangle2.Width > mainTex.Height)
                        itemScale = pixelWidth / rectangle2.Width;
                    else
                        itemScale = pixelHeight / rectangle2.Height;
                }

                position.X += 26 - rectangle2.Width * itemScale / 2f;
                position.Y += 26 - rectangle2.Height * itemScale / 2f;      //魔法数字，是物品栏宽和高
                position += new Vector2(5, 5);

                spriteBatch.Draw(mainTex, position, new Rectangle?(rectangle2), showItem.GetAlpha(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);
                if (showItem.color != default(Color))
                    spriteBatch.Draw(mainTex, position, new Rectangle?(rectangle2), showItem.GetColor(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);

                if (showItem.stack > 1)
                    Utils.DrawBorderString(spriteBatch, showItem.stack.ToString(), center + new Vector2(12, 16), Color.White, 1, 1, 0.5f);
                if (IsMouseHovering)
                {
                    Main.HoverItem = showItem.Clone();
                    Main.hoverItemName = "Coralite: MagikeRemodelRecipe";
                }
            }
        }
    }

    /// <summary>
    /// 按钮，显示钩或×，代表能否合成，和当前魔能量无关
    /// </summary>
    public class RemodelItemButton : UIElement
    {
        public bool canRemodel;
        public RemodelRecipe recipe;

        public RemodelItemButton(RemodelRecipe recipe)
        {
            this.recipe = recipe;
            Width.Set(52 * 2, 0f);
            Height.Set(52, 0f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (recipe is null || MagikeRemodelUI.remodelPool is null)
                return;
            if (!MagikeRemodelUI.remodelPool.CanGetItem())
                return;

            if (recipe.CanRemodel(MagikeRemodelUI.remodelPool.GetItem(), MagikeRemodelUI.remodelPool.magike, MagikeRemodelUI.remodelPool.containsItem.type, MagikeRemodelUI.remodelPool.containsItem.stack))
            {
                MagikeRemodelUI.image.showItem = recipe.resultItem;
                MagikeRemodelUI.remodelPool.chooseRecipe = recipe;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (recipe is null || MagikeRemodelUI.remodelPool is null)
                return;

            Color drawColor = Color.White;
            if (IsMouseHovering)
                Main.LocalPlayer.mouseInterface = true;
            else
                drawColor *= 0.75f;

            Vector2 position = GetDimensions().Position();
            Vector2 center = GetDimensions().Center();
            float height = GetDimensions().Height;

            //绘制背景框
            Texture2D backTex = TextureAssets.InventoryBack.Value;
            spriteBatch.Draw(backTex, position + new Vector2(height, height) / 2, null, drawColor, 0, backTex.Size() / 2, 1, SpriteEffects.None, 0);

            #region 绘制物品
            Item showItem = recipe.resultItem;
            if (showItem is null)
                return;
            if (!showItem.IsAir)
            {
                Main.instance.LoadItem(showItem.type);
                Texture2D mainTex = TextureAssets.Item[showItem.type].Value; ;
                Rectangle rectangle2;

                if (Main.itemAnimations[showItem.type] != null)
                    rectangle2 = Main.itemAnimations[showItem.type].GetFrame(mainTex, -1);
                else
                    rectangle2 = mainTex.Frame();

                float itemScale = 1f;
                float pixelWidth = 40;      //同样的魔法数字，是物品栏的长和宽（去除了边框的）
                float pixelHeight = pixelWidth;
                if (rectangle2.Width > pixelWidth || rectangle2.Height > pixelHeight)
                {
                    if (rectangle2.Width > mainTex.Height)
                        itemScale = pixelWidth / rectangle2.Width;
                    else
                        itemScale = pixelHeight / rectangle2.Height;
                }

                position.X += 26 - rectangle2.Width * itemScale / 2f;
                position.Y += 26 - rectangle2.Height * itemScale / 2f;      //魔法数字，是物品栏宽和高

                spriteBatch.Draw(mainTex, position, new Rectangle?(rectangle2), showItem.GetAlpha(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);
                if (showItem.color != default(Color))
                    spriteBatch.Draw(mainTex, position, new Rectangle?(rectangle2), showItem.GetColor(Color.White), 0f, Vector2.Zero, itemScale, 0, 0f);

                if (showItem.stack > 1)
                    Utils.DrawBorderString(spriteBatch, showItem.stack.ToString(), center + new Vector2(12 - height / 2, 16), Color.White, 1, 1, 0.5f);
                if (IsMouseHovering)
                {
                    Main.HoverItem = showItem.Clone();
                    Main.hoverItemName = "Coralite: MagikeRemodelRecipe";
                }
            }
            #endregion

            //绘制显示条
            Texture2D exTex;
            Item item = MagikeRemodelUI.remodelPool.GetItem();
            bool stackEnough = item is null ? false : item.stack >= recipe.selfRequiredNumber;
            bool conditionCanRemodel = recipe.condition == null ? true : recipe.condition.CanCraft(item);
            bool magikeEnough = MagikeRemodelUI.remodelPool.magike >= recipe.magikeCost;

            canRemodel = conditionCanRemodel && magikeEnough && stackEnough;

            if (canRemodel)
                exTex = MagikeRemodelUI.okTex.Value;
            else
                exTex = MagikeRemodelUI.notOKTex.Value;

            spriteBatch.Draw(exTex, center + new Vector2(height / 2, 0), null, Color.White, 0, exTex.Size() / 2, 1, SpriteEffects.None, 0);

        }
    }
}
