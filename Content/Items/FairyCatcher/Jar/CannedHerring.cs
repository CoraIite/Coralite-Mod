using Coralite.Content.DamageClasses;
using Coralite.Content.Particles;
using Coralite.Content.Prefixes.FairyWeaponPrefixes;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Jar
{
    public class CannedHerring : BaseFairyJar
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override int CatchPower => 40;

        public override ChannelSpeeds ChannelSpeed => ChannelSpeeds.Fast;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<CannedHerringProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 14;
            Item.SetWeaponValues(55, 3);
            Item.SetShopValues(ItemRarityColor.LightRed4, Item.sellPrice(0, 2));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar,12)
                .AddIngredient(ItemID.SoulofFright)
                .AddIngredient(ItemID.SoulofMight)
                .AddIngredient(ItemID.SoulofSight)
                .AddIngredient(ItemID.AtlanticCod)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class CannedHerringProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.FairyCatcherJar + Name;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            this.LoadGore(2);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 28;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (State==(AIStates)2)
            {
                return false;
            }
            return base.CanDamage();
        }


        public override void InitFields()
        {
            MaxChannelTime = 60 + 15;
            MaxChannelDamageBonus = 3f;
            MaxFlyTime = 26;
            XSlowDown = 0.95f;
        }

        public override void FlyingRotation()
        {
            Projectile.rotation +=
                            Math.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 80
                            + Projectile.velocity.X / 75;
        }

        public override void OtherStates()
        {
            Timer++;

            Projectile.velocity.Y += FallAcc * 0.75f;
            Projectile.velocity.Y = Math.Clamp(Projectile.velocity.Y, -MaxYFallSpeed, MaxYFallSpeed);
            Projectile.velocity *= XSlowDown;

            Projectile.rotation += 0.175f;

            if (Projectile.IsOwnedByLocalPlayer() && Timer % 4 == 0)
            {
                Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();

                float damagePercet = 0.7f - 0.4f * Math.Clamp(1 - Timer / 20, 0, 1);
                int damage = (int)(Projectile.damage * damagePercet);

                Projectile.NewProjectileFromThis<CannedHerringGas>(Projectile.Center
                    , dir * Main.rand.NextFloat(5, 8), damage, 2);
            }

            if (Timer % 3 == 0)
            {
                Vector2 dir = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();

                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), DustID.Cloud
                         , dir * Main.rand.NextFloat(2, 8), Alpha: 0, Scale: Main.rand.NextFloat(1, 2f));
                    d.noGravity = true;
                }

                PRTLoader.NewParticle<AnimeFogDark>(Projectile.Center + Main.rand.NextVector2Circular(10, 10)
                    , dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(0.5f, 2f), Color.SkyBlue * 0.6f, Main.rand.NextFloat(0.2f, 0.5f));
            }

            if (Timer > 40)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

            if (Projectile.IsOwnedByLocalPlayer() && FullCharge && (int)State != 2)//直接生成一圈气体
            {
                float rot2 = Main.rand.NextFloat(MathHelper.TwoPi);
                int damage = (int)(Projectile.damage * 0.7f);
                for (int i = 0; i < 9; i++)
                {
                    Projectile.NewProjectileFromThis<CannedHerringGas>(Projectile.Center
                        , rot2.ToRotationVector2() * Main.rand.NextFloat(1, 3), damage, 2);

                    rot2 += MathHelper.TwoPi / 9;
                }
            }

            Helper.PlayPitched(CoraliteSoundID.WafflesIron_Item178, Projectile.Center, pitchAdjust: 0.5f);

            if (!VaultUtils.isServer)
                this.SpawnGore(2);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (FullCharge)//满蓄力命中就会旋转着弹回来
            {
                State = (AIStates)2;
                Projectile.velocity *= -0.7f;
                Timer = 0;
                Projectile.netUpdate = true;
            }
            else
                Projectile.Kill();
        }

        public override void DrawChannelAlpha(SpriteEffects eff, Texture2D tex, Color c, float scale)
        {
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 0, 2, 1), Projectile.Center - Main.screenPosition, c, Projectile.rotation
                , scale, eff);
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
            if (Timer > 60)
            {
                Projectile.Kill();
            }

            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12), DustID.Cloud
                     , Helper.NextVec2Dir(0.5f, 3), Alpha: (int)(Timer/60f*255), Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            Projectile.velocity *= 0.95f;
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
            Projectile.velocity = oldVelocity * 0.95f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer == 0)
                return false;

            float alpha = 0.8f;
            if (Timer > 30)
                alpha = 0.8f * (1 - (Timer - 30) / 30);

            Projectile.QuickFrameDraw(new Rectangle(Projectile.frame, 0, 3, 1), lightColor * alpha, 0);

            return false;
        }
    }
}
