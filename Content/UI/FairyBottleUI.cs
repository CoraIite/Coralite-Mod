using Coralite.Core;
using Coralite.Core.Attributes;
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
                if (time>0)
                {
                    time++;
                    Recalculate();
                }
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
            bottleHang.SetCenter(new Vector2(620, 0));

            Append(bottleHang);
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
        public static ATex Vine { get; set; }

        public FairyBottleHang()
        {
            this.SetSize(52, 60);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Main.playerInventory && GetDimensions().Y > -20)
            {
                Top.Set(Top.Pixels - 4, 0);
                Recalculate();
            }
            else if (Main.playerInventory && GetDimensions().Y < 60)
            {
                Top.Set(Top.Pixels + 4, 0);
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

            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp) || p.selectedItem != 58 || !p.ItemTimeIsZero)
                return;

            //放入
            if (fcp.BottleItem.IsAir && !p.HeldItem.IsAir && p.HeldItem.ModItem is BaseFairyBottle)
            {
                fcp.BottleItem = p.HeldItem.Clone();
                p.HeldItem.TurnToAir();
                return;
            }

            //取出
            if (p.HeldItem.IsAir && !fcp.BottleItem.IsAir)
            {
                p.inventory[58] = fcp.BottleItem.Clone();
                fcp.BottleItem.TurnToAir();
                return;
            }

            //交换
            if (!p.HeldItem.IsAir && !fcp.BottleItem.IsAir && p.HeldItem.ModItem is BaseFairyBottle)
            {
                Item i = fcp.BottleItem;
                fcp.BottleItem = p.HeldItem;
                p.inventory[58] = i;
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


        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //绘制藤蔓线
            Texture2D vineTex = Vine.Value;
            var d = GetDimensions();
            Vector2 pos = d.Center() + new Vector2(0, -d.Height / 2);

            int height = (int)(d.Height + 20);
            Rectangle rect = new Rectangle((int)(pos.X), (int)(pos.Y - height), vineTex.Width, height);

            spriteBatch.Draw(vineTex, rect, Color.White);

            Player p = Main.LocalPlayer;

            if (!p.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return;

            //绘制仙灵环
            if (fcp.BottleItem.IsAir)
            {

            }
            else //绘制仙灵瓶物品
            {

            }
        }
    }
}
