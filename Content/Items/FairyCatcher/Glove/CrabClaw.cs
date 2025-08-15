using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Glove
{
    [AutoloadEquip(EquipType.HandsOn)]
    [PlayerEffect]
    public class CrabClaw : BaseGloveItem, IEquipHeldItem
    {
        public override int CatchPower => 3;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<CrabClawProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 15;
            Item.shootSpeed = 8;
            Item.SetWeaponValues(15, 3);
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 30));
            Item.accessory = true;
            Item.UseSound = CoraliteSoundID.Swing_Item1;
        }

        public void UpdateEquipHeldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(CrabClaw));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Player.tileRangeX += 2;
                Player.tileRangeY += 1;
            }
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

    public class CrabClawProj() : BaseGloveProj(1.2f)
    {
        public override string Texture => AssetDirectory.FairyCatcherGlove + "CrabClaw";
        public override void SetOtherDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 58;
            DistanceController = (-25, 8);
            OffsetAngle = 0.4f;
            MaxTime = 20;
        }
    }
}
