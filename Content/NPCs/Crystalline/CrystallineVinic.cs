using Coralite.Content.Biomes;
using Coralite.Content.Dusts;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.SmoothFunctions;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.IO;

namespace Coralite.Content.NPCs.Crystalline
{
    [AutoLoadTexture(Path = AssetDirectory.CrystallineNPCs)]
    public class CrystallineVinic : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [AutoLoadTexture(Name = "CrystallineVinic_Vine")]
        public static ATex VineTex { get; private set; }

        public Point SpawnPoint
        {
            get => new Point((int)NPC.ai[0], (int)NPC.ai[1]);
            set
            {
                NPC.ai[0] = value.X;
                NPC.ai[1] = value.Y;
            }
        }

        private VinicTypes VinicType
        {
            get=>(VinicTypes)NPC.ai[2];
            set => NPC.ai[2] = (int)value;
        }
        private AIStates State
        {
            get=>(AIStates)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        /// <summary> 能否对玩家造成伤害 </summary>
        public bool CanHit { get; set; } = false;

        private ref float Timer => ref NPC.localAI[0];
        private ref float MouseRotation => ref NPC.localAI[1];

        private SecondOrderDynamics_Vec2 Smoother;

        private enum VinicTypes
        {
            /// <summary> 水晶柱版本，会咬人 </summary>
            Column,
            /// <summary> 激光器版本，会射激光 </summary>
            LASER
        }

        private enum AIStates
        {
            Waiting,
            Attack_Bite,
            Attack_Laser,
            Attack_Rest,
            BackToWait,
        }

        private Player Target => Main.player[NPC.target];

        #region 基础值设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 54;
            NPC.damage = 75;
            NPC.defense = 20;

            NPC.lifeMax = 500;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.5f;
            NPC.HitSound = CoraliteSoundID.Dig;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 25, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Tile spawnT = Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY);
            if (Main.hardMode && spawnInfo.Player.InModBiome<CrystallineSkyIsland>()
                && spawnT.HasTile && Main.tileSolid[spawnT.TileType])//只能生成在实心物块上
                return 0.02f;

            return 0;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 2, 5));

            //概率掉落蕴魔海燕麦母体
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineSeaOatsSeed>(), 12));

            //掉落各种植物
            IItemDropRule[] plants = [
                ItemDropRule.Common(ModContent.ItemType<CrystallineSeaOats>(), 1, 1, 3),
                ItemDropRule.Common(ModContent.ItemType<ChalcedonySapling>(), 1, 1, 2),
                ItemDropRule.Common(ModContent.ItemType<CrystallineLemna>(), 1, 1, 3),
            ];

            npcLoot.Add(new FewFromRulesRule(1, 5, plants));

            //掉落宝石原石
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 3, 1, 3));
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (State == AIStates.Waiting)//伪装状态不绘制血条
                return false;

            return null;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            if (State == AIStates.Waiting)//伪装状态鼠标移上去没效果
                boundingBox = default;
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            return NPC.NewNPC(new EntitySource_SpawnNPC(), tileX * 16 + 8, tileY * 16, NPC.type
                , ai0: tileX, ai1: tileY, ai2: (int)Main.rand.NextFromList(VinicTypes.Column, VinicTypes.LASER));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
            => CanHit;

        #endregion

        #region AI

        /*
         * 蕴魔模仿藤
         * 
         * 待机阶段：原地不动
         * 
         * 攻击阶段：根据不同的类型发动不同的攻击
         * 水晶柱：瞄准后咬出
         * 激光器：瞄准后发射光束
         * 
         */

        public override void AI()
        {
            if (!CheckTile())//基座物块消失就kill
            {
                NPC.Kill();
                return;
            }

            //保护出生点
            FixExploitManEaters.ProtectSpot(SpawnPoint.X, SpawnPoint.Y);

            switch (State)
            {
                case AIStates.Waiting:
                    Waiting();
                    break;
                case AIStates.Attack_Bite:
                    AttackBite();
                    break;
                case AIStates.Attack_Laser:
                    break;
                case AIStates.Attack_Rest:
                    break;
                case AIStates.BackToWait:
                    break;
                default:
                    break;
            }
        }

        //public void Initialize()
        //{
            
        //}

        public void Waiting()
        {
            NPC.velocity = Vector2.Zero;
            NPC.rotation = -MathHelper.PiOver2;
            NPC.Center = SpawnPoint.ToWorldCoordinates() + new Vector2(0, -32);

            if (Timer > 20)
            {
                NPC.TargetClosest(false);
                if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget(16 * 6 + 8))//玩家在6格半以内开始发动攻击
                {
                    StartAttack();
                    return;
                }

                Timer = 0;
            }

            Timer++;
        }

        #region 攻击状态

        public void AttackBite()
        {
            const int ReadyTime = 26;
            const int BiteTime = 6;
            const float BiteMouseAngle = 0.7f;

            Vector2 baseP = SpawnPoint.ToWorldCoordinates();

            do
            {
                if (Timer < ReadyTime)
                {
                    //设置位置和角度
                    Vector2 dir = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

                    NPC.Center = Smoother.Update(1 / 60f, baseP + dir * 64);
                    NPC.rotation = (NPC.Center - baseP).ToRotation();

                    //设置嘴的张开角度
                    MouseRotation =MouseRotation.AngleLerp( BiteMouseAngle * Coralite.Instance.SqrtSmoother.Smoother(Timer / ReadyTime),0.3f);

                    //粒子
                    if (Timer % 3 == 0)
                    {
                        float distance = Main.rand.NextFloat(64, 80);
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                        var p = PRTLoader.NewParticle<ChaseablePixelLine>(pos
                            , (NPC.Center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 4.5f), newColor: Main.rand.NextFromList(Coralite.CrystallineMagikePurple, Coralite.MagicCrystalPink)
                              , Scale: Main.rand.NextFloat(1f, 1.5f));

                        p.entity = NPC;
                        p.fadeFactor = 0.9f;
                        p.TrailCount = 20;
                    }

                    Timer++;
                    break;
                }

                if (Timer == ReadyTime)
                {
                    //冲出
                    MouseRotation = BiteMouseAngle;
                    NPC.velocity = (NPC.Center - baseP).SafeNormalize(Vector2.Zero) * 20;
                    Timer++;
                    CanHit = true;
                    NPC.netUpdate = true;

                    Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, NPC.Center, pitch: -0.4f, volumeAdjust: -0.2f);
                    break;
                }

                if (Timer == ReadyTime + 1)//攻击中
                {
                    for (int i = -1; i < 2; i += 2)//攻击时的粒子
                    {
                        Vector2 p = NPC.Center + (i * MouseRotation).ToRotationVector2() * 24 + Main.rand.NextVector2Circular(12, 12);
                        Dust d = Dust.NewDustPerfect(p, ModContent.DustType<PixelPoint>()
                               , NPC.velocity * Main.rand.NextFloat(-0.3f, -0.1f), newColor: Coralite.CrystallineMagikePurple
                               , Scale: Main.rand.NextFloat(1f, 1.4f));
                        d.noGravity = true;
                    }

                    if (Vector2.Distance(NPC.Center, baseP) > 16 * 18)//冲到底了，回来
                    {
                        NPC.velocity *= 0;
                        Timer++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                if (Timer < ReadyTime + 1 + BiteTime)//攻击最后一下咬合（虽然没什么用处但是看着很合理）
                {
                    float factor = (Timer - ReadyTime - 1) / BiteTime;
                    MouseRotation = BiteMouseAngle * (1 - Coralite.Instance.HeavySmootherInstance.Smoother(factor));

                    if (Timer == ReadyTime + 1 + BiteTime / 2)//咬合的粒子效果和音效
                    {
                        Vector2 dir = NPC.rotation.ToRotationVector2();
                        for (int i = 0; i < 14; i++)
                        {
                            Vector2 p = NPC.Center + dir * Main.rand.NextFloat(-14, 14);
                            Dust d = Dust.NewDustPerfect(p, ModContent.DustType<PixelPoint>()
                                , dir.RotatedBy(Main.rand.NextFromList(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(1, 5)
                                , newColor: Coralite.CrystallineMagikePurple, Scale: Main.rand.NextFloat(1f, 1.4f));
                        }

                        Helper.PlayPitched(CoraliteSoundID.Boom_Item14, NPC.Center,pitch:-0.5f);
                        Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, NPC.Center);
                    }

                    Timer++;
                    break;
                }

                TurnToRest();

            } while (false);
        }

        public void Resting()
        {
            if (Timer > 60 * 3)
            {
                StartAttack();

                return;
            }

            if (Timer % 30 == 0)
            {
                if (!(NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget(16 * 32)))
                    TurnToWaiting();
            }

            Vector2 baseP = SpawnPoint.ToWorldCoordinates();
            Vector2 toTarget = Target.Center - NPC.Center;
            Vector2 dir = toTarget.SafeNormalize(Vector2.Zero);
            float distanceToTarget = toTarget.Length();

            if (distanceToTarget > 86)
                distanceToTarget = 86;

            //在目标点周围随便晃一晃
            Vector2 aimCenter = baseP + dir * distanceToTarget;

            float distance = NPC.Distance(aimCenter);

            Vector2 velocity = NPC.velocity;

            if (distance > 16 * 10)//太远就飞近点
            {
                velocity += (aimCenter - NPC.Center).SafeNormalize(Vector2.Zero) * 0.2f;
                if (velocity.Length() > 14)
                    velocity = velocity.SafeNormalize(Vector2.Zero) * 14;
            }
            else if (distance < 16 * 4)//太近就飞远点
            {
                velocity -= (aimCenter - NPC.Center).SafeNormalize(Vector2.Zero) * 0.3f;
                if (velocity.Length() > 14)
                    velocity = velocity.SafeNormalize(Vector2.Zero) * 14;
            }
            else
            {
                velocity *= 0.96f;
            }

            NPC.velocity = velocity;
            NPC.rotation = (NPC.Center - baseP).ToRotation();
            MouseRotation = MouseRotation.AngleLerp(0.4f + 0.25f * MathF.Sin(Main.GlobalTimeWrappedHourly), 0.4f);

            if (Timer % 80 == 0)
                NPC.velocity = NPC.velocity.RotateByRandom(-MathHelper.Pi, MathHelper.Pi);

            Timer++;
        }

        public void TurnToRest()
        {
            State = AIStates.Attack_Rest;

            Timer = 0;
            CanHit = false;

            NPC.netUpdate = true;
        }

        public void TurnToWaiting()
        {
            State = AIStates.BackToWait;

            Timer = 0;
            CanHit = false;

            NPC.netUpdate = true;
        }

        #endregion

        public void StartAttack()
        {
            if (State == AIStates.Waiting)//突然蹦起来
            {
                Smoother = new SecondOrderDynamics_Vec2(0.8f, 0.75f, 0, NPC.Center);
                Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, NPC.Center);
                for (int i = 0; i < 48; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(24, 24);
                    Dust.NewDustPerfect(pos, ModContent.DustType<CrystallineDust>()
                        , (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(0.8f, 1.4f));
                }
            }

            State = VinicType switch
            {
                VinicTypes.LASER => AIStates.Attack_Laser,
                _ => AIStates.Attack_Bite,
            };

            Timer = 0;
        }

        /// <summary>
        /// 检测基座的物块是否安全
        /// </summary>
        public bool CheckTile()
        {
            Point p = SpawnPoint;
            if (!WorldGen.InWorld(p.X, p.Y))
                return false;

            Tile t = Framing.GetTileSafely(p.X, p.Y);
            if (!t.HasTile || !Main.tileSolid[t.TileType])
                return false;

            return true;
        }

        /// <summary>
        /// 需要玩家不隐身（隐身是指有隐身药水BUFF和不在使用物品）<br></br>
        /// 需要距离小于800，并且可以看到玩家
        /// </summary>
        /// <returns></returns>
        public bool CanHitTarget(float distance)
            => !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < distance &&
                    Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);

        #endregion

        #region 网络同步

        public override void SendExtraAI(BinaryWriter writer)
        {
            BitsByte b = new BitsByte();
            b[0] = CanHit;

            writer.Write(b);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            BitsByte b = reader.ReadBitsByte();
            CanHit = b[0];
        }

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();



            return false;
        }

        #endregion
    }
}
