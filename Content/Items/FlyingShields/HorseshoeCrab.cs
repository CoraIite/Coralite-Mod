using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.FlyingShields
{
    public class HorseshoeCrab : BaseFlyingShieldItem<HorseshoeCrabGuard>
    {
        public HorseshoeCrab() : base(Item.sellPrice(0, 2), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        {
        }

        public bool PowerfulAttack;

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<HorseshoeCrabProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 15;
            Item.damage = 46;
        }

        public override void HoldItem(Player player)
        {
            if (PowerfulAttack)
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(player.Center + (5 * Main.GlobalTimeWrappedHourly + i * MathHelper.Pi).ToRotationVector2() * 32,
                        DustID.Water, Vector2.Zero);
                    d.noGravity = true;
                }
            }
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            int ai2 = 0;
            if (PowerfulAttack)
            {
                ai2 = 1;
                damage = (int)(damage * 1.2f);
            }
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, ai2: ai2);

            PowerfulAttack = false;
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
            flyingTime = 20;
            backTime = 14;
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

        public void SpecialDust()
        {
            if (Powerful == 1)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                Vector2 dir2 = (Projectile.rotation + 1.57f).ToRotationVector2();
                for (int j = 0; j < 3; j++)
                    for (int i = -1; i < 2; i += 2)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center + (j / 3f) * Projectile.velocity + dir * 8 * Projectile.scale + i * dir2 * Projectile.scale * Projectile.width / 2,
                            DustID.Water, -Projectile.velocity * Main.rand.NextFloat(0f, 0.5f), newColor: Color.White);
                        d.noGravity = true;
                    }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (State != (int)FlyingShieldStates.Backing)
            {
                if (Owner.HeldItem.ModItem is HorseshoeCrab pr)
                    pr.PowerfulAttack = true;
                Vector2 dir = Helper.NextVec2Dir();

                Projectile.NewProjectileFromThis<HorseshoeCrabEXProj>(target.Center + dir * 16 * 10, -dir * 10, Projectile.damage, Projectile.knockBack);
            }
        }

        public override Color GetColor(float factor)
        {
            return new Color(110, 91, 255) * factor;
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
            if (Owner.HeldItem.ModItem is HorseshoeCrab pr)
                pr.PowerfulAttack = true;
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
            Main.spriteBatch.Draw(mainTex, pos - dir * 4, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 5, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 10, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 12, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 17, frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class HorseshoeCrabEXProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HorseshoeCrab";

        float alpha = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 32;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.87f);
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(DustID.Water_Corruption, Main.rand.NextFloat(0.1f, 0.7f));

            alpha = MathF.Sin(MathHelper.Pi * Projectile.timeLeft / 32f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Projectile.DrawShadowTrails(new Color(110, 91, 255) * alpha, 0.5f, 0.5f / 6, 0, 6, 1, -1.57f, -1);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * alpha, Projectile.rotation - 1.57f, mainTex.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
    }
}
