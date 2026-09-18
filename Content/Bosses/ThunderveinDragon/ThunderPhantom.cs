using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderPhantom : ModNPC
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;

        public ref float OwnerIndex => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float Timer => ref NPC.ai[2];
        public ref float PhantomDistance => ref NPC.ai[3];

        Player Target => Main.player[NPC.target];

        /// <summary>
        /// 就位时横向让开的基准距离。沿用旧值 ThunderPhantom.cs:129（<c>Main.rand.NextFromList(-1, 1) * 200</c>）。
        /// </summary>
        private const int RepositionSideOffset = 200;

        /// <summary>
        /// 叠在基准距离上的抖动半幅，取值落在 [−250, 249]。沿用旧值 ThunderPhantom.cs:129（<c>Main.rand.Next(-250, 250)</c>）。
        /// </summary>
        private const int RepositionJitter = 250;

        /// <summary>
        /// 联机接线：本体 <see cref="ThunderveinDragon"/> 已退出原版平滑，幻影作为部件同策略（C6 / C10）。<br/>
        /// 这里只用到决策点、心跳与清平滑，<b>不装纠偏器</b>——见 <see cref="AI"/> 开头的说明。
        /// </summary>
        private readonly CoraliteMinionNetSync net = new CoraliteMinionNetSync();

        public override void SetStaticDefaults()
        {
            NPC.SetHideInBestiary();
            NPCID.Sets.CannotDropSouls[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 100;
            NPC.damage = 60;
            NPC.scale = 1.1f;
            NPC.defense = 35;
            NPC.lifeMax = 2200;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((2200 + (numPlayers * 700)) / journeyScale);
                    NPC.damage = 66;
                    NPC.defense = 35;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((2200 + (numPlayers * 1400)) / journeyScale);
                    NPC.damage = 72;
                    NPC.defense = 35;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 35;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 0.4f;
                }

                return;
            }

            NPC.lifeMax = 2200 + (numPlayers * 700);
            NPC.damage = 66;
            NPC.defense = 35;

            if (Main.masterMode)
            {
                NPC.lifeMax = 2200 + (numPlayers * 1400);
                NPC.damage = 72;
                NPC.defense = 35;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 2500 + (numPlayers * 1600);
                NPC.damage = 80;
                NPC.defense = 35;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ElectrificationWing>(), 1, 1, 2));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, NPC.Kill))
                return;

            // C6：与本体同平滑层。只清不纠偏——幻影的位置在阶段 0 每帧被硬写成"玩家头顶"，
            // 阶段 1 / 2 里完全静止，纠偏量下一帧就会被覆盖或根本不产生，装纠偏器是空转。
            CoraliteMinionNetSync.ClearVanillaSmoothing(NPC);

            ThunderveinDragon dragon = owner.ModNPC as ThunderveinDragon;

            switch (State)
            {
                default:
                case 0://先跟踪一段时间，然后让天空闪烁一下
                    {
                        NPC.target = owner.target;
                        NPC.Center = Target.Center + new Vector2(0, -100);
                        Timer++;
                        if (Timer > 60)
                        {
                            State++;
                            Timer = 0;

                            // 旧代码两端各掷一次 Main.rand，客户端的幻影会横向偏到另一处，
                            // 而落雷是服务端按服务端的幻影位置生成的——预警贴图与真正的雷位对不上（C1 / 9.1）。
                            // 改成由 whoAmI + 轮数派生的确定性随机：分布形状与旧值完全一致，只是两端必然同解。
                            // 拍号取本体的幻影轮数（ai[2]，已同步且每轮递增），所以四轮各偏各的，不会变成每轮同一个落点。
                            Random rand = CoraliteMinionNetSync.CreateBeatRandom(NPC.whoAmI, (int)dragon.SonState);
                            float offsetX = ((rand.Next(2) == 0 ? -1 : 1) * RepositionSideOffset)
                                + rand.Next(-RepositionJitter, RepositionJitter);
                            NPC.Center += new Vector2(offsetX, 0);

                            ThunderveinDragon.SetBackgroundLight(0.9f, 50, 18);
                            SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);
                            NPC.dontTakeDamage = false;
                            net.MarkDecision(NPC);//决策点：换拍 + 瞬移
                        }
                    }
                    break;
                case 1://让天空闪烁一下，同时让分身就位
                    {
                        Timer++;
                        float factor = Helper.SqrtEase(Timer / 75);
                        PhantomDistance = factor * 220;
                        if (Timer > 75)
                        {
                            State++;
                            Timer = 0;
                            ThunderveinDragon.SetBackgroundLight(0.6f, 12 * 3, 10);

                            SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);
                            net.MarkDecision(NPC);//决策点：换拍
                        }
                    }
                    break;
                case 2://释放雷暴
                    {
                        if (Timer == 0)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            Vector2 pos = NPC.Center;
                            int damage = Helper.GetProjDamage(60, 70, 80);

                            if (!VaultUtils.isClient)
                                NPC.NewProjectileDirectInAI<StrongThunderFalling>(
                                pos + new Vector2(0, -Main.rand.Next(170, 320)), pos + new Vector2(0, 750), damage, 0, NPC.target
                                , 20, NPC.whoAmI, 70);
                        }
                        else if (Timer % 12 == 0)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            int damage = Helper.GetProjDamage(20, 40, 60);
                            if (!VaultUtils.isClient)
                            {
                                for (int i = -1; i < 2; i += 2)
                                {
                                    Vector2 pos = NPC.Center + new Vector2(i * (Timer / 12) * PhantomDistance, 0);
                                    NPC.NewProjectileDirectInAI<StrongThunderFalling>(
                                        pos + new Vector2(0, -Main.rand.Next(170, 320)), pos + new Vector2(0, 750), damage, 0, NPC.target
                                        , 7, NPC.whoAmI, 70);
                                }
                            }
                        }

                        Timer++;
                        if (Timer > 12 * 4)
                        {
                            PhantomDistance = 0;
                            State = 0;
                            NPC.dontTakeDamage = true;

                            // 轮数推进与自毁都是权威端裁决（C1）：客户端跟着加会污染本体 ai[2]（下一包又被改回去），
                            // 客户端自己 Kill 则是本地假死，要等服务端的下一个包才能被重新激活。
                            if (!VaultUtils.isClient)
                            {
                                dragon.SonState++;
                                owner.netUpdate = true;//轮数是幻影下一轮掷骰的拍号，必须先于那次掷骰到达客户端
                                net.MarkDecision(NPC);//决策点：换拍

                                if (dragon.SonState > 5)
                                    NPC.Kill();
                            }
                        }
                    }
                    break;
            }

            net.Heartbeat(NPC);//慢频兜底，丢包与中途加入靠它自愈
        }

        public override bool PreKill()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
    }
}
