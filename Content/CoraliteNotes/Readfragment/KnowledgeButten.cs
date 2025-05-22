using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
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
    }

    public class KnowledgeButten<T> : UIElement where T : KeyKnowledge
    {


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

            Vector2 position = GetDimensions().Center();
            spriteBatch.Draw(BackTex, position, frameBox, Color.White * 0.3f, 0, frameBox.Size() / 2, 1, 0, 0);

            KeyKnowledge knowledge = CoraliteContent.GetKKnowledge<T>();
            Color c = knowledge.Unlock ? Color.White : Color.Black*0.75f;

            //绘制对应的图标
            Texture2D iconTex = knowledge.Texture2D.Value;
            spriteBatch.Draw(iconTex, position, null, c, 0, iconTex.Size() / 2, IsMouseHovering ? 1.4f : 1, 0, 0);

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
            }

            frameBox = BackTex.Frame(2, 1);
            spriteBatch.Draw(BackTex, position, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
        }
    }
}
