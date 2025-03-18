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
    public class WildMabirdNest : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        private AIStates State
        {
            get => (AIStates)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        private ref float Timer => ref NPC.ai[1];
        private ref float Recorder => ref NPC.ai[2];
        /// <summary>
        /// 当前有多少的魔鸟
        /// </summary>
        private ref float MabirdCount => ref NPC.ai[3];

        protected Player Target => Main.player[NPC.target];

        private bool init = true;

        private enum AIStates
        {
            Waiting,
            Shoot
        }

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;
        }

        public override void SetDefaults()
        {
            NPC.width = 48;
            NPC.height = 80;
            NPC.damage = 50;
            NPC.defense = 15;

            NPC.lifeMax = 600;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(0, 2);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //固定掉落蕴魔水晶
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystallineMagike>(), 1, 4, 12));

            //掉落魔鸟

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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        #endregion

        #region AI

        public override void AI()
        {
            if (init)
            {
                init = false;
                Initialize();
            }

            switch (State)
            {
                case AIStates.Waiting:
                    Waiting();
                    break;
                case AIStates.Shoot:
                    Attack();
                    break;
                default:
                    break;
            }

            GetMabird();
        }

        public void Initialize()
        {
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
        }

        public void Waiting()
        {
            if (Timer > 40)
            {
                NPC.TargetClosest(false);
                if (CanAttack())//玩家在8格以内开始发动攻击
                {
                    StartAttack();
                    return;
                }

                Timer = 0;
            }

            Timer++;
        }

        public void Attack()
        {
            if (Timer > 20)
            {
                Timer = 0;
                NPC.TargetClosest(false);
                if (CanAttack())
                {
                    if (MabirdCount > 0)//魔鸟出动！
                    {

                    }
                }
                else
                    State = AIStates.Waiting;
            }

            Timer++;
        }

        private bool CanAttack()
        {
            return NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead
                                && !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < 16 * 30
                                && Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);
        }

        /// <summary>
        /// 每隔固定时间生产一只魔鸟
        /// </summary>
        public void GetMabird()
        {
            if (Recorder > Helper.ScaleValueForDiffMode(60 * 4, 60 * 3, 60 * 2f, 30))
            {
                Recorder = 0;
                if (MabirdCount < 6)
                {
                    MabirdCount++;
                    NPC.netUpdate = true;
                }
            }

            Recorder++;
        }

        public void StartAttack()
        {
            Timer = 0;
            Recorder = 0;
            State= AIStates.Shoot;
        }

        #endregion

        #region 网络同步

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();
            Rectangle box = mainTex.Frame(1, 7, 0, (int)MabirdCount);
            spriteBatch.Draw(mainTex, NPC.Center - screenPos, box, drawColor, 0, box.Size() / 2, NPC.scale,
                NPC.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;
        }

        #endregion
    }

    public class WildMabird : ModNPC
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

        protected Player Target => Main.player[NPC.target];

        private enum AIStates
        {
            Idle,
            Fly,
            Attack,
        }

        #region 基础设置

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 54;
            NPC.damage = 50;
            NPC.defense = 45;

            NPC.lifeMax = 2000;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0.5f;
            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastHurt;
            NPC.DeathSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            NPC.noGravity = false;
            NPC.value = Item.buyPrice(0, 2);
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => true;

        #endregion

        #region AI

        public override void AI()
        {
            switch (State)
            {
                case AIStates.Idle:
                    break;
                case AIStates.Fly:
                    break;
                case AIStates.Attack:
                    break;
                default:
                    break;
            }
        }

        public void Idle()
        {

        }

        private bool CanAttack()
        {
            return NPC.target >= 0 && NPC.target < 255 && !Main.player[NPC.target].dead
                                && !(Target.invis && Target.itemAnimation == 0) && Vector2.Distance(NPC.Center, Target.Center) < 16 * 30
                                && Collision.CanHit(NPC.Center, 1, 1, Target.TopLeft, Target.width, Target.height);
        }

        private void FlyUpFrame()
        {

        }

        private void FlyDownFrame()
        {

        }

        #endregion

        #region 网络同步

        #endregion

        #region 绘制

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        #endregion
    }
}
