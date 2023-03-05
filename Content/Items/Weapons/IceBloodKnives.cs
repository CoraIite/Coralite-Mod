//using Coralite.Core;
//using Coralite.Core.Prefabs.Projectiles;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace Coralite.Content.Items.Weapons
//{
//    public class IceBloodKnives : ModItem
//    {
//        public override string Texture => AssetDirectory.Weapons_Melee + Name;

//        public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("干将莫邪");
//        }

//        public override void SetDefaults()
//        {
//            Item.width = Item.height = 40;
//            Item.damage = 75;
//            Item.useTime = 15;
//            Item.useAnimation = 15;
//            Item.knockBack = 6;

//            Item.value = Item.sellPrice(0, 1, 0, 0);
//            Item.rare = ItemRarityID.Orange;
//            Item.useStyle = ItemUseStyleID.Rapier;
//            Item.DamageType = DamageClass.Melee;

//            Item.useTurn = false;
//            Item.noMelee = true;
//            Item.noUseGraphic = true;
//            Item.autoReuse = true;

//            //Item.shoot = ProjectileType<CatClawsProj_Slash>();
//        }
//    }

//    public class BloodKniefSwing : BaseSwingProj
//    {
//        public override string Texture => AssetDirectory.Projectiles_Melee + Name;

//        public override void SetDefs()
//        {
//            startAngle = 1.5f;
//            totalAngle = 3f;
//        }
//    }
//}
