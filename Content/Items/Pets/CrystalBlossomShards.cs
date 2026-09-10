using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Coralite.Content.Items.Pets
{
    public class CrystalBlossomShards : ModItem
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        private static PrimitivePRTGroup group;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TenkoFigurine>();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<Kitsune>(), ModContent.BuffType<KitsuneBuff>());
            Item.damage = 20;
            Item.width = 28;
            Item.height = 20;
            Item.rare = ModContent.RarityType<CrystalBlossomShardsRarity>();
            Item.value = Item.sellPrice(0, 50);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true, false);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            group?.Update();
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "HorizontalLight").Value;

                Vector2 origin = new(0, mainTex.Height / 2);
                Color c = Color.Pink;
                c.A = 0;
                c *= 0.25f + (MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.2f);

                for (int i = 0; i < 5; i++)
                {
                    Main.spriteBatch.Draw(mainTex, new Vector2(line.X - 10, line.Y), null, c,
                        i * 0.22f, origin, 0.7f - (i * 0.17f), 0, 0);
                }
            }

            return true;
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                group ??= new PrimitivePRTGroup();
                if (group != null)
                {
                    if (!Main.gamePaused && Main.GameUpdateCount % 20 == 0)
                    {
                        Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
                        group.NewParticle<Petal>(new Vector2(line.X, line.Y) + new Vector2(Main.rand.NextFloat(0, size.X), Main.rand.Next(-8, 0)),
                            Main.rand.NextFloat(0.585f - 0.3f, 0.585f + 0.3f).ToRotationVector2() * Main.rand.NextFloat(0.2f, 0.5f)
                            , Color.Pink, Main.rand.NextFloat(0.8f, 1f));
                    }
                }
                group?.DrawInUI(Main.spriteBatch);
            }
        }
    }

    public class CrystalBlossomShardsRarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(255, 152, 210), Color.Pink, Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly * 3)));
    }

    public class Petal : Particle
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "NightmarePetal";

        public override void SetProperty()
        {
            Frame = new Rectangle(0, Main.rand.Next(8) * 14, 10, 14);
            ShouldKillWhenOffScreen = false;
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Position += Velocity;
            Rotation += Main.rand.NextFloat(0.13f, 0.18f);
            Velocity *= 0.99f;
            if (Opacity > 45)
                Color *= 0.88f;
            if (Opacity % 8 == 0)
            {
                Frame.Y += 14;
                if (Frame.Y > 98)
                    Frame.Y = 0;
            }

            Opacity++;
            if (Opacity > 60)
                active = false;
        }

        public override void DrawInUI(SpriteBatch spriteBatch)
        {
            Rectangle frame = Frame;
            Vector2 origin = new(frame.Width / 2, frame.Height / 2);
            Color c = Color;
            if (Opacity < 6)
            {
                c *= Opacity / 6;
            }
            spriteBatch.Draw(TexValue, Position, frame, c, Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }

    public class KitsuneBuff : ModBuff
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<Kitsune>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, player.miscEquips[0].damage, 0f, player.whoAmI);
            }
        }
    }

    public class Kitsune : BasePetProj
    {
        public override string Texture => AssetDirectory.PetItems + Name;
        protected override int PetBuffType => ModContent.BuffType<KitsuneBuff>();

        /// <summary>
        /// 宠物状态机
        /// </summary>
        private enum AIState
        {
            /// <summary> 地面行走/跟随状态 </summary>
            Walking = 0,
            /// <summary> 飞行跟随状态 </summary>
            Flying = 1,
            /// <summary> 攻击状态 </summary>
            Attacking = 2
        }

        /// <summary> 当前AI状态 </summary>
        private AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        /// <summary> 状态计时器/攻击冷却计时器 </summary>
        private ref float StateTimer => ref Projectile.ai[1];

        /// <summary> 攻击检测范围 </summary>
        private const float AttackRange = 300f;

        /// <summary> 攻击持续时间 </summary>
        private const int AttackDuration = 20;

        protected override void SetPetStaticDefaults()
        {
            Main.projFrames[Type] = 15;
        }

        protected override void SetPetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 34;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 100;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
            Projectile.decidesManualFallThrough = true;
        }

        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // 检查玩家是否存活
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }

            // 检查宠物状态
            CheckActive(player);

            // 默认不友好（不造成伤害），只在攻击状态下才友好
            Projectile.friendly = false;

            // 计算跟随目标位置（玩家身后）
            Vector2 followTarget = player.Center;
            if (player.direction > 0)
                followTarget.X -= 40 * player.direction;
            else
                followTarget.X -= (45 + player.width) * player.direction;

            // 根据玩家位置设置是否穿过平台
            Projectile.shouldFallThrough = player.position.Y + player.height - 12f > Projectile.position.Y + Projectile.height;

            // 查找攻击目标（只在Walking状态下查找）
            int attackTarget = -1;
            if (State == AIState.Walking || State == AIState.Flying)
            {
                Projectile.Minion_FindTargetInRange((int)AttackRange, ref attackTarget, skipIfCannotHitWithOwnBody: true);
                Projectile.ai[2] = attackTarget;
            }

            // 状态机
            switch (State)
            {
                case AIState.Walking:
                    CheckAttack(attackTarget);
                    AI_Walking(player, followTarget, attackTarget);

                    break;

                case AIState.Flying:
                    CheckAttack(attackTarget);
                    AI_Flying(player);
                    break;

                case AIState.Attacking:
                    AI_Attacking(player, (int)Projectile.ai[2]);
                    break;
            }

        }

        // 攻击检测逻辑（独立于状态机，在所有状态后执行）
        // 原始代码在这里检测攻击，可以从任何状态进入攻击状态
        private void CheckAttack(int attackTarget)
        {
            if (Main.hardMode && attackTarget >= 0)
            {
                NPC target = Main.npc[attackTarget];
                Vector2 targetCenter = target.Center;

                if (Projectile.IsInRangeOfMeOrMyOwner(target, AttackRange, out _, out _, out _))
                {
                    Projectile.shouldFallThrough = target.Center.Y > Projectile.Bottom.Y;

                    bool canJump = Projectile.velocity.Y == 0f;
                    if (Projectile.wet && Projectile.velocity.Y > 0f && !Projectile.shouldFallThrough)
                        canJump = true;

                    // 如果目标在上方，跳向目标
                    if (targetCenter.Y < Projectile.Center.Y - 30f && canJump)
                    {
                        float heightDiff = (targetCenter.Y - Projectile.Center.Y) * -1f;
                        float gravity = 0.4f;
                        float jumpSpeed = (float)Math.Sqrt(heightDiff * 2f * gravity);
                        if (jumpSpeed > 26f)
                            jumpSpeed = 26f;

                        Projectile.velocity.Y = 0f - jumpSpeed;
                    }

                    // 距离足够近时进入攻击状态
                    if (Vector2.Distance(Projectile.Center, targetCenter) < 20f * 16)
                    {
                        if (Projectile.velocity.Length() > 10f)
                            Projectile.velocity /= Projectile.velocity.Length() / 10f;

                        State = AIState.Attacking;
                        StateTimer = AttackDuration;
                        Projectile.netUpdate = true;
                        Projectile.direction = targetCenter.X - Projectile.Center.X > 0f ? 1 : -1;
                    }
                }
            }
        }

        /// <summary>
        /// 地面行走状态AI
        /// </summary>
        private void AI_Walking(Player player, Vector2 followTarget, int attackTarget)
        {
            // 更新动画
            UpdateWalkingAnimation();

            // 使用基类封装的地面移动方法
            GroundMovement(
                player.Center,
                maxSpeed: 6f,
                acceleration: 0.5f,
                braking: 0.1f,
                gravity: 0.4f,
                maxFallSpeed: 10f,
                maxFollowDistance: 500f,
                maxVerticalDistance: 300f,
                followOffset: followTarget - player.Center
            );

            // 检查是否应该切换到飞行状态
            Vector2 toPlayer = player.Center - Projectile.Center;
            float verticalDistance = Math.Abs(toPlayer.Y);

            if ((player.velocity.Y != 0f && verticalDistance > 100f) ||
                verticalDistance > 200f ||
                Vector2.Distance(Projectile.Center, player.Center) > 500f)
            {
                State = AIState.Flying;
                StateTimer = 0f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
        }

        /// <summary>
        /// 飞行状态AI
        /// </summary>
        private void AI_Flying(Player player)
        {
            // 使用基类封装的飞行移动方法
            bool isLanding = FlyMovement(
                player: player,
                acceleration: 0.2f,
                maxSpeed: 10f,
                landingDistance: 200f,
                stopDistance: 60f
            );

            // 添加发光效果
            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.3f, 0.3f));

            // 添加粒子效果
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(6, 6) +
                    new Vector2(-Projectile.spriteDirection * 14, 14).RotatedBy(Projectile.rotation),
                    DustID.Firework_Pink,
                    -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.6f),
                    50,
                    Scale: Main.rand.NextFloat(0.7f, 1f)
                );
                d.noGravity = true;
            }

            // 更新动画
            UpdateFlyingAnimation(isLanding);

            // 检查是否应该切换到地面行走状态
            if (isLanding)
            {
                State = AIState.Walking;
                StateTimer = 0f;
                Projectile.netUpdate = true;
            }

            // 如果距离玩家很远，传送回来
            if (Vector2.Distance(Projectile.Center, player.Center) > 2000f)
            {
                TeleportToOwner(player);
            }
        }

        /// <summary>
        /// 攻击状态AI
        /// </summary>
        private void AI_Attacking(Player player, int attackTarget)
        {
            // 启用接触伤害
            Projectile.friendly = true;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = 0f;

            // 根据剩余时间计算攻击帧（与原始代码一致）
            // num9 = AttackDuration = 20
            // frame = 4 + ((num9 - ai[1]) / (num9 / 3))
            // 这会产生: 4, 5, 6 三个帧（当StateTimer从20降到0时）
            int frameOffset = (int)((AttackDuration - StateTimer) / (AttackDuration / 3));
            if (frameOffset > 2)
                frameOffset = 2; // 限制在0-2范围内

            Projectile.frame = 4 + frameOffset;

            // 如果速度够快，使用不同的帧（帧8-10）
            if (Math.Abs(Projectile.velocity.X) > 4.9f)
                Projectile.frame += 4;

            // 应用重力
            Projectile.velocity.Y += 0.4f;
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;

            // 攻击计时器递减
            StateTimer -= 1f;

            if (attackTarget.GetNPCOwner(out NPC target))
            {
                GroundMovement(
                 target.Center,
                 maxSpeed: 8f,
                 acceleration: 0.5f,
                 braking: 0.5f,
                 gravity: 0.4f,
                 maxFallSpeed: 10f,
                 maxFollowDistance: 500f,
                 maxVerticalDistance: 300f,
                 Vector2.Zero);

                if (Projectile.velocity.Y > 0 && target.Bottom.Y < Projectile.Top.Y && MathF.Abs(target.Bottom.Y - Projectile.Top.Y) > 16 * 4)
                {
                    Projectile.velocity.Y = -6 + 10 * Helper.Clamp((target.Center.Y - Projectile.Center.Y) / 100, -1, 0);
                    for (int i = 0; i < 3; i++)
                    {
                        Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Bottom + new Vector2(Main.rand.Next(-20, 20), 0), Vector2.UnitY.RotateByRandom(-0.2f, 0.2f), GoreID.Smoke1 + i);
                    }
                }
            }

            // 攻击结束，返回行走状态
            if (StateTimer <= 0f)
            {
                State = AIState.Walking;
                StateTimer = 0f;
                Projectile.netUpdate = true;
            }
        }

        /// <summary>
        /// 更新飞行动画
        /// </summary>
        private void UpdateFlyingAnimation(bool isLanding)
        {
            if (isLanding)
            {
                // 降落动画
                Projectile.frameCounter = 0;
                Projectile.frame = 14;
            }
            else
            {
                // 飞行动画（帧12-13）
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame < 12 || Projectile.frame > 13)
                        Projectile.frame = 12;
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
        }

        /// <summary>
        /// 更新地面行走动画
        /// </summary>
        private void UpdateWalkingAnimation()
        {
            Projectile.rotation = 0f;

            if (Projectile.velocity.Y != 0f)
            {
                // 跳跃动画
                Projectile.frameCounter = 0;
                Projectile.frame = 14;
            }
            else if (Projectile.velocity.X == 0f)
            {
                // 站立动画
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
            else if (Math.Abs(Projectile.velocity.X) >= 0.5f)
            {
                // 行走动画（帧0-3，每10帧切换）
                Projectile.frameCounter += (int)Math.Abs(Projectile.velocity.X);
                Projectile.UpdateFrameNormally(10, 3);
            }
            else
            {
                // 慢速移动时使用站立动画
                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            var pos = Projectile.Center - Main.screenPosition;
            var frameBox = mainTex.Frame(1, 15, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;
            var effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation, origin, Projectile.scale, effect, 0);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Glow").Value, pos, frameBox, Color.White, Projectile.rotation, origin, Projectile.scale, effect, 0);

            return false;
        }
    }
}
