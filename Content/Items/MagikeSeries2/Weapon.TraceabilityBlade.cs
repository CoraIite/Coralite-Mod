using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.DamageClasses;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class TraceabilityBlade : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page4>();

        public override void SetDefaults()
        {
            Item.SetWeaponValues(50, 5, 4);
            Item.DamageType = MagikeDamage.Instance;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Swing2_Item7;

            Item.useTurn = false;
            Item.noUseGraphic = true;

            Item.GetMagikeItem().MagikeMax = 7500;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //int sp = 0;
            //int count = 2;
            //if (MagikeHelper.TryCosumeMagike(15, Item, player))
            //{
            //    sp = 1;

            //    type = ModContent.ProjectileType<CrystallineSentinelBullet2>();
            //    damage = (int)(damage * 1.35f);
            //    count = 4;

            //    Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, position, pitch: 0.5f);
            //}

            //Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CrystallineTriggerScatterHeldProj>(), 0, knockback, player.whoAmI, ai2: sp);

            //for (int i = 0; i < count; i++)
            //{
            //    Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.1f, 0.1f) * Main.rand.NextFloat(0.8f, 1.15f), type, damage, knockback, player.whoAmI);
            //}
            //Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineEngram>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }
    }
}
