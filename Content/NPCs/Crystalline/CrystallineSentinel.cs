using Coralite.Content.Biomes;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.NPCs.Crystalline
{
    public class CrystallineSentinel : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        private AIStates State
        {
            get => (AIStates)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        private ref float Timer => ref NPC.ai[1];
        private ref float Recorder => ref NPC.ai[2];

        private bool CanHit
        {
            get => NPC.ai[3] == 1;
            set
            {
                if (value)
                    NPC.ai[3] = 1;
                else
                    NPC.ai[3] = 0;
            }
        }

        private TextTypes TextType
        {
            get => (TextTypes)NPC.localAI[2];
            set => NPC.localAI[2] = (int)value;
        }

        private Player Target => Main.player[NPC.target];

        private enum AIStates
        {
            P1Idle,
            P1Walking,

            P1Guard,
            P1Spurt,

            Exchange,

            /// <summary>
            /// 二阶段悬浮
            /// </summary>
            P2Idle,
            /// <summary>
            /// 二阶段螺旋冲刺
            /// </summary>
            P2Rolling,
            /// <summary>
            /// 二阶段挥刀
            /// </summary>
            P2Swing,
        }

        private enum TextTypes
        {
            None,
            Surprise,
            Confusion,
            Hurt,
        }

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 21;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 80;
            NPC.damage = 70;
            NPC.defense = 45;

            NPC.lifeMax = 2000;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.5f;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(0, 2);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 4, 12));


            //掉落宝石原石
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 1, 2, 4));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 1, 2, 4));
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

                    tileY++;
                }

                Tile t2 = Framing.GetTileSafely(spawnInfo.SpawnTileX, tileY);
                if (!t2.HasTile || !Main.tileSolid[t2.TileType] || !Main.tileBlockLight[t2.TileType])//必须得是遮光物块
                    return 0;

                if (Helper.IsPointOnScreen(new Vector2(spawnInfo.SpawnTileX, tileY) * 16 - Main.screenPosition))
                    return 0;
                else
                    return 0.01f;
            }

            return 0;
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            for (int i = 0; i < 45; i++)
            {
                Tile t = Framing.GetTileSafely(tileX, tileY);
                if (t.HasTile && Main.tileSolid[t.TileType])
                    break;

                tileY++;
            }

            return NPC.NewNPC(new EntitySource_SpawnNPC(), tileX * 16 + 8, tileY * 16, NPC.type);
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (State == AIStates.P1Guard)//防御状态不绘制血条
                return false;

            return null;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            if (State == AIStates.P1Guard)//防御状态鼠标移上去没效果
                boundingBox = default;
        }

        #endregion

        #region AI

        public override void AI()
        {
            switch (State)
            {
                case AIStates.P1Idle:
                    P1Idle();
                    break;
                case AIStates.P1Walking:
                    break;
                case AIStates.P1Guard:
                    break;
                case AIStates.P1Spurt:
                    break;
                case AIStates.Exchange:
                    break;
                case AIStates.P2Idle:
                    break;
                case AIStates.P2Rolling:
                    break;
                case AIStates.P2Swing:
                    break;
                default:
                    break;
            }
        }

        public void P1Idle()
        {
            //一动不动
            NPC.velocity.X = 0;
            NPC.frame.X = 0;
            NPC.frame.Y = 0;

            if (Timer % 45 == 0)
                TryTurnToAttack();

            if (Timer > 60 * 5)//随便走走
                SwitchState(AIStates.P1Walking);

            Timer++;
        }

        private void SwitchState(AIStates targetState)
        {
            State = targetState;
            Timer = 0;
            Recorder = 0;
            CanHit = false;

            NPC.netUpdate=true;
        }

        private void TryTurnToAttack()
        {
            NPC.TargetClosest(false);

            //找到玩家了就转向攻击阶段
            if (NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead && CanHitTarget())//不攻击隐身玩家
                StartAttack();
        }

        /// <summary>
        /// 需要玩家不隐身（隐身是指有隐身药水BUFF和不在使用物品）<br></br>
        /// 需要距离小于1000，并且可以看到玩家
        /// </summary>
        /// <returns></returns>
        public bool CanHitTarget()
            => !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < 1000 &&
                    Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);

        public void StartAttack()
        {
            switch (State)
            {
                case AIStates.P1Idle:
                case AIStates.P1Walking:
                    SwitchState(AIStates.P1Spurt);
                    break;
                case AIStates.P2Idle:
                    SwitchState(AIStates.P2Rolling);
                    break;
                default:
                    break;
            }
        }

        #region 一阶段帧图

        #endregion

        #endregion

        #region 网络同步

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effect = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            switch (State)
            {
                case AIStates.P1Idle:
                case AIStates.P1Walking:
                case AIStates.P1Guard:
                case AIStates.P1Spurt://一阶段绘制
                    {
                        Texture2D tex = NPC.GetTexture();

                        Rectangle frameBox = tex.Frame(5, Main.npcFrameCount[NPC.type], NPC.frame.X, NPC.frame.Y);

                        spriteBatch.Draw(tex, NPC.Center - screenPos, frameBox, drawColor
                            , NPC.rotation, frameBox.Size() / 2, NPC.scale, effect, 0);
                    }
                    break;
                case AIStates.Exchange:
                    break;
                case AIStates.P2Idle:
                    break;
                case AIStates.P2Rolling:
                    break;
                case AIStates.P2Swing:
                    break;
                default:
                    break;
            }


            return false;
        }

        #endregion
    }
}
