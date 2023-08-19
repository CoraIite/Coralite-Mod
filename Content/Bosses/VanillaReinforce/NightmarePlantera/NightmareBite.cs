using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0控制它的状态
    ///  0：正常，张开嘴之后咬下
    ///  1：会另外绘制一个幻影，张开嘴后咬下
    ///  2：会另外绘制一个幻影，只是吓唬玩家一下，不会有任何的伤害判定
    ///  在为1和2时不跟踪本体的中心<br></br>
    ///  使用ai1传入蓄力时间<br></br>
    /// 使用ai2传入颜色
    /// -1：黑紫色
    /// -2：红色
    /// 大于0：该颜色所对应的彩色
    /// </summary>
    public class NightmareBite : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float ReadyTime => ref Projectile.ai[1];
        public ref float ColorState => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        /// <summary>
        /// 嘴张开的角度
        /// </summary>
        private float mouseAngle;
        private float alpha;
        private float distanceToOwner;

        public Color DrawColor;
        private bool Init = true;
        private bool canDamage;

        public override void SetDefaults()
        {
            Projectile.width = 116;
            Projectile.height = 116 / 2;
            Projectile.timeLeft = 1000;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (canDamage)
            {
                float length = Projectile.width + distanceToOwner;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation + mouseAngle).ToRotationVector2() *length)
                    || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation - mouseAngle).ToRotationVector2() * length);
            }

            return false;
        }

        public override void AI()
        {
            if (Init)
            {
                if (ColorState == -1)
                    DrawColor = new Color(100, 100, 130);
                else if (ColorState == -2)
                    DrawColor = new Color(255, 20, 20, 130);
                else
                    DrawColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState,0,1f),1f,0.8f));

                Init = false;

                Vector2 center = Projectile.Center;
                Projectile.scale = 1.5f;
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = center;
            }

            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC Owner))
            {
                Projectile.Kill();
                return;
            }

            switch ((int)State)
            {
                default:
                case 0: //普通的
                    {
                        Projectile.Center = Owner.Center;

                        if (Timer < ReadyTime)
                        {
                            float factor = Timer / ReadyTime;

                            Projectile.rotation = Owner.rotation;
                            mouseAngle = Helper.Lerp(0, 1.4f, factor);
                            alpha = Helper.Lerp(0.75f, 0.1f, factor);
                            break;
                        }

                        if (Timer < ReadyTime + 15)
                        {
                            canDamage = true;
                            mouseAngle = Helper.Lerp(mouseAngle, -0.08f, 0.6f);
                            alpha = Helper.Lerp(alpha, 0.8f, 0.6f);
                            if (mouseAngle>0.1f)
                            {
                                distanceToOwner+=32;
                            }
                            break;
                        }

                        if (Timer < ReadyTime + 40)
                        {
                            canDamage = false;
                            mouseAngle = Helper.Lerp(mouseAngle, 0f, 0.05f);

                            alpha = Helper.Lerp(alpha, 0f, 0.1f);
                            break;
                        }

                        Projectile.Kill();
                    }
                    break;
            }

            Timer++;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            NightmarePlantera.NightmareHit(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制上下鄂
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(1, 2, 0, 0);
            Vector2 origin = frameBox.BottomLeft();
            Color c = DrawColor * alpha;

            float rot = Projectile.rotation - mouseAngle;
            Vector2 dir = rot.ToRotationVector2();
            Main.spriteBatch.Draw(mainTex, pos+dir*distanceToOwner, frameBox,c , rot, origin, Projectile.scale ,0, 0);

            rot = Projectile.rotation + mouseAngle;
            dir = rot.ToRotationVector2();
            frameBox = mainTex.Frame(1, 2, 0, 1);
            Main.spriteBatch.Draw(mainTex, pos + dir * distanceToOwner, frameBox, c, rot, Vector2.Zero, Projectile.scale, 0, 0);

            return false;
        }
    }
}
