using Coralite.Content.ModPlayers;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_HeatanBonus : SpecialEnchant
    {
        public SpecialEnchant_HeatanBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        {
        }

        public override void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.TryGetModPlayer(out MagikePlayer mp) && mp.SpecialEnchantCD <= 0)
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
                Projectile.NewProjectile(source, player.Center - (dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * 64), dir * Main.rand.NextFloat(8, 10), ModContent.ProjectileType<HeatanProj>()
                    , (int)(damage * 0.25f), 2, player.whoAmI, Main.rand.NextFloat(0.05f, 0.15f), Main.rand.NextFloat(-0.1f, 0.1f));
                mp.SpecialEnchantCD = 45;
            }
        }

        public override string Description => Language.GetOrRegister($"Mods.Coralite.Systems.MagikeSystem.HeatanBonus", () => "灼：攻击时生成灼火球").Value;
    }

    public class HeatanProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeProjectiles + Name;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 4 * 60;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Torch, Scale: Main.rand.NextFloat(1.5f, 2.5f));
                d.noGravity = true;
            }
        }

        public override void AI()
        {
            float factor = Projectile.timeLeft / 240f;
            //Projectile.velocity *= 0.998f;

            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1] * MathF.Sin(factor * MathHelper.Pi));
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.UpdateFrameNormally(4, 5);

            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                DustID.Torch, Scale: Main.rand.NextFloat(1, 2));
            d.noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    DustID.Torch, Scale: Main.rand.NextFloat(1, 2));
            }

            SoundEngine.PlaySound(CoraliteSoundID.Flame_Item20, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(6, 1, Projectile.frame, 0);
            var origin = frameBox.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation,
                origin, 1, 0, 0);

            return false;
        }

    }
}
