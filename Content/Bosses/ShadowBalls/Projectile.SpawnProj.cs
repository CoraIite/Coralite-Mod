using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.CameraSystem;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Bosses.ShadowBalls
{
    /// <summary>
    /// 向下飘向中心位置
    /// </summary>
    public class SpawnProj : CoraliteBossHostileProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ShadowSplit split;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];


        public static int SplitPointDistance = 48;
        public static int SplitExpandTime = 38;
        public static int ShadowBallAppearTime = 40;



        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void AI()
        {
            /*
             * 在老人或者别的地方中心出现裂隙
             * 首先裂隙出现，然后召唤影子球，之后从裂隙中出现一大堆影子弹幕，随机飞行后回归到影子球身边
             * 再之后锁环依次出现
             * 最后裂隙合并，影子球
             * 
             */
            switch (State)
            {
                default:
                case 0://裂隙出现
                    {

                        if (Timer<SplitExpandTime*3/2)
                        {
                            float f = Timer / (SplitExpandTime * 3 / 2);
                            split.SplitDistance = f;
                        }
                        else
                            split.SplitDistance = 1;

                        if (Timer> SplitExpandTime)
                        {
                            State = 1;
                            Timer = 0;
                            SpawnBoss();
                            return;
                        }
                    }
                    break;
                case 1://影子球出现
                    {
                        if (Timer> ShadowBallAppearTime)
                        {
                            State = 2;
                            Timer = 0;
                            return;
                        }
                    }
                    break;
                case 2:
                    {
                        //Projectile.Kill();
                    }
                    break;
            }

            Timer++;
        }

        public override void Initialize()
        {
            if (!VaultUtils.isServer)
            {
                split = new ShadowSplit(12, new Vector2(0, -1));
                split.InitSplit(Projectile.Center, SplitPointDistance, 16);

                Main.instance.CameraModifiers.Add(new FocusModifyer(Projectile.Center, "ShadowBallFocus", 30, 180));
            }
        }

        public void SpawnBoss()
        {
            if (VaultUtils.isServer)
            {
                int n = NPC.NewNPC(NPC.GetBossSpawnSource(Projectile.owner), (int)Projectile.Center.X, (int)Projectile.Center.Y + ContentSamples.NpcsByNetId[ModContent.NPCType<ShadowBall>()].height/2, ModContent.NPCType<ShadowBall>());

                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[n].GetTypeNetName()), new Color(175, 75, 255));
            }
            else if (VaultUtils.isSinglePlayer)
            {
                int n = NPC.NewNPC(NPC.GetBossSpawnSource(Projectile.owner), (int)Projectile.Center.X, (int)Projectile.Center.Y + ContentSamples.NpcsByNetId[ModContent.NPCType<ShadowBall>()].height / 2, ModContent.NPCType<ShadowBall>());

                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[n].TypeName), 175, 75);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            split?.DrawSplit(180, ShadowSplit.CommonSplitColor);
            return false;
        }
    }
}
