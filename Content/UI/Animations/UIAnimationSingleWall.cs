using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationSingleWall(int WallType, AnimationBlockFrame frame, Vector2 center) : UIAnimationSingleTile(0, frame, center)
    {
        public override int GridSize => 18 * 2;
        public override int GridSizeInner => 16 * 2;

        public override float DeafaultDrawLayer => 1;

        public override void RecalculateOthers()
        {
            this.SetSize(new Vector2(16, 16));

            if (FairySystem.GetWallTypeToItemType.TryGetValue(WallType,out int itemType))
                HoverItemType = itemType;

            scale = 1;
        }

        public override Texture2D GetTex()
        {
            Main.instance.LoadWall(WallType);
            return TextureAssets.Wall[WallType].Value; 
        }
    }
}
