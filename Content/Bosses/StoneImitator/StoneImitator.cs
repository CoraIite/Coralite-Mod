using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.StoneImitator
{
    [AutoloadBossHead]
    public class StoneImitator : ModNPC
    {
        public override string Texture => AssetDirectory.StoneImitator + "StoneImitator";

        public override bool? CanFallThroughPlatforms() => true;

        private float targetX;
        private float targetY;

        private bool spawned;

        internal ref float GlobalTimer => ref NPC.ai[0];
        internal ref float State => ref NPC.ai[1];
        internal ref float StateTimer => ref NPC.ai[2];
        internal ref float RestDuration => ref NPC.ai[3];

        internal Vector2 TargetLocation
        {
            get => new Vector2(targetX, targetY);
            set
            {
                targetX = value.X;
                targetY = value.Y;
            }
        }

        #region tml hooks

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("岩石傀儡");

            Main.npcFrameCount[Type] = 1;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 110;
            NPC.damage = 1;
            NPC.lifeMax = 2000;
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.boss = true;

            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
        }

        public override void OnKill()
        {
            base.OnKill();
        }

        public override void OnSpawn(IEntitySource source)
        {
            State = (int)AIStates.Idle;
            StateTimer = 0;
            RestDuration = 60f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            base.HitEffect(hitDirection, damage);
        }

        #endregion

        #region AI

        public enum AIStates
        {
            Idle = 0,
            Move = 1,
            Attack_Dash = 2,
            Attack_Jump = 3,
            Attack_Shoot = 4,
            Attack_Jump_Fall = 5
        }

        public override void AI()
        {
            //if (Main.netMode == NetmodeID.Server)
            //{
            //}

            Main.NewText(State, Color.White);

            if (!spawned)
            {
                spawned = true;
                NPC.TargetClosest(false);
                State = (int)AIStates.Idle;

                //生成粒子
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 20; i++)
                        Dust.NewDustPerfect(NPC.Bottom, DustID.Stone, Vector2.UnitX.RotatedByRandom(6.28f) * 2f, 0, default, 2f);
                }

                SoundEngine.PlaySound(SoundID.Roar, Main.player[NPC.target].Center);
            }

            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            if (Main.player[NPC.target].dead)
            {
                State = (int)AIStates.Idle;
                NPC.noTileCollide = false;
                NPC.noGravity = true;
                NPC.velocity = Vector2.UnitY;
                NPC.EncourageDespawn(10);
                return;
            }

            Player player = Main.player[NPC.target];


            if (State != (int)AIStates.Attack_Dash && State != (int)AIStates.Attack_Jump)
                NPC.rotation = 0f;

            switch (State)
            {
                case (int)AIStates.Idle:
                    if (Main.netMode == NetmodeID.Server && StateTimer == 0)
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.direction = NPC.spriteDirection = Math.Sign(player.Center.X - NPC.Center.X);
                        NPC.noGravity = false;
                        NPC.netUpdate = true;
                    }

                    StateTimer++;

                    if (StateTimer == RestDuration)
                    {
                        StateTimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            State = Main.rand.Next(2, 5);
                    }

                    break;

                case (int)AIStates.Move:
                    float speed = 1f;
                    if (Main.netMode == NetmodeID.Server && StateTimer == 0)
                    {
                        NPC.velocity = Vector2.Zero;
                        NPC.direction = NPC.spriteDirection = Math.Sign(player.Center.X - NPC.Center.X);
                        NPC.noGravity = false;
                        NPC.noTileCollide = false;
                    }

                    StateTimer++;
                    if (StateTimer != RestDuration)
                        NPC.velocity.X = speed * NPC.direction;
                    else
                    {
                        StateTimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            State = Main.rand.Next(2, 5);
                        }
                    }

                    break;

                case (int)AIStates.Attack_Dash:
                    Dash(player, 8f, 400f);
                    break;

                case (int)AIStates.Attack_Jump:
                    Jump(player, new Vector2(0, -480), 8f);
                    break;

                case (int)AIStates.Attack_Shoot:
                    Shoot(player);
                    break;

                case (int)AIStates.Attack_Jump_Fall:
                    Fall(player, 8f);
                    break;
                default:
                    goto case (int)AIStates.Idle;
            }
        }

        private void Dash(Player target, float speed, float distance)
        {
            StateTimer++;
            //瞄准
            if (StateTimer == 30)
            {
                TargetLocation = target.Center + (target.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * distance;
                NPC.direction = NPC.spriteDirection = Math.Sign(target.Center.X - NPC.Center.X);
                NPC.rotation = NPC.direction > 0 ? NPC.velocity.ToRotation() : NPC.velocity.ToRotation() + 3.14f;

                if (Main.netMode != NetmodeID.Server)
                    Helper.PlayPitched(Name + "/Roar_Dash", 1f, 0.5f, NPC.Center);
            }
            //开冲！
            if (StateTimer > 30)
            {
                float factor = (StateTimer - 15) / 45;
                float accel = Helpers.Helper.BezierEase(factor);

                Vector2 targetNormalize = (TargetLocation - NPC.Center).SafeNormalize(Vector2.UnitY);
                NPC.velocity = targetNormalize * speed * (factor > 0.9f ? 1f : accel);
                NPC.rotation += 0.1f;

                NPC.noTileCollide = true;

                //生成粒子
                if (Main.netMode != NetmodeID.Server && StateTimer % 3 == 0)
                {
                    for (int i = 0; i < 3; i++)
                        Dust.NewDustPerfect(NPC.Bottom, DustID.Stone, -NPC.velocity.SafeNormalize(Vector2.UnitX), 0, default, 2f);
                }

                //冲到位了
                if ((TargetLocation - NPC.Center).Length() < 16f || StateTimer > 1200)
                {
                    NPC.noTileCollide = false;
                    StateTimer = 0;
                    RestDuration = 60f;
                    State = (int)AIStates.Attack_Shoot;
                }
            }
        }

        public void Jump(Player target, Vector2 offset, float speed)
        {
            StateTimer++;

            if (StateTimer < 30)//起跳准备
            {
                NPC.noGravity = true;
                NPC.velocity = Vector2.Zero;

                //生成粒子
                if (Main.netMode != NetmodeID.Server && StateTimer % 3 == 0)
                {
                    for (int i = 0; i < 3; i++)
                        Dust.NewDustPerfect(NPC.Bottom, DustID.Stone, -Vector2.UnitX.RotatedByRandom(3.14f), 0, default, 2f);
                }
            }
            else if (StateTimer == 30)//瞄准
            {
                TargetLocation = target.Center;
                TargetLocation += target.velocity * 2f + target.direction * Vector2.UnitX * 100f + offset;

                NPC.direction = NPC.spriteDirection = Math.Sign(target.Center.X - NPC.Center.X);
                NPC.noTileCollide = true;

                if (Main.netMode != NetmodeID.Server)
                    Helper.PlayPitched(Name + "/Roar_Dash", 1f, 0.5f, NPC.Center);
            }
            else if (StateTimer > 30)
            {
                if ((NPC.Center - TargetLocation).Length() > 16f)//冲到目标点
                {
                    NPC.velocity = (TargetLocation - NPC.Center).SafeNormalize(Vector2.UnitY) * speed;
                    NPC.rotation = NPC.direction > 0 ? NPC.velocity.ToRotation() : NPC.velocity.ToRotation() + 3.14f;
                }
                else
                {
                    StateTimer = 0;
                    State = (int)AIStates.Attack_Jump_Fall;
                }
            }
        }

        public void Shoot(Player target)
        {
            if (StateTimer == 0)
            {
                NPC.direction = NPC.spriteDirection = Math.Sign(target.Center.X - NPC.Center.X);
                TargetLocation = target.Center + target.velocity * 2f;
            }

            StateTimer++;
            if (StateTimer == 30f)
            {
                float targetRotation = (TargetLocation - NPC.Center).ToRotation();
                //生成弹幕
                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + Vector2.One * NPC.direction * 100f, Vector2.UnitX.RotatedBy(targetRotation + Math.PI * i / 8) * 10f, ProjectileType<SIP_StoneBall>(), 10, 0.3f);
                }

                if (Main.netMode != NetmodeID.Server)
                    Helper.PlayPitched(Name + "/Roar_Shoot", 1f, 0.5f, NPC.Center);

                RestDuration = 30f;
                StateTimer = 0;
                State = (int)AIStates.Move;
            }
        }

        public void Fall(Player target, float speedY)
        {
            if (StateTimer == 0)
            {
                NPC.velocity = new Vector2(0, speedY);
                NPC.noGravity = false;
                NPC.noTileCollide = true;

                if (Main.netMode != NetmodeID.Server)
                    Helper.PlayPitched(Name + "/Roar_Fall", 1f, 0.5f, NPC.Center);

            }

            StateTimer++;
            if (NPC.Center.Y <= target.Center.Y - 100f)
                NPC.noTileCollide = true;
            else
            {
                NPC.noTileCollide = false;
                if (NPC.velocity.Y < 0.5f)
                {
                    //生成一个弹幕(效果大概是砸地面的效果)
                    StateTimer = 0;
                    RestDuration = 60f;
                    State = (int)AIStates.Move;
                }
            }
        }

        #endregion

        #region Draw

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = Request<Texture2D>(Texture).Value;

            int frameWidth = mainTex.Width;
            int frameHeight = mainTex.Height / Main.npcFrameCount[NPC.type];
            //Rectangle frameBox = new Rectangle(0, (yFrame * frameHeight), frameWidth, frameHeight);

            Rectangle frameBox = new Rectangle(0, 0, frameWidth, frameHeight);

            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

            if (NPC.spriteDirection != 1)
                effects = SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, NPC.Center - screenPos, frameBox, drawColor, NPC.rotation, origin, NPC.scale, effects, 0f);
            return false;
        }

        #endregion

        #region Network

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetX);
            writer.Write(targetY);

            writer.Write(NPC.target);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetX = reader.ReadSingle();
            targetY = reader.ReadSingle();

            NPC.target = reader.ReadInt32();
        }

        #endregion

    }
}
