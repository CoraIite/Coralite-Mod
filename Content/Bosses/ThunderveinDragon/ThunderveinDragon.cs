using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    [AutoloadBossHead]
    public partial class ThunderveinDragon : ModNPC
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;

        private Player Target => Main.player[NPC.target];

        internal ref float Phase => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        internal ref float SonState => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];
        internal ref float StateRecorder => ref NPC.localAI[2];

        public readonly int trailCacheLength = 12;
        public Point[] oldFrame;
        public int[] oldDirection;

        /// <summary>
        /// 是否绘制残影
        /// </summary>
        public bool canDrawShadows;
        /// <summary>
        /// 是否绘制冲刺是的特殊贴图
        /// </summary>
        public bool isDashing;

        /// <summary>
        /// 残影的透明度
        /// </summary>
        public float shadowAlpha = 1f;
        /// <summary>
        /// 残影的大小
        /// </summary>
        public float shadowScale = 1f;

        public int oldSpriteDirection;

        /// <summary>
        /// 身上有电流环绕，会减伤并生成闪电粒子
        /// </summary>
        public bool currentSurrounding;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 100;
            NPC.damage = 30;
            NPC.defense = 6;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 6, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            //NPC.BossBar = GetInstance<BabyIceDragonBossBar>();

            //BGM：暂无
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((3820 + numPlayers * 1750) / journeyScale);
                    NPC.damage = 35;
                    NPC.defense = 12;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((4720 + numPlayers * 2100) / journeyScale);
                    NPC.damage = 60;
                    NPC.defense = 15;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 15;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 0.4f;
                }

                return;
            }

            NPC.lifeMax = 3820 + numPlayers * 1750;
            NPC.damage = 35;
            NPC.defense = 12;

            if (Main.masterMode)
            {
                NPC.lifeMax = 4720 + numPlayers * 2100;
                NPC.damage = 60;
                NPC.defense = 15;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 5320 + numPlayers * 2200;
                NPC.damage = 80;
                NPC.defense = 15;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<BabyIceDragonRelic>()));
            //npcLoot.Add(ItemDropRule.BossBag(ItemType<BabyIceDragonBossBag>()));
            //npcLoot.Add(ItemDropRule.Common(ItemType<BabyIceDragonTrophy>(), 10));

            //LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            //notExpertRule.OnSuccess(ItemDropRule.Common(ItemType<IcicleCrystal>(), 1, 3, 5));
            //npcLoot.Add(notExpertRule);
        }

        public override bool? CanCollideWithPlayerMeleeAttack(Player player, Item item, Rectangle meleeAttackHitbox)
        {
            return base.CanCollideWithPlayerMeleeAttack(player, item, meleeAttackHitbox);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (currentSurrounding)
                modifiers.SourceDamage -= 0.25f;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.Colliding(projectile.getRect(), HeadHitBox()))
                modifiers.SourceDamage += 0.15f;

            if (currentSurrounding)
                modifiers.SourceDamage -= 0.25f;

            if (projectile.hostile)
                modifiers.SourceDamage -= 0.5f;
        }

        public Rectangle HeadHitBox()
        {
            Vector2 pos = GetMousePos();
            return new Rectangle((int)(pos.X - 25), (int)(pos.Y - 25), 50, 50);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool())
            {
                Particle.NewParticle(NPC.Center.MoveTowards(projectile.Center, 50), Vector2.Zero,
                    CoraliteContent.ParticleType<LightingParticle>(), Scale: Main.rand.NextFloat(1f, 1.5f));
            }
        }

        public override void OnKill()
        {
            DownedBossSystem.DownBabyIceDragon();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.StopRain();
                Main.SyncRain();
            }
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool CheckDead()
        {
            if ((int)State != (int)AIStates.onKillAnim)
            {
                State = (int)AIStates.onKillAnim;
                Timer = 0;
                NPC.dontTakeDamage = true;
                NPC.life = 1;
                return false;
            }

            return true;
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            int width = (int)(95 * NPC.scale);
            int height = (int)(70 * NPC.scale);
            npcHitbox = new Rectangle((int)(NPC.Center.X - width / 2), (int)(NPC.Center.Y - height / 2), width, height);
            return true;
        }

        #endregion

        #region AI

        public enum AIStates
        {
            onSpawnAnmi,
            onKillAnim,
            /// <summary>
            /// 短冲，用于调整身位
            /// </summary>
            SmallDash,
            /// <summary>
            /// 闪电突袭，先3段短冲后进行一次长冲
            /// </summary>
            LightningRaid,
            /// <summary>
            /// 放电，在身体周围生成电流环绕
            /// </summary>
            Discharging,
            /// <summary>
            /// 闪电吐息，原地转一圈后使用吐息
            /// </summary>
            LightingBreath,
            /// <summary>
            /// 电球，吐出一个电球
            /// </summary>
            LightingBall,
        }

        public override void OnSpawn(IEntitySource source)
        {
            ResetAllOldCaches();
            Phase = 1;
            State = (int)AIStates.LightningRaid;
        }

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active || Target.Distance(NPC.Center) > 3000 || !Target.ZoneSnow)
            {
                NPC.TargetClosest();

                if (Target.dead || !Target.active || Target.Distance(NPC.Center) > 4500)//没有玩家存活时离开
                {
                    NPC.dontTakeDamage = false;
                    State = -1;
                    NPC.rotation = NPC.rotation.AngleTowards(0f, 0.14f);
                    NPC.velocity.X *= 0.98f;
                    NPC.EncourageDespawn(30);
                    return;
                }
            }

            switch (State)
            {
                default:
                    ResetStates();
                    break;
                case (int)AIStates.onSpawnAnmi:
                    break;
                case (int)AIStates.onKillAnim:
                    break;
                case (int)AIStates.SmallDash://闪电突袭
                    SmallDash();
                    break;
                case (int)AIStates.LightningRaid://闪电突袭
                    {
                        if (Phase == 1)
                            LightningRaidP1();
                        else
                        {

                        }
                    }
                    break;
                case (int)AIStates.Discharging://闪电突袭
                    Discharging();
                    break;
                case (int)AIStates.LightingBreath://闪电突袭
                    LightingBreath();
                    break;
                case (int)AIStates.LightingBall://闪电吐息
                    LightingBall();
                    break;
            }
        }

        public override void PostAI()
        {
            oldSpriteDirection = NPC.spriteDirection;

            if (currentSurrounding && Main.rand.NextBool(3))
            {
                Vector2 offset = Main.rand.NextVector2Circular(100 * NPC.scale, 70 * NPC.scale);
                ElectricParticle_Follow.Spawn(NPC.Center, offset, () => NPC.Center, Main.rand.NextFloat(0.75f, 1f));
            }
        }

        #endregion

        #region States

        public void ResetStates()
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            shadowAlpha = 1;
            shadowScale = 1;
            canDrawShadows = false;
            isDashing = false;
            currentSurrounding = false;

            Timer = 0;
            SonState = 0;
            Recorder = 0;
            Recorder2 = 0;

            List <int> moves = new List<int>();

            switch (Phase)
            {
                default:
                    {
                        if (NPC.life < NPC.lifeMax / 2)
                            Phase = 2;
                        else
                            Phase = 1;
                    }
                    break;
                case 1://一阶段
                    {
                        int oldState = (int)State;

                        int dir = Target.Center.X > NPC.Center.X ? 1 : -1;
                        if (dir != NPC.spriteDirection)//玩家在背后时大概率使用闪电突袭
                        {
                            for (int i = 0; i < 5; i++)
                                moves.Add((int)AIStates.LightningRaid);
                        }

                        if (Vector2.Distance(NPC.Center, Target.Center) < 480)//距离较近是大概率使用放电
                        {
                            for (int i = 0; i < 3; i++)
                                moves.Add((int)AIStates.Discharging);
                        }

                        if (oldState!=(int)AIStates.SmallDash)//如果上次招式不是小冲刺那就小冲一下
                        {
                            for (int i = 0; i < 5; i++)
                                moves.Add((int)AIStates.SmallDash);
                        }

                        moves.Add((int)AIStates.LightningRaid);
                        moves.Add((int)AIStates.LightingBreath);
                        moves.Add((int)AIStates.LightingBall);

                        //当上次使用的是短距离冲刺的话，额外移除上上次所使用的招式
                        if (oldState == (int)AIStates.SmallDash)
                            moves.RemoveAll(i => i == (int)StateRecorder);
                        //移除上次使用的招式
                            moves.RemoveAll(i => i == oldState);

                        //随机一个招式出来
                        State = Main.rand.NextFromList(moves.ToArray());
                        //State = (int)AIStates.LightingBall;

                        //如果本次使用的是短距离冲刺那么旧记录上一招
                        if ((int)State == (int)AIStates.SmallDash)
                            StateRecorder = oldState;
                    }
                    break;
                case 2:
                    {

                    }
                    break;
            }



        }

        public void ResetToSelectedState(AIStates state)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            shadowAlpha = 1;
            shadowScale = 1;
            canDrawShadows = false;
            isDashing = false;
            currentSurrounding = false;

            Timer = 0;
            SonState = 0;
            Recorder = 0;
            Recorder2 = 0;

            State = (int)state;
        }

        #endregion

        #region HelperMethods

        public void GetLengthToTargetPos(Vector2 targetPos, out float xLength, out float yLength)
        {
            xLength = NPC.Center.X - targetPos.X;
            yLength = NPC.Center.Y - targetPos.Y;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }

        /// <summary>
        /// 向上飞，会改变速度
        /// </summary>
        /// <param name="acc">加速度</param>
        /// <param name="velMax">速度最大值</param>
        /// <param name="slowDownPercent">减速率</param>
        public void FlyingUp(float acc, float velMax, float slowDownPercent)
        {
            FlyingFrame();

            if (NPC.frame.Y <= 4)
            {
                NPC.velocity.Y -= acc;
                if (NPC.velocity.Y > velMax)
                    NPC.velocity.Y = velMax;
            }
            else
                NPC.velocity.Y *= slowDownPercent;
        }

        public void FlyingFrame(bool openMouse = false)
        {
            NPC.frame.X = openMouse ? 1 : 0;

            if (++NPC.frameCounter > 4)
            {
                NPC.frameCounter = 0;
                if (++NPC.frame.Y > 7)
                    NPC.frame.Y = 0;
            }
        }

        public void DashFrame()
        {
            NPC.frame.X = 2;
            NPC.frame.Y = 0;
        }

        public Vector2 GetMousePos()
        {
            return NPC.Center + (NPC.rotation - NPC.direction * 0.1f).ToRotationVector2() * 60;
        }

        /// <summary>
        /// 根据Y方向速度设置旋转
        /// </summary>
        public void SetRotationNormally(float rate=0.08f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;
            float targetRot = NPC.velocity.Y * 0.05f * NPC.spriteDirection + (NPC.spriteDirection > 0 ? 0 : MathHelper.Pi);
            NPC.rotation = NPC.rotation.AngleLerp(targetRot, rate);
        }

        /// <summary>
        /// 将身体回正
        /// </summary>
        /// <param name="rate"></param>
        public void TurnToNoRot(float rate = 0.2f)
        {
            if (NPC.spriteDirection != oldSpriteDirection)
                NPC.rotation += 3.141f;

            NPC.rotation = NPC.rotation.AngleLerp(NPC.spriteDirection > 0 ? 0 : MathHelper.Pi, rate);
        }

        public void InitOldFrame()
        {
            oldFrame ??= new Point[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldFrame[i] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void InitOldDirection()
        {
            oldDirection ??= new int[trailCacheLength];
            for (int i = 0; i < trailCacheLength; i++)
                oldDirection[i] = NPC.spriteDirection;
        }

        public void UpdateOldFrame()
        {
            for (int i = 0; i < oldFrame.Length - 1; i++)
                oldFrame[i] = oldFrame[i + 1];
            oldFrame[^1] = new Point(NPC.frame.X, NPC.frame.Y);
        }

        public void UpdateOldDirection()
        {
            for (int i = 0; i < oldDirection.Length - 1; i++)
                oldDirection[i] = oldDirection[i + 1];
            oldDirection[^1] = NPC.spriteDirection;
        }

        public void ResetAllOldCaches()
        {
            NPC.InitOldPosCache(trailCacheLength);
            NPC.InitOldRotCache(trailCacheLength);
            InitOldFrame();
            InitOldDirection();
        }

        public void UpdateAllOldCaches()
        {
            NPC.UpdateOldPosCache();
            NPC.UpdateOldRotCache();
            UpdateOldFrame();
            UpdateOldDirection();
        }

        #endregion

        #region 绘制部分

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            var frameBox = mainTex.Frame(3, 8, NPC.frame.X, NPC.frame.Y);
            var pos = NPC.Center - screenPos;
            var origin = frameBox.Size() / 2;
            float rot = NPC.rotation;

            //因为贴图是反过来的所以这里也反一下
            SpriteEffects effects = SpriteEffects.None;

            if (NPC.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipVertically;
            }
            
            //绘制残影
            if (canDrawShadows)
            {
                Color shadowColor = new Color(255, 202, 101, 50) * shadowAlpha;

                for (int i = 0; i < trailCacheLength; i++)
                {
                    Vector2 oldPos = NPC.oldPos[i] - screenPos;
                    float oldrot = NPC.oldRot[i];
                    var frameOld = mainTex.Frame(3, 8, oldFrame[i].X, oldFrame[i].Y);
                    float factor = (float)i / trailCacheLength;

                    SpriteEffects oldEffect = oldDirection[i] > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                    Main.spriteBatch.Draw(mainTex, oldPos, frameOld, shadowColor * factor, oldrot, origin
                        , NPC.scale * shadowScale * (1 - (1 - factor) * 0.3f), oldEffect, 0);
                }
            }

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, drawColor, rot, origin, NPC.scale, effects, 0);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Glow").Value
                , pos, frameBox, Color.White*0.75f, rot, origin, NPC.scale, effects, 0);

            //绘制冲刺时的特效
            if (isDashing)
            {
                Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "StrikeTrail").Value;

                Vector2 exOrigin = new Vector2(exTex.Width * 6 / 10, exTex.Height / 2);

                Vector2 scale = new Vector2(1.3f, 1.5f) * NPC.scale;
                Main.spriteBatch.Draw(exTex, pos, null, new Color(255, 202, 101, 0), rot
                    , exOrigin, scale, effects, 0);
                scale.Y *= 1.2f;
                Main.spriteBatch.Draw(exTex, pos-NPC.rotation.ToRotationVector2()*50, null, new Color(255, 202, 101, 0) * 0.5f, rot
                    , exOrigin, scale, effects, 0);
            }

            return false;
        }

        #endregion
    }
}
