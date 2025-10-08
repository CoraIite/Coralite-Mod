using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    [VaultLoaden(AssetDirectory.NoteMagikeS1)]
    public class ActivatedTheMachine : KnowledgePage
    {
        public static LocalizedText BuyActivateStaff { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale2 = new ScaleController(1.4f, 0.2f);
        private ScaleController _scale3 = new ScaleController(1f, 0.1f);

        public static ATex ActivateMachine { get; private set; }

        public override void OnInitialize()
        {
            BuyActivateStaff = this.GetLocalization(nameof(BuyActivateStaff));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            _scale2.ResetScale();
            _scale3.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = PageTop + new Vector2(0, 70);

            //左边激活杖 右边电路
            Vector2 picturePos = pos + new Vector2(-100, 0);
            Helper.DrawMouseOverScaleTex<MagikeActivator>(spriteBatch, picturePos
                , ref _scale1, 4, 5, fadeWithOriginScale: true);

            picturePos = pos + new Vector2(100, 0);
            Helper.DrawMouseOverScaleTex(spriteBatch, picturePos, ItemID.Timer1Second
                , ref _scale2, 4, 5, fadeWithOriginScale: true);

            pos.Y += 70;

            //描述段1
            DrawParaNormal(spriteBatch, BuyActivateStaff, pos.Y, out Vector2 textSize);
            pos.Y += ActivateMachine.Value.Height / 2 * _scale3.targetScale + textSize.Y + 30;

            Helper.DrawMouseOverScaleTex(spriteBatch, ActivateMachine.Value
                , pos, ref _scale3, 10);
        }
    }
}
