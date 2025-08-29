using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    [VaultLoaden(AssetDirectory.HyacinthSeriesItems)]
    public class ArethusaHeldProj : BaseGunHeldProj
    {
        public ref float FireType => ref Projectile.ai[2];

        public ArethusaHeldProj() : base(0.2f, 10, -6, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex ArethusaFire { get; private set; }

        public override void ModifyAI(float factor)
        {
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = ArethusaFire.Value;
            Rectangle frameBox = effect.Frame(2, 4, (int)FireType, Projectile.frame);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 36 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }

    public class ArethusaBullet : BaseHeldProj, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void Initialize()
        {
            Projectile.InitOldPosCache(12);

            for (int j = 0; j < 8; j++)
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ArethusaPetal>(), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.05f, 0.15f));
        }

        public override void AI()
        {
            Projectile.rotation += 0.2f;

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.4f, 0.8f));
            if (Projectile.timeLeft > 192)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.02f);
            else if ((Projectile.timeLeft + 8) % 40 < 20)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.02f);
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(-0.02f);

            //if (Projectile.timeLeft % 2 == 0)
            //{
            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Ice_Purple);
            dust.noGravity = true;
            //}

            for (int i = 0; i < 11; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[11] = Projectile.Center + Projectile.velocity;

            trail ??= new Trail(Main.instance.GraphicsDevice, Projectile.oldPos.Length, new EmptyMeshGenerator(), factor => 2,
                factor =>
                {
                    if (factor.X > 0.7f)
                        return Color.Lerp(new Color(95, 120, 233, 60), new Color(230, 225, 255, 80), (factor.X - 0.7f) / 0.3f);

                    return Color.Lerp(new Color(0, 0, 0, 0), new Color(95, 120, 233, 60), factor.X / 0.7f);//new Color(99, 83, 142, 0)
                });

            trail.TrailPositions = Projectile.oldPos;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ArethusaExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int j = 0; j < 8; j++)
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ArethusaPetal>(), (j * 0.785f).ToRotationVector2() * Main.rand.NextFloat(0.5f, 1.5f));

            SoundEngine.PlaySound(CoraliteSoundID.FireBall_Item45, Projectile.Center);
        }

        public void DrawPrimitives()
        {
            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            EffectLoader.ColorOnlyEffect.World = world;
            EffectLoader.ColorOnlyEffect.View = view;
            EffectLoader.ColorOnlyEffect.Projection = projection;

            trail?.DrawTrail(EffectLoader.ColorOnlyEffect);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            //Color lightColor = Lighting.GetColor((Projectile.Center / 16).ToPoint());

            spriteBatch.Draw(mainTex, center, null, Color.White, Projectile.rotation, new Vector2(9, 9), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, center, null, new Color(255, 255, 255, 100), Projectile.rotation + 0.785f, new Vector2(9, 9), 1.5f, SpriteEffects.None, 0f);
        }
    }

    public class ArethusaExplosion : BaseHeldProj
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public ref float Scale => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 14;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 14;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
        }

        public override void Initialize()
        {
            Projectile.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            if (Alpha == 0f)
            {
                Scale = 0.3f;
                Alpha = 1f;
            }

            Alpha -= 0.068f;
            Scale += 0.068f;
            Projectile.rotation += 0.04f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.4f, 0.8f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White * Alpha, Projectile.rotation, new Vector2(35, 45), Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
