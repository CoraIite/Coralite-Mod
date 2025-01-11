using Coralite.Content.DamageClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Systems.DigSystem
{
    public class ThrownPickaxe:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float TexType => ref Projectile.ai[0];
        public ref float Width => ref Projectile.ai[1];

        private bool init = true;

        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.DamageType = CreatePickaxeDamage.Instance;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (init)
            {
                Projectile.Resize((int)Width, (int)Width);

                init = false;
            }
        }

        /// <summary>
        /// 初始化各种值
        /// </summary>
        public virtual void Initialize()
        {

        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 16;
            height = 16;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public virtual void DrawPickaxe(Color lightColor)
        {
            int itemType = (int)TexType;
            Main.instance.LoadItem(itemType);

            Texture2D mainTex = TextureAssets.Item[itemType].Value;
            Rectangle frameBox;

            if (Main.itemAnimations[itemType] != null)
                frameBox = Main.itemAnimations[itemType].GetFrame(mainTex, -1);
            else
                frameBox = mainTex.Frame();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
        }
    }
}
