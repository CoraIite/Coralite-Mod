using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
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
        private readonly UIGrid grid = new();

        public MabirdControlBar(MabirdController controller,int index)
        {
            SetPadding(6);

            this.SetSize(-4, 66, 1);

            grid.SetSize(1000, 0, 0, 1);

            var slot = new MabirdSlot(controller, index);
            //slot.SetSize(46, 0, 0, 1);
            grid.Add(slot);

            grid.Add(new UIVerticalLine());



            Append(grid);
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
                if (mabird.State != Mabird.MabirdAIState.Rest)
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
            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref inv2, context, position, Color.White);
        }
    }

    public class MabirdVerticalLine() : UIVerticalLine()
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

        }
    }

    public class RouteDrawButton:UIElement
    {
        public RouteDrawButton()
        {

        }
    }

    public class CopyRouteButton:UIElement
    {
        public static MabirdTarget CopyGetItemPos;
        public static MabirdTarget CopyReleaseItemPos;


    }

    public class PasteRouteButton:UIElement
    {

    }

    public class WhiteListSlot:UIElement
    {

    }
}
