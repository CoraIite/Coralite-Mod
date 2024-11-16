using Coralite.Content.Items.Thunder;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ThunderPhantom : ModNPC
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;

        public ref float OwnerIndex => ref NPC.ai[0];
        public ref float State => ref NPC.ai[1];
        public ref float Timer => ref NPC.ai[2];
        public ref float PhantomDistance => ref NPC.ai[3];

        Player Target => Main.player[NPC.target];

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 100;
            NPC.damage = 60;
            NPC.scale = 1.1f;
            NPC.defense = 35;
            NPC.lifeMax = 2200;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((2200 + (numPlayers * 700)) / journeyScale);
                    NPC.damage = 66;
                    NPC.defense = 35;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((2200 + (numPlayers * 1400)) / journeyScale);
                    NPC.damage = 72;
                    NPC.defense = 35;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 35;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 0.4f;
                }

                return;
            }

            NPC.lifeMax = 2200 + (numPlayers * 700);
            NPC.damage = 66;
            NPC.defense = 35;

            if (Main.masterMode)
            {
                NPC.lifeMax = 2200 + (numPlayers * 1400);
                NPC.damage = 72;
                NPC.defense = 35;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 2500 + (numPlayers * 1600);
                NPC.damage = 80;
                NPC.defense = 35;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 0.4f;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ElectrificationWing>(), 1, 1, 2));
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner<ThunderveinDragon>(out NPC owner, NPC.Kill))
                return;

            switch (State)
            {
                default:
                case 0://先跟踪一段时间，然后让天空闪烁一下
                    {
                        NPC.target = owner.target;
                        NPC.Center = Target.Center + new Vector2(0, -100);
                        Timer++;
                        if (Timer > 60)
                        {
                            State++;
                            Timer = 0;
                            NPC.Center += new Vector2((Main.rand.NextFromList(-1, 1) * 200) + Main.rand.Next(-250, 250), 0);
                            ThunderveinDragon.SetBackgroundLight(0.9f, 50, 18);
                            SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);
                            NPC.dontTakeDamage = false;
                        }
                    }
                    break;
                case 1://让天空闪烁一下，同时让分身就位
                    {
                        Timer++;
                        float factor = Coralite.Instance.SqrtSmoother.Smoother((int)Timer, 75);
                        PhantomDistance = factor * 220;
                        if (Timer > 75)
                        {
                            State++;
                            Timer = 0;
                            ThunderveinDragon.SetBackgroundLight(0.6f, 12 * 3, 10);

                            SoundEngine.PlaySound(CoraliteSoundID.Thunder, NPC.Center);
                        }
                    }
                    break;
                case 2://释放雷暴
                    {
                        if (Timer == 0)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            Vector2 pos = NPC.Center;
                            int damage = Helper.GetProjDamage(60, 70, 80);

                            if (!VaultUtils.isClient)
                                NPC.NewProjectileDirectInAI<StrongThunderFalling>(
                                pos + new Vector2(0, -Main.rand.Next(170, 320)), pos + new Vector2(0, 750), damage, 0, NPC.target
                                , 20, NPC.whoAmI, 70);
                        }
                        else if (Timer % 12 == 0)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.NoUse_ElectricMagic_Item122, NPC.Center);
                            int damage = Helper.GetProjDamage(20, 40, 60);
                            if (!VaultUtils.isClient)
                            {
                                for (int i = -1; i < 2; i += 2)
                                {
                                    Vector2 pos = NPC.Center + new Vector2(i * (Timer / 12) * PhantomDistance, 0);
                                    NPC.NewProjectileDirectInAI<StrongThunderFalling>(
                                        pos + new Vector2(0, -Main.rand.Next(170, 320)), pos + new Vector2(0, 750), damage, 0, NPC.target
                                        , 7, NPC.whoAmI, 70);
                                }
                            }
                        }

                        Timer++;
                        if (Timer > 12 * 4)
                        {
                            PhantomDistance = 0;
                            State = 0;
                            NPC.dontTakeDamage = true;
                            (owner.ModNPC as ThunderveinDragon).SonState++;
                            if ((owner.ModNPC as ThunderveinDragon).SonState > 5)
                                NPC.Kill();
                        }
                    }
                    break;
            }
        }

        public override bool PreKill()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
    }
}
