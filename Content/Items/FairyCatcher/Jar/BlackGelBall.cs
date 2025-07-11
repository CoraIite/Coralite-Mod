using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Jar
{
    public class BlackGelBall : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override int CatchPower => 3;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<BlackGelBallProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 14;
            Item.shootSpeed = 10;
            Item.SetWeaponValues(18, 3);
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 30));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gel, 8)
                .AddIngredient(ItemID.BlackInk)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    
    public class BlackGelBallProj: BaseJarProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + "BlackGelBall";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 22;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.t_Slime, Helper.NextVec2Dir(0.3f, 1f), 150, Color.DarkGray, Main.rand.NextFloat(1, 2f));
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(5,8))
                target.AddBuff(BuffID.Confused, 45);
        }
    }
}
