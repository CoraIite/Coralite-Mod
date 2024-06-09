using Coralite.Content.Tiles.RedJades;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PeridotTalisman : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightPurple6, Item.sellPrice(0, 9));
            Item.SetWeaponValues(64, 4);
            Item.useTime = Item.useAnimation = 40;
            Item.mana = 16;

            Item.shoot = ModContent.ProjectileType<RubyScepterProj>();
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
                    (proj.ModProjectile as RubyScepterProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.6f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.25f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(PeridotProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(PeridotProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(PeridotProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, PeridotProj.brightC, PeridotProj.darkC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ruby)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.AvengerEmblem)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class PeridotProj
    {
        public static Color highlightC = new Color(140, 238, 255);
        public static Color brightC = new Color(181, 243, 0);
        public static Color darkC = new Color(70, 126, 0);
    }
}
