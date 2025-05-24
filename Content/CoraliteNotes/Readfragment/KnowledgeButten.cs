using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    /// <summary>
    /// 框线类型
    /// </summary>
    public enum KnowledgeButtonType
    {
        Rune,
        Wild,
        Reel,
        Ball,
        Coral,
    }

    [AutoLoadTexture(Path = AssetDirectory.NoteReadfragment)]
    public class KnowledgeButtenTex
    {
        public static ATex KnowledgeButtenRune { get; private set; }
        public static ATex KnowledgeButtenWild { get; private set; }
        public static ATex KnowledgeButtenReel { get; private set; }
        public static ATex KnowledgeButtenBall { get; private set; }
        public static ATex KnowledgeButtenCoral { get; private set; }

        public static ATex NewKnowledge { get; private set; }
    }

    public class KnowledgeButten<T> : UIElement where T : KeyKnowledge
    {
        private float _scale = 1f;
        private KnowledgeButtonType buttonType;
        public const string Line = "一一一一一一一";

        public KnowledgeButten(KnowledgeButtonType type)
        {
            buttonType = type;
            this.SetSize(80, 80);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            KeyKnowledge knowledge = CoraliteContent.GetKKnowledge<T>();

            if (knowledge.Unlock)
            {
                knowledge.ReadKnowledge = true;
                int index = knowledge.FirstPageInCoraliteNote;
                if (index >= 0)
                {
                    CoraliteNoteUIState.BookPanel.CurrentDrawingPage = index;
                    Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);
                }

                UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();
            }
            else
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0);
            }
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched(CoraliteSoundID.MenuTick);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            //绘制背景板
            Texture2D BackTex = buttonType switch
            {
                KnowledgeButtonType.Rune => KnowledgeButtenTex. KnowledgeButtenRune.Value,
                KnowledgeButtonType.Wild => KnowledgeButtenTex.KnowledgeButtenWild.Value,
                KnowledgeButtonType.Reel => KnowledgeButtenTex.KnowledgeButtenReel.Value,
                KnowledgeButtonType.Ball => KnowledgeButtenTex.KnowledgeButtenBall.Value,
                KnowledgeButtonType.Coral => KnowledgeButtenTex.KnowledgeButtenCoral.Value,
                _ => KnowledgeButtenTex.KnowledgeButtenRune.Value,
            };

            var frameBox = BackTex.Frame(2, 1, 1);

            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 position = calculatedStyle.Center();
            spriteBatch.Draw(BackTex, position, frameBox, Color.White * 0.3f, 0, frameBox.Size() / 2, 1, 0, 0);

            KeyKnowledge knowledge = CoraliteContent.GetKKnowledge<T>();
            Color c = knowledge.Unlock ? Color.White : Color.Black*0.75f;

            float iconRot = 0;

            if (IsMouseHovering)
            {
                string mouseText = knowledge.Unlock ?
                    string.Concat(knowledge.KnowledgeName.Value, Environment.NewLine
                        , Line, Environment.NewLine
                        , knowledge.Description.Value, Environment.NewLine
                        , FragmentPage.ClickToJump.Value)
                    : string.Concat("? ? ?", Environment.NewLine
                        , Line, Environment.NewLine
                        , knowledge.LockTip.Value);

                UICommon.TooltipMouseText(mouseText);

                _scale = Helper.Lerp(_scale, 1.3f, 0.25f);
                iconRot = MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f;
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.25f);

            //绘制对应的图标
            Texture2D iconTex = knowledge.Texture2D.Value;
            spriteBatch.Draw(iconTex, position, null, c, iconRot, iconTex.Size() / 2, _scale, 0, 0);

            //绘制顶部的框
            frameBox = BackTex.Frame(2, 1);
            spriteBatch.Draw(BackTex, position, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);

            if (knowledge.Unlock && !knowledge.ReadKnowledge)
            {
                Vector2 pos = position + new Vector2(calculatedStyle.Width / 4, -calculatedStyle.Height / 4);
                KnowledgeButtenTex.NewKnowledge.Value.QuickCenteredDraw(spriteBatch, new Rectangle(0, (Main.timeForVisualEffects % 20) > 10 ? 0 : 1, 1, 2)
                    , pos, Color.White, 0, 1.1f + MathF.Cos(Main.GlobalTimeWrappedHourly*4) * 0.15f);
            }
        }
    }
}
