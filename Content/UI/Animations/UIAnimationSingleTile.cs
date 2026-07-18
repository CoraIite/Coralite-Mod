using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Content.UI.Animations
{
    public class UIAnimationSingleTile(int tileType, AnimationBlockFrame frame, Vector2 center) : UIAnimationComponent(center)
    {
        public virtual int GridSize => 18;
        public virtual int GridSizeInner => 16;

        public override float DeafaultDrawLayer => 2;

        private int RandFrameType = Main.rand.Next(3);
        public float scale = 1;

        public override void RecalculateOthers()
        {
            this.SetSize(new Vector2(16, 16));
            int itemType = TileLoader.GetItemDropFromTypeAndStyle(tileType);
            if (itemType > 0)
                HoverItemType = itemType;

            scale = 1;
        }

        public override void DrawAnimation(SpriteBatch spriteBatch, int timer, Vector2 center, float fadeFactor)
        {
            Color c = DrawColor * fadeFactor;
            center += FadeOffset * (1 - fadeFactor);

            Texture2D tex = GetTex();
            Rectangle frameBox = GetBlockRect(frame, GridSize, RandFrameType, GridSizeInner);

            if (IsMouseHovering)
                scale = Helper.Lerp(scale, 1.25f, 0.2f);
            else
                scale = Helper.Lerp(scale, 1.05f, 0.2f);

            spriteBatch.Draw(tex, center, frameBox, c, Rotation, frameBox.Size() / 2, scale, 0, 0);

            //Helper.DrawDebugFrame(this,spriteBatch);
        }

        public virtual Texture2D GetTex()
        {
            Main.instance.LoadTiles(tileType);
            return TextureAssets.Tile[tileType].Value;
        }

        public static Rectangle GetBlockRect(AnimationBlockFrame frame, int perFrameSize, int RandFrameType, int gridSize)
        {
            int width = gridSize;
            int height = gridSize;

            RandFrameType *= perFrameSize;

            return frame switch
            {
                AnimationBlockFrame.LeftSide => new Rectangle(0, RandFrameType, width, height),
                AnimationBlockFrame.RightSide => new Rectangle(4 * perFrameSize, RandFrameType, width, height),
                AnimationBlockFrame.TopSide => new Rectangle(1 * perFrameSize + RandFrameType, 0, width, height),
                AnimationBlockFrame.DownSide => new Rectangle(1 * perFrameSize + RandFrameType, 2 * perFrameSize, width, height),
                AnimationBlockFrame.LeftTip => new Rectangle(9 * perFrameSize, RandFrameType, width, height),
                AnimationBlockFrame.RightTip => new Rectangle(12 * perFrameSize, RandFrameType, width, height),
                AnimationBlockFrame.TopTip => new Rectangle(6 * perFrameSize + RandFrameType, 0, width, height),
                AnimationBlockFrame.DownTip => new Rectangle(6 * perFrameSize + RandFrameType, 3 * perFrameSize, width, height),
                AnimationBlockFrame.TopLeftCorner => new Rectangle(RandFrameType * 2, 3 * perFrameSize, width, height),
                AnimationBlockFrame.TopRightCorner => new Rectangle(1 * perFrameSize + RandFrameType * 2, 3 * perFrameSize, width, height),
                AnimationBlockFrame.DownLeftCorner => new Rectangle(RandFrameType * 2, 4 * perFrameSize, width, height),
                AnimationBlockFrame.DownRightCorner => new Rectangle(1 * perFrameSize + RandFrameType * 2, 4 * perFrameSize, width, height),
                AnimationBlockFrame.VerticalLine => new Rectangle(5 * perFrameSize, 4 * perFrameSize + RandFrameType, width, height),
                AnimationBlockFrame.HorizontalLine => new Rectangle(6 * perFrameSize + RandFrameType, 4 * perFrameSize, width, height),
                AnimationBlockFrame.Inside => new Rectangle(1 * perFrameSize + RandFrameType, 1 * perFrameSize, width, height),
                _ => new Rectangle(9 * perFrameSize + RandFrameType, 3 * perFrameSize, width, height),
            };
        }
    }
}
