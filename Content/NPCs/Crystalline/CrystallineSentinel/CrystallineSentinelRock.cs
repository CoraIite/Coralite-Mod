using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 碎岩：从浮石里裂出来的实体岩块，被打碎会继续分裂成更小的块，帧号越大越小。
    /// 存活约 9 秒后失去浮力坠落，描边在坠落前 60 帧转紫预警。
    /// </summary>
    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinelRock : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        [VaultLoaden("{@classPath}" + "CrystallineSentinelRock_Glow")]
        public static ATex GlowTex { get; set; }
        public ref float Timer => ref NPC.ai[0];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.NeverDropsResourcePickups[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.damage = 2;
            NPC.lifeMax = 20;
            NPC.friendly = false;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
        }

        public override bool PreHoverInteract(bool mouseIntersects)
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            NPC.GravityMultiplier *= 2f;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.DisableKnockback();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<Stonebound>(), 2 * 60);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life < 0)
                Split();
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.life < 0)
                Split();
        }

        public override void AI()
        {
            float size = Utils.Remap(NPC.frame.Y, 0, 8, 32, 8);
            NPC.width = NPC.height = (int)size;

            float randFactor = MathF.Sin(NPC.whoAmI * 1241);
            Timer++;
            if (Timer == 60 + (int)(20 * randFactor))
                Split();

            if (Timer == 120 + (int)(20 * randFactor) && NPC.frame.Y < 6)
            {
                //Split();
            }

            if (Timer > 540 + 30 * randFactor)
            {
                NPC.noGravity = false;

                if (NPC.velocity.Y > 2f)
                    Timer--;

                if (Timer > 620)
                    NPC.Kill();

                if (Collision.SolidCollision(NPC.position - Vector2.One * 2, NPC.width + 4, NPC.height + 4)/*Helper.GroundSearch(NPC.BottomLeft.ToTileCoordinates() + new Point(0,2),new Point(1,0),NPC.width / 16)*/)
                {
                    NPC.Kill();
                }
            }
            else
            {

                if (NPC.frame.Y < 7)//漂浮
                {
                    Vector2 vel = -Vector2.UnitY * (0.4f + 0.15f * MathF.Sin(NPC.whoAmI * 498));
                    NPC.velocity = Vector2.Lerp(NPC.velocity, vel, 0.05f);
                }
                else//减速
                {
                    float velDec = Utils.Remap(Timer, 0, 120, 0.98f, 0.94f);
                    NPC.velocity *= velDec;

                }
            }
            if (MathF.Abs(NPC.velocity.Y) > 0.1f)
                NPC.rotation += 0.015f * MathF.Sin(NPC.whoAmI * 1138);
        }

        public override void OnKill()
        {
            int dustCount = 10;
            for (int i = 0; i < dustCount; i++)
            {
                Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Stone);
                d.noGravity = true;
            }
            //Split();
        }

        public void Split(int min = 1, int max = 2)
        {
            if (NPC.frame.Y > 6)
                return;
            int count = Main.rand.Next(min, max + 1);
            bool splitToPlr = Main.rand.NextBool();//是否尝试朝附近玩家分裂
            Player plr = null;
            if (splitToPlr)
            {
                plr = NPC.FindPlayer();
            }
            for (int i = 0; i < count; i++)
            {
                if (Main.rand.NextBool(6))
                    continue;
                int frameIncrease = Main.rand.Next(1, 4);
                int frame = (int)MathHelper.Clamp(NPC.frame.Y + frameIncrease, 0, 8);

                float velFactor = Utils.Remap(NPC.velocity.Length(), 0, 12f, 0f, 0.8f);
                var rock = NPC.NewNPCDirect(this.FromObjectGetParent(), NPC.Center, ModContent.NPCType<CrystallineSentinelRock>());
                Vector2 randVel = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4, 8f) * 0.45f;
                if (splitToPlr && plr != null)
                    randVel.X = (NPC.DirectionTo(plr.Center) * Main.rand.NextFloat(6, 10) * 0.75f).X;
                rock.velocity = Vector2.Lerp(randVel, NPC.velocity * 0.5f, velFactor);
                rock.frame.Y = frame;

                var blast = PRTLoader.NewParticle<CrystallineRockBlast>(NPC.Center + rock.velocity.SafeNormalize(Main.rand.NextVector2Unit()) * 20, Vector2.Zero);
                blast.Rotation = rock.velocity.ToRotation();
                //blast.Scale *= 1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = NPC.GetTexture();
            var frameBox = tex.Frame(1, 9, 0, NPC.frame.Y);

            float alphaFactor = Utils.Remap(Timer, 0, 60, 0f, 1f);
            float breatheAlpha = 1f + 0.2f * MathF.Sin((float)(Main.timeForVisualEffects * 0.1f));
            breatheAlpha = Utils.Remap(Timer, 40, 70, 1f, breatheAlpha);

            //描边变成紫色来预警
            float randFactor = MathF.Sin(NPC.whoAmI * 1241);
            int timeToFall = (int)(480 + 30 * randFactor);
            float telegraphFactor = Utils.Remap(Timer, timeToFall - 60, timeToFall, 0f, 1f);

            Color teleColor = Color.Lerp(Color.White, Color.Violet, telegraphFactor);
            spriteBatch.Draw(GlowTex.Value, NPC.Center - screenPos, frameBox, teleColor * breatheAlpha, NPC.rotation, frameBox.Size() / 2, NPC.scale * 1.1f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, frameBox, Color.White * alphaFactor, NPC.rotation, frameBox.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
