using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Shadow
{
    public class DrawInvertedPlayerProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.ShadowItems + "InvertedShadow";
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public Player Shadow;

        public ref float DrawPlayerOffsetY => ref Projectile.localAI[1];

        public float shadow = 0.001f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 25;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        public override void Initialize()
        {
            //初始化这个影子玩家
            Shadow = new Player();
            Shadow.hair = Owner.hair;
            Shadow.skinColor = Owner.skinColor;
            Shadow.skinVariant = Owner.skinVariant;
            Shadow.Male = Owner.Male;

            for (int i = 0; i < 3; i++)
            {
                Shadow.armor[i] = Owner.armor[i];//装备
                Shadow.armor[i + 10] = Owner.armor[i + 10];//外观装备
                Shadow.dye[i] = Owner.dye[i];
            }

            int worldX = (int)(Owner.Bottom.X / 16);
            int worldY = (int)(Owner.Bottom.Y / 16);
            Tile tile = Main.tile[worldX, worldY];

            if (tile.HasTile)
            {
                if (TileID.Sets.Platforms[tile.TileType])
                    DrawPlayerOffsetY = 8;
                else
                    DrawPlayerOffsetY = 16;
            }
            else
                DrawPlayerOffsetY = 0;

            Shadow.position = new Vector2(Owner.position.X + Owner.width, Owner.position.Y + (Owner.height * 2) + DrawPlayerOffsetY);
            Vector2 shadowCenter = Shadow.position - new Vector2(Shadow.width / 2, Shadow.height / 2);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), shadowCenter, (Main.MouseWorld - shadowCenter).SafeNormalize(Vector2.UnitX) * 15f, ProjectileType<InvertedShadowBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            //Shadow.ResetEffects();    //<--由于不明原因可能会导致闪退，原因是什么初始化海盗船失败，总之很莫名其妙所以还是别用了吧
            Shadow.ResetVisibleAccessories();

            Projectile.velocity *= 0;
            Projectile.rotation = (Main.MouseWorld - shadowCenter).ToRotation() + (Owner.direction > 0 ? 3.141f : 0);
        }

        public override void AI()
        {
            //同步这个影子玩家
            Shadow.itemTime = Shadow.itemAnimation = 2;
            Shadow.direction = -Owner.direction;
            Shadow.itemRotation = Owner.itemRotation;
            Shadow.position = new Vector2(Owner.position.X + Owner.width, Owner.position.Y + (Owner.height * 2) + DrawPlayerOffsetY);

            //控制透明度
            if (Projectile.timeLeft < 19)
            {
                float factor = 1 - (Projectile.timeLeft / 18f);
                shadow = Helper.BezierEase(factor);
                if (shadow > 1)
                    shadow = 1;

                if (shadow == 0)
                    shadow = 0.001f;    //<--这里始终设置为0.001是因为shadow为0的时候会绘制玩家头部，一旦它不为0瞬间就不绘制了，效果很诡异所以还是一直别绘制了吧
            }

            Shadow.UpdateDyes();
            //Shadow.DisplayDollUpdate();
            Shadow.UpdateSocialShadow();
            Shadow.PlayerFrame();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            //这里end begin是为了应用盔甲染料的shader
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            //Shadow.skinDyePacked = PlayerDrawHelper.PackShader(Shadow.hairDye, PlayerDrawHelper.ShaderConfiguration.HairShader);
            Main.PlayerRenderer.DrawPlayer(Main.Camera, Shadow, Shadow.position, MathHelper.Pi, Shadow.fullRotationOrigin, shadow);

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D mainTex = Request<Texture2D>(Texture).Value;
            Vector2 shadowCenter = Shadow.position - new Vector2(Shadow.width / 2, Shadow.height / 2);
            SpriteEffects effects = Owner.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            lightColor.A = (byte)((1 - shadow) * 255);

            sb.Draw(mainTex, shadowCenter - Main.screenPosition - (Projectile.rotation.ToRotationVector2() * Owner.direction * 18f), mainTex.Frame(), lightColor, Projectile.rotation, mainTex.Size() / 2, 1f, effects, 0);
            return false;
        }
    }
}
