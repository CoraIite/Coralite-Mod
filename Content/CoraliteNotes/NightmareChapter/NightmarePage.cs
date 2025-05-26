using Coralite.Content.Items.BossSummons;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.NightmareChapter
{
    [AutoLoadTexture(Path = AssetDirectory.NoteNightmare)]
    public class NightmarePage : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Description { get; private set; }

        private ScaleController _scale1 = new ScaleController(1f, 0.2f);

        public static ATex NightmarePlantera {  get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Description = this.GetLocalization(nameof(Description));
        }

        public override void Recalculate()
        {
            _scale1.ResetScale();
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, Bosses.VanillaReinforce.NightmarePlantera.NightmarePlantera.nightmareRed);
            DrawParaNormal(spriteBatch, Description, Position.Y + TitleHeight, out _);

            Texture2D tex = NightmarePlantera.Value;

            //绘制图2
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, OverrideSamplerState ?? anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);

            tex.QuickBottomDraw(spriteBatch, Bottom - new Vector2(0, 10));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, OverflowHiddenRasterizerState, null, Main.UIScaleMatrix);

            #region 绘制噩梦竖琴

            Vector2 picturePos = new Vector2(Center.X - 205, Center.Y - 170);
            Helper.DrawMouseOverScaleTex<NightmareHarp>(spriteBatch, picturePos
                , ref _scale1, 1.5f, 5, fadeWithOriginScale: true);

            #endregion
        }
    }
}
