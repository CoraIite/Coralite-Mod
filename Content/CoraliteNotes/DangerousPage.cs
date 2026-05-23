using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    public abstract class DangerousPage<T> : KnowledgePage where T : DangerousKnowledge
    {
        public List<INoteLineDraw> nodes;
        public DangerousBar bar;

        public void ClearNodes()
            => nodes?.Clear();

        public abstract int NPCID { get; }

        /// <summary>
        /// 决定线条最大浮动偏移量
        /// </summary>
        public virtual float FlowPercent { get; } = 0.1f;

        public abstract void AddNodes();

        /// <summary>
        /// 如果重写必须调用<see cref="AddNodes"/>
        /// </summary>
        public override void OnInitialize()
        {
            //DangerousKnowledge k = (DangerousKnowledge)CoraliteContent.GetKnowledge<T>();
            //k.Initialize();

            AddNodes();
            AddBar();
        }

        public void AddBar()
        {
            DangerousKnowledge k = (DangerousKnowledge)CoraliteContent.GetKnowledge<T>();
            float width = PageWidth * 0.8f;
            bar = new DangerousBar(new Vector2(width, 8),k);

            bar.SetCenter(new Vector2(0, -40), new Vector2(0.5f, 1));
            Append(bar);

            for (int i = 0; i < k.Rewards.Length; i++)
            {
                DangerousReward r = new DangerousReward(k, i);
                r.SetCenter(new Vector2(PageWidth * 0.1f + width * k.Rewards[i].level / k.MaxDangerousLevel, -40), new Vector2(0, 1));

                Append(r);
            }
        }

        public void RemoveBar()
        {
            for (int i = Elements.Count-1; i >=0; i--)
            {
                var ele = Elements[i];
                if (ele is DangerousBar or DangerousReward)
                    Elements.Remove(ele);
            }
        }

        public override void Recalculate()
        {
#if DEBUG
            ClearNodes();
            RemoveAllChildren();

            AddNodes();
#endif

            RemoveBar();
            AddBar();


            base.Recalculate();
        }

        public DangerousButton NewButton(Vector2 pos, int index, KnowledgeButtonType type)
        {
            var button = new DangerousButton(type, (DangerousKnowledge)CoraliteContent.GetKnowledge<T>(), index, NPCID);
            button.SetCenter(pos, Vector2.One / 2);

            nodes ??= [];
            nodes.Add(button);
            Append(button);
            return button;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (nodes != null && nodes.Count > 0)
            {
                Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

                spriteBatch.End();
                Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);

                Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
                spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
                spriteBatch.GraphicsDevice.RasterizerState = EffectLoader.OverflowHiddenRasterizerState;
                Effect e = ShaderLoader.GetShader("SinLine");
                e.Parameters["flowPercent"].SetValue(0.06f);
                float time = (float)Main.timeForVisualEffects * 0.02f;
                float flowTime = -(float)Main.timeForVisualEffects * 0.003f;
                e.Parameters["uTime"].SetValue(time);
                e.Parameters["uFlowTime"].SetValue(flowTime);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

                //绘制线条
                int i = 0;
                foreach (var node in nodes)
                {
                    node.DrawLine(spriteBatch);
                    i++;
                    e.Parameters["uTime"].SetValue(time + i * 0.23f);
                    e.Parameters["uFlowTime"].SetValue(flowTime - i * 0.3f);
                }

                RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

                spriteBatch.End();
                spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
                spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
            }

            base.DrawChildren(spriteBatch);
        }
    }

    public class DangerousReward : UIElement
    {
        private float _scale = 1f;

        private DangerousKnowledge knowledge;
        private int rewardIndex;

        public DangerousReward( DangerousKnowledge knowledge, int rewardIndex)
        {
            this.SetSize(80, 80);
            this.knowledge = knowledge;
            this.rewardIndex = rewardIndex;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            //获得奖励
            if (!knowledge.RewardsCollect[rewardIndex]&&CanGetReward)
            {
                Main.LocalPlayer.QuickSpawnItem(new EntitySource_Gift(Main.LocalPlayer), knowledge.Rewards[rewardIndex].item);

                knowledge.RewardsCollect[rewardIndex] = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float iconRot = 0;
            if (IsMouseHovering)
            {
                if (knowledge.RewardsCollect[rewardIndex])
                    UICommon.TooltipMouseText(CoraliteNoteSystem.RewardCollected.Value);
                else
                {
                    Main.HoverItem = knowledge.Rewards[rewardIndex].item.Clone();
                    Main.hoverItemName = "a";
                }

                _scale = Helper.Lerp(_scale, 1.3f, 0.25f);
                iconRot = MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.05f;
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.25f);

            //绘制对应的图标
            DrawItem(spriteBatch, GetDimensions().Center(), 34, iconRot);

            string s = $"[c/fbf236:{knowledge.Rewards[rewardIndex].level}]";
            Vector2 size = Helper.GetStringSize(s, Vector2.One);

            Utils.DrawBorderString(spriteBatch, s, GetDimensions().Center() + new Vector2(-size.X / 2, -30), Color.White, 1, 0f, 0.5f);
        }

        public bool CanGetReward
            => knowledge.ChallengeLevel >= knowledge.Rewards[rewardIndex].level;

        public void DrawItem(SpriteBatch spriteBatch, Vector2 pos, float itemSize, float rot)
        {
            Helper.GetItemTexAndFrame(knowledge.Rewards[rewardIndex].item.type, out Texture2D itemTex, out Rectangle frame);

            Vector2 origin = frame.Size() / 2;
            float itemScale = 1f;

            if (frame.Width > itemSize || frame.Height > itemSize)
            {
                float wScale = 1;
                float hScale = 1;

                if (frame.Width > itemSize)
                    wScale = itemSize / frame.Width;
                if (frame.Height > itemSize)
                    hScale = itemSize / frame.Height;

                itemScale = Math.Min(wScale, hScale);
            }

            if (CanGetReward)
            {
                Item i = knowledge.Rewards[rewardIndex].item;
                spriteBatch.Draw(itemTex, pos, frame, i.GetAlpha(Color.White), rot, origin, itemScale, 0, 0f);
                if (i.color != default)
                    spriteBatch.Draw(itemTex, pos, frame, i.GetColor(Color.White), rot, origin, itemScale, 0, 0f);
            }
            else
                spriteBatch.Draw(itemTex, pos, new Rectangle?(frame), Color.Black * 0.75f, rot, origin, itemScale, 0, 0f);
        }
    }

    public class DangerousBar : UIElement
    {
        private DangerousKnowledge knowledge;

        public DangerousBar(Vector2 size, DangerousKnowledge knowledge)
        {
            this.SetSize(size);
            this.knowledge = knowledge;
            this.OverflowHidden = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var d = GetDimensions();

            Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

            spriteBatch.End();

            Effect e = ShaderLoader.GetShader("SinLine");
            e.Parameters["flowPercent"].SetValue(0.06f);
            float time = (float)Main.timeForVisualEffects * 0.02f;
            float flowTime = -(float)Main.timeForVisualEffects * 0.003f;
            e.Parameters["uTime"].SetValue(time);
            e.Parameters["uFlowTime"].SetValue(flowTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

            //绘制线条
            Vector2 pos = d.Position() + new Vector2(0, 24);
            Texture2D tex = CoraliteNoteSystem.NoteConnectLine.Value;
            Vector2 origin = new Vector2(0, tex.Width / 2);
            spriteBatch.Draw(tex, pos, null, Color.White, 0,origin , new Vector2(d.Width, 64) / tex.Size(), 0, 0);
            
            spriteBatch.Draw(tex, pos - new Vector2(0, 4), null, Color.Red, 0, origin, new Vector2(d.Width * knowledge.GeCurrentDangerous() / knowledge.MaxDangerousLevel, 64) / tex.Size(), 0, 0);
            
            spriteBatch.Draw(tex, pos + new Vector2(0, 4), null, Color.Yellow, 0, origin, new Vector2(d.Width * knowledge.ChallengeLevel / knowledge.MaxDangerousLevel, 64) / tex.Size(), 0, 0);

            RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, anisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);

            string s = $"[c/ff1818:{knowledge.GeCurrentDangerous()}] / {knowledge.MaxDangerousLevel} / [c/fbf236:{knowledge.ChallengeLevel}]";
            Vector2 size = Helper.GetStringSize(s, Vector2.One);
            Utils.DrawBorderString(spriteBatch, s, d.Center() + new Vector2(-size.X / 2, 30), Color.White, 1, 0f, 0.5f);
        }
    }
}
