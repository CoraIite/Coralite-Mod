using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairyWindow : UIElement
    {
        private Fairy[] _fairies;

        private UpdateType _updateType;

        public enum UpdateType
        {
            InBottle,
            InCatcher
        }

        public FairyWindow()
        {
            _fairies = new Fairy[3];
        }

        public void ReFresh(int fairyType)
        {
            Height.Set(Parent.Height.Pixels / 2, 0);
            Width.Set(Height.Pixels / 2, 0);

            Recalculate();

            var style = GetDimensions();
            for (int i = 0; i < _fairies.Length; i++)
            {
                _fairies[i] = FairyLoader.GetFairy(fairyType).NewInstance();
                _fairies[i].Center = new Vector2(style.X + Main.rand.NextFloat(0, style.Width), style.Y + Main.rand.NextFloat(0, style.Height)) + Main.screenPosition;
            }


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            switch (_updateType)
            {
                default:
                case UpdateType.InBottle:
                    {

                    }
                    break;
                case UpdateType.InCatcher:
                    break;
            }
        }
    }
}
