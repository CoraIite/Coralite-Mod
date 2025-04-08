using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Wisteria : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(14, 3);
            Item.DefaultToRangedWeapon(ProjectileType<FloetteHeldProj>(), AmmoID.None, 12, 12.5f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
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

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(12)
                .AddIngredient(ItemID.Amethyst)
                .AddTile(TileID.Anvils)
                .Register();
        }

    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class WisteriaHeldProj : BaseGunHeldProj
    {
        public WisteriaHeldProj() : base(0.2f, 6, -6, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex WisteriaFire { get; private set; }

        protected override float HeldPositionY => -2;

        private int FrameX;

        public override void InitializeGun()
        {
            FrameX = Main.rand.Next(4);
        }

        public override void ModifyAI(float factor)
        {
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 3 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = WisteriaFire.Value;
            Rectangle frameBox = effect.Frame(4, 4, FrameX, Projectile.frame);
            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 55 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.White
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale * 2, 0, 0f);
            return false;
        }
    }

    public class WisteriaPetal:ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems+Name;

    }
}
