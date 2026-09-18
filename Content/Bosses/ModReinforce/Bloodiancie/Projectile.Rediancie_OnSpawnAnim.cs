using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    /// <summary>
    /// 入场名牌演出弹幕：纯表现，不造成伤害。走 Boss 敌对弹幕基座以拿到一次性 <see cref="Initialize"/> 钩子
    /// （旧代码自带的 <c>span</c> 闸写反了，Initialize 从未执行过；两个颜色字段的默认值本就是全透明，行为不变）。
    /// </summary>
    public class Bloodiancie_OnSpawnAnim : CoraliteBossHostileProj
    {
        public override string Texture => AssetDirectory.Rediancie + "RediancieNameLine";

        public Color drawCharColor;
        public Color drawPicColor;
        public readonly Color blankColor = new(0, 0, 0, 0);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 260;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;
        public override void Initialize()
        {
            drawCharColor = blankColor;
            drawPicColor = blankColor;
        }

        public override void AI()
        {
            int timer = 260 - Projectile.timeLeft;

            //文字渐出
            if (timer < 21)
            {
                drawCharColor = Color.Lerp(blankColor, Coralite.RedJadeRed, (float)timer / 20);
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
                float cosProgress = (-MathF.Cos((timer - 120) * r) * 0.5f) + 0.5f;
                drawCharColor = Color.Lerp(Coralite.RedJadeRed, Color.White, cosProgress);
            }

            //生成粒子和声音
            if (timer > 190 && timer % 8 == 0)
                Helper.RedJadeExplosion(Main.LocalPlayer.Center - new Vector2(0, 250) + Main.rand.NextVector2Circular(200, 140));

            //文字减淡
            if (timer > 239)
            {
                drawCharColor = Color.Lerp(Coralite.RedJadeRed, blankColor, (float)(timer - 240) / 20);
                drawPicColor = Color.Lerp(Color.White, blankColor, (float)(timer - 240) / 20);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            Texture2D maintex = Projectile.GetTextureValue();
            Vector2 screenPosition = Main.screenPosition;

            Utils.DrawBorderStringBig(sb, ModContent.GetInstance<Bloodiancie>().DisplayName.Value, Main.LocalPlayer.Center - new Vector2(0, 325) - screenPosition, drawCharColor, 1.3f, 0.5f);

            sb.Draw(maintex, Main.LocalPlayer.Center - new Vector2(0, 225) - screenPosition, null, drawPicColor, 0f, new Vector2(maintex.Width / 2, maintex.Height / 2), 2f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
