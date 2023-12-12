using Coralite.Content.UI.UILib;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Content.UI.MagikeGuideBook.Introduce
{
    public class C0_P2_Catalog : UIPage
    {
        public override bool CanShowInBook => true;

        public override string LocalizationCategory => "MagikeSystem";

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Utils.DrawBorderStringBig(spriteBatch, "暂无", Center, Coralite.Instance.MagicCrystalPink, 1, 0.5f, 0.5f);

        }
    }
}
