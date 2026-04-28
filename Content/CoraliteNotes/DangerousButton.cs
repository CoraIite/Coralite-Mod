using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Core;
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
        public int npcID;

        public DangerousKnowledge knowledge;

        public DangerousButton(KnowledgeButtonType buttonType, DangerousKnowledge knowledge, int index,int npcID)
        {
            this.buttonType = buttonType;
            this.SetSize(80, 80);
            this.index = index;
            this.npcID = npcID;
            this.knowledge = knowledge;
        }

        public void DrawLine(SpriteBatch spriteBatch)
        {
            Color c = lineColor;
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 position = GetDimensions().Center();

            if (PostNodes != null)
            {
                foreach (var chainedElement in PostNodes)
                    DrawLineInner(spriteBatch, c, tex, position, chainedElement);
            }

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
            SameLevelNodes.Add(element);

            element.SameLevelNodes ??= [];
            element.SameLevelNodes.Add(this);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (NPC.AnyNPCs(npcID))
            {
                Helper.PlayPitched(AssetDirectory.Sounds.UI + "Error", 0.4f, 0);
                return;
            }

            if (knowledge.DangerousTurnOn[index])//关闭
                ClosePost();
            else//打开
            {
                OpenPrev();
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

        public void ClosePost()
        {
            SetClose();

            if (PostNodes != null)//关闭所有后置节点
                foreach (var item in PostNodes)
                    item.ClosePost();
        }

        public void OpenPrev()
        {
            SetOpen();

            if (PrevNodes != null)//开启所有前置节点
                foreach (var item in PrevNodes)
                    item.OpenPrev();
        }



        public DangerousButton SetColor(Color c)
        {
            lineColor = c;
            return this;
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


            if (knowledge.DangerousTurnOn[index])//绘制危险度星星
            {
                int level = knowledge.DangerousLevels[index];
                float length = calculatedStyle.Width * 0.2f;
                for (int i = 0; i < level; i++)
                {
                    Helper.DrawPrettyStarSparkle(1, 0, position + (-MathHelper.PiOver2 + i * MathHelper.TwoPi / level).ToRotationVector2() * length, Color.Red, Color.Red, 0.5f, 0, 0.5f, 0.5f, 1, 0.785f, Vector2.One*0.5f, Vector2.One/2);
                }
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


            //绘制顶部的框
            if (BackTex != null)
            {
                frameBox = BackTex.Frame(2, 1);
                spriteBatch.Draw(BackTex, position, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
            }

            //绘制对应的图标
            Color drawColor = new Color(50,50,50);
            if (knowledge.DangerousTurnOn[index])
                drawColor = Color.White;

            knowledge.Texes[index].Value.QuickCenteredDraw(spriteBatch, position, drawColor, iconRot, _scale);
        }
    }
}
