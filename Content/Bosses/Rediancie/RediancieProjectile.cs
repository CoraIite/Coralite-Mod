using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    public class Rediancie_Explosion : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 10;
            Projectile.aiStyle = -1;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Helper.PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, Projectile.Center);
            Color red = new Color(221, 50, 50);
            Color grey = new Color(91, 93, 102);
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(6, 6), 0, red, Main.rand.NextFloat(1f, 1.5f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(4, 4), 0, grey, Main.rand.NextFloat(0.8f, 1.2f));
            }
        }

        public override bool PreAI() => false;

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class Rediancie_Strike : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + "RedJadeStrike";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.scale = 2f;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            if (Projectile.velocity.Y < 14)
                Projectile.velocity.Y += 0.08f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, 1f);
                    dust.noGravity = true;
                }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(), lightColor, Projectile.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class Rediancie_Beam : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;

            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.netImportant = true;
            Projectile.scale = 1.6f;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, Projectile.scale);
                    dust.noGravity = true;
                }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class Rediancie_BigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Blank48x48";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 256;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Helper.PlayPitched("RedJade/RedJadeBoom", 0.8f, -1f, Projectile.Center);
            Color red = new Color(221, 50, 50);
            Color grey = new Color(91, 93, 102);
            for (int i = 0; i < 18; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(13, 13), 0, red, Main.rand.NextFloat(1.8f, 2.3f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(9, 9), 0, grey, Main.rand.NextFloat(1.5f, 1.9f));
            }
        }

        public override bool PreAI()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return Vector2.Distance(Projectile.Center, target.Center) < 256;
        }
    }

    public class Rediancie_OnSpawnAnim : ModProjectile
    {
        public override string Texture => AssetDirectory.Rediancie + "RediancieNameLine";

        public Color drawCharColor;
        public Color drawPicColor;
        public readonly Color blankColor = new Color(0, 0, 0, 0);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 260;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;

        public override void OnSpawn(IEntitySource source)
        {
            drawCharColor = new Color(0, 0, 0, 0);
            drawPicColor = new Color(0, 0, 0, 0);
        }

        public override void AI()
        {
            int timer = 260 - Projectile.timeLeft;

            //文字渐出
            if (timer < 21)
            {
                drawCharColor = Color.Lerp(blankColor, Coralite.Instance.RedJadeRed, (float)timer / 20);
                drawPicColor = Color.Lerp(blankColor, Color.White, (float)timer / 20);
                return;
            }

            //闪烁
            if (timer > 119 && timer < 240)
            {
                float r;
                if (timer < 151)
                    r = 0.104f;     //Pi/30
                else if (timer < 171)
                    r = 0.157f;     //Pi/20
                else
                    r = 0.314f;       //Pi/10
                float cosProgress = -MathF.Cos((timer - 120) * r) * 0.5f + 0.5f;
                drawCharColor = Color.Lerp(Coralite.Instance.RedJadeRed, Color.White, cosProgress);
            }

            //生成粒子和声音
            if (timer > 190 && timer % 8 == 0 && Main.netMode != NetmodeID.Server)
            {
                Helper.PlayPitched("RedJade/RedJadeBoom", 0.4f, 0f, Projectile.Center);
                Color red = new Color(221, 50, 50);
                Color grey = new Color(91, 93, 102);
                Vector2 center = Main.LocalPlayer.Center - new Vector2(0, 250) + Main.rand.NextVector2Circular(200, 140);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(6, 6), 0, red, Main.rand.NextFloat(1f, 1.5f));
                    Dust.NewDustPerfect(center, ModContent.DustType<HalfCircleDust>(), Main.rand.NextVector2CircularEdge(4, 4), 0, grey, Main.rand.NextFloat(0.8f, 1.2f));
                }
            }

            //文字减淡
            if (timer > 239)
            {
                drawCharColor = Color.Lerp(Coralite.Instance.RedJadeRed, blankColor, (float)(timer - 240) / 20);
                drawPicColor = Color.Lerp(Color.White, blankColor, (float)(timer - 240) / 20);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Texture2D maintex = TextureAssets.Projectile[Type].Value;
            Vector2 screenPosition = Main.screenPosition;

            Utils.DrawBorderStringBig(sb, "赤玉灵", Main.LocalPlayer.Center - new Vector2(100, 305) - screenPosition, drawCharColor, 1.4f);

            sb.Draw(maintex, Main.LocalPlayer.Center - new Vector2(160, 225) - screenPosition, maintex.Frame(), drawPicColor, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

            Utils.DrawBorderStringBig(sb, "Rediancie", Main.LocalPlayer.Center - new Vector2(75, 180) - screenPosition, drawCharColor, 0.8f);

            //sb.End();
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
