using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlowerGunChapter;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.KeySystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Floette : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlowerGunKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunCollect>();

        public override void SetDefaults()
        {
            Item.SetWeaponValues(24, 4);
            Item.DefaultToRangedWeapon(ProjectileType<FloetteHeldProj>(), AmmoID.Bullet, 18, 12.5f, true);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 1));

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Gun3_Item41;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -10);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<FloetteHeldProj>(), 0, knockback, player.whoAmI);
            if (type == ProjectileID.Bullet)
            {
                Projectile.NewProjectile(source, position, velocity
                    , ProjectileType<PosionedSeedPlantera>(), damage, knockback, player.whoAmI);

                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 15)
                .AddIngredient(ItemID.FlowerPacketPink)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 15)
                .AddIngredient(ItemID.FlowerPacketPink)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.HyacinthSeriesItems)]
    public class FloetteHeldProj : BaseGunHeldProj
    {
        public FloetteHeldProj() : base(0.3f, 14, -8, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex FloetteFire { get; private set; }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.6f, 0.1f));
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = FloetteFire.Value;
            Rectangle frameBox = effect.Frame(1, 4, 0, Projectile.frame);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 16 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }

    public class PosionedSeedPlantera : SeedPlantera
    {
        public override string Texture => "Terraria/Images/Projectile_276";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PoisonSeedPlantera);
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, Main.rand.Next(60 * 2, 60 * 4));
        }
    }
}
