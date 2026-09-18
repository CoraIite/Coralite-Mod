using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera.Core;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 本体上不属于任何单个状态的公共件：扭曲圈与转阶段标题卡的表现层、一阶段的钩爪跟随运动、
    /// 以及两次转阶段的"一次性副作用"钩子（状态只负责演出，这些是跨状态的复位）。
    /// </summary>
    [AutoloadBossHead]
    public sealed partial class NightmarePlantera : IDrawWarp
    {
        #region 表现层：扭曲圈与标题卡

        public static Asset<Texture2D> CircleWarpTex;
        public static Asset<Texture2D> BlackBack;
        public static Asset<Texture2D> NameLine;

        /// <summary>标题卡的自有计时，与 AI 计时完全无关（C8：表现不读 gameplay 时钟）。</summary>
        public static int nameDrawTimer;

        public bool canDrawWarp;
        public float warpScale;
        public float nameScale;
        public float nameAlpha;

        /// <summary>标题卡是否已挂上 <c>Main.OnPostDraw</c>，防止重复订阅。</summary>
        private bool nameCardHooked;

        public void DrawWarp()
        {
            if (!canDrawWarp)
            {
                return;
            }

            Texture2D mainTex = CircleWarpTex.Value;
            Main.spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, null, Color.White, 0, mainTex.Size() / 2, warpScale, 0, 0);
        }

        /// <summary>
        /// 挂上转阶段标题卡。字号与透明度两端同跑（演出的结束条件读 <see cref="nameAlpha"/>，
        /// 服务器上不推进就会卡在演出里），只有真正的绘制钩子是本地的。
        /// </summary>
        internal void StartNameCard()
        {
            nameScale = 0;
            nameAlpha = 1;
            nameDrawTimer = 0;

            if (!Main.dedServ && !nameCardHooked)
            {
                Main.OnPostDraw += DrawName;
                nameCardHooked = true;
            }
        }

        /// <summary>摘掉标题卡。</summary>
        internal void StopNameCard()
        {
            nameScale = 0;
            if (nameCardHooked)
            {
                Main.OnPostDraw -= DrawName;
                nameCardHooked = false;
            }
        }

        private void DrawName(GameTime obj)
        {
            nameDrawTimer++;
            if (nameDrawTimer > 300)
            {
                // 兜底：AI 那边万一没来得及摘钩子（死亡 / 脱战），标题卡自己退场。
                StopNameCard();
                nameAlpha = 0;
            }

            Vector2 basePos = new(Main.screenWidth / 2, Main.screenHeight / 2);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            Main.spriteBatch.Draw(BlackBack.Value, basePos, null, Color.White * nameAlpha, 0, BlackBack.Size() / 2, nameScale, 0, 0);

            // 旧代码这里读的是 ai[3]（当时的状态计时器）；ai[3] 已还给基座，改读标题卡自己的计时，节奏不变。
            if (nameDrawTimer > 8)
            {
                Utils.DrawBorderStringBig(Main.spriteBatch, DisplayName.Value, basePos, Color.Red * nameAlpha, 1.6f, 0.5f, 0.5f);

                Main.spriteBatch.Draw(NameLine.Value, basePos + new Vector2(0, 80), null, Color.Red * nameAlpha, 0f, NameLine.Size() / 2, 1.3f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(NameLine.Value, basePos - new Vector2(0, 120), null, Color.Red * nameAlpha, 0f, NameLine.Size() / 2, 1.3f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
        }

        /// <summary>三阶段的帧动画（与一阶段的 <c>UpdateFrameNormally</c> 同参数，只是分开命名以对齐旧代码）。</summary>
        public void UpdateFrame_P3()
        {
            if (++NPC.frameCounter > NightmarePlanteraDirector.FrameInterval)
            {
                NPC.frameCounter = 0;
                NPC.frame.X++;
                if (NPC.frame.X >= NightmarePlanteraDirector.FrameCount)
                {
                    NPC.frame.X = 0;
                }
            }
        }

        #endregion

        #region 一阶段：钩爪跟随

        /// <summary>补齐三只钩爪。旧代码用 <c>spawnedHook</c> 做一次性生成、又在移动里补漏，这里合成一处。</summary>
        internal void EnsureHooks()
        {
            if (VaultUtils.isClient)
            {
                return;
            }

            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCType<NightmareHook>())
                {
                    count++;
                }
            }

            for (; count < NightmarePlanteraDirector.HookCount; count++)
            {
                NPC.NewNpcInAI_Server<NightmareHook>(NPC.Center, NPC.whoAmI);
            }
        }

        /// <summary>
        /// 一阶段运动：目标点由三只钩爪的重心 + 朝玩家的有限延伸决定，逐轴逼近。<br/>
        /// 原样搬自旧 <c>Phase1_Movement</c>（Phase.P1_Sleeping.cs:44-151），只把数字换成 Director 常量。
        /// 两端同跑：读到的量只有钩爪位置（原版同步）与玩家位置（原版同步），客户端能自己算出同样结果（C1 / C3）。
        /// </summary>
        public void Phase1_Movement()
        {
            float targetX = 0f;
            float targetY = 0f;
            int hookCount = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCType<NightmareHook>())
                {
                    targetX += Main.npc[i].Center.X;
                    targetY += Main.npc[i].Center.Y;
                    hookCount++;
                    if (hookCount >= NightmarePlanteraDirector.HookCount)
                    {
                        break;
                    }
                }
            }

            if (hookCount < 1)
            {
                // 钩爪还没生成出来（出生第一帧 / 客户端尚未收到生成包）：这一帧不动，避免除零。
                return;
            }

            targetX /= hookCount;
            targetY /= hookCount;

            float speedMax = NightmarePlanteraDirector.P1SpeedBase;
            float accel = NightmarePlanteraDirector.P1AccelBase;
            if (NPC.life < NPC.lifeMax * NightmarePlanteraDirector.P1Tier1LifeRatio)
            {
                speedMax = NightmarePlanteraDirector.P1SpeedTier1;
                accel = NightmarePlanteraDirector.P1AccelTier1;
            }

            if (NPC.life < NPC.lifeMax * NightmarePlanteraDirector.P1Tier2LifeRatio)
            {
                speedMax = NightmarePlanteraDirector.P1SpeedTier2;
            }

            if (Main.expertMode)
            {
                speedMax += NightmarePlanteraDirector.P1ExpertSpeedAdd;
                speedMax *= NightmarePlanteraDirector.P1ExpertSpeedMul;
                accel += NightmarePlanteraDirector.P1ExpertAccelAdd;
                accel *= NightmarePlanteraDirector.P1ExpertAccelMul;
            }

            if (Main.getGoodWorld)
            {
                speedMax *= NightmarePlanteraDirector.P1GoodSpeedMul;
                accel *= NightmarePlanteraDirector.P1GoodAccelMul;
            }

            // 钩爪重心朝玩家延伸，但不超过最大延伸距离
            int maxReach = NightmarePlanteraDirector.P1MaxReach;
            if (Main.expertMode)
            {
                maxReach += NightmarePlanteraDirector.P1ExpertReachAdd;
            }

            float toTargetX = Target.Center.X - targetX;
            float toTargetY = Target.Center.Y - targetY;
            float reach = (float)Math.Sqrt((toTargetX * toTargetX) + (toTargetY * toTargetY));
            if (reach >= maxReach)
            {
                reach = maxReach / reach;
                toTargetX *= reach;
                toTargetY *= reach;
            }

            targetX += toTargetX;
            targetY += toTargetY;

            float deltaX = targetX - NPC.Center.X;
            float deltaY = targetY - NPC.Center.Y;
            float length = (float)Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            if (length < speedMax)
            {
                deltaX = NPC.velocity.X;
                deltaY = NPC.velocity.Y;
            }
            else
            {
                length = speedMax / length;
                deltaX *= length;
                deltaY *= length;
            }

            if (NPC.velocity.X < deltaX)
            {
                NPC.velocity.X += accel;
                if (NPC.velocity.X < 0f && deltaX > 0f)
                {
                    NPC.velocity.X += accel * 2f;
                }
            }
            else if (NPC.velocity.X > deltaX)
            {
                NPC.velocity.X -= accel;
                if (NPC.velocity.X > 0f && deltaX < 0f)
                {
                    NPC.velocity.X -= accel * 2f;
                }
            }

            if (NPC.velocity.Y < deltaY)
            {
                NPC.velocity.Y += accel;
                if (NPC.velocity.Y < 0f && deltaY > 0f)
                {
                    NPC.velocity.Y += accel * 2f;
                }
            }
            else if (NPC.velocity.Y > deltaY)
            {
                NPC.velocity.Y -= accel;
                if (NPC.velocity.Y > 0f && deltaY < 0f)
                {
                    NPC.velocity.Y -= accel * 2f;
                }
            }
        }

        #endregion

        #region 转阶段的一次性副作用

        /// <summary>
        /// 一阶段 → 转阶段的前置复位：收钩爪、掐音乐、上无敌。两端都要跑（音乐与无敌不过线）。<br/>
        /// 沿用旧 <c>SetPhase1Exchange</c> 的前半段（Phase.Exchange_P1_P2.cs:209-219）。
        /// </summary>
        internal void PreparePhase1Exchange()
        {
            foreach (NPC hook in Main.npc.Where(n => n.active && n.type == NPCType<NightmareHook>()))
            {
                hook.ai[3] = 1;
            }

            Music = 0;
            NPC.dontTakeDamage = true;
        }

        /// <summary>
        /// 沉眠之雾命中玩家时的外部入口（<c>Projectile.HypnotizeFog.cs</c> 在调）：提前进入转阶段。<br/>
        /// 换态本身只登记请求，由状态基类在 <c>ServerUpdate</c> 里出门（D5）。
        /// </summary>
        public void SetPhase1Exchange()
        {
            PreparePhase1Exchange();
            AiContext?.RequestState(NightmarePlanteraStateId.exchange_P1_P2);
        }

        /// <summary>转阶段演出结束 → 二阶段：解除无敌、复位轮换计数、收镜头。沿用旧 <c>OnExchangeToP2</c>。</summary>
        public void OnExchangeToP2()
        {
            MoveCount = 0;
            NPC.dontTakeDamage = false;

            if (!Main.dedServ && Main.LocalPlayer.TryGetModPlayer(out NightmarePlayerCamera camera))
            {
                camera.useShake = false;
                camera.useScreenMove = false;
            }

            if (VaultUtils.isClient)
            {
                return;
            }

            SetPhase2States();
        }

        /// <summary>
        /// 二阶段 → 三阶段：加防、清掉场上的美梦光、复位梦境计数，并登记转阶段演出。<br/>
        /// 沿用旧 <c>OnExchangeToP3</c>（Phase3_Nightemare.cs:1537-1568）。PhaseController 的 OnFire 与
        /// 二阶段选招时的"血量提前转阶段"都走这一个口。
        /// </summary>
        public void OnExchangeToP3()
        {
            MoveCount = 0;
            NPC.dontTakeDamage = false;
            NPC.defense = NPC.defDefense + NightmarePlanteraDirector.P3ExtraDefense;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc != null && npc.active && npc.type == NPCType<FantasySparkle>())
                {
                    npc.Kill();
                }
            }

            if (VaultUtils.isClient)
            {
                return;
            }

            warpScale = 0;
            NPC.dontTakeDamage = true;
            useMeleeDamage = false;
            useDreamMove = false;
            DreamMoveCount = 0;
            fantasyKillCount = 0;
            alpha = 1;

            AiContext?.RequestState(NightmarePlanteraStateId.exchange_P2_P3);
        }

        #endregion
    }
}
