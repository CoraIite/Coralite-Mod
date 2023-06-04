using System.Collections.Generic;
using Coralite.Content.Buffs.Debuffs;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class SnowdropBud : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SnowdropPetal>(), -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.15f, 0.2f));
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame > 6)
                    Projectile.frame = 0;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.6f, 0.3f));

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff<SnowDebuff>())
            {
                target.DelBuff(target.FindBuffIndex(ModContent.BuffType<SnowDebuff>()));
                if (Main.myPlayer == Projectile.owner)
                {
                    float vel = Main.rand.NextFloat(2.5f, 3.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(0, 16)), Vector2.UnitY * vel, ModContent.ProjectileType<SnowdropBloom>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
                }
                for (int j = 0; j < 6; j++)
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SnowdropPetal>(), Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1f,1f)) * Main.rand.NextFloat(0.5f, 1.5f));

            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Snow, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                    dust.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Snow, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(0.15f, 0.25f), Scale: Main.rand.NextFloat(1.4f, 1.6f));
                dust.noGravity = true;
            }

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(1, 7, 0, Projectile.frame), lightColor, Projectile.rotation, new Vector2(28, 13), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}