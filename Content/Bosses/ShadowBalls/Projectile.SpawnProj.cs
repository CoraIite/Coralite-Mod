using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
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
        public ref float NPCIndex => ref Projectile.ai[2];


        public static int SplitPointDistance = 45;
        public static int SplitExpandTime = 40;
        public static int ShadowBallAppearTime = 40;

        public static int ShadowProjSpawnTime = 150;
        public static int LockAppearTime = 24 * 3+6;
        public static int RoarTime = 80;




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
                        if (Timer < SplitExpandTime /2)
                        {
                            float f = Timer / (SplitExpandTime /2);
                            split.SplitDistance =split.SplitWidthFactor = f;
                        }
                        else
                            split.SplitDistance = 1;

                        if (Timer > SplitExpandTime)
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
                case 2://生成影子弹幕
                    {
                        if (Timer > ShadowProjSpawnTime)
                        {
                            State = 3;
                            Timer = 0;
                            return;
                        }

                        if (Timer < ShadowProjSpawnTime*0.4f && Timer % 3 == 0)
                        {
                            Projectile.NewProjectileFromThis<ShadowProj>(Projectile.Center, Helper.NextVec2Dir() * 24, 0, 0, 0, NPCIndex);
                        }
                    }
                    break;
                case 3:
                    {
                        if (Timer < 30)
                        {
                            float f = Timer / 30;
                            split.SplitWidthFactor = 1 - f;
                        }
                        else
                            split.SplitWidthFactor = 0;

                        if (Timer > LockAppearTime)
                        {
                            State = 4;
                            Timer = 0;
                            return;
                        }
                    }
                    break;
                case 4:
                    {
                        Projectile.Kill();
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

                Main.instance.CameraModifiers.Add(new FocusModifyer(Projectile.Center, "ShadowBallFocus", 30, SplitExpandTime + ShadowBallAppearTime + ShadowProjSpawnTime + LockAppearTime + RoarTime - 30));
            }
        }

        public void SpawnBoss()
        {
            if (VaultUtils.isServer)
            {
                int n = NPC.NewNPC(NPC.GetBossSpawnSource(Projectile.owner), (int)Projectile.Center.X, (int)Projectile.Center.Y + ContentSamples.NpcsByNetId[ModContent.NPCType<ShadowBall>()].height / 2, ModContent.NPCType<ShadowBall>());

                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[n].GetTypeNetName()), new Color(175, 75, 255));

                NPCIndex = n;
                Projectile.netUpdate = true;
            }
            else if (VaultUtils.isSinglePlayer)
            {
                int n = NPC.NewNPC(NPC.GetBossSpawnSource(Projectile.owner), (int)Projectile.Center.X, (int)Projectile.Center.Y + ContentSamples.NpcsByNetId[ModContent.NPCType<ShadowBall>()].height / 2, ModContent.NPCType<ShadowBall>());

                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[n].TypeName), 175, 75);

                NPCIndex = n;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            split?.DrawSplit(250, ShadowSplit.CommonSplitColor);
            return false;
        }
    }
}
