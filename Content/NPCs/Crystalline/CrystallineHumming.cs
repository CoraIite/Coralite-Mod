using Coralite.Content.Biomes;
using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.NPCs.Crystalline
{
    public class CrystallineHumming : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        private AIStates State
        {
            get => (AIStates)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        private ref float Timer => ref NPC.ai[1];

        private enum AIStates
        {
            Idle_Flying,
            Idle_Floating,

            Attack_Dash,
            Attack_Shoot,
            Attack_Floating,
        }

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 38;
            NPC.damage = 60;
            NPC.defense = 20;

            NPC.lifeMax = 1200;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(0, 0, 40, 0);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<CrystallineSkyIsland>())
                return 0.02f;

            return 0;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 2, 5));

            //小概率掉落魔鸟


            //概率掉落蕴魔海燕麦母体
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 8));

            //掉落宝石原石
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryRoughGemstone>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<SeniorRoughGemstone>(), 3, 1, 3));
        }

        #endregion

        #region AI

        /*
         * 蕴魔蜂鸟
         * 
         * 待机阶段：到处飞行，撞墙就反方向飞
         * 飞行一段时间后悬停，朝向另一个方向飞
         * 
         * 每隔一段时间检测周围玩家，如果有可攻击的玩家那么就转变为进攻状态
         * 
         * 进攻阶段：主要有2个招式
         * 冲刺，帧图为5~7，悬停后瞄准玩家冲刺
         * 
         * 弹幕，悬停后短暂蓄力，之后射出水晶刺，水晶刺撞到物块后会爆开生成衍生弹幕
         * 
         * 休息阶段：随意乱飞
         */

        public override void AI()
        {
            switch (State)
            {
                case AIStates.Idle_Flying:
                    IdleFlying();
                    break;
                case AIStates.Idle_Floating:
                    break;
                case AIStates.Attack_Dash:
                    break;
                case AIStates.Attack_Shoot:
                    break;
                case AIStates.Attack_Floating:
                    break;
                default:
                    break;
            }
        }

        public void IdleFlying()
        {

        }



        #endregion

        #region 网络同步

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
