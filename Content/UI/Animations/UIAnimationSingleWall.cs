using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Content.UI.Animations
{
    internal class UIAnimationSingleWall(int WallType, AnimationBlockFrame frame, Vector2 center) : UIAnimationSingleTile(WallType, frame,center)
    {
        public override int GridSize => 18;
        public override int GridSizeInner => 16;
    }
}
