using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Glove
{
    public class EyeInHand : BaseGloveItem
    {
        public override int CatchPower => 5;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<EyeInHandProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useLimitPerAnimation = 2;
            Item.useTime = 12;
            Item.useAnimation = 24;
            Item.reuseDelay = 5;
            Item.shootSpeed = 8;
            Item.SetWeaponValues(16, 3);
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 30));
            Item.UseSound = CoraliteSoundID.Swing_Item1;
        }

        public override void ShootGlove(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int @catch)
        {
            Projectile.NewProjectile(source, position, velocity, type
                , damage, knockback, player.whoAmI, @catch, UseCount % 2 > 0 ? 1 : -1);
        }
    }

    public class EyeInHandProj() : BaseGloveProj(1f)
    {
        public override string Texture => AssetDirectory.FairyCatcherGlove + "EyeInHand";

        public ref float DirectionControl => ref Projectile.ai[1];

        public override void SetOtherDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 64;
            DistanceController = (-35, 35);
            OffsetAngle = 0.8f;
            MaxTime = 16;
            smoother = Coralite.Instance.SqrtSmoother;
        }

        public override void PostInitialize()
        {
            Direction = (int)DirectionControl;
            BaseAngleOffset = -Owner.direction * Direction * 0.3f;
        }
    }
}
