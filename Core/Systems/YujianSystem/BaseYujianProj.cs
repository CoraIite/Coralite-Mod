using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using Coralite.Core.Systems.YujianSystem.YujianAIs;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Systems.YujianSystem
{
    /// <summary>
    /// 御剑弹幕基类，请确保所有的御剑弹幕都继承自这个基类
    /// </summary>
    public class BaseYujianProj : ModProjectile, IDrawNonPremultiplied, IDrawAdditive, IDrawPrimitive
    {
        public readonly Color color1;
        public readonly Color color2;
        public readonly int trailCacheLenth;
        private readonly int Width;
        private readonly int Height;
        private readonly string TexturePath;
        private readonly bool PathHasName;
        private readonly float PowerfulAttackCost;
        private readonly bool TileCollide;
        public bool AimMouse;
        public bool canChannel;
        public int targetIndex;
        public int channelTime;
        private int attackLenth;
        public Vector2 aimCenter;
        public IHuluEffect huluEffect;
        public YujianAI[] yujianAIs;
        public YujianAI specialAI;
        public YujianAI powerfulAI;

        public const float SpecialMoveState = 0.1f;
        public const float PowerfulMoveState = 0.2f;

        public int AttackLenth { get => attackLenth; set => attackLenth = value; }
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public virtual string SlashTexture => AssetDirectory.OtherProjectiles + "NormalSlashTrail";

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name.Replace("Proj", ""));

        public BaseYujianProj(YujianAI[] yujianAIs, YujianAI specialAI, YujianAI powerfulAI, float PowerfulAttackCost, int attackLenth, int width, int height, Color color1, Color color2, int trailCacheLenth = 30, bool tileCollide = true, string texturePath = AssetDirectory.YujianHulu, bool pathHasName = false)
        {
            if (yujianAIs is null || powerfulAI is null)
                throw new Exception("普通攻击或强化攻击不能为null ! ! ! ! ! ! ! ! ! ! ! !");

            this.yujianAIs = yujianAIs;
            this.specialAI = specialAI;
            this.powerfulAI = powerfulAI;
            this.PowerfulAttackCost = PowerfulAttackCost;
            AttackLenth = attackLenth;
            Width = width;
            Height = height;
            this.color1 = color1;
            this.color2 = color2;
            this.trailCacheLenth = trailCacheLenth;
            TileCollide = tileCollide;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        #region SetDefaults

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Width;
            Projectile.height = Height;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 300;
            Projectile.localNPCHitCooldown = 25;
            Projectile.extraUpdates = 2;

            Projectile.ignoreWater = false;
            Projectile.tileCollide = TileCollide;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;

            PostSetDefaults();
        }

        public virtual void PostSetDefaults() { }

        #endregion

        #region AI

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[trailCacheLenth];
            Projectile.oldRot = new float[trailCacheLenth];

            InitTrailCaches();
        }

        public sealed override void AI()
        {
            //如果玩家输入了左键，那么将开始计时，在完成一整个攻击动作后检测计时器状态，时间短的话那么将命令御剑瞄准鼠标位置进行攻击
            //时间长的话就执行特殊攻击动作（如果有的话）
            if (canChannel)
            {
                if (Owner.channel)
                {
                    channelTime++;
                    Owner.itemTime = Owner.itemAnimation = 10;
                    if (channelTime == 30 && specialAI is not null)
                    {
                        State = SpecialMoveState;
                        AimMouse = false;
                        specialAI.OnStart(this);
                    }
                }
                else
                {
                    //判断按住的时长，并根据时长去调整攻击动作
                    if (channelTime > 3)
                        AimMouse = true;

                    channelTime = 0;
                    if (State == SpecialMoveState)
                        ChangeState();
                }
            }

            if (!AIBefore())
                return;

            //如果能运行到这里说明已经找到
            //攻击找到的目标，或是进行特殊攻击动作
            YujianAI currentAI = GetCurrentAI();
            if (!CheckCanAttack(currentAI))
                ChangeState();

            currentAI.AttackAI(this);

            UpdateCaches();
            huluEffect?.AIEffect(Projectile);
            AIEffect();
        }

        public bool AIBefore()
        {
            Player player = Main.player[Projectile.owner];
            CoralitePlayer cp = player.GetModPlayer<CoralitePlayer>();
            if (player.dead)
                cp.ownedYujianProj = false;

            if (cp.ownedYujianProj)
                Projectile.timeLeft = 2;

            if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
            {
                State = 0f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }

            switch (State)
            {
                default:
                    break;
                case -1f:       //回到玩家身边
                    ProjectilesHelper.GetMyProjIndexWhihModProj<BaseYujianProj>(Projectile, out var index, out var totalIndexesInGroup);
                    GetIdlePosition(index, totalIndexesInGroup, out var idleSpot, out var idleRotation);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 10f);
                    Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.08f);
                    Projectile.tileCollide = false;
                    if (Projectile.Distance(idleSpot) < 2f)
                    {
                        State = 0f;
                        Projectile.netUpdate = true;
                    }

                    return false;
                case 0f:        //尝试开始攻击
                    ProjectilesHelper.GetMyProjIndexWhihModProj<BaseYujianProj>(Projectile, out var index2, out var totalIndexesInGroup2);
                    GetIdlePosition(index2, totalIndexesInGroup2, out var idleSpot2, out var idleRotation2);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Vector2.SmoothStep(Projectile.Center, idleSpot2, 0.18f);
                    Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation2, 0.18f);
                    Projectile.tileCollide = false;

                    if (ChangeState(cp))
                        break;

                    if (Main.rand.NextBool(20))
                    {
                        int targetNPCIndex = TryAttackingNPCs(Projectile);
                        if (targetNPCIndex != -1)
                        {
                            //随机一个攻击状态，或进行指定攻击状态
                            targetIndex = targetNPCIndex;
                            State = Main.rand.Next(1, yujianAIs.Length);
                            yujianAIs[(int)State - 1].OnStart(this);
                            break;
                        }
                    }

                    return false;
            }

            return true;
        }

        public void ChangeState()
        {
            CoralitePlayer cp = Owner.GetModPlayer<CoralitePlayer>();

            if (ChangeState(cp))
                return;

            int targetNPCIndex = TryAttackingNPCs(Projectile);
            if (targetNPCIndex != -1)
            {
                //随机一个攻击状态，或进行指定攻击状态
                targetIndex = targetNPCIndex;
                State = Main.rand.Next(1, yujianAIs.Length + 1);
                yujianAIs[(int)State - 1].OnStart(this);
            }
            else
            {
                State = -1f;
                Timer = 0f;
                Projectile.netUpdate = true;
            }
        }

        private bool ChangeState(CoralitePlayer cp)
        {
            bool CanAttack = AimMouse && Vector2.Distance(GetTargetCenter(true), Owner.Center) < AttackLenth;
            if (CanAttack && cp.nianli > PowerfulAttackCost)
            {
                State = PowerfulMoveState;
                powerfulAI.OnStart(this);
                cp.nianli -= PowerfulAttackCost;
                return true;
            }
            else if (CanAttack)
            {
                State = Main.rand.Next(1, yujianAIs.Length + 1);
                yujianAIs[(int)State - 1].OnStart(this);
                return true;
            }

            AimMouse = false;
            return false;
        }

        public Vector2 GetTargetCenter(bool isAimingMouse)
        {
            if (isAimingMouse)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    aimCenter = Main.MouseWorld;
                    Projectile.netUpdate = true;
                }

                return aimCenter;
            }

            return Main.npc[targetIndex].Center;
        }

        /// <summary>
        /// 计算出在玩家身边的位置
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="stackedIndex"></param>
        /// <param name="totalIndexes"></param>
        /// <param name="idleSpot"></param>
        /// <param name="idleRotation"></param>
        public void GetIdlePosition(int stackedIndex, int totalIndexes, out Vector2 idleSpot, out float idleRotation)
        {
            idleRotation = (float)Math.PI + Owner.direction * 0.3f;
            float num2 = (totalIndexes - 1f) / 2f;
            idleSpot = Owner.Center - Vector2.UnitY.RotatedBy(4.3982296f / totalIndexes * (stackedIndex - num2)) * 33f - new Vector2(Owner.direction * 8, 8);
        }

        public virtual void AIEffect() { }

        #region HelperMethods

        public static void StartAttack(Projectile Projectile)
        {
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }

        public int TryAttackingNPCs(Projectile Projectile)
        {
            Vector2 ownerCenter = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;

            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(Projectile))
                {
                    float npcDistance2Owner = Vector2.Distance(ownerCenter, nPC.Center);
                    if (npcDistance2Owner <= AttackLenth &&
                            (npcDistance2Owner <= num || num == -1f) &&
                            Collision.CanHitLine(Projectile.Center, 1, 1, nPC.Center, 1, 1))
                    {
                        num = npcDistance2Owner;
                        result = i;
                    }
                }
            }

            return result;
        }

        public YujianAI GetCurrentAI()
        {
            if (State == SpecialMoveState)
                return specialAI;
            if (State == PowerfulMoveState)
                return powerfulAI;

            return yujianAIs[(int)(State - 1)];
        }

        public bool CheckCanAttack(YujianAI currentAI)
        {
            if (currentAI.IsAimingMouse)
                return true;

            if (!Main.npc.IndexInRange(targetIndex))
                return false;

            NPC target = Main.npc[targetIndex];
            return target.CanBeChasedBy();
        }

        public void ResetTileCollide()
        {
            Projectile.tileCollide = TileCollide;
        }

        public void InitTrailCaches()
        {
            for (int i = 0; i < trailCacheLenth; i++)
            {
                Projectile.oldPos[i] = Projectile.Center;
                Projectile.oldRot[i] = Projectile.rotation;
            }
        }

        public void UpdateCaches()
        {
            for (int i = 0; i < trailCacheLenth - 1; i++)
            {
                Projectile.oldRot[i] = Projectile.oldRot[i + 1];
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];
            }

            Projectile.oldPos[trailCacheLenth - 1] = Projectile.Center;
            Projectile.oldRot[trailCacheLenth - 1] = Projectile.rotation;

        }

        #endregion

        #endregion

        #region Collision

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            height = width;
            return true;
        }

        //public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        //{
        //    Vector2 dir = (Projectile.rotation - 1.57f).ToRotationVector2() * Projectile.height / 2;
        //    float a = 0f;

        //    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + dir, Projectile.Center - dir, Projectile.width / 2, ref  a) &&
        //    Collision.CanHitLine(targetHitbox.Center.ToVector2(), 1, 1, Projectile.Center, 1, 1);
        //}

        public override bool? CanDamage()
        {
            if (State > 0f)
                return GetCurrentAI().canDamage;

            return false;
        }

        public sealed override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            HitEffect(target, damage, knockback, crit);
            huluEffect?.HitEffect(Projectile, target, damage, knockback, crit);
        }

        public virtual void HitEffect(NPC target, int damage, float knockback, bool crit) { }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        #endregion

        public override void Kill(int timeLeft)
        {
            yujianAIs = null;
            specialAI = null;
            powerfulAI = null;
        }

        #region draw

        public sealed override bool PreDraw(ref Color lightColor)
        {
            huluEffect?.PreDrawEffect(Projectile, ref lightColor);
            PreDrawEffect(ref lightColor);
            if (State < 0.01f)
                DrawSelf(Main.spriteBatch, lightColor);
            return false;
        }

        public void DrawPrimitives()
        {
            if (State > 0)
                GetCurrentAI().DrawPrimitives(this);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (State > 0)
                GetCurrentAI().DrawAdditive(spriteBatch, this);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
            if (State > 0)
                DrawSelf(spriteBatch, lightColor);
            PostDrawEffect(spriteBatch, lightColor);
            huluEffect?.PostDrawEffect(Projectile, lightColor);
        }

        //并不好，所以弃用了，来源是抄的BossBag的绘制
        //public virtual void DrawShadowWhenCharged(Color lightColor)
        //{
        //    CoralitePlayer cp = Owner.GetModPlayer<CoralitePlayer>();

        //    if (cp.nianli == cp.nianliMax)
        //    {
        //        Texture2D mainTex = TextureAssets.Projectile[Type].Value;
        //        Rectangle frame=mainTex.Frame();

        //        Vector2 frameOrigin = frame.Size() / 2f;
        //        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        //        float time = Main.GlobalTimeWrappedHourly;
        //        float timer =  time * 0.2f;

        //        time %= 4f;
        //        time /= 2f;

        //        if (time >= 1f)
        //            time = 2f - time;

        //        time = time * 0.5f + 0.5f;

        //        for (float i = 0f; i < 1f; i += 0.25f)
        //        {
        //            float radians = (i + timer) * MathHelper.TwoPi;
        //            Main.spriteBatch.Draw(mainTex, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
        //        }

        //        for (float i = 0f; i < 1f; i += 0.34f)
        //        {
        //            float radians = (i + timer) * MathHelper.TwoPi;
        //            Main.spriteBatch.Draw(mainTex, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), Projectile.rotation, frameOrigin, Projectile.scale, SpriteEffects.None, 0);
        //        }
        //    }
        //}

        public virtual void PreDrawEffect(ref Color lightColor) { }

        public void DrawSelf(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, mainTex.Frame(), lightColor, Projectile.rotation, new Vector2(mainTex.Width / 2, mainTex.Height / 2), Projectile.scale, SpriteEffects.None, 0f);
        }

        public virtual void PostDrawEffect(SpriteBatch spriteBatch, Color lightColor) { }

        #endregion

        #region NetWork

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(targetIndex);
            writer.WriteVector2(aimCenter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetIndex = reader.ReadInt32();
            aimCenter = reader.ReadVector2();
        }

        #endregion
    }
}
