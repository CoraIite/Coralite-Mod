using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0控制状态<br></br>
    /// 为0时尝试缓慢逃离玩家，当玩家接触后触发梦魇花的特殊动作，之后状态变为-1，将会环绕在玩家身边<br></br>
    /// 为1时自身不动，当玩家靠近后同上<br></br>
    /// 为2时会逃离梦魇花，玩家靠近时同上<br></br>
    /// -2状态为聚合所有的自身并生成美梦神<br></br>
    /// </summary>
    public class FantasySparkle : ModNPC, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public ref float State => ref NPC.ai[0];

        public Vector2 mainSparkleScale;
        public float circleSparkleScale;

        public override void SetDefaults()
        {
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.friendly = true;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 500;

            mainSparkleScale = new Vector2(5f, 2f);
            circleSparkleScale = 1.25f;

            NPC.HitSound = CoraliteSoundID.MountSummon_Item25;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            switch ((int)State)
            {
                case -2://聚合并生成美梦神
                    break;
                case -1://在玩家头顶盘旋
                    break;
                default:
                case 0://缓慢逃离玩家
                    break;
                case 1://仅检测玩家靠近，自身不动
                    break;
                case 2://逃离梦魇花
                    break;
            }

            if (Main.rand.NextBool(3))
                for (int i = -1; i < 2; i += 2)
                {
                    int type = Main.rand.NextFromList(DustID.PlatinumCoin, DustID.GoldCoin);
                    Vector2 dir = new Vector2(i, 0);
                    Dust d = Dust.NewDustPerfect(NPC.Center, type, dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(1, 4), Scale: Main.rand.NextFloat(1, 1.5f));
                    //d.noGravity = true;
                }
        }

        public override bool PreKill()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Vector2 pos = NPC.Center - screenPos;
            float rot = NPC.rotation + MathHelper.PiOver2;
            Color shineColor = new Color(252, 233, 194);
            //中心的闪光
            Helpers.ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos, Color.White, shineColor * 0.6f,
                0.5f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale, Vector2.One);

            for (int i = -1; i < 2; i += 2)
            {
                Helpers.ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos + new Vector2(i * 16, 0), Color.White, shineColor * 0.6f,
                    0.5f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale*0.4f, Vector2.One);
            }

            //周围一圈小星星
            for (int i = 0; i < 7; i++)
            {
                Vector2 dir = (Main.GlobalTimeWrappedHourly * 2 + i * MathHelper.TwoPi / 7).ToRotationVector2();
                Helpers.ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos + dir * (36 + factor * 4),  Color.White, NightmarePlantera.phantomColors[i],
                    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, rot, new Vector2(circleSparkleScale, circleSparkleScale), Vector2.One);
            }

            //绘制额外旋转的星星，和上面叠起来变成一个
            //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos, Color.White * 0.3f, shineColor * 0.5f,
            //    0.5f - factor * 0.1f, 0f, 0.5f, 0.5f, 1f, NPC.rotation + MathHelper.PiOver4, new Vector2(mainSparkleScale.Y * 0.75f), Vector2.One);

            //绘制一层小的更加亮的来让星星中心变亮点
            //Helpers.ProjectilesHelper.DrawPrettyStarSparkle(NPC.Opacity, 0, pos, Color.White * 0.7f, Color.White * 0.4f,
            //    0.5f + factor * 0.1f, 0f, 0.5f, 0.5f, 1f, rot, mainSparkleScale * 0.5f, Vector2.One * 2);
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Npc[NPC.type].Value;
            spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, null, Color.White, 0, mainTex.Size() / 2, (1+0.1f*MathF.Sin(Main.GlobalTimeWrappedHourly))*mainSparkleScale.Y/4, 0, 0);
        }
    }
}
