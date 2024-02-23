using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class HolyCharm : BaseAccessory, IFlyingShieldAccessory
    {
        public HolyCharm() : base(ItemRarityID.Pink, Item.sellPrice(0, 2))
        { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<Terracrest>())//上位
                
                && incomingItem.type == ModContent.ItemType<HolyCharm>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldLRMeantime = true;
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.parryTime = 6;
        }

        public bool OnParry(BaseFlyingShieldGuard projectile)
        {
            Player Owner = projectile.Owner;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    Owner.immuneTime = 20;
                    Owner.immune = true;
                }

                int damage = (int)(projectile.Projectile.damage * (1.35f - 0.45f * cp.parryTime / 280f));

                SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, projectile.Projectile.Center);
                Helper.PlayPitched("Misc/ShieldGuard", 0.4f, 0f, projectile.Projectile.Center);

                Vector2 dir = projectile.Projectile.rotation.ToRotationVector2();
                int index = projectile.Projectile.NewProjectileFromThis<HolyCharmProj>(Owner.Center - dir * 64, dir * 12,
                     damage, projectile.Projectile.knockBack, Owner.whoAmI);
                Main.projectile[index].scale = projectile.Projectile.scale;

                ParticleOrchestrator.SpawnParticlesDirect(ParticleOrchestraType.Excalibur, new ParticleOrchestraSettings()
                {
                    MovementVector = Vector2.Zero,
                    PositionInWorld = projectile.Projectile.Center,
                });

                if (cp.parryTime < 280)
                    cp.parryTime += 100;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class HolyCharmProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Accessories + "HallowedShield";

        ref float Timer => ref Projectile.ai[0];
        ref float Timer2 => ref Projectile.ai[1];

        Vector2 top;
        Vector2 bottom;
        float scale;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                top, bottom, 40 * Projectile.scale * scale, ref a);
        }

        public override void AI()
        {
            const int MaxTime = 25;

            float factor = Timer / MaxTime;
            float sqrtFactor = Coralite.Instance.SqrtSmoother.Smoother(factor);

            scale = Helper.Lerp(0.8f, 1.2f, sqrtFactor);

            Projectile.velocity *= 0.9f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            float num8 = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
            Vector2 vector2 = Projectile.Center + num8.ToRotationVector2() * 74f * Projectile.scale * scale;
            Vector2 vector3 = num8.ToRotationVector2();
            if (Main.rand.NextFloat() * 2f < Projectile.Opacity)
            {
                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + num8.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale)
                    , 278, vector3 * 1f, 100, Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
                dust2.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
                dust2.noGravity = true;
            }

            if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
                Dust.NewDustPerfect(vector2, 43, vector3 * 1f, 100, Color.White * Projectile.Opacity, 1.2f * Projectile.Opacity);
            Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 32;

            top = pos + dir * 70 * Projectile.scale * scale;
            bottom = pos - dir * 70 * Projectile.scale * scale;
            Timer++;
            Timer2 += 0.085f;
            if (Timer > MaxTime)
                Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Texture2D extraTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "TerraBladeSlash").Value;

            var frameBox = extraTex.Frame(1, 4, 0, 0);

            var mainOrigin = mainTex.Size() / 2;
            var extraOrigin = frameBox.Size() / 2;

            var pos = Projectile.Center - Main.screenPosition;

            SpriteEffects effects = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float num = Projectile.scale * scale;
            float num2 = Timer / 25;
            float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
            float num4 = 0.975f;
            float fromValue = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);
            Color color = new Color(180, 160, 60);
            float rot = Projectile.rotation;

            Main.spriteBatch.Draw(extraTex, pos, frameBox, color * fromValue * num3, rot
                + Timer2 * MathHelper.PiOver4 * -1f * (1f - num2), extraOrigin, num, 0, 0f);
            Color color2 = new Color(255, 240, 150);
            Color color3 = new Color(255, 255, 80);
            Color color4 = Color.White * num3 * 0.5f;
            color4.A = (byte)(color4.A * (1f - fromValue));
            Color color5 = color4 * fromValue * 0.5f;
            color5.G = (byte)(color5.G * fromValue);
            color5.B = (byte)(color5.R * (0.25f + fromValue * 0.75f));
            Main.spriteBatch.Draw(extraTex, pos, frameBox, color5 * 0.15f, rot + Timer2 * 0.04f, extraOrigin, num, effects, 0f);
            Main.spriteBatch.Draw(extraTex, pos, frameBox, color3 * fromValue * num3 * 0.3f, rot, extraOrigin, num, effects, 0f);   
            Main.spriteBatch.Draw(extraTex, pos, frameBox, color2 * fromValue * num3 * 0.5f, rot, extraOrigin, num * num4, effects, 0f);
            Main.spriteBatch.Draw(extraTex, pos, extraTex.Frame(1, 4, 0, 3), Color.White * 0.6f * num3, rot + Timer2 * 0.02f, extraOrigin, num, effects, 0f);
            Main.spriteBatch.Draw(extraTex, pos, extraTex.Frame(1, 4, 0, 3), Color.White * 0.5f * num3, rot + Timer2 * -0.1f, extraOrigin, num * 0.8f, effects, 0f);
            Main.spriteBatch.Draw(extraTex, pos, extraTex.Frame(1, 4, 0, 3), Color.White * 0.4f * num3, rot + Timer2 * -0.2f, extraOrigin, num * 0.6f, effects, 0f);
            //绘制盾牌
            Color shieldColor = color3 * num3;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + Projectile.rotation.ToRotationVector2() * (extraTex.Width * (0.25f + i * 0.1f) - 6f) * num, null
                    , shieldColor * (0.4f + i * 0.2f), Projectile.rotation, mainOrigin, new Vector2(0.8f - num2 * 0.6f, 1) * Projectile.scale*scale * (1f + i*0.9f), effects, 0f);
            }

            //绘制闪光
            for (float num5 = 0f; num5 < 8f; num5 += 1f)
            {
                float num6 = Projectile.rotation + Timer2 * num5 * (MathHelper.Pi * -2.1f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, MathHelper.PiOver4 * Timer2);
                Vector2 drawpos = pos + num6.ToRotationVector2() * (extraTex.Width * 0.5f - 6f) * num;
                float num7 = num5 / 9f;
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None
                    , drawpos, new Color(255, 255, 255, 0) * num3 * num7, color3, num2, 0f, 0.5f, 0.5f, 1f, num6, new Vector2(0f, Utils.Remap(num2, 0f, 1f, 3f, 0f)) * num, Vector2.One * num);
            }

            //绘制星星
            for (int i = 0; i < 3; i++)
            {
                Vector2 drawpos2 = pos + (Projectile.rotation + Timer2 * (-0.4f + i * 0.4f)).ToRotationVector2() * (extraTex.Width * 0.5f - 4f) * num;

                Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None
                    , drawpos2, new Color(255, 255, 255, 0) * num3 * 0.5f, color3, num2
                    , 0f, 0.5f, 0.5f, 1f, 0f
                    , new Vector2(2f, Utils.Remap(num2, 0f, 1f, 3f + i * 0.5f, 1f)) * num
                    , Vector2.One * num);
            }
            return false;
        }
    }
}
