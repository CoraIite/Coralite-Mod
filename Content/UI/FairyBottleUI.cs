using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class FairyBottleUI : BetterUIState
    {
        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

        public bool visible;
        public override bool Visible => visible;

        public static BaseFairyBottle bottle;
        public static float OffsetX = 0;

        public override void OnInitialize()
        {

        }

        public void CloseUI(UIMouseEvent evt, UIElement listeningElement)
        {
        }


        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public override void Recalculate()
        {
            base.Recalculate();
        }

        public void ShowUI(BaseFairyBottle bottle)
        {
            visible = true;
            FairyBottleUI.bottle = bottle;

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
