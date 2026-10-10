using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class HorseshoeCrab : BaseFlyingShieldItem<HorseshoeCrabGuard>
    {
        public HorseshoeCrab() : base(Item.sellPrice(0, 5), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 27;
            Item.shoot = ModContent.ProjectileType<HorseshoeCrabProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 15;
            Item.damage = 45;
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, ai2: 0);
            Projectile.NewProjectile(source, player.Center, velocity, ModContent.ProjectileType<HorseshoeCrabProjShooter>(), damage, knockback, player.whoAmI);

        }
    }

    public class HorseshoeCrabProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HorseshoeCrab";

        ref float Powerful => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            ShieldSlot = 0.33f;

            flyingTime = 20;
            backTime = 8;
            backSpeed = 16;
            trailCachesLength = 6;
            trailWidth = 8 / 2;
        }

        public override void OnShootDusts()
        {
            SpecialDust();
        }

        public override void OnBackDusts()
        {
            SpecialDust();
        }

        public override void Shooting()
        {
            if (Powerful == 0)
            {
                ShieldSlot = 0.34f;
            }

            if (firstShoot && Powerful != 0 && !canChase)//转弯
            {
                if (Timer < flyingTime - 5)
                    Projectile.velocity = Projectile.velocity.RotatedBy(-Powerful * 0.08f * MathF.Sin(Timer * 0.3f));
            }

            base.Shooting();
        }

        public void SpecialDust()
        {
            if (Powerful!=0)
            {
                return;
            }

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 dir2 = (Projectile.rotation + 1.57f).ToRotationVector2();

            float rot = MathF.Sin(Timer * 0.2f) * 0.3f;

            for (int j = 0; j < 3; j++)
                for (int i = -1; i < 2; i += 2)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + (j / 3f * Projectile.velocity) + (dir * 8 * Projectile.scale) + (i * dir2 * Projectile.scale * Projectile.width / 2),
                        DustID.Water, -Projectile.velocity.RotatedBy(i * rot) * Main.rand.NextFloat(0f, 0.5f), newColor: Color.White);
                    d.noGravity = true;
                }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing && Powerful == 0)
            {
                Vector2 dir = Helper.NextVec2Dir();

                Projectile.NewProjectileFromThis<HorseshoeCrabEXProj>(target.Center + (dir * 16 * 10), -dir * 10, Projectile.damage, Projectile.knockBack);
            }
        }

        public override Color GetColor(float factor)
        {
            return new Color(110, 91, 255) * factor;
        }
    }

    public class HorseshoeCrabProjShooter:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        //public ref float 

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Player p = Main.player[Projectile.owner];
            Projectile.Center = p.Center;

            if (Projectile.ai[1]==0)
            {
                Projectile.ai[1] = 1;
                Projectile.netUpdate = true;
                Projectile.timeLeft = p.itemTimeMax;
            }

            Projectile.ai[2]++;

            if (p.itemTime == (int)(p.itemTimeMax * 0.9f) )
            {
                Projectile.NewProjectileFromThis<HorseshoeCrabProj>(Projectile.Center, Projectile.velocity.RotatedBy(0.05f), Projectile.damage, Projectile.knockBack, ai2: 1);
            }
            if (p.itemTime == (int)(p.itemTimeMax * 0.8f))
            {
                Projectile.NewProjectileFromThis<HorseshoeCrabProj>(Projectile.Center, Projectile.velocity.RotatedBy(-0.05f), Projectile.damage, Projectile.knockBack, ai2: -1);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class HorseshoeCrabGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 58;
        }

        public override void SetOtherValues()
        {
            scalePercent = 1.4f;
            damageReduce = 0.1f;
            extraRotation = MathHelper.Pi;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Jellyfish_NPCHit25, Projectile.Center);

            if (Projectile.IsOwnedByLocalPlayer())
            {
                Vector2 dir = (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.Zero).RotateByRandom(-0.5f, 0.5f);

                Projectile.NewProjectileFromThis<HorseshoeCrabEXProj>(Projectile.Center + (dir * 16 * 10), -dir * 10, Projectile.damage, Projectile.knockBack);
            }
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(3, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 4), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 10), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 12), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 17), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class HorseshoeCrabEXProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HorseshoeCrab";

        float alpha = 0;
        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 40;
            Projectile.timeLeft = 32;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void Initialize()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(DustID.Water_Corruption, Main.rand.NextFloat(0.1f, 0.7f));

            alpha = MathF.Sin(MathHelper.Pi * Projectile.timeLeft / 32f);
            Projectile.scale = 0.5f + alpha * 1.0f;
            int a = (int)(40 * Projectile.scale);
            Projectile.Resize(a,a);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            Projectile.DrawShadowTrails(new Color(110, 91, 255) * alpha, 0.5f, 0.5f / 6, 0, 6, 1, -1.57f, -1);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation - 1.57f, mainTex.Size() / 2, Projectile.scale, 0, 0);

            //绘制一圈光
            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 dir = Projectile.rotation.ToRotationVector2()*Projectile.width*0.65f;
            for (int i = -6; i <= 6; i++)
            {
                Vector2 dir2 = dir.RotatedBy(i * 0.25f);
                Vector2 p = center + dir2;

                Helper.DrawPrettyLine(1, 0, p, new Color(196, 191, 255, 150), new Color(110, 91, 255), Projectile.timeLeft / 32f, 0, 0.5f, 0.5f, 1, dir2.ToRotation()+MathHelper.PiOver2, 1.0f - MathF.Abs(i / 6f) * 0.5f, new Vector2(2.1f- MathF.Abs(i / 6f) * 1.1f, 1.1f));
            }
            
            return false;
        }
    }
}
