using Coralite.Content.Dusts;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Bosses.Originate.Pioneer
{
    //public class Pioneer_Pro5 : ModProjectile
    //{
    //    public override string Texture => AssetDirectory.Pioneer;

    //    public override void Load()
    //    {
    //        string Name = "Pioneer_Pro5_Extra";
    //        OriginateMod.Extra.Add(Name, ModContent.Request<Texture2D>("OriginateMod/Content/NPCs/Pioneer/" + Name));
    //    }
    //    public override void SetStaticDefaults()
    //    {
    //        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    //    }
    //    public override void SetDefaults()
    //    {
    //        Projectile.width = 40;
    //        Projectile.height = 40;
    //        Projectile.scale = 1f;
    //        Projectile.timeLeft = 60 + 600;
    //        Projectile.penetrate = 1;
    //        Projectile.aiStyle = -1;
    //        Projectile.extraUpdates = 1;
    //        Projectile.tileCollide = false;
    //    }
    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        {
    //            Color color = Color.Orange;
    //            color.A = 0;
    //            Projectile.ProShadow(color, null, Projectile.scale);
    //            Projectile.EncircleProShadow(0f, Projectile.rotation, null, lightColor, 1, Projectile.scale);
    //        }
    //        {
    //            float MiltAI1 = Projectile.ai[1] / 60f;
    //            MiltAI1 *= MiltAI1;
    //            Color color = new(35, 160, 200);
    //            color *= MiltAI1;
    //            color.A = 0;
    //            float Scale = 1f;
    //            Texture2D Extra = OriginateMod.Extra.GetValueOrDefault(GetType().Name + "_Extra").Value;
    //            Color White = Color.White;
    //            White *= MiltAI1;
    //            Projectile.EncircleProShadow(0f, Projectile.rotation, Extra, White, 1, Projectile.scale);
    //            if (Projectile.ai[1] < 30f)
    //            {
    //                Scale = Projectile.ai[1] / 30f;
    //            }
    //            else if (Projectile.ai[1] > 40f)
    //            {
    //                Scale = 1f - (Projectile.ai[1] - 40f) / 20f;
    //            }
    //            for (int i = 0; i < 2; i++)
    //            {
    //                Texture2D Tex = OriginateMod.Extra.GetValueOrDefault("Extra_Strike").Value;
    //                Rectangle Rect = new(0, 0, Tex.Width, Tex.Height);
    //                Vector2 Offset = -Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
    //                Main.spriteBatch.Draw(Tex, Projectile.Center + Offset, Rect, color, Projectile.localAI[0], Rect.Size() / 2f, new Vector2(1f, 3f) * Scale, SpriteEffects.None, 0f);
    //                Main.spriteBatch.Draw(Tex, Projectile.Center + Offset, Rect, color, Projectile.localAI[0], Rect.Size() / 2f, new Vector2(3f, 1f) * Scale, SpriteEffects.None, 0f);
    //            }
    //            for (int i = 0; i < 3; i++)
    //            {
    //                Texture2D Tex = TextureAssets.GlowMask[239].Value;
    //                Rectangle Rect = new(0, 0, Tex.Width, Tex.Height);
    //                Vector2 Offset = -Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
    //                Scale = 1f - MiltAI1;
    //                Main.spriteBatch.Draw(Tex, Projectile.Center + Offset, Rect, color, 0f, Rect.Size() / 2f, Scale * (1f + i * 0.5f) * 2f, SpriteEffects.None, 0f);
    //            }
    //        }
    //        if (Projectile.ai[0] == 0f)
    //        {
    //            Texture2D Tex = TextureAssets.Extra[9].Value;
    //            Rectangle Rect = new(0, 0, Tex.Width, Tex.Height);
    //            Vector2 Offset = Main.screenPosition + new Vector2(0f, Projectile.height);
    //            Main.spriteBatch.Draw(Tex, Projectile.Center - Offset, Rect, Color.White, 0f, Rect.Size() / 2f, 1f, SpriteEffects.None, 0f);
    //            Tex = TextureAssets.GlowMask[239].Value;
    //            Rect = new(0, 0, Tex.Width, Tex.Height);
    //            Offset = -Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
    //            float Scale = MathHelper.Clamp(Projectile.Originate().TimeLeft * 5f, 0f, 500f) / 96f * 1.25f;
    //            if (Main.getGoodWorld) Scale *= 3f;
    //            Color color = new(35, 160, 200);
    //            color *= 0.1f;
    //            color.A = 0;
    //            Main.spriteBatch.Draw(Tex, Projectile.Center + Offset, Rect, color, 0f, Rect.Size() / 2f, Scale, SpriteEffects.None, 0f);
    //        }
    //        return false;
    //    }
    //    public override void AI()
    //    {
    //        if (Projectile.soundDelay == 0)
    //        {
    //            Projectile.soundDelay = -1;
    //            SoundEngine.PlaySound(SoundID.Item61, Projectile.Center);
    //        }
    //        Color color = new(35, 160, 200);
    //        if (Main.rand.NextBool(3))
    //        {
    //            int D = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowDust_Circle>(), 0, 0, 100, color, 1f);
    //            Main.dust[D].velocity *= 0.1f;
    //            Main.dust[D].velocity += Projectile.velocity * 0.5f;
    //            Main.dust[D].noGravity = true;
    //        }
    //        Projectile.spriteDirection = -Projectile.direction;
    //        Projectile.rotation += Projectile.velocity.X * 0.05f;
    //        Projectile.velocity *= 0.98f;
    //        if (Projectile.timeLeft <= 60)
    //        {
    //            float Mult = 1f - Projectile.ai[1] / 60f;
    //            Projectile.localAI[0] += 0.3f * Mult * Mult;
    //            Projectile.ai[1] += 1f;
    //        }
    //        else if (Projectile.ai[0] == 0f)
    //        {
    //            foreach (Projectile pro in Main.ActiveProjectiles)
    //            {
    //                if (pro.DamageType.CountsAsClass(DamageClass.Melee) && pro.Originate().TrueMeleePro)
    //                {
    //                    Player player = Main.player[pro.owner];
    //                    if (player.active && !player.dead)
    //                    {
    //                        if (pro.Colliding(pro.Hitbox, Projectile.Hitbox))
    //                        {
    //                            Projectile.ai[0] = 1f;
    //                            Vector2 Vec = (player.Originate().MouseCen - player.Center).SafeNormalize(-Projectile.velocity);
    //                            Projectile.damage = 100;
    //                            Projectile.velocity = Vec * 10f;
    //                            Projectile.friendly = true;
    //                            Projectile.timeLeft = 300;
    //                            Projectile.netUpdate = true;
    //                            return;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        base.AI();
    //    }
    //    public override void OnKill(int timeLeft)
    //    {
    //        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero,
    //            ModContent.ProjectileType<Pioneer_Pro6>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
    //        SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
    //        Color color = new(35, 160, 200, 0);
    //        float Stack = 20f;
    //        if (Main.getGoodWorld) Stack = 40f;
    //        for (int i = 0; i < Stack * 6f; i++)
    //        {
    //            Vector2 Vel = Vector2.One.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(24f);
    //            if (Main.rand.NextBool(4)) Vel *= 1.5f;
    //            else if (Main.rand.NextBool(8)) Vel *= 2f;
    //            if (Main.getGoodWorld) Vel *= 3f;
    //            int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, color, 2.25f);
    //            Main.dust[dust].position = Projectile.Center;
    //            Main.dust[dust].velocity = Vel;
    //            Main.dust[dust].noGravity = true;
    //        }
    //        for (int i = 0; i < Stack * 3f; i++)
    //        {
    //            Vector2 Vel = Vector2.One.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(24f);
    //            if (Main.getGoodWorld) Vel *= 3f;
    //            int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<GlowDust_Circle>(), 0, 0, 100, color, 1.5f);
    //            Main.dust[dust].position = Projectile.Center;
    //            Main.dust[dust].velocity = Vel;
    //            Main.dust[dust].noGravity = false;
    //        }
    //        for (int i = 0; i < Stack * 2f; i++)
    //        {
    //            Vector2 Vel = Vector2.One.RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(12f);
    //            if (Main.getGoodWorld) Vel *= 3f;
    //            int dust = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<FogDust>(), 0, 0, 100, color, 1.5f);
    //            Main.dust[dust].position = Projectile.Center;
    //            Main.dust[dust].velocity = Vel;
    //            Main.dust[dust].noGravity = false;
    //        }
    //        base.OnKill(timeLeft);
    //    }
    //}
    //public class Pioneer_Pro6 : ModProjectile
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
    //        Projectile.timeLeft = 60;
    //        Projectile.tileCollide = false;
    //        Projectile.hostile = true;
    //        Projectile.penetrate = -1;
    //        Projectile.extraUpdates = 1;
    //        Projectile.usesLocalNPCImmunity = true;
    //        Projectile.localNPCHitCooldown = -1;
    //    }
    //    public override void OnSpawn(IEntitySource source)
    //    {
    //        Projectile.scale = 0f;
    //        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
    //    }
    //    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    //    {
    //        if(target.type == ModContent.NPCType<EnergyShield>())
    //        {
    //            modifiers.FinalDamage += target.lifeMax + 999;
    //            if(Main.masterMode)
    //                modifiers.SetMaxDamage(target.lifeMax.IntMult(0.5f));
    //            Projectile.ai[2] = 1f;
    //            Projectile.netUpdate = true;
    //        }
    //        base.ModifyHitNPC(target, ref modifiers);
    //    }
    //    public override bool CanHitPlayer(Player target)
    //    {
    //        if (target.Hitbox.Distance(Projectile.Center) > Projectile.width * 0.5f && Projectile.timeLeft > 40)
    //        {
    //            return false;
    //        }
    //        return base.CanHitPlayer(target);
    //    }
    //    public override bool? CanHitNPC(NPC target)
    //    {
    //        if (Projectile.ai[2] == 1f && target.type == ModContent.NPCType<Pioneer>())
    //        {
    //            return false;
    //        }
    //        if (target.Hitbox.Distance(Projectile.Center) > Projectile.width * 0.5f)
    //        {
    //            return false;
    //        }
    //        return base.CanHitNPC(target);
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
    //        if (Projectile.ai[0] == 1f)
    //        {
    //            Projectile.hostile = false;
    //            Projectile.friendly = true;
    //            Projectile.damage = 100;
    //        }

    //        int Size = 600;
    //        if (Main.getGoodWorld)
    //            Size = 1800;
    //        if (Projectile.ai[1] == 0f)
    //        {
    //            Projectile.ai[1] = 1f;
    //            Projectile.Resize(Size, Size);
    //            Projectile.netUpdate = true;
    //        }
    //        Projectile.scale = MathHelper.Lerp(Projectile.scale, 1.25f, 0.1f);
    //        if (Projectile.alpha < 255)
    //            Projectile.alpha += 5;
    //        else
    //            Projectile.Kill();
    //    }
    //}
}
