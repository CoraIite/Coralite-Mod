using Coralite.Content.Items.Materials;
using Coralite.Content.Tiles.RedJades;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class SapphireHairpin : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightPurple6, Item.sellPrice(0, 9));
            Item.SetWeaponValues(80, 4);
            Item.useTime = Item.useAnimation = 28;
            Item.mana = 20;

            Item.shoot = ModContent.ProjectileType<PeridotTalismanProj>();
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
                    (proj.ModProjectile as PeridotTalismanProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.8f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.7f);
                effect.Parameters["highlightC"].SetValue(SapphireProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(new Color(80,150,255).ToVector4());
                effect.Parameters["darkC"].SetValue(new Color(0, 0, 255).ToVector4());
            }, 0.2f);
        }
        
        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, PeridotProj.brightC, PeridotProj.darkC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Peridot>()
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddIngredient<RegrowthTentacle>()
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class SapphireProj
    {
        public static Color highlightC = Color.White;
        public static Color brightC = new Color(54, 190, 254);
        public static Color darkC = new Color(0, 0, 121);

    }
}
