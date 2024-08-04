using Coralite.Core.Systems.CoraliteActorComponent;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class MagikeComponent : Component
    {
        /// <summary>
        /// 自定义UI中的显示
        /// </summary>
        /// <param name="parent"></param>
        public virtual void ShowInUI(UIElement parent)
        {

        }
    }
}
