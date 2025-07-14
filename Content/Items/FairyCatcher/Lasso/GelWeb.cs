using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Lasso
{
    public class GelWeb : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.FairyCatcherLasso + Name;

        public override int CatchPower => 5;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<GelWebCatcher>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 24;
            Item.shootSpeed = 8;
            Item.SetWeaponValues(20, 3);
            Item.SetShopValues(ItemRarityColor.Blue1, Item.sellPrice(0, 0, 20));
        }
    }

    public class GelWebCatcher() : BaseLassoSwing(4)
    {
        public override float GetShootRandAngle => Main.rand.NextFloat(-0.2f, 0.2f);

        public override void SetSwingProperty()
        {
            minDistance = 52;
            base.SetSwingProperty();

            //DrawOriginOffsetX = 8;
            shootTime = 25;
        }

        public override void SetMaxDistance()
        {
            MaxDistance = 140;
        }

        public override Color GetStringColor(Vector2 pos)
        {
            Color c = Color.SkyBlue;
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            return c*0.8f;
        }

        public override void OnShootFairy()
        {
            base.OnShootFairy();
            if (Catch == 0)
            {
                Projectile.NewProjectileFromThis<GelWebBall>(Projectile.Center,
                    (Projectile.Center - Owner.Center).SafeNormalize(Vector2.Zero) * 8, Projectile.damage / 2, 1);

                Helper.PlayPitched(CoraliteSoundID.Fleshy_NPCHit1, Projectile.Center, pitchAdjust: -0.1f);
            }
        }

        public override void DrawHandle(Texture2D HandleTex)
        {
            Main.spriteBatch.Draw(HandleTex, Owner.itemLocation - Main.screenPosition+_Rotation.ToRotationVector2()*8, null,
                Lighting.GetColor(Owner.Center.ToTileCoordinates()), _Rotation + DirSign * spriteRotation, HandleTex.Size() / 2, 1f, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
        }
    }

    public class GelWebBall : BaseHeldProj
    {
        public override string Texture => AssetDirectory.GelItems + "GelFiberBall";

        public ref float HitTileCount => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.DamageType = FairyDamage.Instance;
        }

        public override void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                      Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.2f, 0.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 15)  //膨胀小动画
                Projectile.localAI[0]++;
            else
            {
                Projectile.velocity.Y += 0.4f;
                if (Projectile.velocity.Y > 12)
                    Projectile.velocity.Y = 12;
            }

            Projectile.rotation += 0.1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.netUpdate = true;

            //简易撞墙反弹
            bool top = oldVelocity.Y < 0 && Math.Sign(oldVelocity.Y + Projectile.velocity.X) < 0;
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX * 0.7f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY * 0.7f;
            if (top)
                Projectile.velocity.Y *= -1;

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                       -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(0.1f, 0.3f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1f, 1.4f));
                dust.noGravity = true;
            }

            HitTileCount++;
            if (HitTileCount>5)
                return true;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Helper.PlayPitched(CoraliteSoundID.NoUse_ToxicBubble2_Item112, Projectile.Center, volumeAdjust: -0.4f);

            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.t_Slime,
                     Helper.NextVec2Dir(1f, 2.5f), 150, new Color(78, 136, 255, 80), Main.rand.NextFloat(1.2f, 1.6f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;

            Color color = lightColor;
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            //绘制影子拖尾
            Projectile.DrawShadowTrails(color, 0.5f, 0.062f, 1, 8, 1, 0.1f, frameBox);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, 1, 0, 0);

            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            color = new Color(194, 75 + (int)(60 * factor), 234);

            //绘制发光
            frameBox = mainTex.Frame(1, 2, 0, 1);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, color, Projectile.rotation, frameBox.Size() / 2, 1, 0, 0);

            return false;
        }
    }
}
