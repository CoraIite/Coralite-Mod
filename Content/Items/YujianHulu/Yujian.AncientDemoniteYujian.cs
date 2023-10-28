using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class AncientDemoniteYujian : BaseYujian
    {
        public AncientDemoniteYujian() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 20, 0), 9, 1f) { }

        public override int ProjType => ModContent.ProjectileType<AncientDemoniteYujianProj>();
    }

    public class AncientDemoniteYujianProj : BaseYujianProj
    {
        public AncientDemoniteYujianProj() : base(
        new YujianAI[]
        {
             new YujianAI_DemoniteDoubleSlash(),
             new YujianAI_PreciseSlash(startTime: 70,
                 slashWidth: 55,
                 slashTime: 65,
                 startAngle: -2.4f,
                 totalAngle: 4f,
                 turnSpeed: 1.5f,
                 roughlyVelocity: 0.8f,
                 halfShortAxis: 1f,
                 halfLongAxis: 1.5f,
                 Coralite.Instance.HeavySmootherInstance),
                new YujianAI_BetterSpurt(75,20,35,180,0.95f),
        },
        null,
        new YujianAI_DemoniteSpurt(50, 12, 25, 180, 0.92f),
        PowerfulAttackCost: 150,
        attackLength: 400,
        width: 30, height: 60,
          new Color(35, 24, 24), new Color(131, 112, 86),
        trailCacheLength: 16
        )
        { }
    }

}
