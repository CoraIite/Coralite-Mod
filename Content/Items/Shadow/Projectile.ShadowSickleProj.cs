using System;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Shadow
{
    /// <summary>
    /// 1.旋转速度由慢到快
    /// 2.蓄力到一定程度后松手释放斩击
    /// 3.斩击伤害和大小根据蓄力时间决定
    /// </summary>
    public class ShadowSickleProj : BaseChannelProj
    {
        public override string Texture => AssetDirectory.ShadowItems + "ShadowSickle";

        public bool colorIn = true;

        public ref float Scale => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.height = 40;
            Projectile.width = 40;
            Projectile.localNPCHitCooldown = 15;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            _Rotation = -1.2f;
            //Projectile.rotation = _Rotation + Owner.direction * (-0.4f + 0.785f);
        }

        protected override void AIBefore()
        {
            if (!completeAndRelease)
                Owner.itemTime = Owner.itemAnimation = 2;//这个东西不为0的时候就无法使用其他物品
            Owner.heldProj = Projectile.whoAmI;
        }

        protected override void AIMiddle()
        {
            Projectile.Center = Owner.MountedCenter + _Rotation.ToRotationVector2() * 40;
            Owner.itemRotation = Owner.direction > 0 ? _Rotation : _Rotation + 3.141f;
        }

        protected override void OnChannel()
        {
            //最大蓄力时间：4秒
            float attackSpeed = Math.Clamp(Owner.itemTimeMax, 15, 40) - 15;

            float factor = timer / (210f - 90f * (1 - (attackSpeed / 25f)));
            if (factor > 1)
                factor = 1;

            _Rotation += Owner.direction * (0.15f + 0.2f * factor);
            Projectile.rotation = _Rotation + Owner.direction * 0.65f;
            if (_Rotation > 6.282f || _Rotation < -6.282f)
            {
                _Rotation = _Rotation % 6.282f;
                SoundEngine.PlaySound(CoraliteSoundID.Slash_Item71, Projectile.Center);
            }

            timer++;
        }

        protected override void OnRelease()
        {
            float attackSpeed = Math.Clamp(Owner.itemTimeMax, 15, 40) - 15;

            float factor = timer / (210f - 90f * (1 - (attackSpeed / 25f)));
            if (factor > 1)
                factor = 1f;
            if (factor < 0.5f)
            {
                Projectile.Kill();
                return;
            }

            OnChannelComplete(45, 45);
            Scale = 0.2f + 0.8f * factor;
            Projectile.damage = (int)(Projectile.damage * (1f + factor * 3f));
            PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, _Rotation.ToRotationVector2(), Scale*6f, 8f, 10, 1000f, "ShadowSickle");
            Main.instance.CameraModifiers.Add(modifier);

            Projectile.netUpdate = true;
        }

        protected override void CompleteAndRelease()
        {
            if (colorIn)
            {
                Alpha += 0.1f;
                if (Alpha > 1f)
                {
                    Alpha = 1f;
                    colorIn = false;
                }
            }

            _Rotation += Owner.direction * 0.4f;
            Projectile.rotation = _Rotation + Owner.direction * 0.65f;
            Projectile.Center = Owner.MountedCenter + _Rotation.ToRotationVector2() * 30;
            Owner.itemRotation = Owner.direction > 0 ? _Rotation : _Rotation + 3.141f;

            Vector2 rotateDir = _Rotation.ToRotationVector2();
            if (_Rotation > 6.282f || _Rotation < -6.282f)
            {
                _Rotation = _Rotation % 6.282f;
                SoundEngine.PlaySound(CoraliteSoundID.Slash_Item71, Projectile.Center);
                PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, rotateDir, Scale*6f, 8f, 10, 1000f, "ShadowSickle");
                Main.instance.CameraModifiers.Add(modifier);
            }

            Vector2 dir = rotateDir.RotatedBy(Owner.direction * 1.57f);
            int width = 20 + (int)(Scale * 80);
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + rotateDir * Main.rand.Next(-20, width), DustID.Granite, dir * Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true;
            }

            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + rotateDir * Main.rand.Next(-20, width), DustID.Shadowflame, dir * Main.rand.NextFloat(1f, 2f));
            dust2.noGravity = true;

            if (Projectile.timeLeft < 16)
            {
                Alpha -= 0.066f;
                Scale -= 0.01f;
                if (Alpha < 0)
                    Alpha = 0;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CanHitLine(Owner.Center, 1, 1, targetHitbox.Center.ToVector2(), 1, 1))
            {
                float a = 0f;
                if (completeAndRelease)
                    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + _Rotation.ToRotationVector2() * Scale * 120, 100, ref a);

                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + _Rotation.ToRotationVector2() * 55, Projectile.width, ref a);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 center = Projectile.Center - Main.screenPosition;

            if (completeAndRelease)
            {
                Asset<Texture2D> slashTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "TerraBladeSlash");
                Rectangle rectangle = slashTex.Frame(1, 4);
                Vector2 origin = rectangle.Size() / 2f;
                SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

                float num = Scale * 1.1f;
                float num2 = timer / 50f/*(float)Owner.itemTimeMax*/;
                float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
                float num4 = 0.975f;
                float fromValue = lightColor.ToVector3().Length() / (float)Math.Sqrt(3.0);
                fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);
                Color color = new Color(40, 20, 60);
                spriteBatch.Draw(slashTex.Value, center, rectangle, color * fromValue * num3, _Rotation + Owner.direction * ((float)Math.PI / 4f) * -1f * (1f - num2), origin, num * num4, effects, 0f);
                Color color2 = new Color(80, 40, 180);
                Color color3 = Color.White * num3 * 0.5f;
                color3.A = (byte)(color3.A * (1f - fromValue));
                Color color4 = color3 * fromValue * 0.5f;
                color4.G = (byte)(color4.G * fromValue);
                color4.R = (byte)(color4.R * (0.25f + fromValue * 0.75f));
                spriteBatch.Draw(slashTex.Value, center, rectangle, color4 * 0.15f, _Rotation + Owner.direction * 0.01f, origin, num, effects, 0f);
                spriteBatch.Draw(slashTex.Value, center, rectangle, new Color(80, 30, 160) * fromValue * Alpha * 0.3f, _Rotation, origin, num * 0.8f, effects, 0f);
                spriteBatch.Draw(slashTex.Value, center, rectangle, color2 * fromValue * Alpha * 0.7f, _Rotation, origin, num * num4, effects, 0f);

                spriteBatch.Draw(slashTex.Value, center, slashTex.Frame(1, 4, 0, 3), Color.White * 0.3f * Alpha * (1f - fromValue * 0.7f), _Rotation + Owner.direction * 0.01f, origin, num, effects, 0f);
                Vector2 drawpos = center + (_Rotation + Utils.Remap(num2, 0f, 1.4f, 0f, (float)Math.PI / 2f) * Owner.direction).ToRotationVector2() * (slashTex.Width() * 0.5f - 4f) * num;
                ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawpos, new Color(255, 255, 255, 0) * num3 * 0.5f, color2, num2, 0f, 0.5f, 0.5f, 1f, (float)Math.PI / 4f, new Vector2(2f, 2f), Vector2.One);
            }

            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            SpriteEffects effect = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, new Vector2(20, 19), 1.2f, effect, 0f);

            return false;
        }
    }
}