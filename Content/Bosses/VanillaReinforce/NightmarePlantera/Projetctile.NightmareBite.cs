using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0控制它的状态
    ///  0：正常，张开嘴之后咬下<br></br>
    ///  1：会另外绘制一个幻影，张开嘴后咬下<br></br>
    ///  2：会另外绘制一个幻影，只是吓唬玩家一下，不会有任何的伤害判定<br></br>
    ///  3：只会对owner造成伤害<br></br>
    ///  在为1和2时不跟踪本体的中心<para></para>
    ///  使用ai1传入蓄力时间<para></para>
    /// 使用ai2传入颜色<br></br>
    /// -1：黑紫色<br></br>
    /// -2：红色<br></br>
    /// 0-1：该数值对应的hue颜色<br></br>
    /// </summary>
    public class NightmareBite : BaseNightmareProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        public Player Target => Main.player[Projectile.owner];

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
        /// <summary>
        /// 生成时随机一个数，之后从梦魇花的phantomColor里拿到指定的颜色，共7种颜色
        /// </summary>
        private int phantomColor;


        public Color DrawColor;
        private bool Init = true;
        private bool canDamage;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 116 / 2;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (canDamage)
            {
                float length = Projectile.width + distanceToOwner;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation + mouseAngle).ToRotationVector2() * length)
                    || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation - mouseAngle).ToRotationVector2() * length);
            }

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)State == 3)
                return false;

            return base.CanHitNPC(target);
        }

        public override bool CanHitPlayer(Player target)
        {
            if ((int)State == 3)
                return target.whoAmI == Projectile.owner;

            return base.CanHitPlayer(target);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.friendly)
            {
                modifiers.SourceDamage += 5;
            }
        }

        public override void AI()
        {
            if (Init)
            {
                if (ColorState == -1)
                    DrawColor = NightmarePlantera.lightPurple;
                else if (ColorState == -2)
                    DrawColor = NightmarePlantera.nightmareRed;
                else
                    DrawColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState, 0, 1f), 1f, 0.8f));

                Init = false;

                phantomColor = Main.rand.Next(7);
                Vector2 center = Projectile.Center;
                Projectile.scale = 1.7f;
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = center;

                if (State == 1)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_DeathCalling2_Item104, Projectile.Center);
                    Projectile.rotation = (Target.Center - Projectile.Center).ToRotation();
                }
                else if (State == 2)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, Projectile.Center);
                    Projectile.rotation = (Target.Center - Projectile.Center).ToRotation();
                }
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
                case 3://强制秒杀
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
                            if (mouseAngle > 0.1f)
                            {
                                distanceToOwner += 32;
                            }

                            if ((int)Timer == (int)ReadyTime + 4)
                            {
                                float length = Projectile.width + distanceToOwner;
                                Vector2 dir = Projectile.rotation.ToRotationVector2();
                                Vector2 velDir = dir.RotatedBy(MathHelper.PiOver2);
                                for (int i = (int)distanceToOwner; i < length; i += 6)
                                {
                                    Vector2 pos = Projectile.Center + dir * i;
                                    for (int j = -3; j < 4; j += 2)
                                    {
                                        Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(6, 6), DustID.VilePowder,
                                            velDir * j * Main.rand.NextFloat(1, 6), 150, DrawColor, Main.rand.NextFloat(1, 2f));
                                        dust.noGravity = true;
                                    }
                                }

                                var modifyer = new PunchCameraModifier(Projectile.Center, (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2(),
                                    20, 14, 12, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);

                                SoundStyle st = CoraliteSoundID.BottleExplosion_Item107;
                                st.Pitch = -0.5f;
                                st.Volume += 0.25f;
                                SoundEngine.PlaySound(st, Projectile.Center);
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
                case 1://幻影咬
                    {
                        if (Timer < ReadyTime * 0.9f)
                        {
                            Projectile.rotation = Projectile.rotation.AngleTowards((Target.Center - Projectile.Center).ToRotation(), 0.3f);
                            if (Vector2.Distance(Projectile.Center, Target.Center) > 200)
                            {
                                float speed = Projectile.velocity.Length();
                                speed += 0.85f;
                                if (speed > 30)
                                    speed = 30;

                                float velRot = Projectile.velocity.ToRotation();
                                Projectile.velocity = velRot.AngleTowards(Projectile.rotation, 0.3f).ToRotationVector2() * speed;
                            }
                            else
                                Projectile.velocity *= 0.7f;
                        }
                        else
                            Projectile.velocity *= 0.7f;

                        if (Timer < ReadyTime)
                        {
                            float factor = Timer / ReadyTime;

                            mouseAngle = Helper.Lerp(0, 1.4f, factor);
                            alpha = Helper.Lerp(0.75f, 0.1f, factor);
                            break;
                        }

                        if (Timer < ReadyTime + 15)
                        {
                            canDamage = true;
                            mouseAngle = Helper.Lerp(mouseAngle, -0.08f, 0.6f);
                            alpha = Helper.Lerp(alpha, 0.8f, 0.6f);
                            if (mouseAngle > 0.1f)
                            {
                                distanceToOwner += 32;
                            }

                            if ((int)Timer == (int)ReadyTime + 4)
                            {
                                float length = Projectile.width + distanceToOwner;
                                Vector2 dir = Projectile.rotation.ToRotationVector2();
                                Vector2 velDir = dir.RotatedBy(MathHelper.PiOver2);
                                for (int i = (int)distanceToOwner; i < length; i += 6)
                                {
                                    Vector2 pos = Projectile.Center + dir * i;
                                    for (int j = -3; j < 4; j += 2)
                                    {
                                        Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(6, 6), DustID.VilePowder,
                                            velDir * j * Main.rand.NextFloat(1, 6), 150, DrawColor, Main.rand.NextFloat(1, 2f));
                                        dust.noGravity = true;
                                    }
                                }

                                var modifyer = new PunchCameraModifier(Projectile.Center, (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2(),
                                    20, 14, 12, 1000);
                                Main.instance.CameraModifiers.Add(modifyer);

                                SoundStyle st = CoraliteSoundID.BottleExplosion_Item107;
                                st.Pitch = -0.7f;
                                st.Volume += 0.25f;
                                SoundEngine.PlaySound(st, Projectile.Center);
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
                case 2://幻影咬，张嘴后消失
                    {
                        if (Timer < ReadyTime)
                        {
                            float factor = Timer / ReadyTime;

                            Projectile.rotation = Projectile.rotation.AngleTowards((Target.Center - Projectile.Center).ToRotation(), 0.3f);
                            if (Vector2.Distance(Projectile.Center, Target.Center) > 200)
                            {
                                float speed = Projectile.velocity.Length();
                                speed += 0.85f;
                                if (speed > 26)
                                    speed = 26;

                                float velRot = Projectile.velocity.ToRotation();
                                Projectile.velocity = velRot.AngleTowards(Projectile.rotation, 0.3f).ToRotationVector2() * speed;
                            }
                            else
                                Projectile.velocity *= 0.78f;

                            mouseAngle = Helper.Lerp(0, 1.4f, factor);
                            alpha = Helper.Lerp(0.75f, 0f, factor);
                            break;
                        }

                        Projectile.Kill();
                    }
                    break;
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            //绘制幻影本体
            if ((State == 1 || State == 2) && NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Texture2D shadowTex = TextureAssets.Npc[np.type].Value;
                Rectangle frame = shadowTex.Frame(1, Main.npcFrameCount[np.type], np.frame.X, np.frame.Y);
                Vector2 origin2 = frame.Size() / 2;

                Main.spriteBatch.Draw(shadowTex, pos, frame, NightmarePlantera.phantomColors[phantomColor] * Utils.Remap(alpha, 0, 0.5f, 0, 1), Projectile.rotation + MathHelper.PiOver2, origin2, np.scale, 0, 0);
            }

            //绘制上下鄂
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frameBox = mainTex.Frame(1, 2, 0, 0);
            Vector2 origin = frameBox.BottomLeft();
            Color c = DrawColor * alpha;

            float rot = Projectile.rotation - mouseAngle;
            Vector2 dir = rot.ToRotationVector2();
            Main.spriteBatch.Draw(mainTex, pos + dir * distanceToOwner, frameBox, c, rot, origin, Projectile.scale, 0, 0);

            rot = Projectile.rotation + mouseAngle;
            dir = rot.ToRotationVector2();
            frameBox = mainTex.Frame(1, 2, 0, 1);
            Main.spriteBatch.Draw(mainTex, pos + dir * distanceToOwner, frameBox, c, rot, Vector2.Zero, Projectile.scale, 0, 0);

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frameBox = mainTex.Frame(1, 2, 0, 0);
            Vector2 origin = frameBox.BottomLeft();
            Color c = DrawColor * alpha;

            float rot = Projectile.rotation - mouseAngle;
            Vector2 dir = rot.ToRotationVector2();
            spriteBatch.Draw(mainTex, pos + dir * distanceToOwner, frameBox, c, rot, origin, Projectile.scale, 0, 0);

            rot = Projectile.rotation + mouseAngle;
            dir = rot.ToRotationVector2();
            frameBox = mainTex.Frame(1, 2, 0, 1);
            spriteBatch.Draw(mainTex, pos + dir * distanceToOwner, frameBox, c, rot, Vector2.Zero, Projectile.scale, 0, 0);
        }
    }
}
