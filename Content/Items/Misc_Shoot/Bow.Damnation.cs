using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Content.Items.Misc_Shoot
{
    //public class Damnation : ModItem
    //{
    //    public override string Texture => AssetDirectory.Misc_Shoot + Name;

    //    public override void SetDefaults()
    //    {
    //        Item.useAnimation = Item.useTime = 14;
    //        Item.useStyle = ItemUseStyleID.Rapier;
    //        Item.shoot = ProjectileID.PurificationPowder;
    //        Item.useAmmo = AmmoID.Arrow;
    //        Item.DamageType = DamageClass.Ranged;
    //        Item.rare = RarityType<NightmareRarity>();
    //        Item.value = Item.sellPrice(0, 30, 0, 0);
    //        Item.SetWeaponValues(80, 3, 0);
    //        Item.autoReuse = true;
    //        Item.noUseGraphic = true;
    //        Item.noMelee = true;
    //        Item.useTurn = false;
    //        Item.shootSpeed = 22;
    //    }

    //    public override bool AltFunctionUse(Player player) => false;

    //    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    //    {
    //        if (Main.myPlayer == player.whoAmI)
    //        {
    //            Vector2 pos = Main.MouseWorld + new Vector2(Main.rand.Next(-150, 150), -1200);
    //            velocity = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);

    //            Projectile.NewProjectile(source, player.Center,
    //                Vector2.Zero, ModContent.ProjectileType<DamnationHeldProj>(), 1, knockback, player.whoAmI);

    //            for (int i = 0; i < 3; i++)
    //                Projectile.NewProjectile(source, pos + Main.rand.NextVector2Circular(32, 32),
    //                    velocity.RotatedBy(Main.rand.NextFloat(-0.08f, 0.08f)) * Main.rand.NextFloat(7f, 9f),
    //                    type, damage, knockback, player.whoAmI, 1);
    //        }
    //        return false;
    //    }
    //}

    //public class DamnationHeldProj : BaseHeldProj
    //{
    //    public override string Texture => AssetDirectory.Misc_Shoot + "Damnation";

    //    public override void SetDefaults()
    //    {
    //        Projectile.width = Projectile.height = 48;
    //        Projectile.friendly = true;
    //        Projectile.tileCollide = false;
    //        Projectile.ignoreWater = true;
    //        Projectile.aiStyle = -1;
    //        Projectile.penetrate = -1;
    //    }

    //    public override bool? CanDamage() => false;

    //    public override void AI()
    //    {
    //        Owner.heldProj = Projectile.whoAmI;

    //        Projectile.Center = Owner.Center + (Projectile.rotation.ToRotationVector2() * 16);
    //        Projectile.rotation = -1.57f + Math.Clamp((Main.MouseWorld.X - Owner.Center.X) / 400f * 0.3f, -0.4f, 0.4f);
    //    }

    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        Texture2D mainTex = Projectile.GetTexture();
    //        Vector2 center = Projectile.Center - Main.screenPosition;

    //        Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1.1f, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

    //        return false;
    //    }
    //}
}
