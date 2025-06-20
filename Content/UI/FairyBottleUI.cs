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

        public bool visible;
        public override bool Visible => visible;

        public static float OffsetX = 0;

        public UIPanel ContainFairyPanel;

        public override void OnInitialize()
        {
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
                visible = false;
                Recalculate();
            }
        }

        public override void Recalculate()
        {
            base.Recalculate();
        }

        public void ShowUI()
        {
            visible = true;

            Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0);

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

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            Helper.PlayPitched("Fairy/ButtonTick", 0.4f, 0);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Player p = Main.LocalPlayer;
            
            if (p.selectedItem == 58 && p.HeldItem.ModItem is BaseFairyBottle && p.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                fcp.BottleItem = p.HeldItem.Clone();
                p.HeldItem.TurnToAir();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //绘制藤蔓线
            Texture2D vineTex = Vine.Value;
            var d = GetDimensions();
            Vector2 p = d.Center() + new Vector2(0, -d.Height / 2);

            Rectangle rect = new Rectangle((int)(d.X), -20, vineTex.Width, (int)(d.Height + 20));

            spriteBatch.Draw(vineTex, rect, Color.White);

            //绘制仙灵环

            //绘制仙灵瓶物品

        }
    }
}
