using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceThornsTrap : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        private ref float Timer => ref NPC.ai[0];
        private ref float State => ref NPC.ai[1];

        public Vector2 Center
        {
            get => new Vector2(NPC.ai[2], NPC.ai[3]);
            set
            {
                NPC.ai[2] = value.X;
                NPC.ai[3] = value.Y;
            }
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 30;
            NPC.scale = 0.2f;
            NPC.width = NPC.height = 80;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return State > 2;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return State > 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            NPC.rotation = Main.rand.NextFloat(6.282f);
            Center = NPC.Center;
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            if (State > 2)
            {

                if (Timer > 60)
                {
                    Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<IceHalo>(), Color.White, 0.25f);
                    NPC.life -= 10;
                    Timer = 0;
                }

                Timer++;
                if (NPC.life <= 1)
                    NPC.Kill();
                return;
            }

            //开始膨胀
            do
            {
                if (Timer < 30)
                    break;

                if (Timer == 30)
                    SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, NPC.Center);

                if (Timer < 45)
                {
                    NPC.scale += 0.02f;
                    NPC.width = NPC.height = (int)(NPC.scale * 80);
                    NPC.rotation += 0.2f;
                    NPC.Center = Center;
                    if (Timer % 2 == 0)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.Ice, -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1.3f, 1.3f)) * Main.rand.NextFloat(2, 3));
                        Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height), DustID.SnowBlock, Vector2.Zero);
                    }
                    break;
                }

                Timer = 0;
                State += 1;
            } while (false);


            Timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
