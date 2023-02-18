using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.Rediancie
{
    public class RediancieMinion : ModNPC
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + "RedBink";

        Player target => Main.player[NPC.target];
        public ref float Timer => ref NPC.ai[0];
        public ref float alpha => ref NPC.ai[1];
        public ref float ReadyRotation => ref NPC.ai[3];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("小赤玉灵");

            Main.npcFrameCount[Type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 20;
            NPC.damage = 5;
            NPC.defense = 0;
            NPC.lifeMax = 10;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
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

                    alpha += 3f;
                    break;
                }

                if (Timer == 100)
                {
                    NPC.TargetClosest();
                    NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
                    NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                    break;
                }

                if (Timer == 158)
                {
                    int damage = NPC.GetAttackDamage_ForProjectiles(10, 15);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.velocity * 9, Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), damage, 5f);
                }

                if (Timer < 150)
                    break;

                if (Timer < 230)
                {
                    NPC.velocity *= 0.96f;
                    NPC.rotation = Helper.Lerp(NPC.rotation, 0, 0.1f);
                    alpha -= 40;
                    if (alpha < 0)
                        alpha = 0;
                    break;
                }

                Timer = 0;
                alpha = 0;
                return;

            } while (false);

            Timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Timer < 101)
            {
                Texture2D mainTex = TextureAssets.Npc[Type].Value;
                spriteBatch.Draw(mainTex, NPC.Center - screenPos, mainTex.Frame(), new Color(248, 40, 24, (int)alpha), NPC.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 1 + alpha / 255, SpriteEffects.None, 0f);
            }
            else
            {
                Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.RedJadeProjectiles + "RedBinkRush").Value;
                int color = (int)alpha;
                spriteBatch.Draw(mainTex, NPC.Center - NPC.velocity - screenPos, mainTex.Frame(), new Color(color, color, color, color), NPC.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), 0.8f, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void OnKill()
        {
            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.GemRuby, Main.rand.NextVector2CircularEdge(5, 5), 0, default, 1.3f);
                    dust.noGravity = true;
                }
        }
    }
}
