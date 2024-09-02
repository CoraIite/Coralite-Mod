using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class SilverYujian : BaseYujian
    {
        public SilverYujian() : base(ItemRarityID.White, Item.sellPrice(0, 0, 15, 0), 9, 1.3f) { }

        public override int ProjType => ModContent.ProjectileType<SilverYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SilverBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SilverYujianProj : BaseYujianProj
    {
        public override string SlashTexture => AssetDirectory.Trails + "LiteSlash";

        public SilverYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(70,22,25,130,0.93f),
            },
            new SilverYujianAI_DoubleSlash(),
            PowerfulAttackCost: 150,
            attackLength: 360,
            width: 30, height: 58,
             new Color(40, 40, 40), new Color(150, 150, 150),
             trailCacheLength: 24
            )
        { }
    }

    public class SilverYujianAI_DoubleSlash : YujianAI_DoubleSlash
    {
        public SilverYujianAI_DoubleSlash() : base(90, 70, 34, -2.5f, 4f, 2, 1f, 1f, 1.5f, Coralite.Instance.NoSmootherInstance) { }

        public override void Reset()
        {
            StartTime = 130;

            SlashTime = 100;
            StartAngle = 2.5f;

            halfShortAxis = 1.8f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public override void Init()
        {
            StartTime = 90;

            SlashTime = 40;
            StartAngle = -2.5f;

            halfShortAxis = 1.3f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.NoSmootherInstance;
        }
    }

}
