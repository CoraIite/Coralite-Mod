using Coralite.Content.Biomes;
using Coralite.Content.Dusts;
using Coralite.Content.Items.Banner;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.Magike.Refractors;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Content.NPCs.Crystalline
{
    [AutoLoadTexture(Path = AssetDirectory.CrystallineNPCs)]
    public abstract class CrystallineVinic : ModNPC
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

        protected AIStates State
        {
            get => (AIStates)NPC.ai[3];
            set => NPC.ai[3] = (int)value;
        }

        /// <summary> 能否对玩家造成伤害 </summary>
        public bool CanHit { get; set; } = false;

        protected ref float Timer => ref NPC.localAI[0];
        protected ref float MouseRotation => ref NPC.localAI[1];

        protected abstract OnTileTypes OnTileType { get; }
        protected abstract int AttackLength { get; }

        protected enum AIStates
        {
            Waiting,
            Attack,
            Attack_Rest,
            BackToWait,
        }

        protected enum OnTileTypes
        {
            OnBottom,
            OnTop
        }

        protected Player Target => Main.player[NPC.target];

        #region 基础值设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPC.QuickTrailSets(Helper.NPCTrailingMode.OnlyPosition, 2);
        }

        public override void Load()
        {
            this.LoadGore(3);
            this.RegisterBestiaryDescription();
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 16 * 3;
            NPC.damage = 110;
            NPC.defense = 24;

            NPC.lifeMax = 340;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.2f;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 25, 0);

            SpawnModBiomes = [ModContent.GetInstance<CrystallineSkyIsland>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.AddTags(
                this.GetBestiaryDescription()
                );
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.hardMode && spawnInfo.Player.InModBiome<CrystallineSkyIsland>())
            {
                int tileY = spawnInfo.SpawnTileY;
                for (int i = 0; i < 45; i++)
                {
                    Tile t = Framing.GetTileSafely(spawnInfo.SpawnTileX, tileY);
                    if (t.HasTile && Main.tileSolid[t.TileType])
                        break;

                    if (OnTileType == OnTileTypes.OnBottom)
                        tileY++;
                    else
                        tileY--;
                }

                Tile t2 = Framing.GetTileSafely(spawnInfo.SpawnTileX, tileY);
                if (!t2.HasTile || !Main.tileSolid[t2.TileType] || !Main.tileBlockLight[t2.TileType])//必须得是遮光物块
                    return 0;

                if (Helper.IsPointOnScreen(new Vector2(spawnInfo.SpawnTileX, tileY) * 16 - Main.screenPosition))
                    return 0;
                else
                    return 0.03f;
            }

            return 0;
        }

        protected static void NormalDrop(ref NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 2, 5));

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
            for (int i = 0; i < 45; i++)
            {
                Tile t = Framing.GetTileSafely(tileX, tileY);
                if (t.HasTile && Main.tileSolid[t.TileType])
                    break;

                if (OnTileType == OnTileTypes.OnBottom)
                    tileY++;
                else
                    tileY--;
            }

            return NPC.NewNPC(new EntitySource_SpawnNPC(), tileX * 16 + 8, tileY * 16, NPC.type
                , ai0: tileX, ai1: tileY);
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
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }

            //保护出生点
            FixExploitManEaters.ProtectSpot(SpawnPoint.X, SpawnPoint.Y);

            switch (State)
            {
                default:
                case AIStates.Waiting:
                    Waiting();
                    break;
                case AIStates.Attack:
                    Attack();

                    Lighting.AddLight(NPC.Center, Coralite.CrystallinePurple.ToVector3() / 2);
                    break;
                case AIStates.Attack_Rest:
                    if (Timer > 60 * 3.5f)//重新开始攻击
                    {
                        if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget() && CanHitCheck())
                            StartAttack();
                        else
                            TurnToBackWaiting();

                        return;
                    }

                    if (Timer > 0 && Timer % 30 == 0)
                        if (!(NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget() && CanHitCheck()))
                            TurnToBackWaiting();

                    Resting();

                    Timer++;
                    break;
                case AIStates.BackToWait:
                    BackToWaiting();
                    break;
            }
        }

        /// <summary>
        /// 从短暂休息到继续攻击的判定，返回true表示能打到玩家，也可以检测之间有无物块之类的
        /// </summary>
        /// <returns></returns>
        public abstract bool CanHitCheck();

        public void Waiting()
        {
            NPC.velocity = Vector2.Zero;
            NPC.rotation = GetVerticalDir() * MathHelper.PiOver2;
            NPC.chaseable = false;
            NPC.Center = SpawnPoint.ToWorldCoordinates() + new Vector2(0, GetVerticalDir() * 32);

            if (Timer > 20)
            {
                NPC.TargetClosest(false);
                if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead
                    && !(Target.invis && Target.itemAnimation == 0) && WaitingToAttackCheck(Target.Center, SpawnPoint.ToWorldCoordinates())
                    && Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height))//玩家在8格以内开始发动攻击
                {
                    StartAttack();
                    return;
                }

                Timer = 0;
            }

            Timer++;
        }

        /// <summary>
        /// 从伪装状态到起身攻击状态的判定，例如水晶柱模仿藤为两个参数距离小于一定值触发
        /// </summary>
        /// <param name="targetCenter"></param>
        /// <param name="basePoint"></param>
        /// <returns></returns>
        public abstract bool WaitingToAttackCheck(Vector2 targetCenter, Vector2 basePoint);

        #region 攻击状态

        /// <summary>
        /// 特殊的攻击动作
        /// </summary>
        public virtual void Attack() { }

        /// <summary>
        /// 根据底座类型返回方向
        /// </summary>
        /// <returns></returns>
        public int GetVerticalDir()
        {
            return OnTileType switch
            {
                OnTileTypes.OnBottom => -1,
                _ => 1
            };
        }

        public void Resting()
        {
            Vector2 baseP = SpawnPoint.ToWorldCoordinates(8, GetVerticalDir() * 12);
            Vector2 toTarget = Target.Center - NPC.Center;
            Vector2 dir = toTarget.SafeNormalize(Vector2.Zero);
            float distanceToTarget = toTarget.Length();

            distanceToTarget /= 2;

            if (distanceToTarget < 86)
                distanceToTarget = 86;

            //在目标点周围随便晃一晃
            Vector2 aimCenter = baseP + dir * distanceToTarget;

            float distance = NPC.Distance(aimCenter);

            Vector2 velocity = NPC.velocity;

            if (distance > 16 * 6.5f)//太远就飞近点
            {
                velocity += (aimCenter - NPC.Center).SafeNormalize(Vector2.Zero) * 0.15f;
                if (velocity.Length() > 4)
                    velocity = velocity.SafeNormalize(Vector2.Zero) * 4;
            }
            else if (distance < 16 * 5.5f)//太近就飞远点
            {
                velocity -= (aimCenter - NPC.Center).SafeNormalize(Vector2.Zero) * 0.05f;
                if (velocity.Length() > 4)
                    velocity = velocity.SafeNormalize(Vector2.Zero) * 4;
            }
            else
            {
                velocity *= 0.96f;
                if (Timer % 60 == 0)
                    velocity += dir * 3;
            }

            NPC.velocity = velocity;
            NPC.rotation = (NPC.Center - baseP).ToRotation();
            MouseRotation = MouseRotation.AngleLerp(0.15f + 0.2f * MathF.Sin((int)Main.timeForVisualEffects * 0.1f), 0.7f);
        }

        public void BackToWaiting()
        {
            Vector2 baseP = SpawnPoint.ToWorldCoordinates();

            const int BackToTopTime = 40;
            const int BackPosTime = 30;

            do
            {
                if (Timer < BackToTopTime)
                {
                    Vector2 aimPos = baseP + new Vector2(0, GetVerticalDir() * 16 * 6);
                    MouseRotation = MouseRotation.AngleLerp(0.3f, 0.2f);
                    NPC.velocity *= 0.8f;
                    NPC.Center = Vector2.SmoothStep(NPC.Center, aimPos, 0.2f);
                    NPC.rotation = (NPC.Center - baseP).ToRotation();
                    break;
                }

                if (Timer < BackToTopTime + 10)
                {
                    MouseRotation = MouseRotation.AngleLerp(0, 0.3f);

                    NPC.rotation = (NPC.Center - baseP).ToRotation();
                    break;
                }

                if (Timer < BackToTopTime + 10 + BackPosTime)
                {
                    MouseRotation = MouseRotation.AngleLerp(0, 0.3f);
                    NPC.rotation = NPC.rotation.AngleLerp(GetVerticalDir() * MathHelper.PiOver2, 0.2f);

                    NPC.Center = Vector2.SmoothStep(NPC.Center, baseP + new Vector2(0, GetVerticalDir() * 32), 0.2f);
                    break;
                }

                for (int i = 0; i < 38; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(24, 24);
                    Dust.NewDustPerfect(pos, ModContent.DustType<CrystallineDust>()
                        , (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(0.8f, 1.4f));
                }

                TurnToWaiting();

            } while (false);

            Timer++;
        }

        public virtual void TurnToRest()
        {
            State = AIStates.Attack_Rest;

            Timer = 0;
            CanHit = false;

            NPC.netUpdate = true;
        }

        public void TurnToBackWaiting()
        {
            State = AIStates.BackToWait;

            Timer = 0;
            CanHit = false;

            NPC.netUpdate = true;
        }

        #endregion

        public void StartAttack()
        {
            NPC.chaseable = true;
            Timer = 0;
            CanHit = false;

            if (State == AIStates.Waiting)//突然蹦起来
            {
                Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, NPC.Center);
                for (int i = 0; i < 38; i++)
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(24, 24);
                    Dust.NewDustPerfect(pos, ModContent.DustType<CrystallineDust>()
                        , (pos - NPC.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(0.8f, 1.4f));
                }
            }

            State = AIStates.Attack;
        }

        public void TurnToWaiting()
        {
            NPC.chaseable = false;
            State = AIStates.Waiting;

            Timer = 0;
            CanHit = false;

            NPC.netUpdate = true;
        }

        /// <summary>
        /// 检测基座的物块是否安全
        /// </summary>
        public bool CheckTile()
        {
            Point p = SpawnPoint;

            if (p == Point.Zero)
            {
                int tileX = (int)NPC.Center.X / 16;
                int tileY = (int)NPC.Center.Y / 16;
                for (int i = 0; i < 30; i++)
                {
                    Tile t2 = Framing.GetTileSafely(tileX, tileY);
                    if (t2.HasTile && Main.tileSolid[t2.TileType])
                        break;

                    switch (OnTileType)
                    {
                        default:
                        case OnTileTypes.OnBottom:
                            tileY++;
                            break;
                        case OnTileTypes.OnTop:
                            tileY--;
                            break;
                    }
                }

                Tile t3 = Framing.GetTileSafely(tileX, tileY);
                if (!t3.HasTile || !Main.tileSolid[t3.TileType] || !Main.tileBlockLight[t3.TileType])//必须得是遮光物块
                    return false;

                SpawnPoint = new Point(tileX, tileY);

                return true;
            }

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
        public bool CanHitTarget()
            => !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(Target.Center, SpawnPoint.ToWorldCoordinates()) < AttackLength;

        #endregion

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (State == AIStates.Attack_Rest)
                Timer += 5;
            else if (State == AIStates.Waiting)
            {
                if (NPC.life < NPC.lifeMax / 2)
                {
                    NPC.TargetClosest(false);
                    StartAttack();
                }
            }


            if (NPC.life <= 0)
            {
                this.SpawnGore(3, 3);
            }
        }


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
            Vector2 dir = NPC.rotation.ToRotationVector2() * 16;

            if (State != AIStates.Waiting)
            {
                int offset = GetVerticalDir() switch
                {
                    1 => 30,
                    _ => -12
                };

                DrawLine(spriteBatch, SpawnPoint.ToWorldCoordinates(8, offset), NPC.Center - dir, screenPos);
            }

            Texture2D mainTex = NPC.GetTexture();
            Rectangle frameBox;
            Vector2 pos = NPC.Center - screenPos;

            float rotation = NPC.rotation;

            if (State == AIStates.Waiting)
            {
                frameBox = mainTex.Frame(1, 4, 0, 0);
                spriteBatch.Draw(mainTex, pos, frameBox, drawColor, (float)rotation, frameBox.Size() / 2, NPC.scale, 0, 0);
            }
            else
            {
                //绘制底层
                frameBox = mainTex.Frame(1, 4, 0, 1);
                spriteBatch.Draw(mainTex, pos, frameBox, drawColor, rotation, frameBox.Size() / 2, NPC.scale, 0, 0);

                pos -= dir;

                //绘制左边嘴
                frameBox = mainTex.Frame(1, 4, 0, 2);
                spriteBatch.Draw(mainTex, pos, frameBox, drawColor, rotation - MouseRotation, new Vector2(frameBox.Width / 4, frameBox.Height / 2), NPC.scale, 0, 0);

                //绘制右边嘴
                frameBox = mainTex.Frame(1, 4, 0, 3);
                spriteBatch.Draw(mainTex, pos, frameBox, drawColor, rotation + MouseRotation, new Vector2(frameBox.Width / 4, frameBox.Height / 2), NPC.scale, 0, 0);

            }
            return false;
        }

        public virtual void DrawLine(SpriteBatch spriteBatch, Vector2 bottomPos, Vector2 tipPos, Vector2 screenpos)
        {
            Texture2D mainTex = VineTex.Value;

            float length = Vector2.Distance(bottomPos, tipPos);
            const int TipHeight = 28;

            Vector2 origin = new Vector2(mainTex.Width / 4, TipHeight);
            Vector2 dir = (tipPos - bottomPos).SafeNormalize(Vector2.Zero);

            float rotation = dir.ToRotation() + MathHelper.PiOver2;

            if (length > TipHeight)
            {
                float middleLength = length - TipHeight / 2;

                int drawCount = (int)middleLength / TipHeight;
                int height = (int)middleLength % TipHeight;
                for (int i = 0; i < drawCount; i++)
                {
                    Vector2 middleP = bottomPos + dir * (i * 28 + TipHeight / 2);
                    int frame;
                    if (i < 4)
                        frame = 6 - i;
                    else
                        frame = 2 - (i - 3) % 3;
                    spriteBatch.Draw(mainTex, middleP - screenpos, mainTex.Frame(2, 8, 0, frame)
                        , Lighting.GetColor(middleP.ToTileCoordinates()), (float)rotation, origin, NPC.scale, 0, 0);
                }

                Vector2 tP = bottomPos + dir * (drawCount * 28 + TipHeight / 2);
                spriteBatch.Draw(mainTex, tP - screenpos, new Rectangle(0, 30 - height, 60, height)
                    , Lighting.GetColor(tP.ToTileCoordinates()), (float)rotation, new Vector2(mainTex.Width / 4, height), NPC.scale, 0, 0);
            }

            //绘制底部
            Rectangle frameBox = new Rectangle(60, 0, 60, 30);
            spriteBatch.Draw(mainTex, bottomPos - screenpos, frameBox
                , Lighting.GetColor(bottomPos.ToTileCoordinates()), OnTileType == OnTileTypes.OnBottom ? 0 : MathHelper.Pi, frameBox.Size() / 2, NPC.scale, 0, 0);

            frameBox = mainTex.Frame(2, 8, 0, 7);
            spriteBatch.Draw(mainTex, bottomPos - screenpos, frameBox
                , Lighting.GetColor(bottomPos.ToTileCoordinates()), rotation, frameBox.Size() / 2, NPC.scale, 0, 0);
        }

        #endregion
    }

    public class CrystallineVinicColumn : CrystallineVinic
    {
        protected override OnTileTypes OnTileType => OnTileTypes.OnBottom;

        protected override int AttackLength => GetBiteLength() + 16 * 5;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = Texture + "_Bestiary",
                //Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CrystallineVinicColumnBannerItem>();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            NormalDrop(ref npcLoot);

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BasicColumn>(), 4));
        }

        public override void Attack()
        {
            int ReadyTime = Helper.ScaleValueForDiffMode(40, 30, 25, 15);
            const int BiteTime = 7;
            const float BiteMouseAngle = 0.8f;

            Vector2 baseP = SpawnPoint.ToWorldCoordinates(8, -12);

            do
            {
                if (Timer < ReadyTime)
                {
                    //设置位置和角度
                    Vector2 dir = (Target.Center - baseP).SafeNormalize(Vector2.Zero);
                    float distance2 = (Target.Center - baseP).Length();
                    if (distance2 > 64)
                        distance2 = 64;

                    NPC.Center = Vector2.SmoothStep(NPC.Center, baseP + dir * distance2, 0.25f);
                    NPC.velocity *= 0.8f;
                    NPC.rotation = (NPC.Center - baseP).ToRotation();

                    //设置嘴的张开角度
                    MouseRotation = MouseRotation.AngleLerp(BiteMouseAngle * Helper.SqrtEase(Timer / ReadyTime), 0.3f);

                    //粒子
                    if (Timer % 3 == 0 && Timer < ReadyTime / 2)
                    {
                        float distance = Main.rand.NextFloat(64, 80);
                        Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                        var p = PRTLoader.NewParticle<ChaseablePixelLine>(pos
                            , (NPC.Center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 4.5f), newColor: Main.rand.NextFromList(Coralite.CrystallinePurple, Coralite.MagicCrystalPink)
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
                    NPC.velocity = (NPC.Center - baseP).SafeNormalize(Vector2.Zero) * 24;
                    Timer++;
                    CanHit = true;
                    NPC.knockBackResist = 0;
                    NPC.netUpdate = true;

                    Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, NPC.Center, pitch: -0.4f, volumeAdjust: -0.2f);
                    break;
                }

                if (Timer == ReadyTime + 1)//攻击中
                {
                    for (int i = -1; i < 2; i += 2)//攻击时的粒子
                    {
                        Vector2 p = NPC.Center + (NPC.rotation + i * MouseRotation).ToRotationVector2() * 24 + Main.rand.NextVector2Circular(6, 6);
                        Dust d = Dust.NewDustPerfect(p, ModContent.DustType<PixelPoint>()
                               , NPC.velocity * Main.rand.NextFloat(-0.3f, -0.1f), newColor: Coralite.CrystallinePurple
                               , Scale: Main.rand.NextFloat(1f, 2f));
                        d.noGravity = true;
                        d = Dust.NewDustPerfect(p, DustID.UnusedWhiteBluePurple
                              , NPC.velocity * Main.rand.NextFloat(-0.3f, -0.1f), newColor: Coralite.CrystallinePurple
                              , Scale: Main.rand.NextFloat(1f, 2f));
                        d.noGravity = true;
                    }

                    int biteLength = GetBiteLength();
                    float l = Vector2.Distance(NPC.Center, baseP);
                    if (l > biteLength || l > Vector2.Distance(Target.Center, baseP) + 16)//冲到底了，回来
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
                    MouseRotation = BiteMouseAngle * (1 - Helper.HeavyEase(factor));
                    NPC.knockBackResist = 0.2f;

                    if (Timer == ReadyTime + 1 + BiteTime / 2)//咬合的粒子效果和音效
                    {
                        Vector2 dir = NPC.rotation.ToRotationVector2();
                        for (int i = 0; i < 16; i++)
                        {
                            Vector2 p = NPC.Center + dir * Main.rand.NextFloat(-24, 24);
                            Dust.NewDustPerfect(p, ModContent.DustType<PixelPoint>()
                                 , dir.RotatedBy(Main.rand.NextFromList(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(1, 6)
                                 , newColor: Coralite.CrystallinePurple, Scale: Main.rand.NextFloat(1f, 2f));
                            Dust d = Dust.NewDustPerfect(p, DustID.UnusedWhiteBluePurple
                                , dir.RotatedBy(Main.rand.NextFromList(-MathHelper.PiOver2, MathHelper.PiOver2)) * Main.rand.NextFloat(1, 6)
                                , newColor: Coralite.CrystallinePurple, Scale: Main.rand.NextFloat(1f, 2f));

                            d.noGravity = true;
                        }

                        Dust d2 = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * 16, ModContent.DustType<VinicBigImpact>()
                            , Vector2.Zero);
                        d2.rotation = NPC.rotation;

                        Helper.PlayPitched(CoraliteSoundID.Boom_Item14, NPC.Center, pitch: -0.5f);
                        Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, NPC.Center);
                    }

                    Timer++;
                    break;
                }

                TurnToRest();

            } while (false);
        }

        public int GetBiteLength()
            => 16 * Helper.ScaleValueForDiffMode(24, 28, 34, 48);

        public override bool CanHitCheck() => true;

        public override bool WaitingToAttackCheck(Vector2 targetCenter, Vector2 basePoint)
        {
            return Vector2.Distance(targetCenter, basePoint) < 16 * 8;
        }
    }

    public class CrystallineVinicLaser : CrystallineVinic
    {
        protected override OnTileTypes OnTileType => OnTileTypes.OnTop;

        protected override int AttackLength => 16 * 40;

        private ref float LaserShootCount => ref NPC.localAI[2];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                CustomTexturePath = Texture + "_Bestiary",
                //Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CrystallineVinicLaserBannerItem>();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            NormalDrop(ref npcLoot);

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LASER>(), 4));
        }

        public override void TurnToRest()
        {
            State = AIStates.Attack_Rest;

            Timer = 0;
            CanHit = false;
            LaserShootCount = 0;

            NPC.netUpdate = true;
        }

        public override void Attack()
        {
            int ReadyTime = Helper.ScaleValueForDiffMode(40, 30, 20, 15);
            const float BiteMouseAngle = 0.8f;

            Vector2 baseP = SpawnPoint.ToWorldCoordinates(8, 12);

            do
            {
                if (Timer < ReadyTime)
                {
                    //设置位置和角度
                    Vector2 dir = (Target.Center - baseP).SafeNormalize(Vector2.Zero);
                    float distance2 = (Target.Center - baseP).Length();
                    if (distance2 > 16 * 6)
                        distance2 = 16 * 6;

                    NPC.Center = Vector2.SmoothStep(NPC.Center, baseP + dir * distance2, 0.25f);
                    NPC.velocity *= 0.8f;
                    NPC.rotation = (NPC.Center - baseP).ToRotation();

                    //设置嘴的张开角度
                    MouseRotation = MouseRotation.AngleLerp(BiteMouseAngle, 0.2f);

                    //粒子
                    Vector2 center = NPC.Center - NPC.rotation.ToRotationVector2() * 24;
                    for (int i = -1; i < 2; i += 2)
                    {
                        Dust d = Dust.NewDustPerfect(center + (NPC.rotation + i * MouseRotation).ToRotationVector2() * Main.rand.NextFloat(8, 40)
                            , DustID.UnusedWhiteBluePurple, (NPC.rotation + i * MouseRotation - i * MathHelper.PiOver2).ToRotationVector2() * Main.rand.NextFloat(2, 4));
                        d.noGravity = true;
                    }

                    Timer++;
                    break;
                }

                if (Timer == ReadyTime)
                {
                    MouseRotation = BiteMouseAngle;
                    Timer++;
                    NPC.velocity = -NPC.rotation.ToRotationVector2() * 3;
                    NPC.netUpdate = true;

                    Helper.PlayPitched(CoraliteSoundID.LaserShoot2_Item75, NPC.Center, pitch: -0.4f, volumeAdjust: -0.2f);

                    Dust d2 = Dust.NewDustPerfect(NPC.Center + NPC.rotation.ToRotationVector2() * 16, ModContent.DustType<VinicBigImpact>()
                        , Vector2.Zero);
                    d2.rotation = NPC.rotation;

                    if (!VaultUtils.isClient)//射激光
                        NPC.NewProjectileInAI<VinicLaser>(NPC.Center, NPC.rotation.ToRotationVector2() * 4
                            , Helper.GetProjDamage(80, 120, 140), 0);

                    break;
                }

                if (Timer < ReadyTime + 20)
                {
                    MouseRotation = MouseRotation.AngleLerp(0, 0.1f);
                    NPC.velocity *= 0.94f;
                    Timer++;
                    NPC.rotation = (NPC.Center - baseP).ToRotation();

                    return;
                }

                if (Main.getGoodWorld && LaserShootCount < 2)
                {
                    LaserShootCount++;
                    Timer = ReadyTime / 3;
                    return;
                }

                TurnToRest();

            } while (false);
        }

        public override bool CanHitCheck()
            => Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.waist, Target.height);

        public override bool WaitingToAttackCheck(Vector2 targetCenter, Vector2 basePoint)
        {
            return Math.Abs(targetCenter.X - basePoint.X) < 16 * 4 && basePoint.Y - 16 < targetCenter.Y && Vector2.Distance(targetCenter, basePoint) < 16 * 40;
        }
    }

    public class VinicLaser : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 180;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Color drawColor = Color.CornflowerBlue;
            Color baseColor = Color.Purple;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 3.14f;
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = -1;
                SoundEngine.PlaySound(CoraliteSoundID.LaserShoot2_Item75, Projectile.Center);
                for (int num345 = 0; num345 < 8; num345++)
                {
                    Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple);
                    dust2.noGravity = true;
                    dust2.velocity *= 3f;
                    dust2.scale = 1.5f;
                    dust2.velocity += Projectile.velocity * Main.rand.NextFloat();
                }
            }

            float factor = Projectile.timeLeft / 180f;
            Color c = Color.Lerp(baseColor, drawColor, factor);
            byte hue = (byte)(Main.rgbToHsl(c).X * 255f);

            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ChlorophyteLeafCrystalShot, new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center,
                MovementVector = Projectile.velocity,
                UniqueInfoPiece = hue
            });

            if (Projectile.timeLeft % 3 == 0)
            {
                int index = Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(4, 4), 0, 0, DustID.RainbowTorch
                    , 0f, 0f, 150, Color.Transparent, 1.2f);
                Main.dust[index].color = Coralite.CrystallinePurple;
                Main.dust[index].noGravity = true;

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), DustID.PurpleTorch
                    , Projectile.velocity.SafeNormalize(Vector2.Zero) * 2f, Scale: 1f);
                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.05f, 0.2f, 0.1f) * 1.5f);
            if (Main.rand.NextBool(5))
            {
                Dust dust12 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple);
                dust12.noGravity = true;
                dust12.velocity *= 0.1f;
                dust12.scale = 1.5f;
                dust12.velocity += Projectile.velocity * Main.rand.NextFloat();
                dust12.color = c;
                dust12.color.A /= 4;
                dust12.alpha = 100;
                dust12.noLight = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class VinicBigImpact : ModDust
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.color = Color.White;
            dust.frame = Texture2D.Frame(1, 6, 0, 0);
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, Coralite.CrystallinePurple.ToVector3());
            dust.fadeIn++;
            if (++dust.fadeIn > 2)
            {
                dust.fadeIn = 0;
                dust.frame.Y += Texture2D.Height() / 6;
                if (dust.frame.Y > Texture2D.Height() / 6 * 5)
                    dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, dust.color, dust.rotation + MathHelper.PiOver2, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
