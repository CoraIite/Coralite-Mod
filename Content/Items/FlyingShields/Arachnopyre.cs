using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Arachnopyre : BaseFlyingShieldItem<ArachnopyreGuard>
    {
        public Arachnopyre() : base(Item.sellPrice(0, 2), ItemRarityID.LightRed, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<ArachnopyreProj>();
            Item.knockBack = 6;
            Item.shootSpeed = 13.5f;
            Item.damage = 45;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpiderFang, 12)
                .AddIngredient(ItemID.LivingFireBlock, 30)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ArachnopyreProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Arachnopyre";

        private bool canShootSpider = true;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 30;
            backTime = 4;
            backSpeed = 15;
            trailCachesLength = 12;
            trailWidth = 30 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(82, 65, 65) * factor;
        }

        public override void Shooting()
        {
            Chasing();
            if (firstShoot && Timer >= flyingTime - 6)
            {
                Projectile.tileCollide = false;
                if (Timer == flyingTime - 6)
                {
                    firstShoot = false;
                    Projectile.tileCollide = recordTileCollide;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Tile tile = Framing.GetTileSafely(Projectile.Center);
            if (tile.WallType != 0)
            {
                Timer -= 0.5f;
            }
            else
                Timer--;

            if (Timer < 0)
                TurnToBack();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State != (int)FlyingShieldStates.Backing && canShootSpider)
            {
                canShootSpider = false;
                Projectile.NewProjectileFromThis<ArachnopyreSpider>(Owner.Center
                    , (target.Center - Owner.Center).SafeNormalize(Vector2.Zero).RotateByRandom(-0.5f, 0.5f) * 6, (int)(Projectile.damage * 0.85f)
                    , Projectile.knockBack, ai2: Main.rand.Next(0, 6) * 20 + 10);

            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }

    public class ArachnopyreGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.2f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Spider_NPCHit29, Projectile.Center);
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
            Main.spriteBatch.Draw(mainTex, pos + dir * 3, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 7, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 9, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 12, frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class ArachnopyreSpider : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public ref float SpiderType => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public int HitNPCCount;

        public Vector2 scale;
        public float exAlpha;
        public bool canDamage = true;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 20;
        }

        public override bool? CanDamage()
        {
            if (!canDamage)
            {
                return false;
            }
            return null;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                scale = new Vector2(Projectile.scale);
                Projectile.localAI[0] = 1;
            }

            switch (State)
            {
                default:
                case 0://直线运动
                    {
                        float factor = Timer * 0.35f;
                        scale.X = Projectile.scale + 0.3f * MathF.Sin(factor);
                        scale.Y = Projectile.scale + 0.3f * MathF.Cos(factor);
                        Timer--;
                        if (Helper.TryFindClosestEnemy(Projectile.Center,800, n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI) && Projectile.localNPCImmunity[n.whoAmI] == 0, out NPC target))
                        {
                            float selfAngle = Projectile.velocity.ToRotation();
                            float targetAngle = (target.Center - Projectile.Center).ToRotation();

                            Projectile.velocity = selfAngle.AngleLerp(targetAngle, 1- Timer / 120).ToRotationVector2() * Projectile.velocity.Length();
                        }

                        Projectile.rotation = Projectile.velocity.ToRotation();
                        if (Timer < 1)
                        {
                            ExchangeToExplosion();
                        }
                    }
                    break;
                case 1://闪光
                case 2:
                case 3:
                    {
                        float factor = Timer / 8;
                        scale = new Vector2(Projectile.scale + MathF.Sin(factor * MathHelper.Pi)*0.2f);
                        exAlpha = 1 - factor;
                        Timer++;
                        if (Timer > 8)
                        {
                            Timer = 0;
                            State++;
                            if (State > 3)//爆炸
                            {
                                canDamage = true;
                                Projectile.ResetLocalNPCHitImmunity();
                                int width = 100 - (int)SpiderType * 50;
                                Projectile.Resize(width, width);
                                if (SpiderType == 0)
                                {
                                    if (HitNPCCount == 0)
                                    {
                                        float angle = Main.rand.NextFloat(6.282f);
                                        for (int i = 0; i < 3; i++)
                                        {
                                            Projectile.NewProjectileFromThis<ArachnopyreSpider>(Projectile.Center
                                                , (angle + i * MathHelper.TwoPi / 3).ToRotationVector2() * 6, Projectile.damage / 2, Projectile.knockBack / 4, 1, ai2: 120);
                                        }
                                    }
                                    SoundEngine.PlaySound(CoraliteSoundID.Boom_Item14, Projectile.Center);
                                }

                                SpawnExplosionDusts();
                            }
                        }
                    }
                    break;
                case 4://爆炸
                    {
                        Projectile.Kill();
                    }
                    break;
            }
        }

        public void ExchangeToExplosion()
        {
            Timer = 0;
            State = 1;
            canDamage = false;
            scale = new Vector2(Projectile.scale);
            exAlpha = 1;
            Projectile.velocity *= 0;
        }

        public void SpawnExplosionDusts()
        {
            Dust d;
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Helper.NextVec2Dir(0.5f, 2.5f), 50, Color.Gray);
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Helper.NextVec2Dir(0.5f, 2.5f),Scale:Main.rand.NextFloat(1f,1.4f));
            }

            for (int i = 0; i < 26; i++)
            {
                d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height) / 3, DustID.Smoke, Helper.NextVec2Dir(0f, 1f), 50, Color.Gray, Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State==0)
            {
                ExchangeToExplosion();
            }

            Projectile.tileCollide = false;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 2, 0, (int)SpiderType);
            var origin = frameBox.Size() / 2;
            var rot = Projectile.rotation + 1.57f;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rot, origin, scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, new Color(255, 255, 255) * exAlpha, rot, origin, scale, 0, 0);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HitNPCCount > 10)
            {
                return false;
            }
            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == 0)
                ExchangeToExplosion();
            HitNPCCount++;
        }
    }
}
