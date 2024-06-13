using Coralite.Content.Tiles.RedJades;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class TopazMirror : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Cyan9, Item.sellPrice(0, 20));
            Item.SetWeaponValues(100, 4);
            Item.useTime = Item.useAnimation = 28;
            Item.mana = 20;

            Item.shoot = ModContent.ProjectileType<SapphireHairpinProj>();
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
                    (proj.ModProjectile as SapphireHairpinProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.4f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.7f);
                effect.Parameters["highlightC"].SetValue(TopazProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(TopazProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(TopazProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, TopazProj.brightC, TopazProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sapphire)
                .AddIngredient(ItemID.MarbleBlock, 30)
                .AddIngredient(ItemID.FallenStar)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class TopazProj
    {
        public static Color highlightC = new Color(255, 234, 186);
        public static Color brightC = new Color(255, 132, 0);
        public static Color darkC = new Color(136, 47, 0);

    }

}
