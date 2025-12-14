using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Terraria.Enums;
using Terraria;
using Terraria.ID;
using Coralite.Content.DamageClasses;
using Coralite.Content.Particles;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Coralite.Content.Items.FairyCatcher.Jar
{
    public class CursedBarrel : BaseFairyJar
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override int CatchPower => 40;

        public override ChannelSpeeds ChannelSpeed => ChannelSpeeds.Slow;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<CursedBarrelProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 14;
            Item.SetWeaponValues(30, 3);
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 2));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Barrel)
                .AddIngredient(ItemID.CursedFlame, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.FairyCatcherJar)]
    public class CursedBarrelProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + "CursedBarrel";

        [VaultLoaden("{@classPath}" + "CursedBarrel_Highlight")]
        public static ATex HighlightTex { get; private set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 34;
        }

        public override void InitFields()
        {
            MaxChannelTime = 60 + 45;
            MaxChannelDamageBonus = 4f;
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
            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), DustID.CursedTorch
                , dir * Main.rand.NextFloat(0.5f, 2f));
            d.noGravity = Main.rand.NextBool(4, 5);

            if (Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle<AnimeFogDark>(Projectile.Center
                    , Helper.NextVec2Dir(0.5f, 1.5f)
                    , Main.rand.NextFromList(Color.Lime, Color.LimeGreen) * 0.1f, Main.rand.NextFloat(0.2f, 0.5f));

            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            //各种粒子
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.CursedTorch, Helper.NextVec2Dir(0.75f, 3f), 50, Color.DarkGray, Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = Main.rand.NextBool(2, 3);
            }

            Helper.PlayPitched(CoraliteSoundID.IDontKnow_ShimmerWeak2
                , Projectile.Center, pitchAdjust: -0.25f);
            Helper.PlayPitched(CoraliteSoundID.Dig
                , Projectile.Center, pitchAdjust: -0.25f);

            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 6; i++)
            {
                PRTLoader.NewParticle<TwistFog>(Projectile.Center
                    , Helper.NextVec2Dir(0.5f, 1.5f)
                    , Main.rand.NextFromList(Color.Lime, Color.LimeGreen) * 0.8f, Main.rand.NextFloat(0.4f, 0.6f));

                PRTLoader.NewParticle<FireParticle>(Projectile.Center + Main.rand.NextVector2Circular(18, 18)
                    , dir.RotateByRandom(-0.9f, 0.9f) * Main.rand.NextFloat(2f, 4f)
                    , Main.rand.NextFromList(Color.Lime, Color.LimeGreen) * 0.8f, Main.rand.NextFloat(0.6f, 0.8f));
                
                var l= PRTLoader.NewParticle<LightBall>(Projectile.Center + Main.rand.NextVector2Circular(24, 24)
                    , dir.RotateByRandom(-0.9f, 0.9f) * Main.rand.NextFloat(6f, 8f)
                    , Main.rand.NextFromList(Color.Lime, Color.LimeGreen) * 0.7f, Main.rand.NextFloat(0.1f, 0.25f));
                l.goesUp = false;
            }

            //向5边生成诅咒焰弹幕
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            if (FullCharge)
            {
                Helper.PlayPitched(CoraliteSoundID.Flamethrower_Item34
                    , Projectile.Center, pitchAdjust: -0.25f);
                for (int i = 0; i < 5; i++)
                {
                    int p = Projectile.NewProjectileFromThis(Projectile.Center
                         , (rot + i * MathHelper.TwoPi / 5 + Main.rand.NextFloat(-0.6f, 0.6f)).ToRotationVector2()
                            * Main.rand.NextFloat(2f, 3f), ProjectileID.CursedFlameFriendly
                         , Projectile.damage / 5, 2);

                    Main.projectile[p].friendly = true;
                    Main.projectile[p].DamageType = FairyDamage.Instance;
                    Main.projectile[p].timeLeft = 90;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FullCharge)
                target.AddBuff(BuffID.CursedInferno, 60 * 8);
        }

        public override void DrawJar(Vector2 pos, Color lightColor, SpriteEffects eff, Texture2D tex)
        {
            base.DrawJar(pos, lightColor, eff, tex);
            HighlightTex.Value.QuickCenteredDraw(Main.spriteBatch, pos, Color.White, Projectile.rotation
                , Projectile.scale, eff);
        }
    }
}
