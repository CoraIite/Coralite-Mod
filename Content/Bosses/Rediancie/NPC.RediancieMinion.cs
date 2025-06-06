﻿using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.Rediancie
{
    public class RediancieMinion : ModNPC
    {
        public override string Texture => AssetDirectory.Rediancie + "RedBink_BossMinion";

        Player Target => Main.player[NPC.target];
        public ref float Timer => ref NPC.ai[0];
        public ref float Alpha => ref NPC.ai[1];
        public ref float ReadyRotation => ref NPC.ai[3];

        public override void SetStaticDefaults()
        {
            NPC.SetHideInBestiary();

            Main.npcFrameCount[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 36;
            NPC.scale = 0.8f;
            NPC.damage = 5;
            NPC.defense = 0;
            NPC.lifeMax = 10;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);

            NPC.HitSound = CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.SpawnedFromStatue = true;
        }

        public override void AI()
        {
            //原地旋转并向玩家冲刺
            if (Timer == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                ReadyRotation = Main.rand.NextFloat(-3.141f, 3.141f);
                NPC.TargetClosest();
                NPC.netUpdate = true;
            }

            do
            {
                if (Timer < 100)//原地旋转
                {
                    NPC.rotation += 0.2f;
                    NPC.velocity += ReadyRotation.ToRotationVector2() * 0.01f;
                    if (NPC.velocity.Length() > 0.8f)
                        NPC.velocity = ReadyRotation.ToRotationVector2() * 0.8f;

                    Alpha += 3f;
                    break;
                }

                if (Timer == 100)
                {
                    NPC.TargetClosest();
                    NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    break;
                }

                if (Timer == 158)
                {
                    int damage = Helper.GetProjDamage(20, 25, 30);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (NPC.velocity * 9), Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), damage, 5f);
                }

                if (Timer < 150)
                    break;

                if (Timer < 230)
                {
                    NPC.velocity *= 0.96f;
                    NPC.rotation = Helper.Lerp(NPC.rotation, 0, 0.1f);
                    Alpha -= 40;
                    if (Alpha < 0)
                        Alpha = 0;
                    break;
                }

                Timer = 0;
                Alpha = 0;
                return;

            } while (false);

            Timer++;
        }

        public override void OnKill()
        {
            if (!VaultUtils.isServer)
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.GemRuby, Main.rand.NextVector2CircularEdge(5, 5), 0, default, 1.3f);
                    dust.noGravity = true;
                }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 origin = mainTex.Size() / 2;

            if (Timer < 101)
                spriteBatch.Draw(mainTex, drawPos, null, new Color(248, 40, 24, (int)Alpha), NPC.rotation, origin, NPC.scale + (0.5f * (Alpha / 255)), SpriteEffects.None, 0f);
            else
            {
                Texture2D extraTex = ModContent.Request<Texture2D>(AssetDirectory.RedJadeProjectiles + "RedBinkRush").Value;
                int color = (int)Alpha;
                spriteBatch.Draw(extraTex, NPC.Center - NPC.velocity - screenPos, null, new Color(color, color, color, color), NPC.rotation, extraTex.Size() / 2, 0.85f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(mainTex, drawPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
