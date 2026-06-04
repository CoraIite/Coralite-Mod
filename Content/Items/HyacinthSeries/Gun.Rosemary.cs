using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlowerGunChapter;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Rosemary : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlowerGunKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunCollect>();

        public int timer;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 1.7f;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(25, 1);
            Item.DefaultToRangedWeapon(ProjectileType<RosemaryBullet>(), AmmoID.Bullet, 6, 10f, true);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightPurple6, Item.sellPrice(0, 6));

            Item.useAnimation = 17;
            Item.reuseDelay = 12;
            Item.crit = 12;

            Item.useStyle = ItemUseStyleID.Rapier;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override void HoldItem(Player player)
        {
            if (timer>0)
                timer--;
        }

        public override void UseAnimation(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                int extraProjType = ProjectileType<RosemaryFog>();
                foreach (var projectile in Main.projectile.Where(p => p.active && p.friendly && p.type == extraProjType && p.owner == player.whoAmI))
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = (Main.MouseWorld - projectile.Center).ToRotation();
                    projectile.netUpdate = true;
                }

                SoundEngine.PlaySound(CoraliteSoundID.TripleGun_Item31, player.Center);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center + new Vector2(0, -8), (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 14, ProjectileType<RosemaryBullet>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<RosemaryHeldProj>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Arethusa>()
            .AddIngredient(ItemID.ClockworkAssaultRifle)
            .AddIngredient(ItemID.SoulofSight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();

            CreateRecipe()
            .AddIngredient<Arethusa>()
            .AddIngredient(ItemID.ClockworkAssaultRifle)
            .AddIngredient(ItemID.SoulofMight, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();

            CreateRecipe()
            .AddIngredient<Arethusa>()
            .AddIngredient(ItemID.ClockworkAssaultRifle)
            .AddIngredient(ItemID.SoulofFright, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}