using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Fishronguard : BaseFlyingShieldItem<FishronguardGuard>
    {
        public Fishronguard() : base(Item.sellPrice(0, 5), ItemRarityID.Yellow, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 18;
            Item.shoot = ModContent.ProjectileType<FishronguardProj>();
            Item.knockBack = 4.5f;
            Item.shootSpeed = 15;
            Item.damage = 77;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DukeFishronSkin>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class FishronguardProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Fishronguard";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void OnShootDusts()
        {
            if (Timer % 9 == 0)
            {
                Projectile.NewProjectileFromThis(Projectile.Center, Helper.NextVec2Dir(1, 4), ProjectileID.FlaironBubble,
                    Projectile.damage, Projectile.knockBack, -10);
            }

            for (int i = 0; i < 3; i++)
                Projectile.SpawnTrailDust(32f, Main.rand.NextBool(3) ? DustID.BlueTorch : DustID.Water, Main.rand.NextFloat(0.1f, 0.6f),
                    Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void OnBackDusts()
        {
            if (Timer < 60 && Timer % 12 == 0)
            {
                Projectile.NewProjectileFromThis(Projectile.Center, Helper.NextVec2Dir(1, 4), ProjectileID.FlaironBubble,
                    (int)(Projectile.damage * 0.8f), Projectile.knockBack, -10);
            }

            for (int i = 0; i < 3; i++)
                Projectile.SpawnTrailDust(32f, Main.rand.NextBool(3) ? DustID.BlueTorch : DustID.Water, Main.rand.NextFloat(0.1f, 0.6f),
                    Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void SetOtherValues()
        {
            flyingTime = 24;
            backTime = 12;
            backSpeed = 17;
            trailCachesLength = 9;
            trailWidth = 24 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(39, 100, 104) * factor;
        }
    }

    public class FishronguardGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.2f;
            distanceAdder = 3;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.DukeFishron_NPCHit14, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.7f;
            c.A = lightColor.A;
            Color c2 = lightColor * 0.5f;
            c2.A = lightColor.A;

            frameBox = mainTex.Frame(3, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - dir * 5, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 8, frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 12, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 17, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 22, frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 30, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 38, frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }
}
