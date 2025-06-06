using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.NPCs.Icicle
{
    public class IcicleSpirit : ModNPC
    {
        public override string Texture => AssetDirectory.IcicleNPCs + Name;

        public Player Target => Main.player[NPC.target];

        public ref float Timer => ref NPC.ai[3];
        public ref float HitTileCount => ref NPC.ai[2];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 5;
            this.RegisterBestiaryDescription();
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 40;
            NPC.lifeMax = 750;
            NPC.damage = 30;
            NPC.defense = 12;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 1);
            NPC.rarity = 3;

            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = CoraliteSoundID.DigIce;
        }

        public override bool? CanFallThroughPlatforms() => true;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.AddTags(
                this.GetBestiaryDescription(),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow
                );
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneRockLayerHeight && DownedBossSystem.downedBabyIceDragon)
                return 0.1f;
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IcicleCrystal>(), 1, 1, 2));
        }

        public override void AI()
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += NPC.frame.Height;
                if (NPC.frame.Y > (NPC.frame.Height * Main.npcFrameCount[Type]) - 1)
                    NPC.frame.Y = 0;
            }

            if (Target.dead || !Target.active || (Target.Center - NPC.Center).Length() > 2000f)
                NPC.EncourageDespawn(10);

            do
            {
                if (Timer == 0)
                {
                    NPC.TargetClosest();
                    NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                    NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                }

                if (Timer < 10)
                {
                    Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.15f, 0.15f, 0.97f);
                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 2f, 0.15f, 0.15f, 0.97f);
                    break;
                }

                if (Timer < 200)
                {
                    if (Timer % 50 == 0)
                    {
                        NPC.TargetClosest();
                        NPC.direction = Target.Center.X > NPC.Center.X ? 1 : -1;
                        NPC.directionY = Target.Center.Y > NPC.Center.Y ? 1 : -1;
                    }
                    Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3f, 0.15f, 0.15f, 0.97f);
                    Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 2f, 0.15f, 0.15f, 0.97f);
                    if (Math.Abs(NPC.oldPos[0].X - NPC.Center.X) < 0.05f)    //X方向检测
                    {
                        HitTileCount += 1f;
                        if (HitTileCount > 4)
                        {
                            NPC.velocity.X *= -1;
                            if (HitTileCount > 20)//多次撞墙的话就换个方向
                            {
                                HitTileCount = 0;
                                NPC.direction *= -1;
                                Timer = -10;
                            }
                        }
                        NPC.netUpdate = true;
                    }

                    if (Math.Abs(NPC.oldPos[0].Y - NPC.Center.Y) < 0.05f)
                    {
                        HitTileCount += 1f;
                        if (HitTileCount > 4)
                        {
                            NPC.velocity.Y *= -1;
                            if (HitTileCount > 20)//多次撞墙的话就换个方向
                            {
                                HitTileCount = 0;
                                NPC.directionY *= -1;
                                Timer = -40;
                            }
                        }
                        NPC.netUpdate = true;
                    }

                    break;
                }

                if (Timer < 300)
                {
                    if (Timer % 30 == 0 && NPC.target == Main.myPlayer)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (Target.Center - NPC.Center).SafeNormalize(Vector2.One) * 8,
                            ModContent.ProjectileType<IcicleProj_Hostile>(), 14, 6f);
                        SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, NPC.Center);
                    }
                    NPC.velocity *= 0.98f;
                    break;
                }

                Timer = 0;
                NPC.netUpdate = true;
            } while (false);

            NPC.oldPos[0] = NPC.Center;
            Lighting.AddLight(NPC.Center, Coralite.IcicleCyan.ToVector3());
            if (Main.rand.NextBool(30))
            {
                Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.FrostStaff, Main.rand.NextFloat(6.282f).ToRotationVector2() * Main.rand.NextFloat(4, 6));
                dust.noGravity = true;
            }

            Timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;

            Vector2 origin = NPC.frame.Size() / 2;
            SpriteEffects effects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(mainTex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }
    }
}