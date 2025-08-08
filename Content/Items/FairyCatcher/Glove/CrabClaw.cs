using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Glove
{
    public class CrabClaw : BaseGloveItem
    {
        public override int CatchPower => 3;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<CrabClawProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.SetWeaponValues(18, 3);
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 30));
        }

        //public override void AddRecipes()
        //{
        //    CreateRecipe()
        //        .AddIngredient(ItemID.Gel, 8)
        //        .AddIngredient(ItemID.BlackInk)
        //        .AddTile(TileID.WorkBenches)
        //        .Register();
        //}
    }

    public class CrabClawProj() : BaseGloveProj(1.4f)
    {
        public override string Texture => AssetDirectory.FairyCatcherGlove + "CrabClaw";
        public override void SetOtherDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 58;
            DistanceController = (-30, 15);
            OffsetAngle = 0.5f;
            MaxTime = 25;
        }
    }
}
