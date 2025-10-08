using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class SmallFirecracker : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override int CatchPower => 8;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<SmallFirecrackerProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 17;
            Item.shootSpeed = 11;
            Item.SetWeaponValues(17, 4);
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 1));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(10)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class SmallFirecrackerProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "SmallFirecracker";

        public override void InitFields()
        {
            MaxChannelTime = 50;
            MaxFlyTime = 12;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 32;
        }

        public override void Load()
        {
            for (int i = 0; i < 2; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, Texture + "Proj_Gore" + i);
        }

        public override void OnHeld()
        {
            Vector2 dir = (Projectile.rotation - Owner.direction * MathHelper.PiOver2).ToRotationVector2();
            Vector2 pos = Projectile.Center + dir * 12;

            Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(4, 4), DustID.Torch
                , dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.8f, 1.5f));
            d.noGravity = Main.rand.NextBool(3, 4);
        }

        public override void SpawnDustOnFlying(bool outofTime)
        {
            Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            Vector2 pos = Projectile.Center + dir * 12;

            Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(4, 4), DustID.Torch
                , dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.8f, 1.5f));
            d.noGravity = Main.rand.NextBool(2);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.Smoke, Helper.NextVec2Dir(1f, 2f), 20, Color.Black, Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;

                d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.Torch, Helper.NextVec2Dir(1f, 2.5f), 150, Color.DarkGray, Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = Main.rand.NextBool();
            }

            for (int i = 0; i < 2; i++)
                Gore.NewGoreDirect(Projectile.GetSource_Death()
                    , Projectile.Center + new Vector2(i > 0 ? 4 : -4, 0)
                    , new Vector2(i > 0 ? 2 : -2, 0), Mod.Find<ModGore>(Name + "_Gore" + i).Type);

            Helper.PlayPitched(CoraliteSoundID.Boom_Item14, Projectile.Center, pitchAdjust: -0.2f);

            if (Catch == 0 && FullCharge)//完全蓄力后生成小炸弹
            {
                Projectile.NewProjectileFromThis<TinyFirecrackerProj>(Projectile.Center, new Vector2(Projectile.direction * 0.25f, -6),
                    Projectile.damage / 2, Projectile.knockBack);
            }
        }
    }

    public class TinyFirecrackerProj : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
        {
            if (Timer > 5)
                return base.CanDamage();

            return false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 6)
                Projectile.tileCollide = true;

            Projectile.rotation += Math.Sign(Projectile.velocity.X) * Math.Abs(Projectile.velocity.Y) / 13;

            if (Projectile.velocity.Y < 10)
                Projectile.velocity.Y += 0.15f;

            Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            Vector2 pos = Projectile.Center + dir * 8;

            Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(2, 2), DustID.Torch
                , dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(0.8f, 1.5f));
            d.noGravity = Main.rand.NextBool(2);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileFromThis<RedJadeBigBoom>(Projectile.Center, Vector2.Zero,
                 Projectile.damage, Projectile.knockBack);
        }
    }
}
