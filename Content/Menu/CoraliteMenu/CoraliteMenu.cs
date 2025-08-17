using Coralite.Content.Biomes;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace Coralite.Content.Menu.CoraliteMenu
{
    [AutoLoadTexture(Path = AssetDirectory.Menus)]
    public class CoraliteMenu : ModMenu
    {
        public static ATex CoraliteLogoTex { get; set; }

        public override ATex Logo => CoraliteLogoTex;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TheCoralite");

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<TempCoraliteMenuBackground>();

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            CoraliteLogoTex.Value.QuickCenteredDraw(spriteBatch, logoDrawCenter, drawColor, 0, 1);

            return false;
        }
    }

    public class TempCoraliteMenuBackground : CrystallineSkyIslandBackground
    {
        public override int FirstLayerHeight => base.FirstLayerHeight;
        public override int SecondLayerHeight => base.SecondLayerHeight;
        public override int ThirdLayerHeight => base.ThirdLayerHeight;
    }
}
