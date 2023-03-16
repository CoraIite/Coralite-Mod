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

namespace Coralite.Content.Bosses.Rediancie
{
    public class Rediancie_OnSpawnAnim : ModProjectile
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

            //文字渐出
            if (timer < 21)
            {
                drawCharColor = Color.Lerp(blankColor, Coralite.Instance.RedJadeRed, (float)timer / 20);
                drawPicColor = Color.Lerp(blankColor, Color.White, (float)timer / 20);
                return;
            }

            //闪烁
            if (timer > 119 && timer < 240)
            {
                float r;
                if (timer < 151)
                    r = 0.104f;     //Pi/30
                else if (timer < 171)
                    r = 0.157f;     //Pi/20
                else
                    r = 0.314f;       //Pi/10
                float cosProgress = -MathF.Cos((timer - 120) * r) * 0.5f + 0.5f;
                drawCharColor = Color.Lerp(Coralite.Instance.RedJadeRed, Color.White, cosProgress);
            }

            //生成粒子和声音
            if (timer > 190 && timer % 8 == 0 && Main.netMode != NetmodeID.Server)
            {
                Helper.PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, Projectile.Center);
                Color red = new Color(221, 50, 50);
                Color grey = new Color(91, 93, 102);
                Vector2 center = Main.LocalPlayer.Center - new Vector2(0, 250) + Main.rand.NextVector2Circular(200, 140);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(6, 6), 0, red, Main.rand.NextFloat(1f, 1.5f));
                    Dust.NewDustPerfect(center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(4, 4), 0, grey, Main.rand.NextFloat(0.8f, 1.2f));
                }
            }

            //文字减淡
            if (timer > 239)
            {
                drawCharColor = Color.Lerp(Coralite.Instance.RedJadeRed, blankColor, (float)(timer - 240) / 20);
                drawPicColor = Color.Lerp(Color.White, blankColor, (float)(timer - 240) / 20);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Texture2D maintex = TextureAssets.Projectile[Type].Value;
            Vector2 screenPosition = Main.screenPosition;

            Utils.DrawBorderStringBig(sb, "赤玉灵", Main.LocalPlayer.Center - new Vector2(100, 305) - screenPosition, drawCharColor, 1.4f);

            sb.Draw(maintex, Main.LocalPlayer.Center - new Vector2(0, 225) - screenPosition, null, drawPicColor, 0f, new Vector2(maintex.Width / 2, maintex.Height / 2), 2f, SpriteEffects.None, 0f);

            Utils.DrawBorderStringBig(sb, "Rediancie", Main.LocalPlayer.Center - new Vector2(75, 180) - screenPosition, drawCharColor, 0.8f);

            return false;
        }
    }
}
