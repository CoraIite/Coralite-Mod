using Coralite.Content.Items.HyacinthSeries;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Donator
{
    public class SurviveAndPerish:ModItem
    {
        public override string Texture => AssetDirectory.Donator+Name;

        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.knockBack = 6;
            Item.shootSpeed = 14;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 2);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<SurviveAndPerishHeldProj>();
            Item.useAmmo = ItemID.WoodenArrow;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override void HoldItem(Player player)
        {
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center
                , Vector2.Zero, ModContent.ProjectileType<SurviveAndPerishHeldProj>(), 0, knockback, player.whoAmI);


            return false;
        }
    }

    public class SurviveAndPerishHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public SurviveAndPerishHeldProj() : base(0.4f, 8, -6, AssetDirectory.Donator) { }

        public override void Initialize()
        {
            int time = Owner.itemTimeMax;
            if (time < 6)
                time = 6;

            Projectile.timeLeft = time;
            MaxTime = time;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (OwnerDirection > 0 ? 0f : MathHelper.Pi);
            }

            Projectile.netUpdate = true;
        }

        public override void AfterAI(float factor)
        {
            Projectile.frame = (int)((1 - Projectile.timeLeft / MaxTime)*6);
            base.AfterAI(factor);
        }

        public override void GetFrame(Texture2D mainTex, out Rectangle? frame, out Vector2 origin)
        {
            frame = mainTex.Frame(1, 6, 0, Projectile.frame);
            origin = frame.Value.Size() / 2;
            origin.X -= OwnerDirection * frame.Value.Width / 6;
        }
    }

    public class MiniDynamite
    {

    }
}
