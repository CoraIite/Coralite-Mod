using Coralite.Content.ModPlayers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.ID;
using static Coralite.Content.WorldGeneration.ShadowCastleRoom;

namespace Coralite.Helpers
{
    public static partial class Helper
    {
        public static void Kill(this NPC NPC)
        {
            bool ModNPCDontDie = NPC.ModNPC?.CheckDead() == false;
            if (ModNPCDontDie)
                return;
            NPC.life = 0;
            NPC.checkDead();
            NPC.HitEffect();
            NPC.active = false;
        }

        public static void InstanceKill(this NPC NPC)
        {
            NPC.life = 0;
            NPC.active = false;
        }

        public static Texture2D GetTexture(this NPC npc)
        {
            return TextureAssets.Npc[npc.type].Value;
        }

        public static Rectangle GetFrameBox(this NPC npc, int xFrame)
        {
            Texture2D tex = npc.GetTexture();
            return tex.Frame(xFrame, Main.npcFrameCount[npc.type], npc.frame.X, npc.frame.Y);
        }

        /// <summary>
        /// 获取NPC的图鉴描述信息
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static FlavorTextBestiaryInfoElement GetBestiaryDescription(this ModNPC npc)
            => new FlavorTextBestiaryInfoElement(npc.GetLocalizationKey("BestiaryDescription"));

        /// <summary>
        /// 在图鉴里隐藏该NPC
        /// </summary>
        /// <param name="npc"></param>
        public static void SetHideInBestiary(this NPC npc)
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(npc.type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            });
        }

        /// <summary>
        /// 注册NPC的图鉴描述信息
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public static void RegisterBestiaryDescription(this ModNPC npc)
        {
            if (Main.dedServ)
                return;
            npc.GetLocalization("BestiaryDescription");
        }

        public static void QuickDraw(this NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor, Rectangle? frameBox = null, SpriteEffects effect = SpriteEffects.None, float exRot = 0)
        {
            Texture2D tex = npc.GetTexture();
            spriteBatch.Draw(tex, npc.Center - screenPos, frameBox, lightColor
                , npc.rotation + exRot, frameBox == null ? tex.Size() / 2 : frameBox.Value.Size() / 2, npc.scale, effect, 0);
        }

        /// <summary>
        /// 简易NPC运动，控制单个方向上的运动，做匀加速和匀减速运动
        /// </summary>
        /// <param name="velocity">输入的单个方向的速度</param>
        /// <param name="direction">方向</param>
        /// <param name="velocityLimit">速度限制</param>
        /// <param name="accel">加速度</param>
        /// <param name="turnAccel">转向加速度</param>
        /// <param name="slowDownPercent">减速系数</param>
        public static void Movement_SimpleOneLine(ref float velocity, int direction, float velocityLimit, float accel, float turnAccel, float slowDownPercent)
        {
            if (Math.Abs(velocity) > velocityLimit)
                velocity *= slowDownPercent;
            else
            {
                velocity += direction * (Math.Sign(velocity) == direction ? accel : turnAccel);
                if (Math.Abs(velocity) > velocityLimit)
                    velocity = direction * velocityLimit;
            }
        }

        /// <summary>
        /// 简易NPC运动，控制单个方向上的运动，做匀加速和匀减速运动
        /// </summary>
        /// <param name="velocity">输入的单个方向的速度</param>
        /// <param name="currentDistance">当前的距离</param>
        /// <param name="direction">方向</param>
        /// <param name="velocityLimit">速度限制</param>
        /// <param name="distanceLimit">距离限制</param>
        /// <param name="accel">加速度</param>
        /// <param name="turnAccel">转向加速度</param>
        /// <param name="slowDownPercent">减速系数</param>
        public static void Movement_SimpleOneLine_Limit(ref float velocity, float currentDistance, int direction, float velocityLimit
            , float distanceLimit, float accel, float turnAccel, float slowDownPercent)
        {
            if (currentDistance > distanceLimit)
                Movement_SimpleOneLine(ref velocity, direction, velocityLimit, accel, turnAccel, slowDownPercent);
            else
                velocity *= slowDownPercent;
        }
        /// <summary>
        /// 原版函数TargetClosest的重写/变体，增加了一个参数可以让NPC忽略某些玩家<br></br>
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="faceTarget"></param>
        /// <param name="indexes"></param>
        public static void TargetCloestIgnoreIndex(NPC npc, bool faceTarget = false, params int[] indexes)
        {
            float minDist = float.MaxValue;
            int targetPlayer = -1;
            int tankProjectile = -1;
            Vector2 npcCenter = npc.Center;

            void UpdateDirection(Vector2 targetCenter)
            {
                npc.direction = targetCenter.X < npcCenter.X ? -1 : 1;
                npc.directionY = targetCenter.Y < npcCenter.Y ? -1 : 1;
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead || player.ghost || indexes.Contains(player.whoAmI))
                    continue;

                Vector2 playerCenter = player.Center;
                float distToPlr = Math.Abs(playerCenter.X - npcCenter.X) + Math.Abs(playerCenter.Y - npcCenter.Y);
                float dist = distToPlr - player.aggro;

                if (player.npcTypeNoAggro[npc.type] && npc.direction != 0)
                    dist += 1000f;

                //更新目标
                if (dist < minDist)
                {
                    minDist = dist;
                    targetPlayer = player.whoAmI;
                    npc.target = targetPlayer;
                    tankProjectile = -1;
                }

                //检查tankPet
                if (player.tankPet >= 0 && !player.npcTypeNoAggro[npc.type])
                {
                    Projectile tankProj = Main.projectile[player.tankPet];
                    Vector2 tankCenter = tankProj.Center;
                    float tankDistance = Math.Abs(tankCenter.X - npcCenter.X) + Math.Abs(tankCenter.Y - npcCenter.Y);
                    float tankPriority = tankDistance - 200f;

                    if (tankPriority < minDist && tankPriority < 200f && Collision.CanHit(npcCenter, 1, 1, tankCenter, 1, 1))
                        tankProjectile = player.tankPet;
                }
            }

            //处理tankPet目标
            if (tankProjectile >= 0)
            {
                Projectile tankProj = Main.projectile[tankProjectile];
                npc.targetRect = new Rectangle((int)tankProj.position.X, (int)tankProj.position.Y, tankProj.width, tankProj.height);
                UpdateDirection(tankProj.Center);
            }
            //处理玩家目标
            else
            {
                if (targetPlayer < 0 || targetPlayer >= 255)
                    targetPlayer = 0;

                npc.target = targetPlayer;
                Player target = Main.player[targetPlayer];
                npc.targetRect = new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height);

                // 检查是否需要面向目标
                bool shouldFaceTarget = faceTarget;
                if (target.dead || (target.npcTypeNoAggro[npc.type] && npc.direction != 0))
                    shouldFaceTarget = false;

                if (shouldFaceTarget)
                {
                    bool oldTargetValid = npc.oldTarget is >= 0 and <= 254;
                    bool lowAggroNoAnim = target.itemAnimation == 0 && target.aggro < 0;
                    bool notBoss = !npc.boss;

                    if (!(lowAggroNoAnim && oldTargetValid && notBoss))
                        UpdateDirection(target.Center);
                }
            }

            if (npc.confused)
                npc.direction *= -1;

            if ((npc.direction != npc.oldDirection || npc.directionY != npc.oldDirectionY || npc.target != npc.oldTarget) && !npc.collideX && !npc.collideY)
                npc.netUpdate = true;
        }

        /// <summary>
        /// 返回NPC的索引，如果没找到则返回-1
        /// </summary>
        /// <param name="npcType"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static int GetNPCByType(int npcType)
        {
            int index = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == npcType)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        public static bool GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper)
        {
            if (!Main.GameModeInfo.IsJourneyMode) //从源码里抄过来的，只能说旅途模式写的什么B玩意
            {
                journeyScale = 1f;
                nPCStrengthHelper = default;
                return false;
            }

            journeyScale = 1f;
            CreativePowers.DifficultySliderPower power = CreativePowerManager.Instance.GetPower<CreativePowers.DifficultySliderPower>();
            if (power != null && power.GetIsUnlocked())
                journeyScale = power.StrengthMultiplierToGiveNPCs;

            nPCStrengthHelper = new NPCStrengthHelper(Main.GameModeInfo, journeyScale, Main.getGoodWorld);
            return true;
        }

        /// <summary>
        /// 找到同类型并且相同target的NPC，输出一共多少个和自身位置<br></br>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <param name="totalIndexesInGroup"></param>
        [DebuggerHidden]
        public static void GetMyNpcIndexWithModNPC<T>(NPC n, out int index, out int totalIndexesInGroup) where T : ModNPC
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.target == n.target && npc.ModNPC is T)
                {
                    if (n.whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }

        /// <summary>
        /// 根据NPC与目标的位置关系快速设置<br></br>
        /// <see cref="NPC.spriteDirection"/><br></br>
        /// <see cref="NPC.directionY"/><br></br>
        /// <see cref="Entity.direction"/>
        /// </summary>
        /// <param name="npc"></param>
        public static void QuickSetDirection(this NPC npc)
        {
            Player p = Main.player[npc.target];

            npc.direction = npc.spriteDirection = p.Center.X > npc.Center.X ? 1 : -1;
            npc.directionY = p.Center.Y > npc.Center.Y ? 1 : -1;
        }

        public enum NPCTrailingMode
        {
            OnlyPosition = 1,
            RecordAll = 3,
        }

        public static void QuickTrailSets(this NPC npc, NPCTrailingMode trailingMode, int trailCacheLength)
        {
            NPCID.Sets.TrailingMode[npc.type] = (int)trailingMode;
            NPCID.Sets.TrailCacheLength[npc.type] = trailCacheLength;
        }

        public static int GetProjDamage(int normalDamage, int expertDamage, int masterDamage)
        {
            return ScaleValueForDiffMode(normalDamage / 2, expertDamage / 4, masterDamage / 6, masterDamage / 6);
        }

        [DebuggerHidden]
        public static int NewProjectileInAI(this NPC npc, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return Projectile.NewProjectile(npc.GetSource_FromAI(), position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
        }

        [DebuggerHidden]
        public static int NewProjectileInAI<T>(this NPC npc, Vector2 position, Vector2 velocity, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
            where T : ModProjectile
        {
            return Projectile.NewProjectile(npc.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<T>(), damage, knockBack, owner, ai0, ai1, ai2);
        }

        [DebuggerHidden]
        public static Projectile NewProjectileDirectInAI(this NPC npc, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return Projectile.NewProjectileDirect(npc.GetSource_FromAI(), position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
        }

        [DebuggerHidden]
        public static Projectile NewProjectileDirectInAI<T>(this NPC npc, Vector2 position, Vector2 velocity, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
            where T : ModProjectile
        {
            return Projectile.NewProjectileDirect(npc.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<T>(), damage, knockBack, owner, ai0, ai1, ai2);
        }

        public static void InitOldPosCache(this NPC npc, int trailCount, bool useCenter = true)
        {
            if (npc.oldPos.Length != trailCount)
                npc.oldPos = new Vector2[trailCount];

            Vector2 pos = npc.position;
            if (useCenter)
                pos = npc.Center;
            for (int i = 0; i < trailCount; i++)
            {
                npc.oldPos[i] = pos;
            }
        }

        public static void InitOldRotCache(this NPC npc, int trailCount)
        {
            if (npc.oldRot.Length != trailCount)
                npc.oldRot = new float[trailCount];

            for (int i = 0; i < trailCount; i++)
            {
                npc.oldRot[i] = npc.rotation;
            }
        }

        public static void UpdateOldPosCache(this NPC npc, bool useCenter = true, bool addVelocity = true)
        {
            for (int i = 0; i < npc.oldPos.Length - 1; i++)
                npc.oldPos[i] = npc.oldPos[i + 1];
            npc.oldPos[^1] = (useCenter ? npc.Center : npc.position) + (addVelocity ? npc.velocity : Vector2.Zero);
        }

        public static void UpdateOldRotCache(this NPC npc)
        {
            for (int i = 0; i < npc.oldRot.Length - 1; i++)
                npc.oldRot[i] = npc.oldRot[i + 1];
            npc.oldRot[^1] = npc.rotation;
        }

        public static void LoadGore(this ModNPC modnpc, int count)
        {
            for (int i = 0; i < count; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(modnpc.Mod, modnpc.Texture + "_Gore" + i);
        }

        public static void SpawnGore(this ModNPC modnpc, int count, float speed = 1)
        {
            for (int i = 0; i < count; i++)
                Gore.NewGoreDirect(modnpc.NPC.GetSource_Death()
                    , Main.rand.NextVector2FromRectangle(modnpc.NPC.Hitbox)
                    , Main.rand.NextVector2Circular(speed, speed), modnpc.Mod.Find<ModGore>(modnpc.Name + "_Gore" + i).Type);
        }

        public static void StartHitLimitChallenge(int hitLimit, Action onFail)
        {
            foreach (var p in Main.ActivePlayers)
            {
                if (p.TryGetModPlayer(out CoralitePlayer cp))
                    cp.StartChallenge(hitLimit, onFail);
            }
        }

        /// <summary>
        /// 返回<see cref="true"/>表示有超过限制的东西。
        /// </summary>
        /// <param name="weaponDamage"></param>
        /// <param name="Rarity"></param>
        /// <returns></returns>
        public static bool WeaponLimitChallenge(int weaponDamage, int Rarity)
        {
            foreach (var p in Main.ActivePlayers)
            {
                for (int i = 0; i < 59; i++)
                {
                    if (i >= 50 && i <= 53)
                        continue;
                    Item item = p.inventory[i];
                    if (CheckItemLimit(p,item, weaponDamage, Rarity,-1))
                        return true;
                }
            }

            return false;

        }

        public static bool ArmorLimitChallenge(int defence, int Rarity)
        {
            foreach (var p in Main.ActivePlayers)
            {
                for (int i = 0; i < 10; i++)
                {
                    Item item = p.armor[i];
                    if (CheckItemLimit(p, item, -1, Rarity, defence))
                        return true;
                }
            }

            return false;
        }

        public static bool CheckItemLimit(Player p, Item i, int weaponDamage, int weaponRarity, int defenct)
        {
            if (i.IsAir)
                return false;

            if (weaponDamage > 0 && i.damage > 0)
            {
                if (p.GetWeaponDamage(i) > weaponDamage)
                    return true;
                if (i.rare == ItemRarityID.Expert || i.rare > weaponRarity)
                    return true;
            }

            if (defenct > 0)
            {
                if (i.rare == ItemRarityID.Expert || i.rare > weaponRarity)
                    return true;
                if (i.defense > defenct)
                    return true;
            }

            return false;
        }

    }
}
