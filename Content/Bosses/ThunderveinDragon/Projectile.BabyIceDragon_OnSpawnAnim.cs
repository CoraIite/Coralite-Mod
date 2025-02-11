using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderveinDragon_OnSpawnAnim : BaseHeldProj
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "ThunderveinDragonNameLine";

        public Color drawCharColor;
        public Color drawPicColor;
        public readonly Color blankColor = new(0, 0, 0, 0);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;

        public override void Initialize()
        {
            drawCharColor = new Color(0, 0, 0, 0);
            drawPicColor = new Color(0, 0, 0, 0);
        }

        public override void AI()
        {
            int timer = 180 - Projectile.timeLeft;

            //文字渐进
            if (timer < 21)
            {
                drawCharColor = Color.Lerp(blankColor, Coralite.ThunderveinYellow, (float)timer / 20);
                drawPicColor = Color.Lerp(blankColor, Color.White, (float)timer / 20);
                return;
            }

            //文字减淡
            if (timer > 159)
            {
                drawCharColor = Color.Lerp(Coralite.ThunderveinYellow, blankColor, (float)(timer - 160) / 20);
                drawPicColor = Color.Lerp(Color.White, blankColor, (float)(timer - 160) / 20);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 screenPosition = Main.screenPosition;

            Utils.DrawBorderStringBig(sb, ModContent.GetInstance<ThunderveinDragon>().DisplayName.Value, Main.LocalPlayer.Center - new Vector2(0, 315) - screenPosition, drawCharColor, 1f, 0.5f);

            sb.Draw(mainTex, Main.LocalPlayer.Center - new Vector2(0, 225) - screenPosition, mainTex.Frame(), drawPicColor, 0f, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 1.5f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
