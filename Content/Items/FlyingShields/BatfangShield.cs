using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class BatfangShield : BaseFlyingShieldItem<BatfangShieldGuard>
    {
        public BatfangShield() : base(Item.sellPrice(0, 0, 5), ItemRarityID.Blue, AssetDirectory.FlyingShieldItems)
        {
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 18;
            Item.shoot = ModContent.ProjectileType<BatfangShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 12.5f;
            Item.damage = 19;
        }
    }

    public class BatfangShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "BatfangShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 32;
        }

        public override void SetOtherValues()
        {
            flyingTime = 17;
            backTime = 12;
            backSpeed = 13.5f;
            trailCachesLength = 6;
            trailWidth = 16 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3, 4) && !Owner.moonLeech && !target.immortal && State == (int)FlyingShieldStates.Shooting)
            {
                float num = damageDone * 0.065f;
                if ((int)num != 0 && !(Owner.lifeSteal <= 0f))
                {
                    Owner.lifeSteal -= num;
                    int num2 = Projectile.owner;
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, 305, 0, 0f, Projectile.owner, num2, num);
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override Color GetColor(float factor)
        {
            return Color.Red * factor;
        }
    }

    public class BatfangShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 34;
            Projectile.height = 32;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.1f;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Fleshy_NPCHit1, Projectile.Center);
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
            Main.spriteBatch.Draw(mainTex, pos - dir * 5, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(2, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 3, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + dir * 7, frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }
}
