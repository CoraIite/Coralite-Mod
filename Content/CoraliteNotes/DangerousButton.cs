using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    public class DangerousButton : UIElement, INoteLineDraw
    {
        public List<DangerousButton> PrevNodes;
        public List<DangerousButton> PostNodes;
        public List<DangerousButton> SameLevelNodes;

        private float _scale = 1f;


        public readonly KnowledgeButtonType buttonType;
        public bool canShow;
        public bool reverseLine;

        public Color lineColor = Color.White;

        public int index;

        public DangerousKnowledge knowledge;

        public DangerousButton(KnowledgeButtonType buttonType, DangerousKnowledge knowledge, int index)
        {
            this.buttonType = buttonType;
            this.SetSize(80, 80);
            this.index = index;
            this.knowledge = knowledge;
        }

        public void DrawLine(SpriteBatch spriteBatch)
        {
            if (PostNodes == null)
                return;

            Color c = lineColor;
            if (!canShow)
                c = new Color(120, 120, 120);
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 position = GetDimensions().Center();

            foreach (var chainedElement in PostNodes)
                DrawLineInner(spriteBatch, c, tex, position, chainedElement);

            if (SameLevelNodes == null)
                return;

            foreach (var chainedElement in SameLevelNodes)
                DrawLineInner(spriteBatch, c, tex, position, chainedElement);
        }

        private void DrawLineInner(SpriteBatch spriteBatch, Color c, Texture2D tex, Vector2 position, DangerousButton chainedElement)
        {
            Vector2 target = chainedElement.GetDimensions().Center();
            if (reverseLine)
                (target, position) = (position, target);

            Vector2 dir = target - position;

            spriteBatch.Draw(tex, position, null, c, dir.ToRotation(), new Vector2(0, tex.Height / 2), new Vector2(dir.Length() / tex.Width, 64f / tex.Height), 0, 0);
        }

        public void AddPostNode(DangerousButton element)
        {
            PostNodes ??= [];
            PostNodes.Add(element);

            element.PrevNodes ??= [];
            element.PrevNodes.Add(this);
        }

        public void AddSameLevelNode(DangerousButton element)
        {
            SameLevelNodes ??= [];
            PrevNodes.Add(element);

            element.PostNodes ??= [];
            element.PostNodes.Add(this);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            
            if (knowledge.DangerousTurnOn[index])//关闭
            {
                SetClose();
                if (PostNodes != null)//关闭所有后置节点
                    foreach (var item in PostNodes)
                        item.SetOpen();
            }
            else//打开
            {
                SetOpen();
                if (PrevNodes != null)//开启所有前置节点
                    foreach (var item in PrevNodes)
                        item.SetOpen();
                if (SameLevelNodes != null)//关闭所有同级节点
                    foreach (var item in SameLevelNodes)
                        item.SetClose();
            }
        }

        public void SetOpen()
        {
            if (!knowledge.DangerousTurnOn[index])
            {
                knowledge.DangerousTurnOn[index] = true;

                if (VaultUtils.isClient)
                    knowledge.SyncDangerousTrunOn();
            }
        }

        public void SetClose()
        {
            if (knowledge.DangerousTurnOn[index])
            {
                knowledge.DangerousTurnOn[index] = false;
                if (VaultUtils.isClient)
                    knowledge.SyncDangerousTrunOn();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D BackTex = KnowledgeButtenTex.GetTex(buttonType);

            Rectangle frameBox;

            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 position = calculatedStyle.Center();
            if (BackTex != null)
            {
                frameBox = BackTex.Frame(2, 1, 1);
                spriteBatch.Draw(BackTex, position, frameBox, Color.White * 0.3f, 0, frameBox.Size() / 2, 1, 0, 0);
            }

            float iconRot = 0;

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(knowledge.Texts[index].Value);

                _scale = Helper.Lerp(_scale, 1.3f, 0.25f);
                iconRot = MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.05f;
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.25f);

            //绘制对应的图标
            Color drawColor = new Color(50,50,50);
            if (knowledge.DangerousTurnOn[index])
                drawColor = Color.White;

            knowledge.Texes[index].Value.QuickCenteredDraw(spriteBatch, position, drawColor, iconRot);

            //绘制顶部的框
            if (BackTex != null)
            {
                frameBox = BackTex.Frame(2, 1);
                spriteBatch.Draw(BackTex, position, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
            }

            if (knowledge.DangerousTurnOn[index])//绘制危险度星星
            {
                int level = knowledge.DangerousLevels[index];
                float length = calculatedStyle.Width * 0.8f;
                for (int i = 0; i < level; i++)
                {
                    Helper.DrawPrettyStarSparkle(1, 0, position + (-MathHelper.PiOver2 + i * MathHelper.TwoPi / length).ToRotationVector2() * length, Color.Red, Color.White * 0.4f, 0.5f, 0, 0, 1, 1, 0, new Vector2(2, 1), Vector2.One / 2);
                }
            }
        }
    }
}
