using Coralite.Content.Dusts;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Bosses.Originate.Pioneer
{
    //public class Pioneer_Pro : ModProjectile
    //{
    //    public override string Texture => AssetDirectory.Pioneer;

    //    public override void SetStaticDefaults()
    //    {
    //        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    //        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
    //    }
    //    public override void SetDefaults()
    //    {
    //        Projectile.width = 12;
    //        Projectile.height = 12;
    //        Projectile.scale = 1f;
    //        Projectile.timeLeft = 60;
    //        Projectile.aiStyle = -1;
    //        Projectile.penetrate = -1;
    //        Projectile.tileCollide = false;
    //        Projectile.hostile = true;
    //    }
    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
    //        Vector2 Cen = Projectile.Center;
    //        Vector2 Vec = Projectile.velocity;
    //        float Rot = Vec.ToRotation() + MathHelper.Pi / 2f;
    //        Color color = new(35, 160, 200)
    //        {
    //            A = 0
    //        };
    //        for (float i = 30f; i < 2000f; i += 30f)
    //        {
    //            Vector2 Pos = Cen + i * Vec;
    //            Main.spriteBatch.Draw(texture, Pos - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 6, 60)), color, Rot, new Vector2(3f, 30f), new Vector2(Projectile.scale * 1.5f, 1f), SpriteEffects.None, 0f);
    //        }
    //        return false;
    //    }
    //    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    //    {
    //        bool? falg = false;
    //        Vector2 Pos = Projectile.Center;
    //        float num = 0f;
    //        float Leg = 2000f;
    //        if (Collision.CheckAABBvLineCollision(Utils.TopLeft(targetHitbox), Utils.Size(targetHitbox),
    //            Pos + Projectile.velocity * Leg, Pos, 8f, ref num))
    //            falg = true;
    //        return falg;
    //    }
    //    public override void SendExtraAI(BinaryWriter writer)
    //    {
    //        writer.Write(Projectile.localAI[0]);
    //        writer.Write(Projectile.rotation);
    //    }
    //    public override void ReceiveExtraAI(BinaryReader reader)
    //    {
    //        Projectile.localAI[0] = reader.ReadSingle();
    //        Projectile.rotation = reader.ReadSingle();
    //    }
    //    public override void AI()
    //    {
    //        Color color = new(35, 160, 200);
    //        if (Projectile.soundDelay == 0)
    //        {
    //            Projectile.soundDelay = -1;
    //            SoundEngine.PlaySound(SoundID.Item68, Projectile.Center);
    //        }
    //        NPC npc = Main.npc[(int)Projectile.ai[0]];
    //        if(!npc.active || npc.type != ModContent.NPCType<Pioneer>())
    //        {
    //            Projectile.Kill();
    //            return;
    //        }
    //        if (Projectile.localAI[0] == 0f)
    //        {
    //            Projectile.scale = 0f;
    //            Projectile.rotation = Projectile.velocity.ToRotation();
    //            for (int i = 0; i <= 20; i++)
    //            {
    //                Vector2 Vel = Projectile.velocity.RotatedByRandom(0.6f);
    //                int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, color, 1.25f);
    //                Main.dust[dust].position = Projectile.Center;
    //                Main.dust[dust].velocity = Vel;
    //                Main.dust[dust].velocity *= Main.rand.NextFloat(1f) * 16f;
    //                Main.dust[dust].noGravity = true;
    //            }
    //            Projectile.localAI[0] = 1f;
    //            Projectile.netUpdate = true;
    //        }
    //        float Time = 15f;
    //        float Add = 1f / Time;
    //        if (Projectile.timeLeft <= Time) Projectile.scale -= Add;
    //        else if (Projectile.scale < 1f) Projectile.scale += Add;
    //        Projectile.Center = npc.Center + new Vector2(0f, npc.height * 0.05f);
    //        if (npc.ai[1] != 0f)
    //        {
    //            Projectile.rotation += Projectile.ai[2] * Projectile.ai[1] / 60f * Projectile.scale;
    //        }
    //        Projectile.velocity = Projectile.rotation.ToRotationVector2();
    //        for (int i = 0; i < 4; i++)
    //        {
    //            if (Main.rand.NextBool(2))
    //            {
    //                Vector2 Pos = Projectile.position + Projectile.velocity * Main.rand.NextFloat(30f, 2000f);
    //                int a = Dust.NewDust(Pos, 12, 12, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, color, 1.25f);
    //                Main.dust[a].noGravity = true;
    //                Main.dust[a].velocity *= 0.1f;
    //                Main.dust[a].velocity = Projectile.velocity * 4f;
    //            }
    //            else
    //            {
    //                Vector2 Pos = Projectile.position + Projectile.velocity * Main.rand.NextFloat(30f, 2000f);
    //                int a = Dust.NewDust(Pos, 12, 12, ModContent.DustType<GlowDust_Circle>(), 0, 0, 100, color, 0.75f);
    //                Main.dust[a].noGravity = true;
    //                Main.dust[a].velocity *= 0.1f;
    //                Main.dust[a].velocity = Projectile.velocity * 4f;
    //            }
    //        }
    //    }
    //}
}
