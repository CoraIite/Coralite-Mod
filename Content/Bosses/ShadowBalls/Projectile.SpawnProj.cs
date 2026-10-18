using Coralite.Core;
using Coralite.Core.Systems.BossSystem;
using Coralite.Core.Systems.CameraSystem;
using Terraria;

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


        public static int SplitPointDistance = 60;
        public static int SplitExpandTime = 26;
        public static int ShadowBallAppearTime = 40;



        public override void SetDefaults()
        {
            
        }

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
                case 0:
                    {
                        float f = Timer / SplitExpandTime;
                        split.SplitDistance = f;

                        if (Timer> SplitExpandTime)
                        {
                            State = 1;
                            Timer = 0;
                            return;
                        }
                    }
                    break;
            }

            Timer++;
        }

        public override void Initialize()
        {
            split = new ShadowSplit(13, new Vector2(0, -1));
            split.InitSplit(Projectile.Center, SplitPointDistance, 60);

            if (!VaultUtils.isServer)
            {
                Main.instance.CameraModifiers.Add(new FocusModifyer(Projectile.Center, "ShadowBallFocus", 30, 180));
            }
        }
    }
}
