using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Helpers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class ZumurudRing : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(0, 7));
            Item.SetWeaponValues(40, 4);
            Item.useTime = Item.useAnimation = 20;
            Item.mana = 8;

            Item.shoot = ModContent.ProjectileType<PinkDiamondRoseProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as PinkDiamondRoseProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.4f,1f ) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.45f);
                effect.Parameters["addC"].SetValue(0.35f);
                effect.Parameters["highlightC"].SetValue(ZumurudProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(ZumurudProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(ZumurudProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, ZumurudProj.brightC, ZumurudProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Zumurud>()
                .AddIngredient(ItemID.BorealWood, 12)
                .AddIngredient(ItemID.FlowerPacketPink)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class ZumurudRingProj : BaseGemWeaponProj<ZumurudRing>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "ZumurudRing";

        private Vector2 offset;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void Initialize()
        {
            TargetPos = Owner.Center;
        }

        public override void BeforeMove()
        {
        }

        public override void Move()
        {
        }

        public override void Attack()
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(ZumurudProj.darkC, 0.3f, 0.3f / 4, 0, 4, 1);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    public class ZumurudProj : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "Zumurud";

        public static Color highlightC = new Color(206, 248, 239);
        public static Color brightC = new Color(49, 230, 127);
        public static Color darkC = new Color(19, 112, 60);
    }
}
