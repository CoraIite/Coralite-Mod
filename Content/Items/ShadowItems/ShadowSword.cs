using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.ShadowItems
{
    public class ShadowSword : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        private float rotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("潜伏");

            Tooltip.SetDefault("尽管是把剑，但却充满魔力！");
        }

        public override void SetDefaults()
        {
            Item.height = Item.width = 40;

            Item.damage = 20;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.knockBack = 6f;
            Item.reuseDelay = 20;
            Item.mana = 9;
            Item.crit = 10;

            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item9;

            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noMelee = false;

            Item.shoot = ModContent.ProjectileType<ShadowSwordProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
            {
                Projectile.NewProjectile(source, player.Center + new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * 50f, new Vector2(0, -0.03f), type, damage, 6, player.whoAmI);
                rotation += 3;
            }

            rotation -= 5.5f;

            if (rotation > 8)
                rotation = 0;

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ShadowCrystal>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class ShadowSwordProj : ProjStateMachine
    {
        public override string Texture => AssetDirectory.ShadowProjectiles + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("潜伏分身");
        }

        public override void SetDefaults()
        {
            Projectile.height = 44;
            Projectile.width = 44;

            Projectile.friendly = true;
            Projectile.timeLeft = 900;
        }
        
        /// <summary>
        /// 状态1 原地闪烁45帧
        /// </summary>
        private class SpawnState : ProjState
        {
            public override void AI(ProjStateMachine proj)
            {
                Projectile projectile = proj.Projectile;
                projectile.rotation = projectile.velocity.ToRotation() + 0.785f;
                projectile.velocity = new Vector2(0, -0.03f);

                if (proj.Timer % 3 == 0 && Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.position + new Vector2(Main.rand.Next(projectile.width), Main.rand.Next(projectile.height)), DustID.Granite);
                    dust.noGravity = true;
                }

                proj.Timer++;
                float factor = proj.Timer / 45f;

                if (factor < 1)
                    projectile.alpha = 255 - (int)(factor * 255);
                else
                {
                    proj.Timer = 0;
                    projectile.alpha = 0;
                    proj.SetState<ShootState>();
                }
            }
        }

        /// <summary>
        /// 状态2 射向最近的敌人
        /// </summary>
        private class ShootState : ProjState
        {
            public override void AI(ProjStateMachine proj)
            {
                Projectile projectile = proj.Projectile;
                
                if (projectile.velocity.Length() < 1f) 
                    ProjectilesHelper.AimingTheNearestNPC(projectile, 10, 1000f);

                projectile.alpha = 60 - (int)(Math.Cos(proj.Timer * 0.0314f) * 60);

                proj.Timer++;
                projectile.rotation = projectile.velocity.ToRotation() + 0.785f;
                if (proj.Timer % 3 == 0 && Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.position + new Vector2(Main.rand.Next(projectile.width), Main.rand.Next(projectile.height)), DustID.Granite);
                    dust.noGravity = true;
                }


                if (proj.Timer >= 300)
                {
                    proj.Timer = 0;
                    proj.SetState<TrackingState>();
                }
            }
        }

        /// <summary>
        /// 状态3 追踪最近的敌人
        /// </summary>
        private class TrackingState : ProjState
        {
            public override void AI(ProjStateMachine proj)
            {
                Projectile projectile = proj.Projectile;
                //自转
                projectile.alpha = 60 - (int)(Math.Cos(proj.Timer * 0.0314f) * 60);
                proj.Timer++;
                float factor = proj.Timer / 550f;
                projectile.rotation += 0.18f * factor;

                if (proj.Timer % 3 == 0 && Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.position + new Vector2(Main.rand.Next(projectile.width), Main.rand.Next(projectile.height)), DustID.Granite);
                    dust.noGravity = true;
                }

                projectile.velocity = new Vector2(0, -0.5f * factor);

                ProjectilesHelper.AutomaticTracking(projectile, 0.1f, 10, 150f);
                if (factor >= 1)
                    Dust.NewDustPerfect(projectile.position + new Vector2(Main.rand.Next(projectile.width), Main.rand.Next(projectile.height)), DustID.Granite ,null,0,default,1.2f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Main.netMode != NetmodeID.Server)
                Dust.NewDustDirect(Projectile.Center, 32, 32, DustID.Granite, 0.2f, 0.2f, 0, default, 1.2f);

            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.netMode != NetmodeID.Server)
                Dust.NewDustDirect(Projectile.Center, 32, 32, DustID.Granite, 0.2f, 0.2f, 0, default, 1.2f);
        }

        public override void Initialize()
        {
            RegisterState(new SpawnState());
            RegisterState(new ShootState());
            RegisterState(new TrackingState());
        }
    }
}
