using Coralite.Content.Items.Icicle;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class AmethystNeckLace : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.SetWeaponValues(20, 4);
            Item.useTime = Item.useAnimation = 24;
            Item.mana = 10;

            Item.shoot = ModContent.ProjectileType<PyropeCrownProj>();
            Item.shootSpeed = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient(ItemID.ShadowScale,5)
                .AddIngredient(ItemID.GoldCoin)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient(ItemID.TissueSample,5)
                .AddIngredient(ItemID.GoldCoin)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Amethyst)
                .AddIngredient<IcicleScale>(3)
                .AddIngredient(ItemID.GoldCoin)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class AmethystNeckLaceProj:BaseHeldProj,IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems+Name;

        public ref float Index => ref Projectile.ai[0];
        public ref float AttackTime => ref Projectile.ai[1];
        public ref float AttackCD => ref Projectile.ai[2];

        public ref float LineIndex => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6000;
        }

        public override void AI()
        {
            if (Owner.HeldItem.type == ModContent.ItemType<PyropeCrown>())
                Projectile.timeLeft = 2;

            Move();
            Attack();
        }

        public void Move()
        {

        }

        public void Attack()
        {
            if (AttackTime > 0)
            {
                AttackTime--;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制连线

            return false;
        }

        public void DrawLine()
        {

        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Projectile.QuickDraw(Color.White, 0);
        }
    }

    public class AmethystLaser
    {

    }
}
