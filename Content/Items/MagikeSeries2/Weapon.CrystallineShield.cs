using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineShield : BaseFlyingShieldItem<CrystallineShieldGuard>
    {
        public CrystallineShield() : base(Item.sellPrice(0, 1, 50), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<CrystallineShieldProj>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 16;
            Item.damage = 50;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineEngram>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }
    }

    public class CrystallineShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            flyingTime = 16;
            backTime = 22;
            backSpeed = 17;
            trailCachesLength = 6;
            trailWidth = 30 / 2;
        }

        public override void OnShootDusts()
        {
        }

        public override Color GetColor(float factor)
        {
            return new Color(32, 180, 186, 0) * factor;
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var pos = Projectile.Center - Main.screenPosition;

            Rectangle frameBox = mainTex.Frame(2, 1, 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation - 1.57f + extraRotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
        }
    }

    public class CrystallineShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 44;
            Projectile.height = 54;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(2, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(2, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 10), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 15), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }
}
