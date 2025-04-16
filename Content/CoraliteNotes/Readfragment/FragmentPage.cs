using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    public class FragmentPage : KnowledgePage
    {
        public static LocalizedText ClickToJump { get; set; }

        public FixedUIGrid SlotGrid;

        public override void OnInitialize()
        {
            ClickToJump = this.GetLocalization(nameof(ClickToJump));
            SlotGrid = new FixedUIGrid();
            SlotGrid.QuickInvisibleScrollbar();
        }

        public override void Recalculate()
        {
            RemoveAllChildren();
            SlotGrid.SetSize(-30, -60, 1, 1);
            SlotGrid.SetTopLeft(30, 15);
            SlotGrid.Clear();
            AddFragments(SlotGrid);
            Append(SlotGrid);

            base.Recalculate();
        }

        public static void AddFragments(FixedUIGrid grid)
        {
            //遍历知识，把所有的知识都加入进去
            List<KeyKnowledge> knowledges = [];

            foreach (var item in KeyKnowledgeLoader.knowledgesF)
            {
                knowledges.Add(item.Value);
            }

            knowledges.Sort((k1, k2) => k1.Type.CompareTo(k2.Type));

            foreach (var item in knowledges)
            {
                var slot = new FragmentSlot(item.InnerType);
                grid.Add(slot);
            }
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.NoteReadfragment)]
    public class FragmentSlot : UIElement
    {
        public int KnowledgeID;
        public const string Line = "一一一一一一一";

        [AutoLoadTexture(Name = "KnowledgeSlot")]
        public static ATex SlotTex { get; private set; }

        public FragmentSlot(int knowledgeID)
        {
            this.SetSize(54, 54);
            KnowledgeID = knowledgeID;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            KeyKnowledge knowledge = CoraliteContent.GetKKnowledge(KnowledgeID);
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
            Texture2D BackTex = SlotTex.Value;
            KeyKnowledge knowledge = CoraliteContent.GetKKnowledge(KnowledgeID);

            spriteBatch.Draw(BackTex, GetDimensions().Center(), null, Color.White, 0, BackTex.Size() / 2, 1, 0, 0);

            Color c = knowledge.Unlock ? Color.White : Color.Black;

            //绘制对应的图标
            Texture2D iconTex = knowledge.Texture2D.Value;
            spriteBatch.Draw(iconTex, GetDimensions().Center(), null, c, 0, iconTex.Size() / 2, IsMouseHovering ? 1.2f : 1, 0, 0);

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
        }
    }
}
