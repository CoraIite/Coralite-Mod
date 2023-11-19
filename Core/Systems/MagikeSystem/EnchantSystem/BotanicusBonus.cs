using Coralite.Content.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.MagikeSystem.EnchantSystem
{
    public class SpecialEnchant_BotanicusBonus : SpecialEnchant
    {
        public SpecialEnchant_BotanicusBonus(Enchant.Level level, float bonus0, float bonus1, float bonus2) : base(level, bonus0, bonus1, bonus2)
        { }

        public override void Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.TryGetModPlayer(out MagikePlayer mp) && mp.SpecialEnchantCD <= 0)
            {
                float rot = Main.rand.NextFloat(6.282f);
                Projectile.NewProjectile(source, player.Center + rot.ToRotationVector2(), Vector2.Zero, ModContent.ProjectileType<LeafShield>()
                    , (int)(damage * 0.4f), 4, player.whoAmI, rot);
                mp.SpecialEnchantCD = 60;
            }
        }

        public override string Description => Language.GetOrRegister($"Mods.Coralite.Systems.MagikeSystem.BotanicusBonus", () => "植生：攻击时生成叶刃环绕自身").Value;
    }

    public class LeafShield : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeProjectiles + Name;
        public ref float Rot => ref Projectile.ai[0];
        public ref float Distance => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;

            Projectile.timeLeft = 5 * 60;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            float factor = (5 * 60f - Projectile.timeLeft) / (5 * 60f);

            Distance = 32 + MathF.Sin(factor * MathHelper.Pi) * 100;
            Rot += MathHelper.TwoPi / (2 * 60);

            Projectile.Center = owner.Center + Rot.ToRotationVector2() * Distance;
            Projectile.rotation += 0.15f;

            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                Main.rand.NextBool() ? DustID.Grass : DustID.JungleGrass);
            d.noGravity = true;

            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 4)
                    Projectile.frame = 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
                    Main.rand.NextBool() ? DustID.Grass : DustID.JungleGrass);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            var frameBox = mainTex.Frame(1, 5, 0, Projectile.frame);
            var origin = new Vector2(0, frameBox.Height / 2);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation + i * MathHelper.TwoPi / 3,
                    origin, 1, 0, 0);
            }
            return false;
        }
    }
}
