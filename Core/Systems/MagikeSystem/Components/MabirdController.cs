using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MabirdController : ItemContainer
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


        }

        #endregion
    }

    /// <summary>
    /// 魔鸟的UI条
    /// </summary>
    public class MabirdControlBar : UIElement
    {
        public MabirdControlBar(MabirdController controller, int index)
        {
            SetPadding(6);

            this.SetSize(0, 66, 1);

            //添加物品格
            var slot = new MabirdSlot(controller, index);
            //slot.SetSize(46, 0, 0, 1);
            Append(slot);
            float left = slot.Width.Pixels;

            //添加分隔线
            MabirdVerticalLine line = new MabirdVerticalLine(controller, index);
            line.SetTopLeft(0, left);
            Append(line);
            left += line.Width.Pixels;

            //添加画线段按钮

            //添加复制黏贴按钮

            //添加白名单物品格
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var d = GetDimensions();
            var p = d.Position();
            Texture2D mainTex = MagikeAssets.AlphaBar.Value;

            var target = new Rectangle((int)p.X, (int)p.Y, (int)d.Width, (int)d.Height);
            var self = mainTex.Frame();

            spriteBatch.Draw(mainTex, target, self, MagikeApparatusPanel.BackgroundColor);
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

            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is Mabird && i.IsAir) //鼠标有物品并且UI内为空，放入
            {
                controller.AddItemByIndex(Main.mouseItem, index);
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }

            if (!Main.mouseItem.IsAir && Main.mouseItem.ModItem is Mabird && !i.IsAir) //都有物品，进行交换
            {
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
        }
    }

    public class MabirdVerticalLine(MabirdController controller, int index) : UIVerticalLine()
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Item item = controller[index];
            if (!item.IsAir && item.ModItem is Mabird mabird)
            {
                Texture2D tex = MagikeAssets.MabirdStateButton.Value;

                var center = GetDimensions().Center();
                int frameY = 1;
                if (mabird.GetItemPos != null && mabird.ReleaseItemPos != null)//有目标点时变绿色，否则为红色
                    frameY = 0;

                var frameBox = tex.Frame(1, 2, 0, frameY);
                spriteBatch.Draw(tex, center, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
            }

        }
    }

    public class RouteDrawButton : UIElement
    {
        public RouteDrawButton()
        {

        }
    }

    public class CopyRouteButton : UIElement
    {
        public static MabirdTarget CopyGetItemPos;
        public static MabirdTarget CopyReleaseItemPos;


    }

    public class PasteRouteButton : UIElement
    {

    }

    public class WhiteListSlot(MabirdController controller, int index) : UIElement
    {
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
                if (IsMouseHovering)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Item i2 = mabird.WhiteListItem;
                    ItemSlot.OverrideHover(ref i2, context);
                    ItemSlot.MouseHover(ref i2, context);
                }
            }

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref inv2, context, position, Color.White);
        }
    }

    public class MabirdRouteDrawProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        private Vector2 selfPos;
        private Vector2 aimPos;
        private Color c;
        private bool start = true;

        public ref float Index => ref Projectile.ai[2];
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
            selfPos = Helper.GetMagikeTileCenter(position);

            Point16 currentPoint = InMousePos.ToTileCoordinates16();// new((int)InMousePos.X / 16, (int)InMousePos.Y / 16);
            aimPos = currentPoint.ToWorldCoordinates();

            if (!MagikeHelper.TryGetEntityWithComponent<MabirdController>(position.X, position.Y, MagikeComponentID.ItemContainer
                , out MagikeTP controller))
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                default:
                case 0://点击左键查找取出点
                    {

                    }
                    break;
                case 1://点击左键查找放入点
                    {

                    }
                    break;
            }

            MagikeHelper.TryGetEntity(currentPoint.X, currentPoint.Y, out MagikeTP receiver);

            if (receiver != null)
            {
                currentPoint = receiver.Position;
                aimPos = Helper.GetMagikeTileCenter(currentPoint);
            }

            if (controller == receiver)
                return;

            MagikeLinerSender senderComponent = controller.GetSingleComponent<MagikeLinerSender>(MagikeComponentID.MagikeSender);

            bool canConnect = senderComponent.CanConnect(currentPoint, out string failText);
            c = canConnect ? Color.GreenYellow : Color.MediumVioletRed;

            if (start)
            {
                if (!Owner.controlUseItem)
                {
                    start = false;
                }

                return;
            }

            if (Owner.controlUseItem)
            {
                do
                {
                    if (receiver == null || !canConnect)//无法连接，并写明原因
                    {
                        Helper.PlayPitched("UI/Error", 0.4f, 0, Owner.Center);

                        PopupText.NewText(new AdvancedPopupRequest()
                        {
                            Color = Coralite.MagicCrystalPink,
                            Text = failText,
                            DurationInFrames = 60,
                            Velocity = -Vector2.UnitY
                        }, Main.MouseWorld - (Vector2.UnitY * 32));

                        break;
                    }

                    senderComponent.Connect(receiver.Position);
                    PopupText.NewText(new AdvancedPopupRequest()
                    {
                        Color = Coralite.MagicCrystalPink,
                        Text = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.Connect_Success),
                        DurationInFrames = 60,
                        Velocity = -Vector2.UnitY
                    }, Main.MouseWorld - (Vector2.UnitY * 32));

                    MagikeHelper.SpawnLozengeParticle_WithTopLeft(controller.Position);
                    MagikeHelper.SpawnLozengeParticle_WithTopLeft(receiver.Position);
                    MagikeHelper.SpawnDustOnSend(controller.Position, receiver.Position);

                    Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0, Owner.Center);

                } while (false);

                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            MagikeSystem.DrawConnectLine(spriteBatch, selfPos, aimPos, Main.screenPosition, c);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
