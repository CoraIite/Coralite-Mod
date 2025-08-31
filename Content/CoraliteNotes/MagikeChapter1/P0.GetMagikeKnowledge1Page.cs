using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    [VaultLoaden(AssetDirectory.NoteMagikeS1)]
    public class GetMagikeKnowledge1Page : KnowledgePage
    {
        public static LocalizedText Title { get; private set; }
        public static LocalizedText Welcome { get; private set; }
        public static LocalizedText ClickButton { get; private set; }
        public static LocalizedText ContiuneRead { get; private set; }

        public static ATex MagikePage { get; private set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            Welcome = this.GetLocalization(nameof(Welcome));
            ClickButton = this.GetLocalization(nameof(ClickButton));
            ContiuneRead = this.GetLocalization(nameof(ContiuneRead));
        }

        //public override void Recalculate()
        //{
        //    //RemoveAllChildren();
        //    //Append(new CheckButton());
        //    base.Recalculate();
        //}

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, Coralite.MagicCrystalPink);

            float pos = Position.Y + TitleHeight;
            DrawParaNormal(spriteBatch, Welcome, pos, out Vector2 textSize);

            pos += textSize.Y + 30;

            DrawParaNormal(spriteBatch, ContiuneRead, pos, out _);

            MagikePage.Value.QuickBottomDraw(spriteBatch, Bottom + new Vector2(0, -10));
        }
    }

    //public class CheckButton : UIElement
    //{
    //    private float _scale = 1f;

    //    public CheckButton()
    //    {
    //        Width.Set(CoraliteAssets.MagikeChapter1.KnowledgeCheckButton.Width(), 0f);
    //        Height.Set(CoraliteAssets.MagikeChapter1.KnowledgeCheckButton.Height(), 0f);

    //        this.SetTopLeft(-Width.Pixels / 2, -Height.Pixels / 2, 0.8f, 0.5f);
    //    }

    //    protected override void DrawSelf(SpriteBatch spriteBatch)
    //    {
    //        if (IsMouseHovering)
    //        {
    //            Main.LocalPlayer.mouseInterface = true;
    //            _scale = Helper.Lerp(_scale, 1.1f, 0.2f);
    //        }
    //        else
    //            _scale = Helper.Lerp(_scale, 1f, 0.2f);

    //        CalculatedStyle dimensions = GetDimensions();
    //        Texture2D tex = CoraliteAssets.MagikeChapter1.KnowledgeCheckButton.Value;

    //        var frame = tex.Frame(2, 1);
    //        spriteBatch.Draw(tex, dimensions.Center(), frame, Color.White, 0, frame.Size() / 2, _scale, 0, 0);

    //        if (MagikeSystem.learnedMagikeBase)
    //        {
    //            frame = tex.Frame(2, 1, 1);
    //            spriteBatch.Draw(tex, dimensions.Center(), frame, Color.White, 0, frame.Size() / 2, _scale, 0, 0);
    //        }
    //    }

    //    public override void MouseOver(UIMouseEvent evt)
    //    {
    //        base.MouseOver(evt);
    //        Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
    //    }

    //    public override void LeftClick(UIMouseEvent evt)
    //    {
    //        if (!MagikeSystem.learnedMagikeBase)
    //        {
    //            MagikeSystem.learnedMagikeBase = true;
    //            //TODO: 同步

    //            Helper.PlayPitched("UI/Success", 0.4f, 0f);
    //        }
    //        base.LeftClick(evt);
    //    }
    //}
}
