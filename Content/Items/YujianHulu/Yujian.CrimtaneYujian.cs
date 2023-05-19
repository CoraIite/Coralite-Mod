using Coralite.Core.Systems.YujianSystem;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.YujianHulu
{
    public class CrimtaneYujian : BaseYujian
    {
        public CrimtaneYujian() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 20, 0), 13, 1f) { }

        public override int ProjType => ModContent.ProjectileType<CrimtaneYujianProj>();

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar,12)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrimtaneYujianProj: BaseYujianProj
    {
        public CrimtaneYujianProj() : base(
        new YujianAI[]
        {
             new YujianAI_CrimtaneDoubleSlash(),
             new YujianAI_PreciseSlash(startTime: 55,
                 slashWidth: 60,
                 slashTime: 50,
                 startAngle: -2.4f,
                 totalAngle: 4f,
                 turnSpeed: 1.5f,
                 roughlyVelocity: 0.8f,
                 halfShortAxis: 1f,
                 halfLongAxis: 1.5f,
                 Coralite.Instance.HeavySmootherInstance),
                new YujianAI_Slash(startTime: 56,
                    slashWidth: 60,
                    slashTime: 36,
                    startAngle: -1.4f,
                    totalAngle: 1.7f,
                    turnSpeed: 1.5f,
                    roughlyVelocity: 0.8f,
                    halfShortAxis: 1f,
                    halfLongAxis: 3f,
                    Coralite.Instance.NoSmootherInstance),
        },
        null,
        new YujianAI_CrimtaneHeavySash(),
        PowerfulAttackCost: 150,
        attackLength: 430,
        width: 30, height: 60,
          new Color(50, 23, 29), new Color(103, 41, 49),
        trailCacheLength: 12
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

    public class YujianAI_CrimtaneDoubleSlash : YujianAI_DoubleSlash
    {
        public YujianAI_CrimtaneDoubleSlash() : base(90, 65, 40, -2f, 4f, 1.5f, 0.8f, 1f, 1f, Coralite.Instance.NoSmootherInstance) { }

        public override void Reset()
        {
            StartTime = 90;

            SlashTime = 70;
            StartAngle = 2f;



            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public override void Init()
        {
            StartTime = 85;

            SlashTime = 35;
            StartAngle = -2f;



            smoother = Coralite.Instance.NoSmootherInstance;
        }

        public override SpriteEffects GetSpriteEffect(BaseYujianProj yujianProj)
        {
            if (doubleSlash)
                return SpriteEffects.None;

            return SpriteEffects.FlipHorizontally;
        }
    }

    public class YujianAI_CrimtaneHeavySash : YujianAI_DoubleSlash
    {
        public YujianAI_CrimtaneHeavySash() : base(90, 70, 70, -2.4f, 4f, 1.5f, 0.8f, 1f, 1f, Coralite.Instance.NoSmootherInstance) { }

        public override void Reset()
        {
            StartTime = 90;

            SlashTime = 70;
            StartAngle = 2f;

            halfShortAxis = 1f;
            halfLongAxis = 1f;
            smoother = Coralite.Instance.HeavySmootherInstance;
        }

        public override void Init()
        {
            StartTime = 85;

            SlashTime = 35;
            StartAngle = -2f;

            halfShortAxis = 1f;
            halfLongAxis = 2f;
            smoother = Coralite.Instance.NoSmootherInstance;
        }

        public override void Slash(Projectile Projectile, int time)
        {
            base.Slash(Projectile, time);
            int index = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
            Main.dust[index].noGravity = true;
        }

        public override SpriteEffects GetSpriteEffect(BaseYujianProj yujianProj)
        {
            if (doubleSlash)
                return SpriteEffects.None;

            return SpriteEffects.FlipHorizontally;
        }

    }
}
