using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MabirdController : ItemContainer, IUIShowable
    {
        public override void Update()
        {

        }

        #region 绘制

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in Items)
            {
                if (CoraliteSets.Mabird[item.type])
                    (item.ModItem as Mabird).DrawMabird(spriteBatch);
            }
        }

        #endregion


        #region 存储

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);
        }

        #endregion

        #region 同步

        #endregion

        #region UI

        public void ShowInUI(UIElement parent)
        {
        }

        #endregion
    }
}
