using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 碎岩攻击抛出的生长浮石：30 帧后裂出一块 <see cref="CrystallineSentinelRock"/>，之后悬停、变紫预警、坠落触地消散。
    /// </summary>
    public class CrystallineSentinelFloatStoneGrow : ModNPC
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public ref float Timer => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 15;
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

        public override bool PreHoverInteract(bool mouseIntersects) => false;

        public override void OnSpawn(IEntitySource source)
        {
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

        public override void AI()
        {
            float size = Utils.Remap(NPC.frame.Y, 0, 14, 8, 32);
            NPC.width = NPC.height = (int)size;

            NPC.velocity *= 0.99f;
            Timer++;

            if (State < 1)
            {
                if (Timer == 30)
                {
                    var rock = NPC.NewNPCDirect(this.FromObjectGetParent(), NPC.Center, ModContent.NPCType<CrystallineSentinelRock>());
                    rock.velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 16) * 0.75f + NPC.velocity * 0.5f;

                    var blast = PRTLoader.NewParticle<CrystallineRockBlast>(NPC.Center, rock.velocity * 0.4f);
                    blast.Rotation = blast.Velocity.ToRotation();
                }

                if (Timer > 60)
                {
                    State = 2;
                    Timer = 0;
                }

                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = NPC.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20) * 0.5f;
                    Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 3f) * 0.5f + NPC.velocity * 0.5f;
                    Dust d = Dust.NewDustPerfect(pos, DustID.Stone, vel);
                    d.noGravity = true;
                }
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else
            {
                if (Timer > 500)
                {
                    NPC.noGravity = false;

                    if (NPC.velocity.Y > 2f)
                        Timer--;

                    if (Timer > 560)
                        NPC.active = false;
                    if (Collision.SolidCollision(NPC.position - Vector2.One * 2, NPC.width + 4, NPC.height + 4))
                    {

                        int dustCount = 10;
                        for (int i = 0; i < dustCount; i++)
                        {
                            Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Stone);
                            d.noGravity = true;
                        }
                        NPC.Kill();
                    }
                }
                if (MathF.Abs(NPC.velocity.Y) > 0.1f)
                    NPC.rotation += 0.015f * MathF.Sin(NPC.whoAmI * 1138);
            }

            float frameRate = 5;
            if (NPC.frameCounter++ % frameRate == 0 && NPC.frame.Y < 14)
            {
                NPC.frame.Y++;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = NPC.GetTexture();
            Rectangle frameBox = tex.Frame(1, 15, 0, NPC.frame.Y);

            float breatheAlpha = 1f + 0.2f * MathF.Sin((float)(Main.timeForVisualEffects * 0.1f));
            breatheAlpha = Utils.Remap(Timer, 0, 30, 1f, breatheAlpha);

            //描边变成紫色来预警
            int timeToFall = 440;
            float telegraphFactor = Utils.Remap(Timer, timeToFall - 60, timeToFall, 0f, 1f);
            Color teleColor = Color.Lerp(Color.White, Color.Violet, telegraphFactor);
            if (State < 1)
                breatheAlpha = 1;
            spriteBatch.Draw(tex, NPC.Center - screenPos, frameBox, teleColor * breatheAlpha, NPC.rotation,
                frameBox.Size() / 2 + new Vector2(0, 7), NPC.scale * 1.1f, SpriteEffects.None, 0);

            if (State > 1)
            {
                Texture2D rockTex = TextureAssets.Npc[ModContent.NPCType<CrystallineSentinelRock>()].Value;
                var rockFrame = rockTex.Frame(1, 9, 0, 0);

                float alphaFactor = Utils.Remap(Timer, 0, 60, 0f, 1f);
                spriteBatch.Draw(rockTex, NPC.Center - screenPos, rockFrame, Color.White * alphaFactor, NPC.rotation,
                    rockFrame.Size() / 2, NPC.scale * 1f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
