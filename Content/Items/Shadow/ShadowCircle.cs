using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowCircle : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowProjectiles + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Energy => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];
        public ref float visualAlpha => ref Projectile.localAI[1];
        public ref float visualScale => ref Projectile.localAI[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 40;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override bool? CanDamage() => false;

        #region AI
        private enum AIState : int
        {
            weaklyAttack = -1,
            idle = 0,
            melee = 1,
            shoot = 2,
            magic = 3,
            summon = 4,
            yujian = 5,
            nothing = 6,
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];

            if (Owner.active && !Owner.dead && Owner.armor[0].type == ModContent.ItemType<ShadowHead>()
                && Owner.armor[0].ModItem.IsArmorSet(Owner.armor[0], Owner.armor[1], Owner.armor[2]))
                Projectile.timeLeft = 2;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 2)
                    Projectile.frame = 0;
            }

            if (Main.myPlayer == Projectile.owner && (int)State == (int)AIState.idle && Owner.ItemAnimationJustStarted)//只有不在攻击的时候才能加能量
            {
                bool chargeNotComplete = Energy < 1000;
               //Energy += 1500; //测试时专用
                Energy += Owner.itemTimeMax*1.5f;    //使用物品时根据使用时间获得能量
                if (chargeNotComplete && Energy >= 1000)
                {
                    for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 8)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, i.ToRotationVector2() * 6f, Scale: 3);
                        dust.noGravity = true;
                    }
                    SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, Projectile.Center);
                }
                if (Energy > 1500)
                    Energy = 1500;
            }

            Vector2 targetPos = Owner.Center + new Vector2(Owner.direction * -16, -24);
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.3f);

            switch ((int)State)
            {
                default:
                case (int)AIState.idle:
                    break;
                case (int)AIState.weaklyAttack:
                    NormalUpdate(100, () =>
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 targetCenter = Main.MouseWorld;
                            NPC target = ProjectilesHelper.FindClosestEnemy(Projectile.Center, 1000, (n) =>
                            {
                                return n.CanBeChasedBy() &&
                                !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1);
                            });
                            if (target != null)
                                targetCenter = target.Center;

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * 16,
                                ModContent.ProjectileType<InvertedShadowBullet>(), Projectile.damage/2, 0, Projectile.owner);
                        }

                        SoundEngine.PlaySound(CoraliteSoundID.Gun3_Item41, Projectile.Center);
                    });
                    break;

                case (int)AIState.melee:    //近战攻击，爪击
                    NormalUpdate(150, () =>
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 targetCenter = Main.MouseWorld;
                            NPC target = ProjectilesHelper.FindClosestEnemy(Projectile.Center, 1000, (n) =>
                            {
                                return n.CanBeChasedBy() &&
                                !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1);
                            });
                            if (target != null)
                                targetCenter = target.Center;

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (targetCenter - Projectile.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 16,
                                ModContent.ProjectileType<ShadowCircle_MeleeTrack>(), (int)(Projectile.damage * 1.5f), 0, Projectile.owner);
                        }
                    });
                    break;

                case (int)AIState.shoot:    //射击高速子弹
                    NormalUpdate(100, () =>
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 targetCenter = Main.MouseWorld;
                            NPC target = ProjectilesHelper.FindClosestEnemy(Projectile.Center, 1000, (n) =>
                            {
                                return n.CanBeChasedBy() &&
                                !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1);
                            });
                            if (target != null)
                                targetCenter = target.Center;

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Main.rand.NextVector2Circular(32, 32), (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * 14,
                                ModContent.ProjectileType<ShadowCircle_HighSpeedShoot>(), Projectile.damage, 0, Projectile.owner);
                        }
                        SoundEngine.PlaySound(CoraliteSoundID.Gun_Item11, Projectile.Center);
                    });
                    break;

                case (int)AIState.magic:    //射出追踪球
                    NormalUpdate(100, () =>
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 targetCenter = Main.MouseWorld;
                            NPC target = ProjectilesHelper.FindClosestEnemy(Projectile.Center, 1000, (n) =>
                            {
                                return n.CanBeChasedBy() &&
                                !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1);
                            });
                            if (target != null)
                                targetCenter = target.Center;

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (targetCenter - Projectile.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * 14,
                                ModContent.ProjectileType<ShadowCircle_ChasingMagicBall>(), Projectile.damage, 0, Projectile.owner);
                        }
                        SoundEngine.PlaySound(CoraliteSoundID.ClingerStaff_Item100, Projectile.Center);
                    });
                    break;

                case (int)AIState.summon:   //召唤出影子火焰
                    NormalUpdate(200, () =>
                    {
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero,
                                ModContent.ProjectileType<ShadowCircle_CrystalMinion>(), (int)(Projectile.damage*0.6f), 0, Projectile.owner);

                        SoundEngine.PlaySound(CoraliteSoundID.ClingerStaff_Item100, Projectile.Center);
                    });
                    break;

                case (int)AIState.nothing:
                    NormalUpdate(100, () =>
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 targetCenter = Main.MouseWorld;
                            NPC target = ProjectilesHelper.FindClosestEnemy(Projectile.Center, 1000, (n) =>
                            {
                                return n.CanBeChasedBy() &&
                                !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1);
                            });
                            if (target != null)
                                targetCenter = target.Center;

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (targetCenter - Projectile.Center).SafeNormalize(Vector2.One) * 16,
                                ModContent.ProjectileType<InvertedShadowBullet>(), Projectile.damage, 0, Projectile.owner);
                        }

                        SoundEngine.PlaySound(CoraliteSoundID.Gun3_Item41, Projectile.Center);
                    });
                    break;
            }
        }

        public void NormalUpdate(int energyCost,Action how2Shoot)
        {
            if (Energy > energyCost)
                do
                {
                    if ((int)Timer == 0)
                    {
                        visualAlpha = 1;
                        visualScale = 1;
                        break;
                    }

                    visualScale += 0.1f;

                    if (Timer > 5)
                        visualAlpha -= 0.15f;
                    if (Timer > 10)
                    {
                        how2Shoot();
                        for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.Pi / 8)
                        {
                            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Shadowflame, i.ToRotationVector2() * 4.5f);
                            dust.noGravity = true;
                        }
                        Energy -= energyCost;
                        Projectile.netUpdate = true;
                        Timer = -1;
                    }

                } while (false);
            else
            {
                State = (int)AIState.idle;
                Timer = 0;
                Projectile.netUpdate = true;
                return;
            }

            Timer++;
        }

        public void StartAttack()
        {
            do
            {
                if (Energy < 500) //能量太低啥也干不了
                    return;
                if (Energy < 1000)//较低能量只能进行很弱的攻击（拜托，你很弱欸！）
                {
                    State = (int)AIState.weaklyAttack;
                    break;
                }

                //能量足够的时候根据玩家目前手持物品发射不同的弹幕
                int minionType = ModContent.ProjectileType<ShadowCircle_CrystalMinion>();
                foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.owner == Projectile.owner && p.type == minionType))
                    proj.Kill();

                Player Owner = Main.player[Projectile.owner];
                int type = Owner.HeldItem.DamageType.Type;
                if (type == DamageClass.Melee.Type)
                    State = (int)AIState.melee;
                else if (type == DamageClass.Ranged.Type)
                    State = (int)AIState.shoot;
                else if (type == DamageClass.Magic.Type)
                    State = (int)AIState.magic;
                else if (type == DamageClass.Summon.Type)
                    State = (int)AIState.summon;
                else
                    State = (int)AIState.nothing;

            } while (false);
            Timer = 0;
            Projectile.netUpdate = true;
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Rectangle frameBox = mainTex.Frame(1, 3, 0, Projectile.frame);
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 6);

            Main.spriteBatch.Draw(mainTex, center, frameBox, lightColor, 0,origin , 1.3f, SpriteEffects.None, 0);
            if ((int)State != (int)AIState.idle)
                Main.spriteBatch.Draw(mainTex, center, frameBox, Color.White * visualAlpha, 0, origin, visualScale*1.3f, SpriteEffects.None, 0);
            return false;
        }
    }

    public class ShadowCircle_MeleeTrack : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float npcIndex => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 1000;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            #region 同叶绿弹的追踪，但是范围更大
            float velLength = Projectile.velocity.Length();
            float localAI0 = Projectile.localAI[0];
            if (localAI0 == 0f)
            {
                Projectile.localAI[0] = velLength;
                localAI0 = velLength;
            }

            float num186 = Projectile.position.X;
            float num187 = Projectile.position.Y;
            float chasingLength = 900f;
            bool flag5 = false;
            int targetIndex = 0;
            if (npcIndex == 0)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile))
                    {
                        float targetX = Main.npc[i].Center.X;
                        float targetY = Main.npc[i].Center.Y;
                        float num193 = Math.Abs(Projectile.Center.X - targetX) + Math.Abs(Projectile.Center.Y - targetY);
                        if (num193 < chasingLength)
                        {
                            chasingLength = num193;
                            num186 = targetX;
                            num187 = targetY;
                            flag5 = true;
                            targetIndex = i;
                        }
                    }
                }

                if (flag5)
                    npcIndex = targetIndex + 1;

                flag5 = false;
            }

            if (npcIndex > 0f)
            {
                int targetIndex2 = (int)npcIndex - 1;
                if (Main.npc[targetIndex2].active && Main.npc[targetIndex2].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[targetIndex2].dontTakeDamage)
                {
                    float num195 = Main.npc[targetIndex2].Center.X;
                    float num196 = Main.npc[targetIndex2].Center.Y;
                    if (Math.Abs(Projectile.Center.X - num195) + Math.Abs(Projectile.Center.Y - num196) < 1000f)
                    {
                        flag5 = true;
                        num186 = Main.npc[targetIndex2].Center.X;
                        num187 = Main.npc[targetIndex2].Center.Y;
                    }
                    if (Vector2.Distance(Projectile.position, Main.npc[targetIndex2].Center) < 48)
                    {
                        //生成斩击
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Vector2 rot = Helper.NextVec2Dir();
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.npc[targetIndex2].Center + rot * 128, Main.npc[targetIndex2].Center - rot * 128,
                                ModContent.ProjectileType<ShadowCircle_MeleeClaw>(), Projectile.damage, 0, Projectile.owner);
                        }
                        SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, Projectile.Center);
                        Projectile.Kill();
                    }
                }
                else
                    npcIndex = 0;

                Projectile.netUpdate = true;
            }

            if (flag5)
            {
                float num197 = localAI0;
                Vector2 center = Projectile.Center;
                float num198 = num186 - center.X;
                float num199 = num187 - center.Y;
                float dis2Target = MathF.Sqrt(num198 * num198 + num199 * num199);
                dis2Target = num197 / dis2Target;
                num198 *= dis2Target;
                num199 *= dis2Target;
                int chase = 16;

                Projectile.velocity.X = (Projectile.velocity.X * (chase - 1) + num198) / chase;
                Projectile.velocity.Y = (Projectile.velocity.Y * (chase - 1) + num199) / chase;
            }

            #endregion

            Dust dust = Dust.NewDustDirect(Projectile.position, 16, 16, DustID.Shadowflame);
            dust.noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    /// <summary>
    /// 使用位置选定起始点，velocity选定结束点
    /// </summary>
    public class ShadowCircle_MeleeClaw : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "ShadowClaw";

        public ref float PosFactor => ref Projectile.ai[0];
        public ref float ScaleFactor => ref Projectile.ai[1];

        public ref float MaxScale => ref Projectile.localAI[0];
        public ref float YScale => ref Projectile.localAI[1];

        private bool init = true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 14;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft < 6)
                return false;
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.velocity, 48, ref a);
        }

        public override void AI()
        {
            if (init)
            {
                PosFactor = 0;
                ScaleFactor = 0;
                MaxScale = Vector2.Distance(Projectile.Center, Projectile.velocity) / 226f;//226是图片的宽度
                YScale = Main.rand.NextFloat(0.3f, 0.4f);
                Projectile.rotation = (Projectile.velocity - Projectile.Center).ToRotation();
                init = false;
            }

            if (Projectile.timeLeft > 9)
            {
                PosFactor += 0.1f;
                ScaleFactor += 0.2f;
            }
            else if (Projectile.timeLeft < 6)
            {
                PosFactor += 0.1f;
                ScaleFactor -= 0.2f;
            }

        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            Vector2 center = Vector2.Lerp(Projectile.Center, Projectile.velocity, PosFactor);
            Vector2 scale = new Vector2(ScaleFactor * MaxScale, YScale);
            Vector2 origin = mainTex.Size() / 2;

            spriteBatch.Draw(mainTex, center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
        }
    }

    public class ShadowCircle_HighSpeedShoot : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + "HyacinthBullet2";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = -1;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 7;
            Projectile.scale = 1.18f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.alpha > 160)
                return Color.Transparent;

            Color color = Color.MediumPurple;
            color.A = 100;
            return color;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }

        public override void AI()
        {
            float velLength = Projectile.velocity.Length();
            if (Projectile.alpha > 0)
                Projectile.alpha -= (byte)(velLength * 0.5f);

            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.85f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            return true;
        }
    }

    public class ShadowCircle_ChasingMagicBall : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float npcIndex => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 1000;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            #region 同叶绿弹的追踪，但是范围更大
            float velLength = Projectile.velocity.Length();
            float localAI0 = Projectile.localAI[0];
            if (localAI0 == 0f)
            {
                Projectile.localAI[0] = velLength;
                localAI0 = velLength;
            }

            float num186 = Projectile.position.X;
            float num187 = Projectile.position.Y;
            float chasingLength = 900f;
            bool flag5 = false;
            int targetIndex = 0;
            if (npcIndex == 0)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile))
                    {
                        float targetX = Main.npc[i].Center.X;
                        float targetY = Main.npc[i].Center.Y;
                        float num193 = Math.Abs(Projectile.Center.X - targetX) + Math.Abs(Projectile.Center.Y - targetY);
                        if (num193 < chasingLength)
                        {
                            chasingLength = num193;
                            num186 = targetX;
                            num187 = targetY;
                            flag5 = true;
                            targetIndex = i;
                        }
                    }
                }

                if (flag5)
                    npcIndex = targetIndex + 1;

                flag5 = false;
            }

            if (npcIndex > 0f)
            {
                int targetIndex2 = (int)npcIndex - 1;
                if (Main.npc[targetIndex2].active && Main.npc[targetIndex2].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[targetIndex2].dontTakeDamage)
                {
                    float num195 = Main.npc[targetIndex2].Center.X;
                    float num196 = Main.npc[targetIndex2].Center.Y;
                    if (Math.Abs(Projectile.Center.X - num195) + Math.Abs(Projectile.Center.Y - num196) < 1000f)
                    {
                        flag5 = true;
                        num186 = Main.npc[targetIndex2].Center.X;
                        num187 = Main.npc[targetIndex2].Center.Y;
                    }
                }
                else
                    npcIndex = 0;

                Projectile.netUpdate = true;
            }

            if (flag5)
            {
                float num197 = localAI0;
                Vector2 center = Projectile.Center;
                float num198 = num186 - center.X;
                float num199 = num187 - center.Y;
                float dis2Target = MathF.Sqrt(num198 * num198 + num199 * num199);
                dis2Target = num197 / dis2Target;
                num198 *= dis2Target;
                num199 *= dis2Target;
                int chase = 16;

                Projectile.velocity.X = (Projectile.velocity.X * (chase - 1) + num198) / chase;
                Projectile.velocity.Y = (Projectile.velocity.Y * (chase - 1) + num199) / chase;
            }

            #endregion
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, 16, 16, DustID.Shadowflame, Scale: Main.rand.NextFloat(1.3f, 1.4f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class ShadowCircle_CrystalMinion : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowItems + "ShadowEnergy";

        public ref float DieTimer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 3000;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 6)
                    Projectile.frame = 0;
            }

            int halfTime = 40;
            int startTime = halfTime * 2;
            int startTime_less1 = startTime - 1;
            int halfTime_Plus1 = halfTime + 1;

            #region 距离玩家太远直接进入尝试开始攻击的状态（该状态下会急速回到idle点位上）
            if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
            {
                Projectile.ai[0] = 0f;
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            #endregion
            #region 回到玩家身边，回到玩家身边后将ai0设为0，进入尝试攻击阶段
            if (Projectile.ai[0] == -1f)
            {
                ProjectilesHelper.GetMyProjIndexWithModProj<ShadowCircle_CrystalMinion>(Projectile, out var index, out var totalIndexesInGroup);
                GetIdlePosition(index, totalIndexesInGroup, player,out var idleSpot, out var idleRotation);
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 32f);
                Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.2f);
                if (Projectile.Distance(idleSpot) < 2f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }

                return;
            }
            #endregion
            #region  尝试开始攻击
            if (Projectile.ai[0] == 0f)
            {
                DieTimer+=1;
                if (DieTimer>600)
                {
                    Projectile.Kill();
                    return;
                }
                ProjectilesHelper.GetMyProjIndexWithModProj<ShadowCircle_CrystalMinion>(Projectile, out var index, out var totalIndexesInGroup);
                GetIdlePosition(index, totalIndexesInGroup, player, out var idleSpot, out var idleRotation);
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Vector2.SmoothStep(Projectile.Center, idleSpot, 0.45f);
                Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.45f);
                if (Main.rand.NextBool(20))
                {
                    int targetNPCIndex = AI_156_TryAttackingNPCs(Projectile);
                    if (targetNPCIndex != -1)
                    {
                        AI_156_StartAttack(Projectile);
                        Projectile.ai[0] = startTime;
                        Projectile.ai[1] = targetNPCIndex;
                        Projectile.netUpdate = true;
                    }
                }

                return;
            }
            #endregion

            bool skipBodyCheck = true;
            int targetNPCIndex_ = (int)Projectile.ai[1];

            #region 各种防止NPC不存在或是不可攻击所做的...预防？
            //如果目标NPC不存在重新寻找一次，如果仍然未找到的话就进入回到玩家身边的阶段（ai0设为-1）
            if (!Main.npc.IndexInRange(targetNPCIndex_))
            {
                int _targetNPCIndex = AI_156_TryAttackingNPCs(Projectile, skipBodyCheck);
                if (_targetNPCIndex != -1)
                {
                    Projectile.ai[0] = Main.rand.NextFromList(halfTime, startTime);
                    Projectile.ai[1] = _targetNPCIndex;
                    AI_156_StartAttack(Projectile);
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }

                return;
            }

            NPC target = Main.npc[targetNPCIndex_];

            //如果目标NPC不能被攻击到，那么重新寻找，具体内容和上面的一模一样 就这么喜欢复制黏贴是吧？
            if (!target.CanBeChasedBy(this))
            {
                int _targetNPCIndex = AI_156_TryAttackingNPCs(Projectile, skipBodyCheck);
                if (_targetNPCIndex != -1)
                {
                    Projectile.ai[0] = Main.rand.NextFromList(halfTime, startTime);
                    AI_156_StartAttack(Projectile);
                    Projectile.ai[1] = _targetNPCIndex;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }

                return;
            }
            #endregion

            Projectile.ai[0] -= 1f;//计时器递减
            if (Projectile.ai[0] >= startTime_less1)//这部分仅有在ai0等于startTime以及HalfTime，还有他们的下一帧的时候才会执行
            {
                Projectile.direction = (Projectile.Center.X < target.Center.X) ? 1 : (-1);
                if (Projectile.ai[0] == startTime_less1)      //这里是上面写的“他们的下一帧”的时候才会执行的
                {
                    Projectile.localAI[0] = Projectile.Center.X;
                    Projectile.localAI[1] = Projectile.Center.Y;
                }
            }

            float lerpValue2 = Utils.GetLerpValue(startTime_less1, halfTime_Plus1, Projectile.ai[0], clamped: true);
            #region 泰拉棱镜的冲刺
            //在一般时间之前先缓慢地靠近
            //之后快速突刺到目标身后
            Vector2 originCenter = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            originCenter += new Vector2(0f, Utils.GetLerpValue(0f, 0.4f, lerpValue2, clamped: true) * -100f);
            Vector2 v = target.Center - originCenter;
            Vector2 vector6 = v.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(v.Length(), 60f, 150f);
            Vector2 value = target.Center + vector6;
            float lerpValue3 = Utils.GetLerpValue(0.4f, 0.6f, lerpValue2, clamped: true);
            float lerpValue4 = Utils.GetLerpValue(0.6f, 1f, lerpValue2, clamped: true);
            float targetAngle = v.SafeNormalize(Vector2.Zero).ToRotation() - (float)Math.PI / 2f;   //这里调整了角度
            Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle, (float)Math.PI / 5f);
            Projectile.Center = Vector2.Lerp(originCenter, target.Center, lerpValue3);
            if (lerpValue4 > 0f)
                Projectile.Center = Vector2.Lerp(target.Center, value, lerpValue4);

            #endregion
            #region 上一个状态结束时的重新寻找NPC，以及随机状态
            if (Projectile.ai[0] == halfTime_Plus1)
            {
                int targetNPCIndex = AI_156_TryAttackingNPCs(Projectile, skipBodyCheck);
                if (targetNPCIndex != -1)
                {
                    //随机一个攻击状态，冲刺或是挥砍
                    Projectile.ai[0] = startTime;
                    Projectile.ai[1] = targetNPCIndex;
                    AI_156_StartAttack(Projectile);
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            #endregion
        }

        public void GetIdlePosition(int stackedIndex, int totalIndexes,Player Owner, out Vector2 idleSpot, out float idleRotation)
        {
            idleRotation = 0;
            float num2 = (totalIndexes - 1f) / 2f;
            idleSpot = Owner.Center - Vector2.UnitY.RotatedBy(4.3982296f / totalIndexes * (stackedIndex - num2)) * 33f +new Vector2(16,0);
            idleSpot += Main.GlobalTimeWrappedHourly.ToRotationVector2() * 8;
        }

        /// <summary>
        /// 获取目标NPC的索引
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="blackListedTargets"></param>
        /// <param name="skipBodyCheck"></param>
        /// <returns></returns>
        public int AI_156_TryAttackingNPCs(Projectile Projectile, bool skipBodyCheck = false)
        {
            Vector2 ownerCenter = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;
            //如果有锁定的NPC那么就用锁定的，没有或不符合条件在从所有NPC里寻找
            NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this))
            {
                bool flag = true;
                if (!ownerMinionAttackTargetNPC.boss)
                    flag = false;

                if (ownerMinionAttackTargetNPC.Distance(ownerCenter) > 1000f)
                    flag = false;

                if (!skipBodyCheck && !Projectile.CanHitWithOwnBody(ownerMinionAttackTargetNPC))
                    flag = false;

                if (flag)
                    return ownerMinionAttackTargetNPC.whoAmI;
            }

            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(Projectile) || nPC.boss)
                {
                    float npcDistance2Owner = nPC.Distance(ownerCenter);
                    if (npcDistance2Owner <= 1000f && (npcDistance2Owner <= num || num == -1f) && (skipBodyCheck || Projectile.CanHitWithOwnBody(nPC)))
                    {
                        num = npcDistance2Owner;
                        result = i;
                    }
                }
            }

            return result;
        }

        public void AI_156_StartAttack(Projectile Projectile)
        {
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Dig, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Rectangle frameBox = mainTex.Frame(1, 7, 0, Projectile.frame);
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 14);

            if (Projectile.ai[0]>0)
                for (int i = 1; i < 6; i ++)
                {
                    Color color = Color.Purple * (0.34f - i * 0.02f);
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, frameBox, color, Projectile.oldRot[i], origin, 1.2f, SpriteEffects.None, 0);
                }

            Main.spriteBatch.Draw(mainTex, Projectile.position - Main.screenPosition, frameBox, lightColor, Projectile.rotation, origin, 1.4f, SpriteEffects.None, 0);
            return false;
        }
    }
}
