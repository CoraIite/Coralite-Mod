using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class BabyIceDragon_OnSpawnAnim : ModProjectile
    {
        public override string Texture => AssetDirectory.BabyIceDragon + "BIDNameLine";

        public Color drawCharColor;
        public Color drawPicColor;
        public readonly Color blankColor = new Color(0, 0, 0, 0);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 260;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;

        public override void OnSpawn(IEntitySource source)
        {
            drawCharColor = new Color(0, 0, 0, 0);
            drawPicColor = new Color(0, 0, 0, 0);
        }

        public override void AI()
        {
            int timer = 260 - Projectile.timeLeft;

            //文字渐进
            if (timer < 21)
            {
                drawCharColor = Color.Lerp(blankColor, Coralite.Instance.IcicleCyan, (float)timer / 20);
                drawPicColor = Color.Lerp(blankColor, Color.White, (float)timer / 20);
                return;
            }

            //文字减淡
            if (timer > 239)
            {
                drawCharColor = Color.Lerp(Coralite.Instance.IcicleCyan, blankColor, (float)(timer - 240) / 20);
                drawPicColor = Color.Lerp(Color.White, blankColor, (float)(timer - 240) / 20);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 screenPosition = Main.screenPosition;

            Utils.DrawBorderStringBig(sb, ModContent.GetInstance<BabyIceDragon>().DisplayName.Value, Main.LocalPlayer.Center - new Vector2(0, 315) - screenPosition, drawCharColor, 1f, 0.5f);

            sb.Draw(mainTex, Main.LocalPlayer.Center - new Vector2(0, 225) - screenPosition, mainTex.Frame(), drawPicColor, 0f, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 2f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
