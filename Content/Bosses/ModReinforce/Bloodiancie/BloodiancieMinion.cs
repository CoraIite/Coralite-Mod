using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public class BloodiancieMinion : ModNPC
    {
        public override string Texture => AssetDirectory.Bloodiancie + "BloodBink";

        public Player Target => Main.player[NPC.target];
        public ref float Timer => ref NPC.ai[0];
        public ref float IdleTime => ref NPC.ai[1];
        public ref float State => ref NPC.ai[2];

        public ref float ReadyRotation => ref NPC.ai[3];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPC.SetHideInBestiary();
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 36;
            NPC.scale = 0.8f;
            NPC.damage = 5;
            NPC.defense = 0;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = 250 + (numPlayers * 50);
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

            switch (State)
            {
                default:
                case 0://原地旋转
                    {
                        if (Timer < 100)//原地旋转
                        {
                            NPC.rotation += 0.2f;
                            NPC.velocity += ReadyRotation.ToRotationVector2() * 0.01f;
                            if (NPC.velocity.Length() > 0.8f)
                                NPC.velocity = ReadyRotation.ToRotationVector2() * 0.8f;

                            NPC.alpha += 3;
                            break;
                        }

                        Timer = 0;
                        State = Main.rand.NextFromList(1, 2);
                        break;
                    }
                case 1://冲刺并爆炸
                    {
                        if (Timer < 3)
                        {
                            NPC.TargetClosest();
                            NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
                            break;
                        }

                        if (Timer % 50 == 0)
                        {
                            NPC.TargetClosest();

                            NPC.velocity = Vector2.Lerp(NPC.velocity, (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f, 0.6f);
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

                            int damage = NPC.GetAttackDamage_ForProjectiles(15, 25);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (NPC.velocity * 9), Vector2.Zero, ModContent.ProjectileType<Rediancie_Explosion>(), damage, 5f);
                        }

                        if (Timer < 160)
                            break;

                        if (Timer == 160)
                        {
                            int damage = NPC.GetAttackDamage_ForProjectiles(15, 25);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (NPC.velocity * 9), Vector2.Zero, ModContent.ProjectileType<Rediancie_BigBoom>(), damage, 5f);
                        }

                        if (Timer < 200)
                        {
                            NPC.velocity *= 0.96f;
                            NPC.rotation = Helper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.alpha -= 40;
                            if (NPC.alpha < 0)
                                NPC.alpha = 0;
                            break;
                        }

                        Timer = 0;
                        NPC.alpha = 0;
                        IdleTime = Main.rand.Next(60, 80);
                        State = 3;
                        break;
                    }
                case 2://冲刺并射弹幕
                    {
                        if (Timer < 3)
                        {
                            NPC.TargetClosest();
                            NPC.velocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 8f;
                            NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

                            int damage = NPC.GetAttackDamage_ForProjectiles(15, 25);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, (Target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * 12f
                                , ModContent.ProjectileType<BloodiancieBeam>(), damage, 5f);
                            break;
                        }

                        if (Timer < 60)
                            break;

                        if (Timer == 60)
                        {
                            int damage = NPC.GetAttackDamage_ForProjectiles(15, 25);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + (NPC.velocity * 9), Vector2.Zero, ModContent.ProjectileType<Rediancie_BigBoom>(), damage, 5f);
                        }

                        if (Timer < 100)
                        {
                            NPC.velocity *= 0.96f;
                            NPC.rotation = Helper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.alpha -= 40;
                            if (NPC.alpha < 0)
                                NPC.alpha = 0;
                            break;
                        }

                        Timer = 0;
                        NPC.alpha = 0;
                        IdleTime = Main.rand.Next(80, 100);
                        State = 3;
                        break;
                    }
                case 3://idle
                    {
                        NPC.rotation = Helper.Lerp(NPC.rotation, 0, 0.1f);

                        Helper.Movement_SimpleOneLine(ref NPC.velocity.X, NPC.direction, 3.5f, 0.05f, 0.1f, 0.97f);

                        //控制Y方向的移动
                        float yLength2 = Math.Abs(Target.Center.Y - NPC.Center.Y);
                        if (yLength2 > 50)
                            Helper.Movement_SimpleOneLine(ref NPC.velocity.Y, NPC.directionY, 3.5f, 0.05f, 0.1f, 0.97f);
                        else
                            NPC.velocity.Y *= 0.96f;

                        if (Timer > IdleTime)
                        {
                            Timer = 0;
                            State = 0;
                        }
                        break;
                    }
            }

            Timer++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[Type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Vector2 origin = mainTex.Size() / 2;

            if (State == 0)
                spriteBatch.Draw(mainTex, drawPos, null, new Color(248, 40, 24, NPC.alpha), NPC.rotation, origin, NPC.scale + (0.5f * (NPC.alpha / 255f)), SpriteEffects.None, 0f);
            else if (State != 3)
            {
                Texture2D extraTex = ModContent.Request<Texture2D>(AssetDirectory.RedJadeProjectiles + "RedBinkRush").Value;
                int color = NPC.alpha;
                spriteBatch.Draw(extraTex, NPC.Center - NPC.velocity - screenPos, null, new Color(color, color, color, color), NPC.rotation, extraTex.Size() / 2, 0.85f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(mainTex, drawPos, null, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreKill()
        {
            return false;
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
        }
    }
}
