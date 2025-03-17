using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class MechRioter : BaseFlyingShieldItem<MechRioterGuard>
    {
        public MechRioter() : base(Item.sellPrice(0, 4), ItemRarityID.LightPurple, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ModContent.ProjectileType<MechRioterProj>();
            Item.knockBack = 4;
            Item.shootSpeed = 16;
            Item.damage = 44;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 20)
                .AddIngredient(ItemID.SoulofFright)
                .AddIngredient(ItemID.SoulofSight)
                .AddIngredient(ItemID.SoulofMight)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class MechRioterProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "MechRioterProj";

        public bool hited;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 46;
        }

        public override void OnShootDusts()
        {
            ShootLaser();
        }

        public override void OnBackDusts()
        {
            ShootLaser();
        }

        public void ShootLaser()
        {
            if (hited)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + (Projectile.velocity * (i / 3f)),
                        DustID.TheDestroyer, -Projectile.velocity * 0.2f);
                    d.noGravity = true;
                }

                if (Timer == 24 && Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy(), out NPC target))
                {
                    int index = Projectile.NewProjectileFromThis(Projectile.Center
                        , (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12,
                        ProjectileID.DeathLaser, Projectile.damage*4, Projectile.knockBack, ai1: 1);
                    Main.projectile[index].hostile = false;
                    Main.projectile[index].friendly = true;
                    Main.projectile[index].penetrate = 1;
                }
            }
        }

        public override void SetOtherValues()
        {
            flyingTime = 30;
            backTime = 15;
            backSpeed = 12;
            trailCachesLength = 6;
            trailWidth = 16 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hited && State == (int)FlyingShieldStates.Shooting)
            {
                int width = (int)(38 * Projectile.scale);
                Projectile.Resize(width, width);

                Projectile.NewProjectileFromThis<SmallMechRioter>(target.Center, Helper.NextVec2Dir() * 12, (int)(Projectile.damage * 0.9f), Projectile.knockBack);
                Vector2 offset = Helper.NextVec2Dir();
                Projectile.NewProjectileFromThis<SmallMechRioter>(target.Center + (offset * 16), offset * 2, (int)(Projectile.damage * 0.9f), Projectile.knockBack, 1);
                hited = true;
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override Color GetColor(float factor)
        {
            return Color.Gray * factor;
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(2, 1, hited ? 1 : 0, 0);
            var origin = frameBox.Size() / 2;
            var pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);
        }
    }

    public class MechRioterGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 46;
            Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.2f;
        }

        public override void OnGuard()
        {
            base.OnGuard();
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 3; i++)
            {
                int index = Projectile.NewProjectileFromThis(Projectile.Center
                    , dir.RotateByRandom(-0.2f, 0.2f) * 12,
                    ProjectileID.DeathLaser, Projectile.damage, Projectile.knockBack, ai1: 1);
                Main.projectile[index].hostile = false;
                Main.projectile[index].friendly = true;
                Main.projectile[index].penetrate = 1;
                Main.projectile[index].soundDelay = 10000;
            }
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

    /// <summary>
    /// 使用ai0传入状态，0：机械佝偻王，1：双子魔眼
    /// </summary>
    public class SmallMechRioter : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "MechRioterProj2";
        ref float Phase => ref Projectile.ai[0];
        ref float State => ref Projectile.ai[1];
        ref float Target => ref Projectile.ai[2];
        ref float Timer => ref Projectile.localAI[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 24;
        }

        public override bool? CanDamage()
        {
            if (Phase == 1)
                return false;
            if (State == 0)
                return null;

            return false;
        }

        public override void Initialize()
        {
            Target = -1;
            if (Phase == 1)
            {
                Projectile.tileCollide = false;
            }
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://就位并执行特定行为
                    {
                        const int AttackTime = 60;

                        if (!Target.GetNPCOwner(out NPC target))//没有就找一下，再没有就直接返回
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 650
                                , n => n.CanBeChasedBy(), out target))
                                Target = target.whoAmI;
                            else
                                TurnToBack();
                        }

                        if (target == null)
                        {
                            TurnToBack();
                            return;
                        }

                        if (Phase == 0)//机械佝偻王 转转乐
                        {
                            float length = Vector2.Distance(target.Center, Projectile.Center);
                            if (length > 60)
                                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 14;
                            else if (length > 50 && length < 60)
                                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-0.5f, 0.5f));
                            else if (Projectile.velocity.Length() < 4)
                            {
                                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 14;
                            }
                            Projectile.rotation += 0.3f;
                        }
                        else//双子魔眼 射激光和火球
                        {
                            float length = Vector2.Distance(target.Center, Projectile.Center);

                            if (length < 160)
                                Projectile.velocity = (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero) * 12;
                            else if (length > 220)
                                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                            else
                                Projectile.velocity *= 0.7f;

                            Projectile.velocity += (Projectile.rotation + 1.57f).ToRotationVector2() / 2;
                            Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
                            if (Timer > 0)
                            {
                                if (Timer == 30)//射激光
                                {
                                    for (int i = -1; i < 2; i += 2)
                                    {
                                        int index = Projectile.NewProjectileFromThis(Projectile.Center + ((Projectile.rotation + (1.57f * i)).ToRotationVector2() * 12)
                                            , (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12,
                                            ProjectileID.EyeLaser, (int)(Projectile.damage * 0.8f), Projectile.knockBack, ai1: i + 1);
                                        Main.projectile[index].hostile = false;
                                        Main.projectile[index].friendly = true;
                                        Main.projectile[index].penetrate = 1;
                                    }
                                }
                                if (Timer == 50)//射火球
                                {
                                    int index = Projectile.NewProjectileFromThis(Projectile.Center
                                        , (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12,
                                        ProjectileID.CursedFlameFriendly, Projectile.damage, Projectile.knockBack);

                                    Main.projectile[index].penetrate = 1;
                                }
                            }

                            //生成粒子
                            Vector2 dustVel = (Projectile.rotation + MathHelper.Pi).ToRotationVector2() * 2;
                            for (int i = -1; i < 2; i += 2)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center + ((Projectile.rotation + (1.57f * i)).ToRotationVector2() * 12),
                                    DustID.TheDestroyer, dustVel);
                                d.noGravity = true;
                            }
                        }

                        Timer++;
                        if (Timer > AttackTime)
                            TurnToBack();
                    }
                    break;
                case 1://返回身边
                    {
                        float angle = Projectile.velocity.ToRotation();
                        float targetAngle = (Owner.Center - Projectile.Center).ToRotation();
                        Projectile.velocity = angle.AngleLerp(targetAngle, 1).ToRotationVector2() * 8;
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Phase == 1)
                        {
                            Vector2 dustVel = (Projectile.rotation + MathHelper.Pi).ToRotationVector2() * 2;

                            for (int i = -1; i < 2; i += 2)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center + ((Projectile.rotation + (1.57f * i)).ToRotationVector2() * 12),
                                    DustID.TheDestroyer, dustVel);
                                d.noGravity = true;
                            }
                        }

                        if (Vector2.Distance(Projectile.Center, Owner.Center) < 20)
                        {
                            Projectile.Kill();
                        }

                        Timer++;
                        if (Timer > 200)
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public void TurnToBack()
        {
            Timer = 0;
            State = 1;
            Projectile.tileCollide = false;
            Projectile.frame = 1;
            Projectile.extraUpdates += 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            if (Phase == 0)
            {
                var frameBox = mainTex.Frame(3, 1, Projectile.frame, 0);
                Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 8, 1, 8, 1, 0.9f, frameBox, -1.57f);

                Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation - 1.57f, frameBox.Size() / 2, Projectile.scale, 0, 0);
            }
            else
            {
                var frameBox = mainTex.Frame(3, 1, 2, 0);

                Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation - 1.57f, frameBox.Size() / 2, Projectile.scale, 0, 0);
            }

            return false;
        }
    }
}
