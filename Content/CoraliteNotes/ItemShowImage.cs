using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    public class ItemShowImage : UIElement
    {
        private float _scale = 1f;

        public UIElement chainedElement;

        public readonly int itemType;
        public readonly KnowledgeButtonType buttonType;
        public readonly Condition[] conditions;
        public bool canShow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="buttonType"></param>
        /// <param name="conditions"></param>
        public ItemShowImage(int itemType, KnowledgeButtonType buttonType,params Condition[] conditions)
        {
            this.itemType = itemType;
            this.buttonType = buttonType;
            this.conditions = conditions;
            this.SetSize(80, 80);
        }

        public void SetChainedElement(UIElement element)
            => chainedElement = element;

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched(CoraliteSoundID.MenuTick);
        }

        public override void Recalculate()
        {
            base.Recalculate();
            if (conditions != null)
                foreach (var condition in conditions)
                    if (!condition.IsMet())
                    {
                        canShow = false;
                        return;
                    }

            canShow = true;
        }

        public void DrawLine(SpriteBatch spriteBatch)
        {
            if (chainedElement == null)
                return;

            Texture2D tex = CoraliteAssets.Misc.White32x32.Value;
            Vector2 position = GetDimensions().Center();
            Vector2 target = chainedElement.GetDimensions().Center();
            Vector2 dir = target - position;

            spriteBatch.Draw(tex, position, null, Color.Brown, dir.ToRotation(), new Vector2(0, tex.Height / 2), new Vector2(dir.Length() / tex.Width, 3 / tex.Height), 0, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D BackTex = KnowledgeButtenTex.GetTex(buttonType);

            var frameBox = BackTex.Frame(2, 1, 1);

            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 position = calculatedStyle.Center();
            spriteBatch.Draw(BackTex, position, frameBox, Color.White * 0.3f, 0, frameBox.Size() / 2, 1, 0, 0);

            float iconRot = 0;

            if (IsMouseHovering)
            {
                if (canShow)//能显示就直接显示物品
                {
                    Main.HoverItem = ContentSamples.ItemsByType[itemType].Clone();
                    Main.hoverItemName = "a";
                }
                else//不能就显示文本
                {
                    string mouseText = "";
                    foreach (var condition in conditions)
                        if (!condition.IsMet())
                        {
                            if (string.IsNullOrEmpty(mouseText))
                                mouseText = condition.Description.Value;
                            else
                                mouseText = string.Concat(mouseText, Environment.NewLine, condition.Description.Value);
                        }

                    UICommon.TooltipMouseText(mouseText);
                }

                _scale = Helper.Lerp(_scale, 1.3f, 0.25f);
                iconRot = MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.05f;
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.25f);

            //绘制对应的图标
            DrawItem(spriteBatch, position, 75, iconRot);
            //绘制顶部的框
            frameBox = BackTex.Frame(2, 1);
            spriteBatch.Draw(BackTex, position, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
        }

        public void DrawItem(SpriteBatch spriteBatch,  Vector2 pos, float itemSize,float rot)
        {
            Helper.GetItemTexAndFrame(itemType, out Texture2D itemTex, out Rectangle frame);

            Vector2 origin = frame.Size() / 2;
            float itemScale = 1f;

            if (frame.Width > itemSize || frame.Height > itemSize)
            {
                if (frame.Width > itemSize)
                    itemScale = itemSize / frame.Width;
                else
                    itemScale = itemSize / frame.Height;
            }

            itemScale *= _scale;

            if (canShow)
            {
                Item i = ContentSamples.ItemsByType[itemType];
                spriteBatch.Draw(itemTex, pos, new Rectangle?(frame), i.GetColor(Color.White), rot, origin, itemScale, 0, 0f);
                if (i.color != default)
                    spriteBatch.Draw(itemTex, pos, new Rectangle?(frame), i.GetColor(Color.White), rot, origin, itemScale, 0, 0f);
            }
            else
                spriteBatch.Draw(itemTex, pos, new Rectangle?(frame), Color.Black * 0.75f, rot, origin, itemScale, 0, 0f);
        }
    }
}
