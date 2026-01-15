using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Lasso
{
    public class FlowerLasso : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + Name;

        public override int CatchPower => 5;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<FlowerLassoSwing>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 8;
            Item.SetWeaponValues(20, 3);
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 20));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VineLasso>()
                .AddIngredient(ItemID.FlowerPacketWild)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.FairyCatcherLasso)]
    public class FlowerLassoSwing() : BaseLassoSwing(4)
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + "FlowerLassoCatcher";

        public override float GetShootRandAngle => Main.rand.NextFloat(-0.2f, 0.2f);

        public static ATex FlowerChain { get; set; }

        public override void SetSwingProperty()
        {
            base.SetSwingProperty();
            minDistance = 58;
            shootTime = 22;
            spriteRotation = 0;
        }

        public override void OnShootFairy()
        {
            base.OnShootFairy();
            if (Catch == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectileFromThis<FlowerLassoExProj>(Projectile.Center,
                        (Projectile.Center - Owner.Center).RotateByRandom(-0.4f, 0.4f).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(4,7), Projectile.damage / 2, 1);
                }

                //Helper.PlayPitched(CoraliteSoundID.Fleshy_NPCHit1, Projectile.Center, pitchAdjust: -0.1f);
            }
        }

        public override Texture2D GetStringTex() => FlowerChain.Value;
        public override Color GetStringColor(Vector2 pos) => Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));

        public override void DrawHandle(Texture2D HandleTex)
        {
            Main.spriteBatch.Draw(HandleTex, Owner.itemLocation - Main.screenPosition + _Rotation.ToRotationVector2() * 4, null,
                Lighting.GetColor(Owner.Center.ToTileCoordinates()), _Rotation + DirSign * spriteRotation, HandleTex.Size() / 2, 1f, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
        }
    }

    public class FlowerLassoExProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + Name;

        public ref float Timer => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float TexType => ref Projectile.localAI[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 14;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1;
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                TexType = Main.rand.Next(3);
            }

            Timer++;
            switch (State)
            {
                default:
                case 0://刚发射出，减速
                    {
                        if (Timer > 45)
                        {
                            State++;
                            Timer = 0;
                        }

                        Projectile.velocity *= 0.95f;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.whoAmI+MathHelper.PiOver4)*0.03f);
                        Projectile.rotation += Projectile.velocity.Length() / 10;
                    }
                    break;
                case 1://瞄准敌人，如果有就朝目标发射，否则消失
                    {
                        if (Helper.TryFindClosestEnemy(Projectile.Center, 200, n => n.CanBeChasedBy(), out NPC target))
                        {
                            State++;
                            Timer = 0;
                            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                        }
                        else
                            Projectile.Kill();
                    }
                    break;
                case 2://一段时间后消失
                    {
                        if (Timer > 60 + 20)
                            Projectile.Kill();

                        Projectile.rotation += Projectile.velocity.Length() / 30;
                        Projectile.SpawnTrailDust(DustID.Grass, Main.rand.NextFloat(0.2f, 0.4f),100);
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Grass, Helper.NextVec2Dir(0.5f, 1.5f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var frameBox = new Rectangle((int)TexType, 0, 3, 1);
            Projectile.DrawFramedShadowTrails(lightColor, 0.3f, 0.3f / 10, 1, 10, 2, Projectile.scale, frameBox, 0, 0);
            Projectile.QuickFrameDraw(frameBox, lightColor, 0);

            if (State == 0)//绘制一个颜色很淡的圆圈
            {
                float f = Timer / 45;
                Color c = Color.Lerp(Color.Transparent, new Color(1, 15, 1, 0), Helper.SinEase(f));
                float scale = 400f / CoraliteAssets.Halo.FadeCircle.Value.Width * (0.2f+0.8f* Helper.SqrtEase(f));

                CoraliteAssets.Halo.FadeCircle.Value.QuickCenteredDraw(Main.spriteBatch, Projectile.Center-Main.screenPosition, c
                    , Projectile.whoAmI * MathHelper.PiOver2 + f * MathHelper.Pi, scale);
            }
            return false;
        }
    }
}
