using Coralite.Content.Items.BossSummons;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class SlimePage2 : KnowledgePage
    {
        public static LocalizedText GelInvitationDescription { get; private set; }

        private ScaleController _scale1 = new ScaleController(1.5f, 0.2f);

        public override void OnInitialize()
        {
            GelInvitationDescription = this.GetLocalization(nameof(GelInvitationDescription));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();

            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = TitlePos;
            DrawParaNormal(spriteBatch, GelInvitationDescription, pos.Y, out _);

            float scale1 = 1f;
            Texture2D tex = CoraliteAssets.Slime1.SlimeEmperor.Value;

            //绘制图2
            tex.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10), scale: scale1);

            #region 绘制凝胶邀请函

            Vector2 picturePos = new Vector2(pos.X - 170, pos.Y + 505);
            Helper.DrawMouseOverScaleTex<GelInvitation>(spriteBatch, picturePos
                , ref _scale1, 4, 5, fadeWithOriginScale: true);

            #endregion
        }
    }
}
