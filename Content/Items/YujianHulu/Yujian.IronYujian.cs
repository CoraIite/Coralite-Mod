using Coralite.Core;
using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.YujianHulu
{
    public class IronYujian : BaseYujian
    {
        public IronYujian() : base(ItemRarityID.White, Item.sellPrice(0, 0, 10, 0), 8, 1.3f) { }

        public override int ProjType => ModContent.ProjectileType<IronYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class IronYujianProj : BaseYujianProj
    {
        public override string SlashTexture => AssetDirectory.OtherProjectiles + "LiteSlash";

        public IronYujianProj() : base(
            new YujianAI[]
            {
                new YujianAI_BetterSpurt(70,22,25,130,0.93f),
            },
            null,
            new YujianAI_IronDoubleSlash(),
            PowerfulAttackCost: 150,
            attackLength: 340,
            width: 30, height: 58,
            new Color(42, 37, 41), new Color(169, 162, 135),
             trailCacheLength: 24
            )
        { }
    }

    public class YujianAI_IronDoubleSlash : YujianAI_DoubleSlash
    {
        public YujianAI_IronDoubleSlash() : base(90, 65, 40, -2f, 4f, 2, 1f, 1f, 1.5f, Coralite.Instance.NoSmootherInstance) { }

        public override void Reset()
        {
            StartTime = 130;

            SlashTime = 100;
            StartAngle = 2f;

            halfShortAxis = 1.5f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public override void Init()
        {
            StartTime = 90;

            SlashTime = 40;
            StartAngle = -2f;

            halfShortAxis = 1.5f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.NoSmootherInstance;
        }
    }
}
