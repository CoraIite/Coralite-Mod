using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceCube : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        public SlotId soundSlotID;

        public ref float ExtendTrigger => ref NPC.ai[0];
        public ref float ExtendCount => ref NPC.ai[1];
        public ref float Timer => ref NPC.localAI[0];

        public override bool CanHitNPC(NPC target)/* tModPorter Suggestion: Return true instead of null */ => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 138;
            NPC.scale = 0.2f;
            NPC.lifeMax = 300;
            NPC.knockBackResist = 0f;
            NPC.defense = 5;
            NPC.damage = 2;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(175 * bossAdjustment) + numPlayers * 100;
            NPC.defense = 6;
            if (Main.masterMode)
                NPC.defense = 8;
        }

        public override void AI()
        {
            if (Timer > 20)
            {
                ExtendTrigger = 1f;
                Timer = 0;
            }

            //随着ai1的增大而增大，随后爆炸
            if (ExtendTrigger == 1f)
            {
                if (ExtendCount < 17)
                    SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, NPC.Center);
                ExtendTrigger = 0f;
                ExtendCount++;
                Vector2 center = NPC.Center;
                NPC.scale += 0.04f;
                NPC.width = (int)(68 * NPC.scale);
                NPC.height = (int)(138 * NPC.scale);
                NPC.Center = center;
                NPC.netUpdate = true;
            }

            //生成特效粒子
            if (ExtendCount < 17 && Timer % 6 == 0)
                IceStarLight.Spawn(NPC.Center + Main.rand.NextVector2CircularEdge(80, 80), Main.rand.NextVector2CircularEdge(5, 5), 0.5f, () => NPC.Center, 12);

            if (ExtendCount == 14 && NPC.localAI[2] == 0f)
            {
                soundSlotID = Helper.PlayPitched("Icicle/Burst", 0.4f, 0f, NPC.Center);
                NPC.localAI[2] = 1f;
            }

            if (ExtendCount == 18f && NPC.localAI[1] == 0f)
            {
                //生成闪光粒子以及蓄力效果
                Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 3f);
                Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 4f);
                Particle.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.Instance.IcicleCyan, 2.5f);
                NPC.localAI[1] = 1f;
            }

            if (ExtendCount >= 19)
            {
                //大于多少后产生爆炸
                PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, new Vector2(2f, 2f), 16f, 20f, 25, 1000f, "BabyIceDragon");
                Main.instance.CameraModifiers.Add(modifier);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IceBurst>(), 90, 10f);
                NPC.Kill();
            }

            Timer += 1f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[NPC.type].Value;

            spriteBatch.Draw(mainTex, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnKill()
        {
            int index = Helper.GetNPCByType(ModContent.NPCType<BabyIceDragon>());
            if (index == -1)
                return;

            if (ExtendCount >= 19)
            {
                (Main.npc[index].ModNPC as BabyIceDragon).HaveARest(40);
                return;
            }

            //寻找到冰龙宝宝并让它眩晕，把爆炸音效给停了
            if (ExtendCount >= 14)
                if (SoundEngine.TryGetActiveSound(soundSlotID, out ActiveSound result))
                    result.Stop();
            if ((int)Main.npc[index].ai[1] != -5)
                (Main.npc[index].ModNPC as BabyIceDragon).Dizzy(300);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Particle.NewParticle(projectile.Center, -projectile.velocity.RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) * Main.rand.NextFloat(0.05f, 0.2f),
                   CoraliteContent.ParticleType<SnowFlower>(), Color.White, Main.rand.NextFloat(0.4f, 0.6f));
                Dust.NewDustPerfect(projectile.Center, DustID.FrostStaff, -projectile.velocity.RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)) * Main.rand.NextFloat(0.05f, 0.2f));
            }
        }
    }
}