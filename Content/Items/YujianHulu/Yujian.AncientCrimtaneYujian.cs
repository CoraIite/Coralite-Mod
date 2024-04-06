using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class AncientCrimtaneYujian : BaseYujian
    {
        public AncientCrimtaneYujian() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 20, 0), 9, 1f) { }

        public override int ProjType => ModContent.ProjectileType<AncientCrimtaneYujianProj>();
    }

    public class AncientCrimtaneYujianProj : BaseYujianProj
    {
        public AncientCrimtaneYujianProj() : base(
        new YujianAI[]
        {
             new YujianAI_CrimtaneDoubleSlash(),
             new YujianAI_PreciseSlash(startTime: 65,
                 slashWidth: 60,
                 slashTime: 55,
                 startAngle: -2.4f,
                 totalAngle: 4f,
                 turnSpeed: 1.5f,
                 roughlyVelocity: 0.7f,
                 halfShortAxis: 1f,
                 halfLongAxis: 1.5f,
                 Coralite.Instance.HeavySmootherInstance),
             new YujianAI_Slash(startTime: 60,
                 slashWidth: 60,
                 slashTime: 40,
                 startAngle: -1.4f,
                 totalAngle: 1.7f,
                 turnSpeed: 1.5f,
                 roughlyVelocity: 0.7f,
                 halfShortAxis: 1f,
                 halfLongAxis: 3f,
                 Coralite.Instance.NoSmootherInstance),
        },
        new YujianAI_CrimtaneHeavySash(),
        PowerfulAttackCost: 150,
        attackLength: 430,
        width: 30, height: 60,
          new Color(50, 23, 29), new Color(103, 41, 49),
        trailCacheLength: 12,
        yujianAIsRandom: new int[]
        {
            3,
            3,
            2
        }
        )
        { }

        public override void HitEffect(NPC target, int damage, float knockback, bool crit)
        {
            //TODO: 1.4.4之后新增给NPC加血腥屠杀的DEBUFF
            if (State == PowerfulMoveState)
            {

            }
        }

    }

}
