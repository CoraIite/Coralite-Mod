using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class BabyIceDragon_OnSpawnAnim : ModProjectile
    {
        public override string Texture => AssetDirectory.Rediancie + "RediancieNameLine";

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

            Texture2D maintex = TextureAssets.Projectile[Type].Value;
            Vector2 screenPosition = Main.screenPosition;

            Utils.DrawBorderStringBig(sb, "冰龙宝宝", Main.LocalPlayer.Center - new Vector2(100, 305) - screenPosition, drawCharColor, 1.4f);

            sb.Draw(maintex, Main.LocalPlayer.Center - new Vector2(0, 225) - screenPosition, maintex.Frame(), drawPicColor, 0f, new Vector2(maintex.Width / 2, maintex.Height / 2), 2f, SpriteEffects.None, 0f);

            Utils.DrawBorderStringBig(sb, "BabyIceDragon", Main.LocalPlayer.Center - new Vector2(75, 180) - screenPosition, drawCharColor, 0.8f);

            return false;
        }
    }
}
