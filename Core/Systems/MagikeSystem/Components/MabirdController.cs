using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MabirdController : ItemContainer, IUpgradeable
    {
        public override void Update()
        {
            Vector2 center = Helper.GetMagikeTileCenter(Entity.Position);

            foreach (var item in Items)
            {
                if (!item.IsAir && CoraliteSets.Mabird[item.type])
                    (item.ModItem as Mabird).UpdateMabird(center);
            }
        }

        public abstract MALevel DefaultLevel { get; }

        public override void Initialize()
        {
            Upgrade(DefaultLevel);
        }

        public virtual void Upgrade(MALevel incomeLevel) { }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override bool CanAddItem(int itemType, int stack)
        {
            if (ContentSamples.ItemsByType[itemType].ModItem is not Mabird)
                return false;

            return base.CanAddItem(itemType, stack);
        }

        #region 绘制

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in Items)
            {
                if (!item.IsAir && CoraliteSets.Mabird[item.type])
                    (item.ModItem as Mabird).DrawMabird(spriteBatch);
            }
        }

        #endregion

        #region 存储

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);
        }

        #endregion

        #region 同步

        #endregion

        #region UI

        public override void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.MabirdController, parent);

            UIList list = new UIList();
            list.SetTopLeft(title.Height.Pixels + 8, 0);
            list.SetSize(0, -title.Height.Pixels - 8, 1, 1);

            for (int i = 0; i < Capacity; i++)
            {
                MabirdControlBar controller = new MabirdControlBar(this, i);
                list.Add(controller);
            }

            list.QuickInvisibleScrollbar();
            parent.Append(list);
        }

        #endregion
    }

    /// <summary>
    /// 魔鸟的UI条
    /// </summary>
    public class MabirdControlBar : UIElement
    {
        private MabirdController controller;
        private int index;

        public MabirdControlBar(MabirdController controller, int index)
        {
            this.controller = controller;
            this.index = index;

            SetPadding(6);

            this.SetSize(0, 80, 1);

            //添加物品格
            var slot = new MabirdSlot(controller, index);
            slot.VAlign = 0.5f;
            slot.SetTopLeft(0, 6);
            //slot.SetSize(46, 0, 0, 1);
            Append(slot);
            float left = slot.Left.Pixels + slot.Width.Pixels + 6;

            //添加启用按钮
            MabirdActiveButton mabirdActiveButton = new MabirdActiveButton(controller, index);
            mabirdActiveButton.SetTopLeft(0, slot.Width.Pixels - mabirdActiveButton.Width.Pixels / 2);
            Append(mabirdActiveButton);

            //添加分隔线
            UIVerticalLine line = new UIVerticalLine();
            line.SetTopLeft(0, left);
            Append(line);
            left += line.Width.Pixels+6;

            //添加画线段按钮
            RouteDrawButton routeDrawButton = new RouteDrawButton(controller, index);
            routeDrawButton.SetTopLeft(8, left);
            Append(routeDrawButton);
            left += routeDrawButton.Width.Pixels + 8;

            //添加复制黏贴按钮
            CopyRouteButton copyRouteButton = new CopyRouteButton(controller, index);
            copyRouteButton.SetTopLeft(routeDrawButton.Top.Pixels + routeDrawButton.Height.Pixels + 2, routeDrawButton.Left.Pixels);
            Append(copyRouteButton);

            PasteRouteButton pasteRouteButton = new PasteRouteButton(controller, index);
            pasteRouteButton.SetTopLeft(copyRouteButton.Top.Pixels
                , copyRouteButton.Left.Pixels + copyRouteButton.Width.Pixels + 2);
            Append(pasteRouteButton);

            //添加白名单物品格
            WhiteListSlot whiteListSlot = new WhiteListSlot(controller, index);
            whiteListSlot.VAlign = 0.5f;
            whiteListSlot.SetTopLeft(0, left);
            Append(whiteListSlot);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var d = GetDimensions();
            var p = d.Position();
            Texture2D mainTex = MagikeAssets.AlphaBar.Value;

            var target = new Rectangle((int)p.X, (int)p.Y, (int)d.Width, (int)d.Height);
            var self = mainTex.Frame();

            spriteBatch.Draw(mainTex, target, self, MagikeApparatusPanel.BackgroundColor);

            if (IsMouseHovering && !controller[index].IsAir && controller[index].ModItem is Mabird mabird)
            {
                bool hasGetItemPos = mabird.GetItemPos != null;
                bool hasReleaseItemPos = mabird.ReleaseItemPos != null;

                if (hasGetItemPos && hasReleaseItemPos)
                {
                    MagikeApparatusPanel.DrawExtras.Add((spriteBatch) =>
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                        Color drawColor = Color.Lerp(Color.White, Color.Coral, (MathF.Sin((int)Main.timeForVisualEffects * 0.1f) / 2) + 0.5f);

                        Vector2 selfPos = Helper.GetMagikeTileCenter(controller.Entity.Position);
                        Vector2 getItemPos = mabird.GetItemPos.Pos;
                        Vector2 releaseItemPos = mabird.ReleaseItemPos.Pos;

                        //绘制从鸟巢到取物品点的线
                        MagikeSystem.DrawConnectLine(spriteBatch, selfPos, getItemPos, Main.screenPosition, drawColor * 0.3f);
                        //绘制从取物品点到放物品点的线
                        MagikeSystem.DrawConnectLine(spriteBatch, getItemPos, releaseItemPos, Main.screenPosition, drawColor);
                        //绘制从放物品点到鸟巢的线
                        MagikeSystem.DrawConnectLine(spriteBatch, releaseItemPos, selfPos, Main.screenPosition, drawColor * 0.3f);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
                    });
                }
            }
        }
    }

    public class MabirdSlot : UIElement
    {
        private MabirdController controller;
        private int index;

        public MabirdSlot(MabirdController controller, int index)
        {
            var tex = TextureAssets.InventoryBack2;

            this.SetSize(tex.Size());
            this.controller = controller;
            this.index = index;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (!Main.LocalPlayer.ItemTimeIsZero)
                return;

            Item i = controller[index];

            if (!i.IsAir && i.ModItem is Mabird mabird)
            {
                if (mabird.State != Mabird.MabirdAIState.Rest)//魔鸟外出中，无法交互
                    return;
            }

            //快捷取回到玩家背包中
            if (PlayerInput.Triggers.Current.SmartSelect)
            {
                int invSlot = Helper.GetFreeInventorySlot(Main.LocalPlayer);

                if (!i.IsAir && invSlot != -1)
                {
                    Main.LocalPlayer.GetItem(Main.myPlayer, i.Clone(), GetItemSettings.InventoryUIToInventorySettings);
                    i.TurnToAir();
                    SoundEngine.PlaySound(SoundID.Grab);
                }

                return;
            }

            if (Main.mouseItem.IsAir && !i.IsAir) //鼠标物品为空且UI内有物品，直接取出
            {
                Main.mouseItem = i.Clone();
                i.TurnToAir();
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is Mabird mabird2 && i.IsAir) //鼠标有物品并且UI内为空，放入
            {
                mabird2.Reset(Helper.GetMagikeTileCenter(controller.Entity.Position));
                controller.AddItemByIndex(Main.mouseItem, index);
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is Mabird mabird1 && !i.IsAir) //都有物品，进行交换
            {
                mabird1.Reset(Helper.GetMagikeTileCenter(controller.Entity.Position));

                var temp = i;
                controller.AddItemByIndex(Main.mouseItem, index);
                Main.mouseItem = temp;
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Item inv2 = controller[index];

            int context = ItemSlot.Context.ChestItem;

            if (IsMouseHovering)
            {
                if (!inv2.IsAir && inv2.ModItem is Mabird mabird)
                    if (mabird.State != Mabird.MabirdAIState.Rest)
                    {
                        UICommon.TooltipMouseText(MagikeSystem.GetUIText(MagikeSystem.UITextID.MabirdOuting));
                        goto Draw;
                    }

                Main.LocalPlayer.mouseInterface = true;
                ItemSlot.OverrideHover(ref inv2, context);
                ItemSlot.MouseHover(ref inv2, context);
            }

        Draw:
            Vector2 center = GetDimensions().Center();
            Texture2D slotTex;
            if (inv2.IsAir)
                slotTex = MagikeAssets.MabirdSlot.Value;
            else
                slotTex = TextureAssets.InventoryBack15.Value;

            spriteBatch.Draw(slotTex, center, null, Color.White, 0, slotTex.Size() / 2, 1, 0, 0);
            if (!inv2.IsAir)
            {
                MagikeHelper.DrawItem(spriteBatch, inv2, center, 50, Color.White);
            }
        }
    }

    //public class MabirdVerticalLine(MabirdController controller, int index) : UIVerticalLine()
    //{
    //    protected override void DrawSelf(SpriteBatch spriteBatch)
    //    {
    //        base.DrawSelf(spriteBatch);

    //        Item item = controller[index];
    //        if (!item.IsAir && item.ModItem is Mabird mabird)
    //        {
    //            Texture2D tex = MagikeAssets.MabirdStateButton.Value;

    //            var center = GetDimensions().Center();
    //            int frameY = 1;
    //            if (mabird.GetItemPos != null && mabird.ReleaseItemPos != null)//有目标点时变绿色，否则为红色
    //                frameY = 0;

    //            var frameBox = tex.Frame(1, 2, 0, frameY);
    //            spriteBatch.Draw(tex, center, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
    //        }

    //    }
    //}

    public class MabirdActiveButton : UIElement
    {
        private readonly MabirdController controller;
        private readonly int index;

        public MabirdActiveButton(MabirdController controller, int index)
        {
            var tex = MagikeAssets.MabirdStateButton;

            Vector2 size = tex.Size();
            size.Y /= 3;
            this.SetSize(size);
            this.controller = controller;
            this.index = index;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Item item = controller[index];
            if (!item.IsAir && item.ModItem is Mabird mabird)
            {
                mabird.Active = !mabird.Active;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeAssets.MabirdStateButton.Value;
            var center = GetDimensions().Center();

            Item item = controller[index];
            int frameY = 0;

            if (!item.IsAir && item.ModItem is Mabird mabird)
            {
                if (mabird.Active)//有目标点时变绿色，否则为红色
                    frameY = 1;
                else
                    frameY = 2;

                if (IsMouseHovering)
                {
                    if (mabird.Active)
                        UICommon.TooltipMouseText(MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToInactiveMabird));
                    else
                        UICommon.TooltipMouseText(MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToActiveMabird));
                }
            }

            var frameBox = tex.Frame(1, 3, 0, frameY);
            spriteBatch.Draw(tex, center, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
        }
    }

    public class RouteDrawButton : UIElement
    {
        private float _alpha = 0.75f;
        private MabirdController controller;
        private int index;

        public RouteDrawButton(MabirdController controller, int index)
        {
            Texture2D tex = MagikeAssets.RouteDrawButton.Value;

            this.SetSize(tex.Width, tex.Height);
            this.controller = controller;
            this.index = index;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (controller[index].IsAir)
                return;

            Main.playerInventory = false;
            MagikeSystem.CloseUI();

            MagikeTP entity = controller.Entity;
            Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromAI(), Helper.GetMagikeTileCenter(entity.Position)
                , Vector2.Zero, ModContent.ProjectileType<MabirdRouteDrawProj>(), 0, 0, Main.myPlayer
                , entity.Position.X, entity.Position.Y, index);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
            {
                _alpha = 1f;
                if (controller[index].IsAir)
                    UICommon.TooltipMouseText(MagikeSystem.GetUIText(MagikeSystem.UITextID.NoItem));
                else
                    UICommon.TooltipMouseText(MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToDrawRoute));
            }
            else
                _alpha = 0.75f;

            spriteBatch.Draw(MagikeAssets.RouteDrawButton.Value, GetDimensions().Position(), null, Color.White * _alpha
                , 0, Vector2.Zero, 1, 0, 0);
        }
    }

    public class CopyRouteButton : UIElement
    {
        public static Point CopyGetItemPos;
        public static Point CopyReleaseItemPos;

        private float _alpha = 0.75f;
        private MabirdController controller;
        private int index;

        public CopyRouteButton(MabirdController controller, int index)
        {
            Texture2D tex = MagikeAssets.CopyRouteButton.Value;

            this.SetSize(tex.Width, tex.Height);
            this.controller = controller;
            this.index = index;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            //复制路径
            if (controller[index].IsAir)
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0);
                return;
            }

            if (controller[index].ModItem is Mabird mabird)
            {
                if (mabird.GetItemPos == null || mabird.ReleaseItemPos == null)
                {
                    Helper.PlayPitched("UI/Error", 0.4f, 0);
                    return;
                }

                CopyGetItemPos = mabird.GetItemPos.TopLeft;
                CopyReleaseItemPos = mabird.ReleaseItemPos.TopLeft;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
            {
                _alpha = 1f;
                string text = "";

                if (controller[index].IsAir)
                    text = MagikeSystem.GetUIText(MagikeSystem.UITextID.NoItem);
                else if (controller[index].ModItem is Mabird mabird)
                {
                    if (mabird.GetItemPos == null || mabird.ReleaseItemPos == null)
                        text = MagikeSystem.GetUIText(MagikeSystem.UITextID.NoRouteCanCopy);
                    else
                        text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToCopyRoute);
                }
                UICommon.TooltipMouseText(text);
            }
            else
                _alpha = 0.75f;

            spriteBatch.Draw(MagikeAssets.CopyRouteButton.Value, GetDimensions().Position(), null, Color.White * _alpha
                , 0, Vector2.Zero, 1, 0, 0);
        }
    }

    public class PasteRouteButton : UIElement
    {
        private float _alpha = 0.75f;
        private MabirdController controller;
        private int index;

        public PasteRouteButton(MabirdController controller, int index)
        {
            Texture2D tex = MagikeAssets.PasteRouteButton.Value;

            this.SetSize(tex.Width, tex.Height);
            this.controller = controller;
            this.index = index;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            //黏贴路径
            if (controller[index].IsAir)
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0);
                return;
            }

            if (controller[index].ModItem is Mabird mabird)
            {
                Vector2 pos = Helper.GetMagikeTileCenter(controller.Entity.Position);
                if (mabird.CanSend(pos, Helper.GetMagikeTileCenter(CopyRouteButton.CopyGetItemPos))
                    && mabird.CanSend(pos, Helper.GetMagikeTileCenter(CopyRouteButton.CopyReleaseItemPos)))
                {
                    mabird.SetGetItemPos(CopyRouteButton.CopyGetItemPos);
                    mabird.SetReleaseItemPos(CopyRouteButton.CopyReleaseItemPos);
                }
                else
                {
                    Helper.PlayPitched("UI/Error", 0.4f, 0);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (IsMouseHovering)
            {
                _alpha = 1f;
                string text = "";

                if (controller[index].IsAir)
                    text = MagikeSystem.GetUIText(MagikeSystem.UITextID.NoItem);
                else if (controller[index].ModItem is Mabird mabird)
                {
                    Vector2 pos = Helper.GetMagikeTileCenter(controller.Entity.Position);

                    if (CopyRouteButton.CopyGetItemPos == default || CopyRouteButton.CopyReleaseItemPos == default)
                        text = MagikeSystem.GetUIText(MagikeSystem.UITextID.NoRouteCanCopy);
                    else if (mabird.CanSend(pos, Helper.GetMagikeTileCenter(CopyRouteButton.CopyGetItemPos))
                         && mabird.CanSend(pos, Helper.GetMagikeTileCenter(CopyRouteButton.CopyReleaseItemPos)))
                        text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToPasteRoute);
                }

                UICommon.TooltipMouseText(text);
            }
            else
                _alpha = 0.75f;

            spriteBatch.Draw(MagikeAssets.PasteRouteButton.Value, GetDimensions().Position(), null, Color.White * _alpha
                , 0, Vector2.Zero, 1, 0, 0);
        }
    }

    public class WhiteListSlot : UIElement
    {
        private readonly MabirdController controller;
        private readonly int index;

        public WhiteListSlot(MabirdController controller, int index)
        {
            this.controller = controller;
            this.index = index;

            this.SetSize(52, 52);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (!Main.LocalPlayer.ItemTimeIsZero)
                return;

            Item i = controller[index];

            //设置白名单物品
            if (!i.IsAir && !Main.mouseItem.IsAir && i.ModItem is Mabird mabird)
            {
                mabird.WhiteListItem = Main.mouseItem.Clone();
                mabird.WhiteListItem.stack = 1;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Item inv2 = controller[index];
            int context = ItemSlot.Context.ChestItem;

            if (inv2.ModItem is Mabird mabird)
            {
                Item whitelist = mabird.WhiteListItem;
                if (whitelist != null)
                {
                    inv2 = whitelist;
                    if (IsMouseHovering)
                    {
                        Main.LocalPlayer.mouseInterface = true;
                        ItemSlot.OverrideHover(ref whitelist, context);
                        ItemSlot.MouseHover(ref whitelist, context);
                    }
                }
                else
                    inv2 = ContentSamples.ItemsByType[0];
            }

            if (inv2 != null)
            {
                Main.inventoryScale = 1;
                Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
                ItemSlot.Draw(spriteBatch, ref inv2, context, position, Color.White);
            }
        }
    }

    /// <summary>
    /// ai0与ai1 传入自身位置，ai2传入物品索引
    /// </summary>
    public class MabirdRouteDrawProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        private Vector2 aimPos;
        private Color c;
        private bool start = true;
        public Point RecordGetItemPos;

        public int Index => (int)Projectile.ai[2];
        public float State { get; set; }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void AI()
        {
            Owner.itemTime = Owner.itemAnimation = 5;

            Projectile.timeLeft = 2;

            Point16 position = new((int)Projectile.ai[0], (int)Projectile.ai[1]);
            Point16 currentPoint = InMousePos.ToTileCoordinates16();// new((int)InMousePos.X / 16, (int)InMousePos.Y / 16);
            aimPos = Helper.GetMagikeTileCenter_NotTopLeft(currentPoint.X, currentPoint.Y);

            if (!MagikeHelper.TryGetEntityWithComponent<MabirdController>(position.X, position.Y, MagikeComponentID.ItemContainer
                , out MagikeTP controller))
            {
                Projectile.Kill();
                return;
            }

            if (start)//一点小检测，防止出问题
            {
                if (!Owner.controlUseItem)
                    start = false;

                return;
            }

            if (controller.TryGetComponent(MagikeComponentID.ItemContainer, out MabirdController mabirdController)
                && mabirdController[Index].ModItem is Mabird mabird)
            {
                c = mabird.CanSend(Helper.GetMagikeTileCenter(controller.Position), aimPos) ? Color.GreenYellow : Color.MediumVioletRed;
            }
            else
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                default:
                case 0://点击左键查找取出点
                    {
                        if (!Owner.controlUseItem)
                            break;

                        Point16 topLeft = MagikeHelper.ToTopLeft(currentPoint.X, currentPoint.Y) ?? currentPoint;

                        bool b1 = mabird.CanSend(Helper.GetMagikeTileCenter(controller.Position), aimPos);
                        bool b2 = HasContainer(topLeft);

                        if (b1 && b2)
                        {
                            FindPosSuccess(controller.Position, topLeft);

                            RecordGetItemPos = topLeft.ToPoint();

                            State = 1;
                            start = true;
                        }
                        else
                        {
                            FailSelectPoint(b1, b2);

                            Projectile.Kill();
                        }
                    }
                    break;
                case 1://点击左键查找放入点
                    {
                        if (!Owner.controlUseItem)
                            break;

                        Point16 topLeft = MagikeHelper.ToTopLeft(currentPoint.X, currentPoint.Y) ?? currentPoint;

                        bool b1 = mabird.CanSend(Helper.GetMagikeTileCenter(controller.Position), aimPos);
                        bool b2 = HasContainer(topLeft);
                        if (b1 && b2)
                        {
                            FindPosSuccess(controller.Position, topLeft);

                            mabird.SetGetItemPos(RecordGetItemPos);
                            mabird.SetReleaseItemPos(topLeft.ToPoint());
                        }
                        else
                            FailSelectPoint(b1, b2);

                        Projectile.Kill();
                    }
                    break;
            }
        }

        private void FindPosSuccess(Point16 pos1, Point16 pos2)
        {
            MagikeHelper.SpawnLozengeParticle_WithTopLeft(pos1);
            MagikeHelper.SpawnLozengeParticle_WithTopLeft(pos2);
            MagikeHelper.SpawnDustOnSend(pos1, pos2);

            Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0, Owner.Center);
        }

        private void FailSelectPoint(bool b1, bool b2)
        {
            Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

            string text = "";
            if (b1)
                text = MagikeSystem.GetUIText(MagikeSystem.UITextID.MabirdSendLengthTooFar);
            if (b2)
                text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ItemContainerNotFound);

            PopupText.NewText(new AdvancedPopupRequest()
            {
                Color = Coralite.CrystallinePurple,
                Text = text,
                DurationInFrames = 60,
                Velocity = -Vector2.UnitY
            }, Main.MouseWorld - (Vector2.UnitY * 32));
        }

        private static bool HasContainer(Point16 topLeft)
        {
            if (MagikeHelper.TryGetEntityWithTopLeft(topLeft, out MagikeTP receiver))
                return true;

            if (Chest.FindChest(topLeft.X, topLeft.Y) != -1)
                return true;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Point16 position = new((int)Projectile.ai[0], (int)Projectile.ai[1]);

            if (!MagikeHelper.TryGetEntityWithComponent<MabirdController>(position.X, position.Y, MagikeComponentID.ItemContainer
                , out MagikeTP controller))
                return false;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 selfPos = Helper.GetMagikeTileCenter(controller.Position);

            switch (State)
            {
                default:
                case 0:
                    MagikeSystem.DrawConnectLine(spriteBatch, selfPos, aimPos, Main.screenPosition, c);
                    break;
                case 1:
                    if (controller.TryGetComponent(MagikeComponentID.ItemContainer, out MabirdController mabirdController)
                        && mabirdController[Index].ModItem is Mabird mabird)
                    {
                        Vector2 getItemPos = Helper.GetMagikeTileCenter(RecordGetItemPos);
                        MagikeSystem.DrawConnectLine(spriteBatch, selfPos, getItemPos, Main.screenPosition, Coralite.CrystallinePurple);
                        MagikeSystem.DrawConnectLine(spriteBatch, getItemPos, aimPos, Main.screenPosition, c);
                        MagikeSystem.DrawConnectLine(spriteBatch, aimPos, selfPos, Main.screenPosition, c);
                    }

                    break;
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}

