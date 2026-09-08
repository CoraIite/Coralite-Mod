using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlowerGunChapter;
using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class EternalBloom : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlowerGunKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunCollect>();

        public override void SetDefaults()
        {
            Item.SetWeaponValues(66, 5.5f);
            Item.DefaultToRangedWeapon(ProjectileType<EternalBloomHeldProj>(), AmmoID.Bullet, 11, 14f, true);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Lime7, Item.sellPrice(0, 7));

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Gun3_Item41;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -8);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<EternalBloomHeldProj>(), 0, knockback, player.whoAmI);
            if (type == ProjectileID.Bullet)
            {
                type = Main.rand.NextBool(4) ? ProjectileType<ThornBall>() : ProjectileType<SeedPlantera>();

                Projectile.NewProjectile(source, position, velocity
                     , type, damage, knockback, player.whoAmI);
                return false;
            }
            else if (Main.rand.NextBool(4))
            {
                Projectile.NewProjectile(source, position, velocity
                     , ProjectileType<ThornBall>(), damage, knockback, player.whoAmI);
                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<Floette>()
            .AddIngredient<RegrowthTentacle>(5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }

    [VaultLoaden(AssetDirectory.HyacinthSeriesItems)]
    public class EternalBloomHeldProj : BaseGunHeldProj
    {
        public EternalBloomHeldProj() : base(0.2f, 16, -8, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex EternalBloomFire { get; private set; }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.6f, 0.1f));
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 4)
                return false;

            Texture2D effect = EternalBloomFire.Value;
            Rectangle frameBox = effect.Frame(1, 5, 0, Projectile.frame);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 38 + n.ToRotationVector2() * 8 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }

    public class SeedPlantera : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_275";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SeedPlantera);
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.UpdateFrameNormally(3, 1);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.WoodFurniture);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 60 * 3);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            var pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 2, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, new Color(255, 200, 100, 0), Projectile.rotation + 1.57f,
                origin, Projectile.scale * 1.3f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation + 1.57f,
                origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class ThornBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_277";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ThornBall);
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 16)
                Projectile.velocity.Y += 0.3f;

            Projectile.rotation += Projectile.velocity.X / 20;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0]++;
            Projectile.netUpdate = true;

            //简易撞墙反弹
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -Math.Sign(oldVelocity.X) * oldVelX * 0.8f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -Math.Sign(oldVelocity.Y) * oldVelY * 0.8f;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.position -= oldVelocity;
            return Projectile.ai[0] > 5;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 60 * 3);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(new Color(255, 200, 100, 0), Projectile.scale * 1.2f, 0);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }
}
