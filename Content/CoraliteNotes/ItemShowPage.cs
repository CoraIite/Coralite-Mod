using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    public abstract class ItemShowPage : KnowledgePage
    {
        public List<ItemShowImage> images;

        public void ClearImages()
            => images?.Clear();

        /// <summary>
        /// 决定线条最大浮动偏移量
        /// </summary>
        public virtual float FlowPercent { get; } = 0.1f;

        /// <summary>
        /// 新建物品显示，默认中心是书页中心
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public ItemShowImage NewImage<T>(Vector2 pos, KnowledgeButtonType type = KnowledgeButtonType.Normal, params Condition[] conditions) where T : ModItem
        {
            var image = new ItemShowImage(ModContent.ItemType<T>(), type, conditions);
            image.SetCenter(pos, Vector2.One / 2);

            Append(image);
            images ??= [];
            images.Add(image);
            return image;
        }

        /// <summary>
        /// 新建物品显示，默认中心是书页中心
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public ItemShowImage NewImage(int itemType, Vector2 pos, KnowledgeButtonType type = KnowledgeButtonType.Normal, params Condition[] conditions)
        {
            var image = new ItemShowImage(itemType, type, conditions);
            image.SetCenter(pos, Vector2.One / 2);

            Append(image);
            images ??= [];
            images.Add(image);
            return image;
        }

        /// <summary>
        /// 新建标注，默认中心是书页中心
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public ItemShowMark NewMark(Vector2 pos,ItemShowMark.MarkType type, Color c, float rot=0,  float scale=1)
        {
            var mark = new ItemShowMark(type, rot, c, scale);
            mark.SetSize(32, 32);
            mark.SetCenter(pos, Vector2.One / 2);

            Append(mark);
            return mark;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (images != null && images.Count > 0)
            {
                Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
                SamplerState anisotropicClamp = SamplerState.AnisotropicClamp;

                spriteBatch.End();
                Rectangle clippingRectangle = GetClippingRectangle(spriteBatch);

                Rectangle adjustedClippingRectangle = Rectangle.Intersect(clippingRectangle, spriteBatch.GraphicsDevice.ScissorRectangle);
                spriteBatch.GraphicsDevice.ScissorRectangle = adjustedClippingRectangle;
                spriteBatch.GraphicsDevice.RasterizerState = OverflowHiddenRasterizerState;
                Effect e = ShaderLoader.GetShader("SinLine");
                e.Parameters["flowPercent"].SetValue(0.06f);
                float time = (float)Main.timeForVisualEffects * 0.02f;
                float flowTime = -(float)Main.timeForVisualEffects * 0.003f;
                e.Parameters["uTime"].SetValue(time);
                e.Parameters["uFlowTime"].SetValue(flowTime);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, OverflowHiddenRasterizerState, e, Main.UIScaleMatrix);

                //绘制线条
                int i = 0;
                foreach (var image in images)
                {
                    image.DrawLine(spriteBatch);
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

        //public void AddImage(ItemShowImage image)
        //{
        //    Append(image);
        //    images ??= [];
        //    images.Add(image);
        //}
    }

    public class ItemShowMark(ItemShowMark.MarkType type, float rot, Color c, float scale) : UIElement
    {
        public enum MarkType
        {
            NodeSmall,
            NodeBig,
            Arrow
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CoraliteNoteSystem.ItemShowMarkTex.Value.QuickCenteredDraw(spriteBatch, new Rectangle((int)type, 0, 3, 1), GetDimensions().Center()
                , c, rot, scale);
        }
    }
}
