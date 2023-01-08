using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Projectiles.ShadowProjectiles
{
    public class DrawInvertedPlayerProj : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowItems + "InvertedShadow";
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public Player Owner => Main.player[Projectile.owner];
        public Player Shadow;
        public bool OnStart = true;

        public float shadow = 0.001f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 25;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (OnStart)//初始化这个影子玩家
            {
                Projectile.velocity *= 0;

                Shadow = new Player();
                Shadow.hair = Owner.hair;
                Shadow.skinColor = Owner.skinColor;
                Shadow.skinVariant = Owner.skinVariant;
                Shadow.Male = Owner.Male;
                
                for (int i = 0; i < 3; i++)
                {
                    Shadow.armor[i] = Owner.armor[i];
                    Shadow.armor[i + 10] = Owner.armor[i + 10];
                    Shadow.dye[i] = Owner.dye[i];
                }

                Shadow.position = new Vector2(Owner.position.X + Owner.width, Owner.position.Y + Owner.height * 2 + 16);
                Vector2 shadowCenter = Shadow.position - new Vector2(Shadow.width / 2, Shadow.height / 2);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), shadowCenter, (Main.MouseWorld - shadowCenter).SafeNormalize(Vector2.UnitX) * 30f, ProjectileType<InvertedShadowBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                OnStart = false;
                //Shadow.ResetEffects();
                Shadow.ResetVisibleAccessories();

                Projectile.rotation = (Main.MouseWorld - shadowCenter).ToRotation() + (Owner.direction>0 ? 3.141f : 0);
            }

            //同步这个影子玩家
            Shadow.itemTime = Shadow.itemAnimation = 2;
            Shadow.direction = -Owner.direction;
            Shadow.itemRotation = Owner.itemRotation;
            Shadow.position = new Vector2(Owner.position.X + Owner.width, Owner.position.Y + Owner.height * 2 + 16);
            
            //控制透明度
            if (Projectile.timeLeft < 19)
            {
                float factor = 1 - Projectile.timeLeft / 18f;
                shadow = Helper.BezierEase(factor);
                if (shadow > 1)
                    shadow = 1;

                if (shadow == 0)
                    shadow = 0.001f;
            }

            Shadow.UpdateDyes();
            //Shadow.DisplayDollUpdate();
            Shadow.UpdateSocialShadow();
            Shadow.PlayerFrame();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            //Shadow.skinDyePacked = PlayerDrawHelper.PackShader(Shadow.hairDye, PlayerDrawHelper.ShaderConfiguration.HairShader);
            Main.PlayerRenderer.DrawPlayer(Main.Camera, Shadow, Shadow.position, 3.141f, Shadow.fullRotationOrigin, shadow);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D mainTex = Request<Texture2D>(Texture).Value;
            Vector2 shadowCenter = Shadow.position - new Vector2(Shadow.width / 2, Shadow.height / 2);
            bool ownerDir = Owner.direction > 0;
            SpriteEffects effects = ownerDir ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            lightColor.A = (byte)((1-shadow)*255);
            sb.Draw(mainTex, shadowCenter-Main.screenPosition-Projectile.rotation.ToRotationVector2()* Owner.direction*18f, mainTex.Frame(), lightColor, Projectile.rotation, mainTex.Size() / 2, 1f, effects, 0);
            return false;
        }
    }
}
