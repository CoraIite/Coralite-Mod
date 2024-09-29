//using Coralite.Content.Items.Glistent;
//using Coralite.Core;
//using Coralite.Core.Systems.FlyingShieldSystem;
//using Coralite.Helpers;
//using Terraria;
//using Terraria.Audio;
//using Terraria.DataStructures;
//using Terraria.ID;

//namespace Coralite.Content.Items.ShieldPlus
//{
//    /*
//     * 左键甩葱，大葱碰到盾或者飞盾后变为卷饼重新射出
//     */
//    public class GreenOnionBurritos()
//        : BaseShieldPlusWeapon<GreenOnionBurritosGuard>(Item.sellPrice(0, 1), ItemRarityID.Orange, AssetDirectory.ShieldPlusItems)
//        , IFlyingShieldAccessory
//    {
//        public override int FSProjType => ModContent.ProjectileType<GreenOnionBurritosFSProj>();

//        public override void SetDefaults2()
//        {
//            base.SetDefaults2();
//            Item.useStyle = ItemUseStyleID.Rapier;
//            Item.useTime = Item.useAnimation = 15;
//            Item.shoot = ModContent.ProjectileType<TerranascenceSwing>();
//            Item.shootSpeed = 12f;
//            Item.knockBack = 3f;
//            Item.damage = 28;

//            Item.UseSound = null;
//            Item.useTurn = false;
//            Item.autoReuse = true;
//        }

//        public override void HoldItem(Player player)
//        {
//            if (player.ownedProjectileCounts[ModContent.ProjectileType<TerranascenceTag>()] < 1)
//            {
//                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<TerranascenceTag>()
//                    , 0, 0, player.whoAmI);
//            }
//        }

//        #region 攻击相关

//        public override void LeftAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
//        {
//            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
//        }

//        public override void ShootFlyingShield(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
//        {
//            damage = (int)(damage * 0.75f);
//            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, player.Center);
//            base.ShootFlyingShield(player, source, velocity, type, damage, knockback);
//        }

//        public override void RightShoot(Player player, IEntitySource source, int damage)
//        {
//            damage = (int)(damage * 0.7f);
//            base.RightShoot(player, source, damage);
//        }

//        #endregion

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//                .AddRecipeGroup(RecipeGroupID.Wood, 35)
//                .AddIngredient<GlistentBar>(20)
//                .AddTile(TileID.LivingLoom)
//                .Register();
//        }
//    }

//    public class GreenOnionBurritosFSProj : BaseFlyingShield
//    {
//        public override string Texture => AssetDirectory.ShieldPlusItems + "Pancake";

//        public bool hited;

//        public override void SetDefaults()
//        {
//            base.SetDefaults();
//            Projectile.width = Projectile.height = 50;
//        }

//        public override void SetOtherValues()
//        {
//            flyingTime = 25 * 2;
//            backTime = 14 * 2;
//            backSpeed = 10;
//            trailCachesLength = 9;
//            trailWidth = 25 / 2;
//        }

//        public override void OnShootDusts()
//        {
//            extraRotation += 0.4f;
//            SpawnDusts();
//        }

//        public override void OnBackDusts()
//        {
//            extraRotation += 0.4f;
//            SpawnDusts();
//        }

//        public void SpawnDusts()
//        {
//            if (Timer % 4 == 0)
//                for (int i = 0; i < 2; i++)
//                {
//                    Projectile.SpawnTrailDust((float)(Projectile.width / 2), DustID.AncientLight, Main.rand.NextFloat(0.2f, 0.5f),
//                        newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.4f));
//                }
//        }

//        public override Color GetColor(float factor)
//        {
//            return new Color() * factor;
//        }
//    }

//    public class GreenOnionBurritosGuard : BaseFlyingShieldPlusGuard
//    {
//        public override string Texture => AssetDirectory.ShieldPlusItems + "Pancake";

//        public override void SetDefaults()
//        {
//            base.SetDefaults();
//            Projectile.width = 42;
//            Projectile.height = 42;
//        }

//        public override void SetOtherValues()
//        {
//            scalePercent = 2f;
//            distanceAdder = 2;
//            delayTime = 10;
//            parryTime = 6;
//        }

//        public override void LimitFields()
//        {
//            base.LimitFields();
//        }

//        public override void OnGuard()
//        {
//            DistanceToOwner /= 2;
//            Helper.PlayPitched("TheLegendOfZelda/Guard_Wood_Wood_" + Main.rand.Next(4), 0.5f, 0.2f, Projectile.Center);
//            SpawnWoodDust();
//        }

//        public void SpawnWoodDust()
//        {
//            for (int i = 0; i < 8; i++)
//            {
//                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), DustID.WoodFurniture,
//                    (Projectile.rotation + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(2, 6)
//                    , Scale: 1);

//                d.noGravity = true;
//            }
//        }

//        public override float GetWidth()
//        {
//            return Projectile.width * 0.5f / Projectile.scale;
//        }
//    }

//    public class SpringOnion : ModProjectile
//    {
//        public override string Texture => AssetDirectory.ShieldPlusItems + Name;

//        public ref float State => ref Projectile.ai[0];

//        public override void Load()
//        {
//        }

//        public override void Unload()
//        {
//        }

//        public override void SetDefaults()
//        {
//        }

//        public override void AI()
//        {
//            switch (State)
//            {
//                default:
//                case 0://常规大葱飞出
//                    break;
//                case 1://常规大葱飞回来
//                    break;
//                case 2://大葱卷饼飞出
//                    break;
//                case 3://大葱卷饼飞回来
//                    break;
//            }
//        }

//        public override bool PreDraw(ref Color lightColor)
//        {
//            return false;
//        }
//    }
//}
