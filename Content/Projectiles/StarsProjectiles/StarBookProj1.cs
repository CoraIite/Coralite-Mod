using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Projectiles.StarsProjectiles
{
    public class StarBookProj1 : BaseChannelProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.StarsProjectiles + "StarBookMagic";

        public ref float Count => ref Projectile.ai[0];
        public float lightScale;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("星之书-星光球");
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;

            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        #region AI

        protected override void AIBefore()
        {
            if (!completeAndRelease)
                Owner.itemTime = Owner.itemAnimation = 2;
        }

        protected override void AIAfter()
        {
            timer++;
        }

        protected override void CompleteAndRelease()
        {
            //控制大球飞出
            if (timer<2)
            {
                Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 18f;
            }


        }

        protected override void OnChannel()
        {
            if (Count > 8)
            {
                OnChannelComplete(1500, 15);
                for (int i = 0; i < 20; i++)
                    Projectile.oldPos[i] = Owner.Center;

            }

            if (timer < 22 && timer % 3 == 0)
            {
                //发射小符文
                float rotate = -1.57f + 0.785f * (timer / 3);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center + rotate.ToRotationVector2() * 80f, Vector2.Zero, ProjectileType<StarBookProj2>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
            }

            Projectile.Center = Owner.Center;
        }

        protected override void OnRelease()
        {
            canChannel = false;

            //控制所有的小符文转状态
            int projType = ProjectileType<StarBookProj2>();
            for (int i = 0; i < 1000; i++)
                if (Main.projectile[i].type == projType && Main.projectile[i].owner == Projectile.owner)
                {
                    Main.projectile[i].ai[0] = 1;
                    Main.projectile[i].velocity = Vector2.Normalize(Main.projectile[i].Center - Owner.Center) * 18f;
                }

            Projectile.Kill();
        }

        #endregion

        #region 绘制

         public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            if (completeAndRelease)
            {
                //绘制拖尾
                DrawTrail();
                //绘制自己
                DrawSelf();
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void DrawTrail()
        {
            
        }

        public void DrawSelf()
        {
            Texture2D mainTex = Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(mainTex.Width / 2, mainTex.Height / 6);

            Rectangle source = new Rectangle(0, 0, 256, 256);       //<---因为知道贴图多大所以就暴力填数字了
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, source,
                                    new Color(255, 254, 191, 255), Projectile.rotation, origin,lightScale, SpriteEffects.None, 0f);
        }

        public void DrawWarp()
        {

        }



        #endregion

        #region 碰撞

        public override bool? CanDamage()
        {
            if (completeAndRelease)
                return true;

            return false;
        }

        #endregion
    }


}
