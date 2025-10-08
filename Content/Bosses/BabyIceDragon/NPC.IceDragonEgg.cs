using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.UI;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    [VaultLoaden(AssetDirectory.BabyIceDragon)]
    public class IceDragonEgg : ModNPC, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        private Player Target => Main.player[NPC.target];

        [VaultLoaden("{@classPath}" + "IceDragonEggBurst")]
        public static ATex BurstTex { get; private set; }

        public ref float State => ref NPC.ai[1];
        private ref float Timer => ref NPC.localAI[0];

        public override void SetDefaults()
        {
            NPC.chaseable = false;
            NPC.npcSlots = 0;
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 600;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.netAlways = true;

            NPC.HitSound = CoraliteSoundID.DigIce;
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 4; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.BabyIceDragon + "IceDragonEgg_Gore" + i);

        }

        public override void AI()
        {
            if ((int)State == 1)
            {
                Lighting.AddLight(NPC.Center, new Vector3(1f, 1f, 1f));
                do
                {
                    if (Timer < 100)
                    {
                        if ((int)Timer % 45 == 0)
                        {
                            if ((int)Timer != 0)
                                NPC.frame.Y++;

                            SoundEngine.PlaySound(CoraliteSoundID.DigIce, NPC.Center);
                        }

                        break;
                    }

                    if (Timer > 150)
                    {
                        for (int j = 0; j < 20; j++)
                        {
                            Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(32, 32), ModContent.DustType<CrushedIceDust>(),
                                -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f)) * Main.rand.Next(2, 7), Scale: Main.rand.NextFloat(1f, 1.4f));
                        }

                        for (int i = 0; i < 3; i++)
                            PRTLoader.NewParticle(NPC.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo>(), Color.White, 0.15f);

                        Helper.PlayPitched("Icicle/Broken", 0.4f, 0f, NPC.Center);
                        NPC.Kill();
                    }
                } while (false);

                Timer++;
                return;
            }

            if (NPC.ai[0] > 0f)
            {
                float sinProgress = MathF.Sin(NPC.ai[0]);
                NPC.rotation = sinProgress * 0.2f;
                NPC.ai[0] -= MathHelper.Pi / 16;     //1/16 Pi
            }

            if (NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active)
            {
                NPC.TargetClosest();
                if (!(NPC.target < 0 || NPC.target == 255 || Target.dead || !Target.active))
                    KnowledgeSystem.CheckForUnlock<IceDragon1Knowledge>(Target.Center, Coralite.IcicleCyan);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            //受击时摇晃一下
            NPC.ai[0] += 6.282f;

            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {
                //生成蛋壳gore
                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(3, 3), Mod.Find<ModGore>("IceDragonEgg_Gore0").Type);
                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(3, 3), Mod.Find<ModGore>("IceDragonEgg_Gore1").Type);
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(3, 3), Mod.Find<ModGore>("IceDragonEgg_Gore2").Type);
                }
                for (int i = 0; i < 3; i++)
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), Main.rand.NextVector2Circular(3, 3), Mod.Find<ModGore>("IceDragonEgg_Gore3").Type);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            if ((int)State != 1)
            {
                NPC.rotation = 0;
                NPC.life = 1;
                NPC.ai[0] = 0;
                State = 1;
                NPC.dontTakeDamage = true;
                return false;
            }

            return true;
        }

        public override void OnKill()
        {
            //生成冰龙宝宝
            if (NPC.lastInteraction != 255)
            {
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BabyIceDragon>());
                IceEggSpawner.EggDestroyed();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            Vector2 position = NPC.Bottom - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(2, 1, 1);

            Vector2 origin = new Vector2(frameBox.Width / 2, frameBox.Height);
            //绘制蛋
            spriteBatch.Draw(mainTex, position, frameBox, drawColor, NPC.rotation, origin, 1, SpriteEffects.None, 0f);
            frameBox = mainTex.Frame(2, 1);

            //绘制底座
            spriteBatch.Draw(mainTex, position, frameBox, drawColor, 0, origin, 1, SpriteEffects.None, 0f);
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (State == 1)
            {
                Texture2D glowTex = BurstTex.Value;
                var frame = glowTex.Frame(1, 3, 0, NPC.frame.Y);

                spriteBatch.Draw(glowTex, NPC.Center - Main.screenPosition, frame, Color.White, NPC.rotation, frame.Size() / 2, 1, SpriteEffects.None, 0f);
            }
        }
    }

    [VaultLoaden(AssetDirectory.BabyIceDragon)]
    public class IceDragonEggDrawer : ModMapLayer
    {
        public static ATex IceDragonEgg_Head { get; private set; }

        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            NPC egg = null;

            for (int i = 0; i < 200; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<IceDragonEgg>())
                {
                    egg = Main.npc[i];
                    break;
                }
            }

            if (egg == null)
                return;

            const float scaleIfNotSelected = 1f;
            const float scaleIfSelected = scaleIfNotSelected * 2f;

            if (context.Draw(IceDragonEgg_Head.Value, egg.Center / 16, Color.White
                , new SpriteFrame(1, 1, 0, 0)
                , scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver)
            {
                text = ContentSamples.NpcsByNetId[ModContent.NPCType<IceDragonEgg>()].FullName;
            }
        }
    }
}
