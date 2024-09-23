//using Coralite.Content.Items.Botanical.Plants;
//using Coralite.Content.Items.Materials;
//using Coralite.Core;
//using Coralite.Core.Configs;
//using Terraria;
//using Terraria.Audio;
//using Terraria.DataStructures;
//using Terraria.ID;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Items.Misc_Melee
//{
//    public class WatermelonKnife : ModItem
//    {
//        public override string Texture => AssetDirectory.Misc_Melee + Name;

//        public override void SetStaticDefaults()
//        {
//            // DisplayName.SetDefault("");

//            // Tooltip.SetDefault("");
//        }

//        public override void SetDefaults()
//        {
//            Item.width = Item.height = 40;
//            Item.damage = 11;
//            Item.useTime = 15;
//            Item.useAnimation = 15;
//            Item.knockBack = 2;
//            Item.shootSpeed = 12;
//            Item.maxStack = 999;

//            Item.value = Item.sellPrice(0, 0, 0, 16);
//            Item.rare = ItemRarityID.White;
//            Item.useStyle = ItemUseStyleID.Swing;
//            Item.DamageType = DamageClass.Ranged;
//            Item.UseSound = SoundID.Item1;

//            Item.useTurn = false;
//            Item.noMelee = true;
//            Item.noUseGraphic = true;
//            Item.autoReuse = false;
//            Item.consumable = true;

//            Item.shoot = ProjectileType<WatermelonKnifeProj>();
//        }

//        public override void AddRecipes()
//        {
//            CreateRecipe(40)
//            .AddIngredient(ItemID.IronBar)
//            .AddIngredient(ItemID.Wood)
//            .AddTile(TileID.Anvils)
//            .Register();

//            CreateRecipe(60)
//            .AddIngredient(ItemID.IronBar)
//            .AddIngredient(ItemType<WoodStick>(), 3)
//            .AddTile(TileID.Anvils)
//            .Register();
//        }
//    }

//    public class WatermelonKnifeProj : ModProjectile
//    {
//        public override string Texture => AssetDirectory.Projectiles_Melee + Name;

//        public override void SetDefaults()
//        {
//            Projectile.width = Projectile.height = 8;
//            Projectile.aiStyle = -1;
//            Projectile.penetrate = 2;
//            Projectile.alpha = 0;
//            Projectile.timeLeft = 1800;

//            Projectile.hostile = false;
//            Projectile.friendly = true;
//            Projectile.tileCollide = true;
//            Projectile.ignoreWater = false;
//        }

//        public override void OnSpawn(IEntitySource source)
//        {
//            Projectile.localAI[0] = ItemType<Watermelon>();
//        }

//        public override void AI()
//        {
//            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
//            if (Projectile.timeLeft < 1780)
//            {
//                Projectile.velocity.Y += 0.45f;
//                if (Projectile.velocity.Y > 10)
//                    Projectile.velocity.Y = 10;

//                Projectile.rotation += Projectile.direction * 0.25f;
//            }
//            else
//                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction > 0 ? 0.785f : 2.355f);

//            for (int i = 0; i < 200; i++)//遍历前200个物品，如果是西瓜而且距离小了就把他切成西瓜片
//            {
//                if (Main.item[i].IsAir)
//                    continue;
//                if (Main.item[i].type != Projectile.localAI[0])
//                    continue;
//                if ((Projectile.Center - Main.item[i].Center).Length() < 16)
//                {
//                    if (Main.item[i].stack > 1)
//                        Main.item[i].stack--;
//                    else
//                        Main.item[i].TurnToAir();
//                    Item.NewItem(Projectile.GetSource_FromAI(), Projectile.Center, ItemType<WatermelonSlices>(), Main.rand.Next(3, 9));
//                }
//            }
//        }

//        public override void OnKill(int timeLeft)
//        {
//            if (VisualEffectSystem.HitEffect_Dusts)
//                for (int i = 0; i < 8; i++)
//                    Dust.NewDustPerfect(Projectile.Center, DustID.Iron, Main.rand.NextVector2Circular(2, 2), 0, default, 0.8f);

//            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
//        }
//    }
//}
