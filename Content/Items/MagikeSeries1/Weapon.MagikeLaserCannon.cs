using Coralite.Content.DamageClasses;
using Coralite.Content.Items.Magike.Refractors;
using Coralite.Content.NPCs.Magike;
using Coralite.Content.Particles;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagikeLaserCannon : MagikeChargeableItem, IMagikeCraftable
    {
        public MagikeLaserCannon() : base(120, Item.sellPrice(0, 0, 60),
            ModContent.RarityType<MagicCrystalRarity>(), -1, AssetDirectory.MagikeSeries1Item)
        { }

        public override void SetDefs()
        {
            Item.SetWeaponValues(64, 5f);
            Item.DamageType = MagikeDamage.Instance;

            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<MagikeLaserCannonHeldProj>()] < 1)
                {
                    int damage = player.GetWeaponDamage(Item);
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item),
                        player.Center, Vector2.Zero, ModContent.ProjectileType<MagikeLaserCannonHeldProj>(), damage, 0, player.whoAmI);
                }
            }
            //尽可能消耗玩家身上的魔能来充能
            //var magikeItem = Item.GetMagikeItem();
            //MagikeHelper.TryCosumeMagike(player, magikeItem.MagikeMax - magikeItem.Magike, out int magikeToAdd);
            //magikeItem.Charge(magikeToAdd);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var magike = Item.GetMagikeItem();
            position += Main.screenPosition;
            Main.instance.DrawHealthBar(position.X, position.Y + 12, magike.Magike, magike.MagikeMax, 1, 1);
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<LASERCore>(), ModContent.ItemType<MagikeLaserCannon>(), 200, 1)
                .AddIngredient<MagicCrystal>(20)
                .AddIngredient<Basalt>(20)
                .AddIngredient(ItemID.HellstoneBar)//TODO：用于占位符，要改成武器残页
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.MagikeSeries1Item)]
    public class MagikeLaserCannonHeldProj : BaseChannelProj
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        [VaultLoaden("{@classPath}" + "MagikeLaserCannonHeldProj_Glow")]
        public static ATex GlowTex { get; private set; }
        public ref float State => ref Projectile.ai[0];

        //ai1被channelproj用了作_Rotation

        public ref float Timer => ref Projectile.ai[2];
        /// <summary>
        /// 目前武器魔能百分比
        /// </summary>
        public ref float MagikePercent => ref Projectile.localAI[0];
        public ref float RecoilFactor => ref Projectile.localAI[1];

        private int frameX = -1;

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;

        protected override void AIBefore()
        {
            //取消了基类的设置物品时间

            if (Owner.Alives() && Owner.HeldItem.type == ModContent.ItemType<MagikeLaserCannon>())
                Projectile.timeLeft = 4;
            SetHeld();
            _Rotation = Utils.AngleLerp(_Rotation, ToMouseA, 0.1f);
            Projectile.rotation = _Rotation;
            Owner.itemRotation = Owner.direction > 0 ? _Rotation : _Rotation + 3.141f;
            //Main.NewText($"{Owner.itemTime} {Owner.itemAnimation}");
        }

        protected override void OnChannel()
        {
            Owner.itemTime = Owner.itemAnimation = 3;
            Owner.itemRotation = Owner.direction > 0 ? _Rotation : _Rotation + 3.141f;
            if (State == 0)
            {
                SwitchState(1);
            }
            Timer++;
            if (Timer % 3==0)
                TryCostMagike(Owner);

            //var magikeIncrease = Main.GameUpdateCount % 3 == 0 ? 1 : 0;//每3帧加一次，因为物品最大魔能写多的话太烧魔能了
            //MagikePercent = CostMagike(Owner, magikeIncrease);
        }

        protected override void OnRelease()
        {
            //base.OnRelease();
            if (State is not (0 or 5 or 3 or 4))
            {
                SwitchState(0);
            }
            if (State == 4)
            {
                if (MagikePercent > 0.99f)
                {
                    Vector2 dir = (Owner.MountedCenter - Main.MouseWorld).SafeNormalize(Vector2.One);
                    if (VisualEffectSystem.HitEffect_ScreenShaking)
                    {
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 4, 7, 12, 800));
                    }
                    SwitchState(5);
                }
                else
                    SwitchState(0);
            }
            Timer++;
        }

        protected override void AIAfter()
        {
            int frameRate = 5;
            bool noAnimation = false;
            switch (State)
            {
                default: break;
                case 0://阶段0：闲置，非攻击状态
                    {
                        Projectile.frame = 0;
                        noAnimation = true;
                    }
                    break;
                case 1://阶段1：开始攻击
                    {
                        int timeToAttack = 30;
                        if (Timer == timeToAttack)
                        {
                            int type = ModContent.ProjectileType<MagikeLaserCannonLaser>();
                            if (Owner.ownedProjectileCounts[type] < 1 && Collision.CanHitLine(GetGunpoint(6), 1, 1, Owner.Center, 1, 1))
                            {
                                int damage = (int)(Projectile.damage * 0.75f);
                                Projectile.NewProjectileFromThis<MagikeLaserCannonLaser>(Projectile.Center, Vector2.Zero, damage, Projectile.knockBack, Projectile.whoAmI);
                            }

                            float width = 14;
                            Vector2 pos = GetGunpoint();
                            for (int i = 0; i < 32; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(width * 1.5f, width * 1.5f),
                                    DustID.FireworksRGB, Vector2.Zero, newColor: Coralite.MagicCrystalPink, Scale: 1.5f);
                                dust.noGravity = true;
                                dust.velocity = (dust.position - pos).SafeNormalize(Vector2.UnitX) * width / 10;
                            }
                        }
                        if(Timer < timeToAttack)
                        {
                            float width = Utils.Remap(Timer, 0, timeToAttack, 50, 2);
                            Vector2 pos = GetGunpoint();
                            if (Main.GameUpdateCount % 2 == 0)
                            {
                                Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(width * 1.5f, width * 1.5f),
                                    DustID.FireworksRGB, Vector2.Zero, newColor: Coralite.MagicCrystalPink);
                                dust.noGravity = true;
                                dust.velocity = (pos - dust.position).SafeNormalize(Vector2.UnitX) * width / 10;
                            }
                            for (int i = 0; i < 2; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(width, width), DustID.LastPrism, Vector2.Zero, newColor: Coralite.MagicCrystalPink);
                                dust.noGravity = true;
                            }
                        }

                        if (Main.GameUpdateCount % 28 == 0)
                            GenerateBurstPRT(big: false);
                        if (Main.GameUpdateCount % 6 == 0)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                Vector2 pos = GetGunpoint(offset: -6, offsetY: Main.rand.NextFloat(10, 14) * i);
                                GenerateFlashPRT(pos, 0, _Rotation, 2, 5);
                            }
                        }
                        if (MagikePercent > 0.4f && Timer >= 40 + frameRate * 7)
                            SwitchState(2);
                    }
                    break;
                case 2://阶段2，激光变粗
                    {
                        if (Main.GameUpdateCount % 28 == 0)
                            GenerateBurstPRT(middle: false);
                        if (Main.GameUpdateCount % 3 == 0)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                Vector2 pos = GetGunpoint(offset: -6, offsetY: Main.rand.NextFloat(12, 16) * i);
                                GenerateFlashPRT(pos, 0, _Rotation, 4, 10);
                            }
                        }
                        if (MagikePercent > 0.8f && Timer >= frameRate * 6)
                            SwitchState(3);
                    }
                    break;
                case 3://阶段2.5，转阶段过渡
                    {
                        if (Main.GameUpdateCount % 28 == 0)
                            GenerateBurstPRT(middle: false);
                        if (Main.GameUpdateCount % 3 == 0)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                Vector2 pos = GetGunpoint(offset: -6, offsetY: Main.rand.NextFloat(12, 16) * i);
                                GenerateFlashPRT(pos, 0, _Rotation, 4, 10);
                            }
                        }
                        frameRate = 6;
                        if (Timer >= frameRate * 7)
                            SwitchState(4);
                    }
                    break;
                case 4://阶段3，激光最粗
                    {
                        if (Main.GameUpdateCount % 28 == 0)
                            GenerateBurstPRT(small: false, middle: false);
                        if (Main.GameUpdateCount % 14 == 0)
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                if (Main.rand.NextBool()) continue;

                                GenerateBurstPRT(offsetY: 14 * i, big: false);
                            }
                        }
                        frameRate = 6;
                        if (MagikePercent > 0.99f && Timer > 0)
                        {
                            SoundEngine.PlaySound(CoraliteSoundID.MountSummon_Item25, Projectile.Center);

                            float width = 14;
                            Vector2 pos = GetGunpoint();
                            for (int i = 0; i < 32; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2CircularEdge(width * 1.5f, width * 1.5f),
                                    DustID.FireworksRGB, Vector2.Zero, newColor: Coralite.MagicCrystalPink, Scale: 1f);
                                dust.noGravity = true;
                                dust.velocity = (dust.position - pos).SafeNormalize(Vector2.UnitX) * width / 10;
                            }
                            Timer = int.MinValue / 200;
                        }
                    }
                    break;
                case 5://阶段4，松手爆发
                    {
                        frameRate = 3;
                        if(Timer == 8)
                        {
                            RecoilFactor = 1f;
                        }
                        if(Timer == frameRate * 4 - 3)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Helper.PlayPitched("Crystal/CrystalBurst", 0.8f, 0f, Projectile.Center, (s) => s.PitchVariance = 1f);
                            }
                        }
                        if(Timer == frameRate * 5)
                        {
                            Vector2 pos = GetGunpoint(-10);
                            for (int i = 0; i < 48; i++)
                            {
                                Vector2 dir = _Rotation.ToRotationVector2().RotateRandom(MathHelper.PiOver4);
                                Vector2 position = pos;
                                Vector2 vel = dir * Main.rand.NextFloat(3, 14);
                                var prt = PRTLoader.NewParticle<CrystalFlashParticle>(position, vel);
                            }
                        }
                        if(Timer == frameRate * 6)
                        {
                            Vector2 pos = GetGunpoint(-10);

                            int burstCount = Main.rand.Next(4, 7);
                            for(int i = 0; i < burstCount; i++)
                            {
                                Vector2 dir = _Rotation.ToRotationVector2().RotateRandom(MathHelper.PiOver4);
                                Vector2 vel = dir * Main.rand.NextFloat(1, 9);
                                var prt = PRTLoader.NewParticle<CrystalBurstParticle>(pos, vel);
                                int prtFrame = 0;
                                if (Main.rand.NextBool(3))
                                    prtFrame++;
                                prt.SetFrameX(prtFrame);
                            }

                            ConsumeAllMagike(Owner);

                            {
                                SoundEngine.PlaySound(CoraliteSoundID.CrystalSerpent_Item109 with
                                {
                                    Volume = 1.5f,
                                    Pitch = 0.5f
                                }, Projectile.Center);

                                int type = ModContent.ProjectileType<MagicCrystalMissile>();
                                float speed = 9f;
                                Vector2 vel = Projectile.rotation.ToRotationVector2() * speed;
                                int damage = (int)(Projectile.damage * 3f);
                                var missile = Projectile.NewProjectileFromThis(GetGunpoint(), vel, type, damage, Projectile.knockBack);
                            }
                        }
                        if (Timer > frameRate * 7)
                            SwitchState(0);
                    }
                    break;
            }
            if (!noAnimation) 
                Projectile.UpdateFrameNormally(frameRate, 6);
            RecoilFactor = MathHelper.Lerp(RecoilFactor, 0f, 0.08f);

            if ((State == 1 && Timer > 30) || State > 1)
                TryRegenerateLaser();
            //Main.NewText($"{State} {Timer} {MagikePercent}");
        }

        public void TryRegenerateLaser(float damageMult = 0.75f)
        {
            int type = ModContent.ProjectileType<MagikeLaserCannonLaser>();
            if (Owner.ownedProjectileCounts[type] < 1)
            {
                Timer--;//如果没激光就不能继续蓄力，没物块遮挡才能发射激光
                if (Collision.CanHitLine(GetGunpoint(6), 1, 1, Owner.Center, 1, 1))
                {
                    int damage = (int)(Projectile.damage * damageMult);
                    Projectile.NewProjectileFromThis<MagikeLaserCannonLaser>(Projectile.Center, Vector2.Zero, damage, Projectile.knockBack, Projectile.whoAmI);
                }
            }
        }

        public void SwitchState(int nextState)
        {
            State = nextState;
            Timer = 0;
            Projectile.frame = 0;
            if(nextState > 2)
            {
                SoundEngine.PlaySound(CoraliteSoundID.IcePlaced_Item30 with
                {
                    Volume = 1.2f,
                    Pitch = Utils.Remap(nextState, 2, 5, 0.4f, 1f)
                }, Projectile.Center);

                SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27 with
                {
                    Volume = 1.4f,
                    Pitch = Utils.Remap(nextState, 2, 5, 0.4f, 1f) + 0.2f
                }, Projectile.Center);
            }
        }

        /// <summary>
        /// 给手持魔能激光炮充能，如果物品不对就不会执行
        /// </summary>
        /// <param name="player"></param>
        /// <param name="magikeToCharge"></param>
        /// <returns></returns>
        protected void TryCostMagike(Player player)
        {
            if (player.HeldItem.IsAir || player.HeldItem.type != ModContent.ItemType<MagikeLaserCannon>())
                return ;

            if (player.HeldItem.TryCosumeMagike(1))
            {
                MagikePercent += 0.05f;
                return;
            }

            if (player.TryCosumeMagike(1))
            {
                MagikePercent += 0.05f;
            }
        }

        /// <summary>
        /// 消耗手持魔能激光炮的所有魔能，如果物品不对就不会执行
        /// </summary>
        /// <param name="player"></param>
        protected static void ConsumeAllMagike(Player player)
        {
            if (player.HeldItem.IsAir || player.HeldItem.type != ModContent.ItemType<MagikeLaserCannon>())
                return;
            var magikeItem = player.HeldItem.GetMagikeItem();
            magikeItem.ClearMagike();
        }

        /// <summary>
        /// 基于视觉位置获取枪口位置
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        public Vector2 GetGunpoint(float offset = 0, float offsetY = 0)
        {
            Vector2 dir = _Rotation.ToRotationVector2();
            return GetVisualPosition() + dir * (74 + offset) + dir.RotatedBy(MathHelper.PiOver2) * offsetY;
        }

        /// <summary>
        /// 获取带后座的视觉位置
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVisualPosition()
        {
            return Projectile.Center + -_Rotation.ToRotationVector2() * RecoilFactor * 36;
        }

        public void GenerateBurstPRT(float offsetX = -6, float offsetY = 0, bool small = true, bool middle = true, bool big = true)
        {
            HashSet<int> list = new HashSet<int>();
            if (small) 
                list.Add(0);
            if (middle) 
                list.Add(1);
            if (big) 
                list.Add(2);

            var prt = PRTLoader.NewParticle<CrystalBurstParticle>(GetGunpoint(offsetX, offsetY), Vector2.Zero);
            prt.OffsetX = offsetX; 
            prt.OffsetY = offsetY;
            prt.Rotation = _Rotation;
            prt.FollowProjIndex = Projectile.whoAmI;
            var frame = Main.rand.NextFromList([.. list]);
            prt.SetFrameX(frame);
        }

        public static void GenerateFlashPRT(Vector2 pos, float range, float dir, float minSpeed, float maxSpeed)
        {
            Vector2 position = pos + Main.rand.NextVector2Unit() * Main.rand.NextFloat(range);
            Vector2 vel = dir.ToRotationVector2() * Main.rand.NextFloat(minSpeed, maxSpeed);
            var prt = PRTLoader.NewParticle<CrystalFlashParticle>(position, vel);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            frameX = State switch
            {
                0 or 1 => 0,
                2 => 1,
                3 => 2,
                4 => 3,
                5 => 4,
                _ => 0,
            };

            Texture2D mainTex = Projectile.GetTextureValue();

            var frameBox = mainTex.Frame(5, 7, frameX, Projectile.frame);

            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 drawPos = GetVisualPosition() + _Rotation.ToRotationVector2() * 56 - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, drawPos, frameBox, lightColor, _Rotation, frameBox.Size() / 2, Projectile.scale, effects, 0f);
            Main.spriteBatch.Draw(GlowTex.Value, drawPos, frameBox, Color.White, _Rotation, frameBox.Size() / 2, Projectile.scale, effects, 0f);
            return false;
        }
    }

    /// <summary>
    /// ai0控制弹幕主人
    /// </summary>
    public class MagikeLaserCannonLaser : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Lasers + "CrystalCoreA";

        /// <summary>
        /// 弹幕主人
        /// </summary>
        public ref float Owner => ref Projectile.ai[0];
        /// <summary>
        /// 激光角度
        /// </summary>
        public ref float LaserRotation => ref Projectile.ai[1];
        /// <summary>
        /// 激光宽度
        /// </summary>
        public ref float LaserWidth => ref Projectile.ai[2];
        /// <summary>
        /// 激光长度
        /// </summary>
        public ref float LaserLength => ref Projectile.localAI[0];
        /// <summary>
        /// 最大穿透敌人数
        /// </summary>
        public ref float MaxPenetrate => ref Projectile.localAI[1];
        public Vector2 endPos;
        private SlotId laserSoundSlot;
        private SlotId raySoundSlot;
        private SlotId gemSoundSlot;

        private readonly HashSet<int> hitNPCIndexes = new HashSet<int>();

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundStyle style = CoraliteSoundID.PhantasmalDeathray_Zombie104;
            style.IsLooped = true;
            style.Volume = 0.2f;
            style.Pitch = 1.6f;
            raySoundSlot = SoundEngine.PlaySound(style);
            
            //用helper.playpitched上islooped没用，不知道为什么
            style = new SoundStyle($"{nameof(Coralite)}/Sounds/Crystal/CrystalLaser")
            {
                Volume = 0.4f,
                Pitch = 0f,
                MaxInstances = 0,
                IsLooped = true
            };
            laserSoundSlot = SoundEngine.PlaySound(style);
            style = new SoundStyle($"{nameof(Coralite)}/Sounds/Crystal/Gem_shatter_wind")
            {
                Volume = 0.2f,
                Pitch = 0.2f,
                MaxInstances = 0,
                IsLooped = true
            };
            gemSoundSlot = SoundEngine.PlaySound(style);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPos, LaserWidth * 0.4f, ref point);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float damageMult = 1f;
            if(Owner.GetProjectileOwner<MagikeLaserCannonHeldProj>(out Projectile owner))
            {
                damageMult = owner.ai[0] switch
                {
                    0 or 1 => 0.6f,
                    2 or 3 => 0.8f,
                    4 or 5 => 1f,
                    _ => 1f
                };
            }
            modifiers.FinalDamage *= damageMult;
        }

        public override void AI()
        {
            if (!Owner.GetProjectileOwner<MagikeLaserCannonHeldProj>(out Projectile owner, Projectile.Kill))
                return;
            var player = Main.player[owner.owner];
            if (owner.ai[0] < 1 || !Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                //如果不在攻击状态或激光与玩家有物块遮挡
                Projectile.Kill();
                return;
            }
            if (owner.ai[0] >= 1)//发射后死亡射线音效逐渐降低
            {
                if (SoundEngine.TryGetActiveSound(raySoundSlot, out var sound))
                {
                    sound.Volume -= 0.035f;
                }
            }
            
            for (int i = 0; i < 120; i++)
            {
                Vector2 posCheck = Projectile.Center + (Vector2.UnitX.RotatedBy(owner.rotation) * i * 8);

                if(Helper.PointInTile(posCheck) || i == 119)
                {
                    endPos = posCheck;
                    break;
                }
            }

            MaxPenetrate = owner.ai[0] switch //根据不同的阶段调整最大穿透数
            {
                //一阶段 0穿
                0 or 1 => 1,
                //二阶段 1穿
                2 or 3 => 2,
                //三阶段 2穿
                4 or 5 => 3,
                _ => 1
            };
            CheckNPCCollision();

            float targetScale = owner.ai[0] switch //激光炮的state
            {
                0 or 1 => 0.8f,
                2 => 1.3f,
                3 => 1.9f,
                4 => 2.2f,
                5 => 0f,
                _ => 1f
            };
            float lerpRate = owner.ai[0] switch
            {
                5 => 0.2f,
                _ => 0.08f
            };
            Projectile.scale = MathHelper.Lerp(Projectile.scale, targetScale, lerpRate);
            if (owner.ai[0] == 5)//淡出
                Projectile.Opacity = Utils.Remap(Projectile.scale, 0f, 2f, 0f, 1f);

            LaserWidth = 60f * Projectile.scale;
            Projectile.timeLeft = 2;
            MaxPenetrate = 2;

            float dist = Vector2.Distance(Projectile.Center, endPos);
            LaserLength = dist;
            LaserRotation = (endPos - Projectile.Center).ToRotation();
            if (owner.ModProjectile is MagikeLaserCannonHeldProj heldProj)
            {
                Projectile.Center = heldProj.GetGunpoint(6);
            }

            //在激光末端生成粒子
            if (Main.rand.NextBool())
                for (int i = 0; i < MaxPenetrate; i++)
                {
                    if (Main.rand.NextBool(2,3))
                    {
                        Vector2 v = LaserRotation.ToRotationVector2() * Main.rand.NextFloat(1f, 3f);
                        float dot = Vector2.Dot(v, LaserRotation.ToRotationVector2());
                        Vector2 vel = v.RotateRandom(MathHelper.PiOver4) * (-dot * 0.75f);
                        var prt = PRTLoader.NewParticle<CrystalFlashParticle>(endPos, vel * Main.rand.NextFloat(0.5f,1.5f));
                        prt.Lifetime = (int)MathHelper.Clamp(prt.Lifetime - 10, 0, 100);
                    }

                    //for (int k = 0; k < 2; k++)
                    {
                        Vector2 v = LaserRotation.ToRotationVector2() * Main.rand.NextFloat(1f, 3f);
                        float dot = Vector2.Dot(v, LaserRotation.ToRotationVector2());
                        Vector2 vel = v.RotateRandom(MathHelper.PiOver4) * (-dot * 0.75f);
                        PRTLoader.NewParticle(endPos, vel * 1.25f, CoraliteContent.ParticleType<SpeedLine>(),
                            Coralite.MagicCrystalPink, Main.rand.NextFloat(0.1f, 0.4f) * 0.75f);
                    }
                }
        }

        public void CheckNPCCollision()
        {
            Vector2 originalEndPos = endPos; // 保存原始激光终点
            Vector2 currentEndPos = originalEndPos;
            List<NPC> hitNPCs = GetHitNPCsAlongLaser();

            if (hitNPCs.Count <= 0)//没碰撞直接跳过
                return;

            //按距离排序（由近到远）
            hitNPCs.Sort((a, b) =>
                Vector2.Distance(Projectile.Center, a.Center)
                .CompareTo(Vector2.Distance(Projectile.Center, b.Center)));

            //穿透数达到上限时处理末端位置
            if (hitNPCs.Count >= MaxPenetrate)
            {
                NPC targetNPC = hitNPCs[(int)MaxPenetrate - 1];
                Rectangle npcHitbox = targetNPC.getRect();

                float collisionPoint = 0f;
                //检测激光与NPC碰撞箱的交点
                bool hasCollision = Collision.CheckAABBvLineCollision(
                    npcHitbox.TopLeft(),
                    npcHitbox.Size(),
                    Projectile.Center,
                    originalEndPos,
                    LaserWidth * 0.4f,
                    ref collisionPoint
                );

                if (hasCollision)
                {
                    //用上面的结果计算激光末端位置
                    Vector2 laserDir = (originalEndPos - Projectile.Center).SafeNormalize(Vector2.UnitX);
                    currentEndPos = Projectile.Center + laserDir * collisionPoint;
                    currentEndPos += laserDir * 1.5f;
                }

                endPos = currentEndPos;
            }
        }

        public List<NPC> GetHitNPCsAlongLaser()
        {
            List<NPC> hitNPCs = [];
            hitNPCIndexes.Clear();

            foreach(NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || !npc.CanBeChasedBy())
                    continue;

                if (Convert.ToBoolean(Colliding(Projectile.getRect(), npc.getRect())))
                {
                    if(!hitNPCIndexes.Contains(npc.whoAmI))
                    {
                        hitNPCIndexes.Add(npc.whoAmI);
                        hitNPCs.Add(npc);
                    }
                }
            }
            return hitNPCs;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(laserSoundSlot, out var laser))
            {
                laser.Stop();
            }
            if (SoundEngine.TryGetActiveSound(gemSoundSlot, out var gem))
            {
                gem.Stop();
            }
            if(SoundEngine.TryGetActiveSound(raySoundSlot, out var ray))
            {
                ray.Stop();
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D laserTex = CoraliteAssets.Laser.CrystalCoreA.Value;
            Texture2D flowTex = CoraliteAssets.Laser.CrystalFlowA.Value;
            Color color = new Color(162, 42, 131) * Projectile.Opacity;

            Effect effect = ShaderLoader.GetShader("GlowingDust");
            effect.Parameters["uColor"].SetValue(color.ToVector3());
            effect.Parameters["uOpacity"].SetValue(1);

            float width = LaserWidth * laserTex.Height / 256f;

            Vector2 startPos = Projectile.Center - Main.screenPosition;
            Vector2 endPoint = endPos - Main.screenPosition;

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, (int)LaserLength, (int)(width * 1.2f));
            var flowTarget = new Rectangle((int)endPoint.X, (int)endPoint.Y, (int)LaserLength, (int)(width * 0.8f));

            var laserSource = new Rectangle((int)(-Main.timeForVisualEffects / 30f * laserTex.Width), 0, (int)(LaserLength * 0.5f), laserTex.Height);
            var flowSource = new Rectangle((int)(Main.timeForVisualEffects / 45f * flowTex.Width), 0, (int)(LaserLength * 0.75f), flowTex.Height);

            var origin = new Vector2(0, laserTex.Height / 2);
            var origin2 = new Vector2(0, flowTex.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            //绘制流动效果
            spriteBatch.Draw(laserTex, laserTarget, laserSource, color, LaserRotation, origin, 0, 0);
            spriteBatch.Draw(flowTex, flowTarget, flowSource, color * 0.5f, LaserRotation + MathHelper.Pi, origin2, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            //绘制主体光束
            Texture2D bodyTex = CoraliteAssets.Laser.Body.Value;

            color = new Color(211, 103, 156);

            spriteBatch.Draw(bodyTex, laserTarget, laserSource, color * 0.9f, LaserRotation, new Vector2(0, bodyTex.Height / 2), 0, 0);

            //绘制用于遮盖首尾的亮光
            Texture2D glowTex = CrystalLaser.GlowTex.Value;
            Texture2D starTex = CrystalLaser.StarTex.Value;

            float minorScale = (Projectile.scale - 1f) * 0.15f + 1f;

            spriteBatch.Draw(glowTex, endPoint, null, color * (width * 0.012f), 0, glowTex.Size() / 2, 1.4f * minorScale, 0, 0);
            spriteBatch.Draw(starTex, endPoint, null, color * (width * 0.03f), Main.GlobalTimeWrappedHourly, starTex.Size() / 2, 0.2f * minorScale, 0, 0);

            spriteBatch.Draw(glowTex, startPos, null, color * (width * 0.02f), 0, glowTex.Size() / 2, 0.8f, 0, 0);
            spriteBatch.Draw(starTex, startPos, null, color * (width * 0.05f), Main.GlobalTimeWrappedHourly * -2, starTex.Size() / 2, 0.14f, 0, 0);
        }
    }

    public class MagicCrystalMissile : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        private const int HitboxLength = 26;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 30);
        }

        public override void SetDefaults()
        {
            Projectile.width = HitboxLength;
            Projectile.height = HitboxLength;
            Projectile.friendly = true;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate >= 0)
                BreakToShard();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            BreakToShard(true);
            return base.OnTileCollide(oldVelocity);
        }

        public void BreakToShard(bool reversedDir = false)
        {
            if (VisualEffectSystem.HitEffect_ScreenShaking)
            {
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Main.LocalPlayer.Center, dir, 12, 7, 12, 800));
            }
            for (int i = 0; i < 5; i++)
            {
                SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27 with 
                { 
                    Volume = 1.2f,
                    PitchVariance = 0.4f
                }, Projectile.Center);
            }
            int shardCount = Main.rand.Next(5, 7);
            int damage = (int)(Projectile.damage * 0.5f);
            float extraRot = reversedDir ? MathHelper.Pi : 0;
            for (int i = 0; i < shardCount; i++)
            {
                Vector2 vel = Projectile.velocity / Main.rand.NextFloat(2.7f, 3.3f) * 2f;
                vel = vel.RotatedBy(-MathHelper.PiOver2 + MathHelper.Pi / shardCount * i + extraRot).RotateRandom(0.7f);
                int frame = Main.rand.Next(0, 3);
                Projectile.NewProjectileFromThis<MagicCrystalShard>(Projectile.Center, vel, damage, Projectile.knockBack, frame);

                var prt = PRTLoader.NewParticle<CrystalBurstParticle>(Projectile.Center, vel * Main.rand.NextFloat(0.1f,0.35f) * 0.25f);
                prt.SetFrameX(2);
            }

            for(int i = 0; i < 32; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(22);
                Vector2 vel = Projectile.rotation.ToRotationVector2().RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(1, 7);
                _ = PRTLoader.NewParticle<CrystalFlashParticle>(position, vel);
            }
        }

        public override void AI()
        {
            Projectile.velocity += Projectile.velocity / Projectile.velocity.Length() * 0.0175f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.UpdateFrameNormally(6, 8);
            
            float scale = MathHelper.Lerp(Projectile.scale, 2f, 0.02f);
            SetScale(scale);
            Helper.AutomaticTracking(Projectile, 0.2f, Projectile.velocity.Length(), 100);

            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(5, 5);
                Vector2 vel = Vector2.Lerp(Main.rand.NextVector2Unit() * Main.rand.NextFloat(2), Projectile.velocity, 0.25f) * 0.25f;
                PRTLoader.NewParticle<CrystalFlashParticle>(pos, vel);
            }
        }

        public void SetScale(float scale)
        {
            Projectile.position = Projectile.Center;
            Projectile.scale = scale;
            Projectile.width = (int)(HitboxLength * scale);
            Projectile.height = (int)(HitboxLength * scale);
            Projectile.Center = Projectile.position;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.damage = Projectile.damage * 2;
            int size = 280;
            Projectile.penetrate = -1;
            Projectile.SetHitbox(size);
            Projectile.Damage();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var frameBox = mainTex.Frame(1, 9, 0, Projectile.frame);

            var star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            float alpha = Projectile.Opacity;
            int length = Projectile.oldPos.Length;
            Color trailBaseColor = Coralite.MagicCrystalPink with { A = 0 } * alpha * 0.7f;
            Vector2 origin = star.Size() / 2;
            Vector2 scale = new Vector2(0.7f, 1f) * Projectile.scale;
            for (int i = 0; i < length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || i % 2 == 0) continue;
                float factor = 1f - (float)i / Projectile.oldPos.Length;
                float rot = i == 0 ? Projectile.rotation : Projectile.oldRot[i];
                Vector2 drawpos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Color trailColor = trailBaseColor * factor * 0.8f;
                Main.spriteBatch.Draw(star, drawpos, null, trailColor, rot + MathHelper.PiOver2, origin, scale, 0, 0);

                if (i % 3 == 0)
                    Main.spriteBatch.Draw(mainTex, drawpos, frameBox, Color.White * factor * 0.2f, rot, frameBox.Size() / 2, Projectile.scale, 0, 0);
            }


            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 1f)
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0f);
            return false;
        }
    }

    public class MagicCrystalShard : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;
        public ref float Frame => ref Projectile.ai[0];
        /// <summary>
        /// 分裂时机的随机偏移
        /// </summary>
        public ref float Offset => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.timeLeft = 30;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = (int)Frame;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Offset = Main.rand.Next(-5, 5);
        }

        public void SetFrame(int frame)
        {
            Projectile.frame = frame;
        }

        /// <summary>
        /// 随机设置粒子帧，帧越小粒子越大
        /// </summary>
        /// <param name="dust"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetFrameRand(int min = 0, int max = 10)
        {
            Projectile.frame = Main.rand.Next(min, max);
        }

        public override void AI()
        {
            float velMult = 0.97f;
            if (Frame > 2)
                velMult -= 0.01f;
            Projectile.velocity *= velMult;

            Projectile.velocity.Y += 0.01f;

            if(Projectile.timeLeft == 20 + Offset && Frame < 6)
            {
                //影响剩余分裂次数
                int frameIncrease = Main.rand.Next(2, 4);
                if (Frame > 3)
                {
                    //frameIncrease++;
                    frameIncrease += Main.rand.Next(-1, 2);
                }

                int splitCount = 2;
                if (Frame > 4) 
                    splitCount--;
                if (Frame > 7) 
                    splitCount--;

                int frame = (int)MathHelper.Clamp(Frame + frameIncrease, 0, 9);
                for (int i = 0; i < splitCount; i++)
                {
                    //if (Frame > 3 && Main.rand.NextBool())
                    //    continue;
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10);
                    Vector2 vel = Projectile.velocity.RotateRandom(MathHelper.PiOver4) * 1.3f * Main.rand.NextFloat(0.66f, 1.5f);
                    int damage = (int)(Projectile.damage * 0.75f);
                    var p = Projectile.NewProjectileFromThis<MagicCrystalShard>(pos, vel, damage, Projectile.knockBack, frame);
                    Main.projectile[p].rotation += Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

                    int prtFrame = frame switch
                    {
                        3 or 4 or 5 => 1,
                        6 or 7 or 8 => 0,
                        _ => 0
                    };
                    var prt = PRTLoader.NewParticle<CrystalBurstParticle>(pos, vel * 0.25f);
                    prt.SetFrameX(prtFrame);
                }

                for (int i = 0; i < splitCount * 4; i++)
                {
                    Vector2 position = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(22);
                    Vector2 vel = Projectile.rotation.ToRotationVector2().RotateRandom(MathHelper.Pi) * Main.rand.NextFloat(2, 8);
                    PRTLoader.NewParticle<CrystalFlashParticle>(position, vel * 0.5f + Projectile.velocity * 0.5f);
                }

                SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
            }

            float timeFactor = Projectile.timeLeft / 30f;
            //淡出
            Projectile.Opacity = Utils.Remap(timeFactor, 0, 0.4f, 0f, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            var frameBox = mainTex.Frame(1, 10, 0, Projectile.frame);

            float alpha = Projectile.Opacity;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White * alpha
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0f);
            return false;
        }
    }

    public class CrystalFlashParticle() : BaseFrameParticle(2, 4, 4)
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;
        public ref float Alpha => ref ai[1];
        /// <summary>
        /// 偏移，用于随机闪烁
        /// </summary>
        public ref float Offset => ref ai[2];

        public override void SetProperty()
        {
            base.SetProperty();
            Lifetime = 32 + Main.rand.Next(-18, 12);
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Color = Color.White;
            Rotation = Velocity.ToRotation();
            Scale = Main.rand.NextFloat(0.5f, 1.5f);
            Offset = Main.rand.Next(100);
        }

        public override void AI()
        {
            base.AI();
            Alpha = Utils.Remap(LifetimeCompletion, 0f, 1f, 1f, 0f);

            if(!active && LifetimeCompletion < 0.7f)
            {
                active = true;
                Frame.Y = 0;
                Opacity = 0;
            }
        }

        public override Color GetColor()
        {
            return Color * Alpha;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            var frameBox = tex.Frame(2, 4, Frame.X, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , GetColor(), Rotation, new Vector2(0, 25), Scale, Effects, 0);
            if((Main.GameUpdateCount + Offset) % 10 == 0)
            {
                for (int i = 0; i < 10; i++)
                    spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                        , GetColor() with { A = 0 }, Rotation, new Vector2(0, 25), Scale, Effects, 0);
            }
            return false;
        }
    }

    public class CrystalBurstParticle() : BaseFrameParticle(3, 7, 4)
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;
        public ref float OffsetX => ref ai[0];
        public ref float OffsetY => ref ai[1];

        public override void SetProperty()
        {
            ShouldKillWhenOffScreen = false;
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
            Color = Color.White * Main.rand.NextFloat(0.7f, 1f);
            Scale = Main.rand.NextFloat(0.7f, 1.3f);
        }

        public void SetFrameX(int x)
        {
            Frame.X = x;
        }

        public override void AI()
        {
            base.AI();
            if (FollowProjIndex < 0)
            {
                Velocity *= 0.96f;
                Opacity++;
                Rotation = Velocity.ToRotation();
            }
        }

        public override void Follow(Projectile proj)
        {
            if (proj.type != ModContent.ProjectileType<MagikeLaserCannonHeldProj>())
                return;

            var laserCannon = proj.ModProjectile as MagikeLaserCannonHeldProj;
            Vector2 gunpoint = laserCannon.GetGunpoint(OffsetX, OffsetY);
            Position = gunpoint;
            Rotation = laserCannon._Rotation;
        }

        public override Color GetColor()
        {
            return Color.White;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            var frameBox = tex.Frame(3, 7, Frame.X, Frame.Y);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , GetColor(), Rotation, new Vector2(0, 25), Scale, Effects, 0);

            return false;
        }
    }
}
