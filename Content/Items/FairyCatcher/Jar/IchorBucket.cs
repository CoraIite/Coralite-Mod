using Coralite.Content.DamageClasses;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Jar
{
    public class IchorBucket : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override int CatchPower => 40;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<IchorBucketProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 14;
            Item.SetWeaponValues(30, 3);
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 2));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EmptyBucket)
                .AddIngredient(ItemID.Ichor,12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoLoadTexture(Path =AssetDirectory.FairyCatcherJar)]
    public class IchorBucketProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + "IchorBucket";

        [AutoLoadTexture(Name = "IchorBucket_Highlight")]
        public static ATex HighlightTex { get;private set; }   

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void InitFields()
        {
            MaxFlyTime = 20;
        }

        public override void FlyingRotation()
        {
            Projectile.rotation +=
                            Math.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 120
                            + Projectile.velocity.X / 75;
        }

        public override void SpawnDustOnFlying(bool outofTime)
        {
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.Ichor
                , dir * Main.rand.NextFloat(0.5f, 2f));
            d.noGravity = Main.rand.NextBool(4, 5);

            if (Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle<AnimeFogDark>(Projectile.Center
                    , Helper.NextVec2Dir(0.5f, 1.5f)
                    , Main.rand.NextFromList(Color.Gold, Color.DarkGoldenrod) * 0.1f, Main.rand.NextFloat(0.2f, 0.5f));

            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            //各种粒子
            for (int i = 0; i < 24; i++)
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.Ichor, Helper.NextVec2Dir(0.75f, 3f), 50, Color.DarkGray, Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = Main.rand.NextBool(2,3);
            }

            Helper.PlayPitched(CoraliteSoundID.IDontKnow_ShimmerWeak2
                , Projectile.Center, pitchAdjust: -0.25f);
            Helper.PlayPitched(CoraliteSoundID.BloodyDeath3_NPCDeath19
                , Projectile.Center, pitchAdjust: -0.25f);

            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 5; i++)
            {
                PRTLoader.NewParticle<AnimeFogDark>(Projectile.Center 
                    , Helper.NextVec2Dir(0.5f,1.5f)
                    , Main.rand.NextFromList(Color.Gold, Color.DarkGoldenrod) * 0.2f, Main.rand.NextFloat(0.4f, 0.8f));

                PRTLoader.NewParticle<TwistFog>(Projectile.Center + Main.rand.NextVector2Circular(18, 18)
                    , dir.RotateByRandom(-0.9f, 0.9f) * Main.rand.NextFloat(0.5f, 2f)
                    , Main.rand.NextFromList(Color.Gold, Color.DarkGoldenrod) * 0.6f, Main.rand.NextFloat(0.4f, 0.6f));
            }

            //向5边生成黄金雨弹幕
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            if (FullCharge)
                for (int i = 0; i < 5; i++)
                {
                    int p = Projectile.NewProjectileFromThis(Projectile.Center
                         , (rot + i * MathHelper.TwoPi / 5 + Main.rand.NextFloat(-0.6f, 0.6f)).ToRotationVector2()
                            * Main.rand.NextFloat(1.5f, 2f), ProjectileID.GoldenShowerFriendly
                         , Projectile.damage / 5, 2);

                    Main.projectile[p].DamageType = FairyDamage.Instance;
                    Main.projectile[p].timeLeft = 60;
                }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FullCharge)
                target.AddBuff(BuffID.Ichor, 60 * 8);
        }

        public override void DrawJar(Vector2 pos, Color lightColor, SpriteEffects eff, Texture2D tex)
        {
            base.DrawJar(pos,lightColor, eff, tex);
            HighlightTex.Value.QuickCenteredDraw(Main.spriteBatch, pos, Color.White, Projectile.rotation
                , Projectile.scale, eff);
        }
    }
}
