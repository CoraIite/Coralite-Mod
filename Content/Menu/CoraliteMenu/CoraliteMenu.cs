using Coralite.Content.Biomes;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Menu.CoraliteMenu
{
    [AutoLoadTexture(Path = AssetDirectory.Menus)]
    public class CoraliteMenu : ModMenu
    {
        public static ATex CoraliteLogoTex { get; set; }

        public override ATex Logo => CoraliteLogoTex;

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/TheCoralite");

        public override ATex SunTexture =>CoraliteAssets.Blank;
        public override ATex MoonTexture => CoraliteAssets.Blank;

        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<TempCoraliteMenuBackground>();

        public override void Update(bool isOnTitleScreen)
        {
            Main.dayTime = true;
            Main.time = Main.dayLength / 2;
            for (int i = 0; i < Main.maxClouds; i++)
                Main.cloud[i].Alpha *= 0.8f;
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            CoraliteLogoTex.Value.QuickCenteredDraw(spriteBatch, logoDrawCenter+new Vector2(0,20), Color.White, 0, 1);

            return false;
        }
    }

    public class TempCoraliteMenuBackground : CrystallineSkyIslandBackground
    {
        public override int FirstLayerHeight => 10;
        public override int SecondLayerHeight => -200;
        public override int ThirdLayerHeight => -100;

        public override float ScreenY => 0;
    }
}
