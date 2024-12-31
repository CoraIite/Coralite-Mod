using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class PlasmaBow : ModItem
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public static short GlowMaskID;

        public override void SetStaticDefaults()
        {
            Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + 1);
            TextureAssets.GlowMask[^1] = ModContent.Request<Texture2D>(Texture + "_Glow");
            GlowMaskID = (short)(TextureAssets.GlowMask.Length - 1);
        }

        public override void SetDefaults()
        {
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 45;
            Item.shootSpeed = 11f;
            Item.knockBack = 5;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Pink;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 1);

            Item.useTurn = false;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.glowMask = GlowMaskID;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Helper.PlayPitched(CoraliteSoundID.ElectricExplosion_Item94, player.Center, volumeAdjust: -0.6f);

            Vector2 dir = velocity.SafeNormalize(Vector2.Zero);
            float speed = 8f;
            int damage2 = (int)(damage * 0.5f);
            for (int i = -1; i < 2; i++)
            {
                Vector2 velocity2 = dir.RotatedBy(i * 0.5f) * speed;
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                    , position + dir.RotatedBy(i * 0.5f) * 18 - velocity * 2, velocity2, ModContent.ProjectileType<PlasmaBall>()
                    , damage2, knockback, player.whoAmI);
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TremblingBow>()
                .AddIngredient(ItemID.AncientBattleArmorMaterial, 2)
                .AddIngredient(ItemID.SoulofFright, 15)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class PlasmaBall : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Halos + "Circle";

        public ref float State => ref Projectile.ai[0];
        public ref float Hited => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        public int NPCIndex = -1;
        public float Alpha;
        public float fade = 0;

        private Vector2 TargetCenter;

        public ThunderTrail trail;

        LinkedList<Vector2> trailList;

        public static ATex HorizontalStar;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            HorizontalStar = ModContent.Request<Texture2D>(AssetDirectory.Particles + "HorizontalStar");
        }

        public override void Unload()
        {
            HorizontalStar = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool? CanDamage()
        {
            return State == 1 && Hited == 0;
        }

        public override float GetAlpha(float factor)
        {
            if (factor < fade)
                return 0;

            return ThunderAlpha * (factor - fade) / (1 - fade);
        }

        public virtual Color ThunderColorFunc(float factor)
        {
            return new Color(103, 255, 255);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, new Color(103, 255, 255).ToVector3());
            //生成后以极快的速度前进
            switch (State)
            {
                default:
                case 0://刚生成，等待透明度变高后开始寻敌
                    {
                        Alpha += 0.03f;
                        Projectile.velocity *= 0.95f;
                        if (Main.rand.NextBool(3))
                            Projectile.SpawnTrailDust(DustID.Electric, Main.rand.NextFloat(0.1f, 0.3f), Scale: Main.rand.NextFloat(0.4f, 0.8f));

                        if (Alpha > 1)
                        {
                            Alpha = 1;

                            if (Helper.TryFindClosestEnemy(Projectile.Center, 800
                                , n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
                            {
                                NPCIndex = target.whoAmI;

                                TargetCenter = Projectile.Center +
                                    (Projectile.Center - Main.player[Projectile.owner].Center).SafeNormalize(Vector2.Zero) * 125;

                                StartAttack();
                            }
                            else
                                Projectile.Kill();
                        }
                    }
                    break;
                case 1://找到敌人，以极快的速度追踪
                    Chase();
                    break;
                case 2://后摇，闪电逐渐消失
                    {
                        Timer++;
                        fade = Coralite.Instance.X2Smoother.Smoother((int)Timer, 30);
                        ThunderWidth = Coralite.Instance.X2Smoother.Smoother(60 - (int)Timer, 60) * 14;

                        float factor = Timer / 30;
                        float sinFactor = MathF.Sin(factor * MathHelper.Pi);

                        if (Timer > 30)
                            Projectile.Kill();
                    }
                    break;
            }
        }

        public void StartAttack()
        {
            Projectile.tileCollide = true;
            State = 1;
            ThunderAlpha = 1;
            ThunderWidth = 14;
            Projectile.extraUpdates = 6;
            Projectile.timeLeft = 10 * 100;
            trailList = new LinkedList<Vector2>();

            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;

            ATex trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ThunderTrail2");

            trail = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc, GetAlpha)
            {
                CanDraw = true,
                UseNonOrAdd = true,
                PartitionPointCount = 3,
                BasePositions =
                [
                    Projectile.Center,Projectile.Center,Projectile.Center
                ]
            };
            trail.SetRange((0, 7));
            trail.SetExpandWidth(7);
        }

        public void Chase()
        {
            Timer++;
            Vector2 targetCenter = TargetCenter;

            if (NPCIndex.GetNPCOwner(out NPC target))
            {
                float speed = Projectile.velocity.Length();

                if (Projectile.Center.Distance(targetCenter) < speed * 4)//距离目标点近了就换一个
                {
                    if (Projectile.Center.Distance(target.Center) < speed * 10)
                    {
                        targetCenter = target.Center;
                        TargetCenter = target.Center;
                    }
                    else
                    {
                        Vector2 dir2 = target.Center - Projectile.Center;
                        float length2 = dir2.Length();
                        if (length2 > 100)
                            length2 = 100;
                        dir2 = dir2.SafeNormalize(Vector2.Zero);
                        Vector2 center2 = Projectile.Center + dir2 * length2;
                        Vector2 pos = center2 + dir2.RotatedBy(Main.rand.NextFromList(1.57f, -1.57f)) * length2;// Main.rand.NextVector2Circular(length2,length2);

                        targetCenter = pos;
                        TargetCenter = pos;
                        Projectile.velocity = (targetCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
                    }
                }
            }
            else
            {
                Fade();
                return;
            }

            float selfAngle = Projectile.velocity.ToRotation();
            float targetAngle = (targetCenter - Projectile.Center).ToRotation();

            float factor = 1 - Math.Clamp(Vector2.Distance(targetCenter, Projectile.Center) / 500, 0, 1);

            Projectile.velocity = selfAngle.AngleLerp(targetAngle, 0.5f + 0.5f * factor).ToRotationVector2() * 24f;

            if (Main.rand.NextBool(8))
                Projectile.SpawnTrailDust(DustID.Electric, Main.rand.NextFloat(0.1f, 0.3f), Scale: Main.rand.NextFloat(0.4f, 0.8f));

            trailList.AddLast(Projectile.Center);

            if (Timer % Projectile.MaxUpdates == 0)
            {
                trail.BasePositions = [.. trailList];//消失的时候不随机闪电
                trail.RandomThunder();
            }
        }

        public void Fade()
        {
            if (State == 0)
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 0;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            Hited = 1;
            Timer = 0;
            State = 2;

            if (trail != null)
            {
                trail.BasePositions = [.. trailList];
                if (trail.BasePositions.Length > 3)
                    trail.RandomThunder();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Fade();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Fade();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Helper.NextVec2Dir(2f, 5f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Hited == 0)//没碰到任何东西就绘制本体
            {
                Texture2D mainTex = Projectile.GetTexture();

                Color c = Lighting.GetColor(Projectile.Center.ToTileCoordinates(), new Color(103, 255, 255));
                c.A = 0;
                c *= Alpha;

                Vector2 position = Projectile.Center - Main.screenPosition;

                Main.spriteBatch.Draw(mainTex, position, null, c, 0,
                    mainTex.Size() / 2, 0.15f, 0, 0);

                Texture2D exTex = HorizontalStar.Value;

                Vector2 origin = exTex.Size() / 2;
                Main.spriteBatch.Draw(exTex, position, null, c, 0,
                    origin, 0.5f, 0, 0);

                c = lightColor;
                c.A = 0;
                c *= Alpha;
                Main.spriteBatch.Draw(exTex, position, null, c, 0,
                    origin, 0.2f, 0, 0);
            }

            if (State > 0)
            {
                if (State == 1 && Timer < 3)
                    return false;

                trail?.DrawThunder(Main.instance.GraphicsDevice);
            }

            return false;
        }
    }
}
