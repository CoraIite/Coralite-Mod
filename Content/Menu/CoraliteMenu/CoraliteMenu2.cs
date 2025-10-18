using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Menu.CoraliteMenu
{
#if DEBUG
    public class CoraliteMenu2 : CoraliteMenu
    {
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<CoraliteMenuBack>();
    }

#endif
    [VaultLoaden(AssetDirectory.Menus)]
    public class CoraliteMenuBack : ModSurfaceBackgroundStyle
    {
        public static ATex CoraliteMenuBackBack { get; set; }

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(CoraliteMenuBackBack.Value,
                Vector2.Zero, null, Color.White, 0, Vector2.Zero, Main.ScreenSize.X / (float)CoraliteMenuBackBack.Width(), 0, 0);

            return false;
        }
    }
}
