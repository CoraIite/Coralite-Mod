using Coralite.Content.Items.Shadow;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    public class DemonsProtection : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public DemonsProtection() : base(ItemRarityID.Orange, Item.sellPrice(0, 0, 80))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 65;
            Item.DamageType = DamageClass.Generic;
            Item.defense = 2;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<Terracrest>()//上位
                || equippedItem.type == ModContent.ItemType<AmberAmulet>())

                && incomingItem.type == ModContent.ItemType<DemonsProtection>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.parryTime = 6;
            projectile.strongGuard += 0.20f;
            projectile.damageReduce *= 1.1f;
            projectile.distanceAdder *= 1.1f;
        }

        public bool OnParry(BaseFlyingShieldGuard projectile)
        {
            Player Owner = projectile.Owner;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    Owner.AddImmuneTime(ImmunityCooldownID.General, 25);
                }

                int damage = (int)(projectile.Owner.GetWeaponDamage(Item) * (1.2f - (0.15f * cp.parryTime / 280f)));

                SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, projectile.Projectile.Center);
                Helper.PlayPitched("Misc/ShieldGuard", 0.4f, 0f, projectile.Projectile.Center);

                Vector2 dir = projectile.Projectile.rotation.ToRotationVector2();

                for (int i = 0; i < 8; i++)
                {
                    int index = Dust.NewDust(projectile.Projectile.position, projectile.Projectile.width, projectile.Projectile.height, DustID.Shadowflame,
                        Scale: Main.rand.NextFloat(1f, 2f));
                    Main.dust[index].velocity = dir.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(2f, 5f);
                    Main.dust[index].noGravity = true;
                }

                int dir2 = Main.rand.NextFromList(1, -1);
                Vector2 vector54 = new Vector2(Owner.direction, dir2 * 4f).SafeNormalize(Vector2.UnitY).RotatedBy((float)Math.PI * 2f * Main.rand.NextFloatDirection() * 0.08f);
                Vector2 searchCenter = Owner.MountedCenter + (dir * 35) + (vector54 * -35f);
                searchCenter += Main.rand.NextVector2Circular(20f, 20f);

                dir2 *= projectile.Owner.direction;
                projectile.Projectile.NewProjectileFromThis<DemonsProtectionClaw>(searchCenter, vector54 * 7.5f
                    , damage, projectile.Projectile.knockBack, 1, dir2);
                projectile.Projectile.NewProjectileFromThis<DemonsProtectionClaw>(searchCenter + (dir * 25), vector54 * 5f
                    , damage, projectile.Projectile.knockBack, 2, dir2);
                projectile.Projectile.NewProjectileFromThis<DemonsProtectionClaw>(searchCenter + (dir * 25 * 2), vector54 * 7.5f
                    , damage, projectile.Projectile.knockBack, 1, dir2);

                if (cp.parryTime < 280)
                    cp.parryTime += 100;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SnowflakeCharm>()
                .AddIngredient<AmberAmulet>()
                .AddIngredient<JungleTurtleShell>()
                .AddIngredient(ItemID.Bone, 20)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient<SnowflakeCharm>()
                .AddIngredient<AmberAmulet>()
                .AddIngredient<JungleTurtleShell>()
                .AddIngredient<ShadowCrystal>(8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class DemonsProtectionClaw : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.ArmorPenetration = 5; // Added by TML
            Projectile.DamageType = DamageClass.Melee;

            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.LightsBane;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 30;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float f2 = Projectile.rotation;
            float collisionPoint3 = 0f;
            float num11 = 46f * Projectile.scale;
            float num12 = 8f;
            Vector2 vector6 = f2.ToRotationVector2();
            Rectangle hitbox = Projectile.Hitbox;
            hitbox.Inflate((int)num11, (int)num11);
            if (hitbox.Intersects(targetHitbox) && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - (vector6 * num11), Projectile.Center + (vector6 * num11), num12 * Projectile.scale, ref collisionPoint3))
                return true;

            return false;
        }

        public override void AI()
        {
            Projectile.scale = Projectile.ai[0];
            Projectile.localAI[0] += 1f;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 12)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.velocity = Projectile.velocity.RotatedBy(0.03f * Projectile.ai[1]);
            Projectile.rotation = Projectile.velocity.ToRotation();
            float f = Projectile.rotation;
            float num = 46f * Projectile.scale;
            Vector2 vector = f.ToRotationVector2();
            float num2 = Projectile.localAI[0] / 36f * 3f;
            if (num2 >= 0f && num2 <= 1.5f)
            {
                Dust dust = Dust.NewDustPerfect(Vector2.Lerp(Projectile.Center - (vector * num), Projectile.Center + (vector * num), Projectile.localAI[0] / 36f), DustID.FireworksRGB, vector.RotatedBy((float)Math.PI * 2f * Main.rand.NextFloatDirection() * 0.02f) * 6f * Main.rand.NextFloat(), 0
                    , Main.rand.NextBool(3) ? new Color(60, 0, 150) : new Color(150, 0, 40), 0.6f * num2);
                dust.noGravity = true;
                dust.noLight = dust.noLightEmittence = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();

            BlendState multiplyBlendState = EffectLoader.ReverseBlendState;
            spriteBatch.Begin(SpriteSortMode.Deferred, multiplyBlendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Main.instance.LoadProjectile(ProjectileID.LightsBane);
            Asset<Texture2D> asset = TextureAssets.Projectile[ProjectileID.LightsBane];
            Rectangle rectangle = asset.Frame(1, 13, 0, Projectile.frame);
            Vector2 vector = rectangle.Size() / 2f;
            Vector2 vector2 = new Vector2(0.7f, 0.7f) * Projectile.scale;
            float num = Utils.Remap(Projectile.frame, 0f, 3f, 0f, 1f) * Utils.Remap(Projectile.frame, 4f, 12f, 1f, 0f);
            Rectangle value = asset.Frame(1, 13, 0, 12);
            Vector2 origin = vector + new Vector2(0f, 0f);
            Color c = new(255, 0, 155, 255);

            spriteBatch.Draw(asset.Value, position, value, Color.White * 0.4f * num, Projectile.rotation, origin, new Vector2(1f, 6f) * vector2, SpriteEffects.None, 0f);
            spriteBatch.Draw(asset.Value, position, value, Color.White * 0.4f * num, Projectile.rotation, origin, new Vector2(1.75f, 2f) * vector2, SpriteEffects.None, 0f);
            spriteBatch.Draw(asset.Value, position, rectangle, Color.White, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            spriteBatch.Draw(asset.Value, position, rectangle, Color.White, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            spriteBatch.Draw(asset.Value, position, rectangle, Color.White, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            spriteBatch.Draw(asset.Value, position, rectangle, Color.White, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            spriteBatch.Draw(asset.Value, position, rectangle, c, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);
            spriteBatch.Draw(asset.Value, position, rectangle, c, Projectile.rotation, vector, vector2, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return false;
        }
    }
}
