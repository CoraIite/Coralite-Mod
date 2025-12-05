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
using static System.Net.Mime.MediaTypeNames;

namespace Coralite.Content.Items.FairyCatcher.Jar
{
    public class CannedHerring : BaseFairyJar
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override int CatchPower => 40;

        public override ChannelSpeeds ChannelSpeed => ChannelSpeeds.Fast;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<IchorBucketProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 14;
            Item.SetWeaponValues(18, 3);
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 2));
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.EmptyBucket)
            //    .AddIngredient(ItemID.Ichor, 12)
            //    .AddTile(TileID.WorkBenches)
            //    .Register();
        }
    }

    public class CannedHerringProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 28;
        }

        public override void InitFields()
        {
            MaxChannelTime = 60 + 30;
            MaxChannelDamageBonus = 3.5f;
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

        public override void OtherStates()
        {
            Timer++;

            if (Projectile.IsOwnedByLocalPlayer() && Timer % 3 == 0)
            {
                Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();

                float damagePercet = 0.7f - 0.4f * Math.Clamp(1 - Timer / 20, 0, 1);
                int damage = (int)(Projectile.damage * damagePercet);

                Projectile.NewProjectileFromThis<CannedHerringGas>(Projectile.Center
                    , dir * Main.rand.NextFloat(4, 7), damage, 2);
            }

            if (Timer > 50)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            if (Projectile.IsOwnedByLocalPlayer() && (int)State != 2)//直接生成一圈气体
            {
                float rot2 = Main.rand.NextFloat(MathHelper.TwoPi);
                int damage = (int)(Projectile.damage * 0.7f);
                for (int i = 0; i < 9; i++)
                {
                    Projectile.NewProjectileFromThis<CannedHerringGas>(Projectile.Center
                        , rot2.ToRotationVector2() * Main.rand.NextFloat(4, 7), damage, 2);

                    rot2 += MathHelper.TwoPi / 9;
                }
            }

            //各种粒子
            for (int i = 0; i < 24; i++)
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.Ichor, Helper.NextVec2Dir(0.75f, 3f), 50, Color.DarkGray, Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = Main.rand.NextBool(2, 3);
            }

            Helper.PlayPitched(CoraliteSoundID.IDontKnow_ShimmerWeak2
                , Projectile.Center, pitchAdjust: -0.25f);
            Helper.PlayPitched(CoraliteSoundID.BloodyDeath3_NPCDeath19
                , Projectile.Center, pitchAdjust: -0.25f);

            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 5; i++)
            {
                PRTLoader.NewParticle<AnimeFogDark>(Projectile.Center
                    , Helper.NextVec2Dir(0.5f, 1.5f)
                    , Main.rand.NextFromList(Color.Gold, Color.DarkGoldenrod) * 0.2f, Main.rand.NextFloat(0.4f, 0.8f));

                PRTLoader.NewParticle<TwistFog>(Projectile.Center + Main.rand.NextVector2Circular(18, 18)
                    , dir.RotateByRandom(-0.9f, 0.9f) * Main.rand.NextFloat(0.5f, 2f)
                    , Main.rand.NextFromList(Color.Gold, Color.DarkGoldenrod) * 0.6f, Main.rand.NextFloat(0.4f, 0.6f));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FullCharge)//满蓄力命中就会旋转着弹回来
            {
                State = (AIStates)2;
                Projectile.velocity *= -1;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }

        public override void DrawJar(Vector2 pos, Color lightColor, SpriteEffects eff, Texture2D tex)
        {
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)State == 2 ? 1 : 0, 0, 2, 1), pos
                , lightColor, Projectile.rotation
                , Projectile.scale, eff);
        }
    }

    public class CannedHerringGas : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 38;
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                Projectile.frame = Main.rand.Next(3);
                Projectile.localAI[0] = 1;
            }

            Timer++;
            if (Timer > 90)
            {
                Projectile.Kill();
            }


            Projectile.rotation += MathF.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 20;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.damage<2)
            {
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer == 0)
                return false;

            float alpha = 0.8f;
            if (Timer > 60)
                alpha = 0.8f * (1 - (Timer - 60) / 30);

            Projectile.QuickDraw(new Rectangle(Projectile.frame, 1, 3, 1), lightColor * alpha, 0);

            return false;
        }
    }
}
