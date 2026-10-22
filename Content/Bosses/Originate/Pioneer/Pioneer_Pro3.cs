using Coralite.Content.Dusts;
using Coralite.Core;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Bosses.Originate.Pioneer
{
    //public class Pioneer_Pro3 : ModProjectile
    //{
    //    public override string Texture => AssetDirectory.Pioneer;

    //    public override void SetStaticDefaults()
    //    {
    //        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    //    }
    //    public override void SetDefaults()
    //    {
    //        Projectile.width = 14;
    //        Projectile.height = 18;
    //        Projectile.scale = 1f;
    //        Projectile.timeLeft = 300;
    //        Projectile.aiStyle = -1;
    //        Projectile.penetrate = -1;
    //        Projectile.tileCollide = false;
    //        Projectile.hostile = true;
    //    }
    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        Color color = Color.Orange;
    //        color.A = 0;
    //        Projectile.ProShadow(color, null, Projectile.scale);
    //        Projectile.EncircleProShadow(2f, Projectile.rotation, null, color, 8, Projectile.scale);
    //        return true;
    //    }
    //    public override void SendExtraAI(BinaryWriter writer)
    //    {
    //        writer.Write(Projectile.localAI[0]);
    //    }
    //    public override void ReceiveExtraAI(BinaryReader reader)
    //    {
    //        Projectile.localAI[0] = reader.ReadSingle();
    //    }
    //    public override void OnKill(int timeLeft)
    //    {
    //        Color color = new(35, 160, 200);
    //        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Pioneer_Pro4>(), Projectile.damage, 0, 0);
    //        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
    //        for (int i = 0; i < 60f; i++)
    //        {
    //            Vector2 Vel = Vector2.One.RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(8f);
    //            if (Main.rand.NextBool(4)) Vel *= 1.5f;
    //            else if (Main.rand.NextBool(8)) Vel *= 2f;
    //            int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, color, 1.5f);
    //            Main.dust[dust].position = Projectile.Center;
    //            Main.dust[dust].velocity = Vel;
    //            Main.dust[dust].noGravity = true;
    //        }
    //        for (int i = 0; i < 30f; i++)
    //        {
    //            Vector2 Vel = Vector2.One.RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(8f);
    //            int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<GlowDust_Circle>(), 0, 0, 100, color, 1.25f);
    //            Main.dust[dust].position = Projectile.Center;
    //            Main.dust[dust].velocity = Vel;
    //            Main.dust[dust].noGravity = false;
    //        }
    //        for (int i = 0; i < 20f; i++)
    //        {
    //            Vector2 Vel = Vector2.One.RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(4f);
    //            int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<FogDust>(), 0, 0, 100, color, 1.25f);
    //            Main.dust[dust].position = Projectile.Center;
    //            Main.dust[dust].velocity = Vel;
    //            Main.dust[dust].noGravity = false;
    //        }
    //    }
    //    public override void AI()
    //    {
    //        Color color = new(35, 160, 200);
    //        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    //        if (Projectile.soundDelay == 0)
    //        {
    //            Projectile.soundDelay = -1;
    //            SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, Projectile.Center);
    //        }
    //        if (Main.rand.NextBool(2))
    //        {
    //            int dust = Dust.NewDust(Projectile.position, 16, 16, ModContent.DustType<GlowDust_Circle>(), 0, 0, 100, color, 1f);
    //            Main.dust[dust].velocity *= 0.1f;
    //            Main.dust[dust].velocity += Projectile.velocity * 0.5f;
    //            Main.dust[dust].noGravity = true;
    //        }
    //        if (Projectile.timeLeft < 300 && Projectile.timeLeft % 15 == 0)
    //        {
    //            OriginateHelper.CircularDust(Projectile.Center, Projectile.rotation, ModContent.DustType<GlowDust_Circle>(), 12, new Vector2(3f, 1f), color, 1f);
    //        }
    //        if (Projectile.localAI[0] == 0f)
    //        {
    //            Projectile.localAI[0] = 1f;
    //            Projectile.netUpdate = true;
    //        }
    //        if (Projectile.ai[0] == 0f)
    //        {
    //            Player player = null;
    //            float MaxDis = 1000f;
    //            foreach (Player Target in Main.ActivePlayers)
    //            {
    //                float Dis = Vector2.Distance(Target.Center, Projectile.Center);
    //                if (Dis < MaxDis)
    //                {
    //                    MaxDis = Dis;
    //                    player = Target;
    //                }
    //            }
    //            if(player != null && Projectile.ai[1] < 45f)
    //            {
    //                Vector2 Vec = (player.Center - Projectile.Center).SafeNormalize(Projectile.velocity);
    //                float Speed = Projectile.velocity.Length();
    //                Projectile.velocity = (Projectile.velocity + Vec * MathHelper.Pi * 0.33f).SafeNormalize(Projectile.velocity) * Speed;
    //            }
    //            Projectile.ai[1] += 1f;
    //        }
    //        if (Projectile.ai[0] == 1f)
    //        {
    //            Projectile.hostile = false;
    //            Vector2 ToPos = new(Projectile.ai[1], Projectile.ai[2]);
    //            Vector2 Vec = (ToPos - Projectile.Center).SafeNormalize(Projectile.velocity);
    //            float Speed = Projectile.velocity.Length();
    //            Projectile.velocity = (Projectile.velocity + Vec * MathHelper.Pi).SafeNormalize(Projectile.velocity) * Speed;
    //            if(Vector2.Distance(Projectile.Center, ToPos) < 50f)
    //            {
    //                Projectile.Kill();
    //                Projectile.netUpdate = true;
    //            }
    //        }
    //        base.AI();
    //    }
    //}
    //public class Pioneer_Pro4 : ModProjectile
    //{
    //    public override void SetStaticDefaults()
    //    {
    //        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    //    }
    //    public override void SetDefaults()
    //    {
    //        Projectile.width = 16;
    //        Projectile.height = 16;
    //        Projectile.scale = 1f;
    //        Projectile.timeLeft = 30;
    //        Projectile.tileCollide = false;
    //        Projectile.hostile = true;
    //        Projectile.penetrate = -1;
    //        Projectile.extraUpdates = 1;
    //    }
    //    public override void OnSpawn(IEntitySource source)
    //    {
    //        Projectile.scale = 0f;
    //        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
    //    }
    //    public override bool CanHitPlayer(Player target)
    //    {
    //        if (target.Hitbox.Distance(Projectile.Center) > Projectile.width * 0.5f && Projectile.timeLeft > 20)
    //        {
    //            return false;
    //        }
    //        return base.CanHitPlayer(target);
    //    }
    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        Color color = new(35, 160, 200, 0);
    //        float Scale = Projectile.width / 256f * Projectile.scale;
    //        Projectile.EncircleProShadow(0, Projectile.rotation, null, Projectile.GetAlpha(color), 1, Scale);
    //        return false;
    //    }
    //    public override void SendExtraAI(BinaryWriter writer)
    //    {
    //        writer.Write(Projectile.width);
    //        writer.Write(Projectile.height);
    //        writer.Write(Projectile.scale);
    //    }
    //    public override void ReceiveExtraAI(BinaryReader reader)
    //    {
    //        Projectile.width = reader.ReadInt32();
    //        Projectile.height = reader.ReadInt32();
    //        Projectile.scale = reader.ReadSingle();
    //    }
    //    public override void AI()
    //    {
    //        int Size = 200;
    //        if (Projectile.ai[1] == 0f)
    //        {
    //            Projectile.ai[1] = 1f;
    //            Projectile.Resize(Size, Size);
    //            Projectile.netUpdate = true;
    //        }
    //        Projectile.scale = MathHelper.Lerp(Projectile.scale, 1.25f, 0.2f);
    //        if (Projectile.alpha < 255)
    //            Projectile.alpha += 10;
    //        else
    //            Projectile.Kill();
    //    }
    //}
}
