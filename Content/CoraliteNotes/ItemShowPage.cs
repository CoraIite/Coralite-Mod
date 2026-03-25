using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.CoraliteNotes
{
    public abstract class ItemShowPage : KnowledgePage
    {
        public List<ItemShowImage> images;

        public void ClearImages()
            => images?.Clear();

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
            image.SetCenter(pos,Vector2.One/2);

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

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (images != null && images.Count > 0)
            {
                foreach (var image in images)
                    image.DrawLine(spriteBatch);
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
}
