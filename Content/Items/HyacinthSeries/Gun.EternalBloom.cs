using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlowerGunChapter;
using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class EternalBloom : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlowerGunKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunCollect>();

        public override void SetDefaults()
        {
            Item.SetWeaponValues(64, 5.5f);
            Item.DefaultToRangedWeapon(ProjectileType<EternalBloomHeldProj>(), AmmoID.Bullet, 11, 14f, true);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Lime7, Item.sellPrice(0, 7));

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Gun3_Item41;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -8);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<EternalBloomHeldProj>(), 0, knockback, player.whoAmI);
            if (type == ProjectileID.Bullet)
            {
                type = Main.rand.NextBool(4) ? ProjectileType<ThornBall>() : ProjectileType<SeedPlantera>();

                Projectile.NewProjectile(source, position, velocity
                     , type, damage, knockback, player.whoAmI);
                return false;
            }
            else if (Main.rand.NextBool(4))
            {
                Projectile.NewProjectile(source, position, velocity
                     , ProjectileType<ThornBall>(), damage, knockback, player.whoAmI);
                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Floette>()
            .AddIngredient<RegrowthTentacle>(5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}
