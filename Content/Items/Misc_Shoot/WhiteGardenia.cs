using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class WhiteGardenia : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public static Color LightGreen = new Color(57, 255, 133);

        public int shootCount;
        public static short GlowMaskID;

        public override void SetStaticDefaults()
        {
            Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + 1);
            TextureAssets.GlowMask[^1] = ModContent.Request<Texture2D>(Texture + "_Glow");
            GlowMaskID = (short)(TextureAssets.GlowMask.Length - 1);
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(180, 7f);
            Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Bullet, 25, 15f);
            Item.reuseDelay = 5;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.glowMask = GlowMaskID;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2 && shootCount < 10)
                return false;

            return base.CanUseItem(player);
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WhiteGardeniaHeldProj>()] < 1)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                        , player.Center, Vector2.Zero, ModContent.ProjectileType<WhiteGardeniaHeldProj>(), 0, 0, player.whoAmI);
                }

                if (player.ownedProjectileCounts[ModContent.ProjectileType<WhiteGardeniaFloat>()] < 2)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                        , player.Center, Vector2.Zero, ModContent.ProjectileType<WhiteGardeniaFloat>(), 0, 0, player.whoAmI);
                }
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Projectile heldProj = Main.projectile.FirstOrDefault(p => p.active && p.friendly && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<WhiteGardeniaHeldProj>());
            if (heldProj != null && heldProj.localAI[1] > 0)
            {
                if (player.altFunctionUse == 2 && shootCount == 10)
                    (heldProj.ModProjectile as WhiteGardeniaHeldProj).SpecialAttack();
                else
                    (heldProj.ModProjectile as WhiteGardeniaHeldProj).Attack();
            }

            if (heldProj.ai[2].GetNPCOwner(out NPC owner))
            {
                float length = velocity.Length();
                velocity = (owner.Center - player.MountedCenter).SafeNormalize(Vector2.Zero) * length;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2 && shootCount == 10)
            {
                shootCount = 0;

                return false;
            }

            Helper.PlayPitched(CoraliteSoundID.Gun2_Item40, player.Center);
            Helper.PlayPitched(CoraliteSoundID.AetheriumBlock, player.Center);


            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.Misc_Shoot)]
    public class WhiteGardeniaHeldProj : BaseGunHeldProj
    {
        public WhiteGardeniaHeldProj() : base(0.02f, 50, -10, AssetDirectory.Misc_Shoot) { }

        [AutoLoadTexture(Name = "WhiteGardenia_Glow")]
        public static ATex Highlight { get; private set; }

        [AutoLoadTexture(Name = "WhiteGardeniaSpawn")]
        public static ATex SpawnAnmi { get; private set; }

        [AutoLoadTexture(Name = "WhiteGardeniaFade")]
        public static ATex KillAnmi { get; private set; }

        [AutoLoadTexture(Name = "WhiteGardeniaAim")]
        public static ATex AimMouse { get; private set; }

        [AutoLoadTexture(Name = "WhiteGardeniaNumber")]
        public static ATex NumberTex { get; private set; }

        [AutoLoadTexture(Name = "WhiteGardeniaSP")]
        public static ATex SpecialAttackTex { get; private set; }

        public ref float Timer => ref Projectile.localAI[0];
        public ref float State => ref Projectile.localAI[1];
        public ref float Target => ref Projectile.ai[2];

        public Vector2 AimPosition { get => Projectile.velocity; set => Projectile.velocity = value; }

        public override void AI()
        {
            Projectile.timeLeft = 2;
            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;

            switch (State)
            {
                default:
                case 0://生成动画
                    {
                        Lighting.AddLight(Projectile.Center, WhiteGardenia.LightGreen.ToVector3() * (1 - Projectile.frame / 12f));

                        SpawnDust();
                        ApplyRecoil(0);

                        if (Timer % 2 == 0)
                        {
                            Projectile.frame++;
                            if (Projectile.frame > 11)
                            {
                                State = 1;
                                Timer = 0;
                                Projectile.frame = 0;
                                break;
                            }
                        }

                        Timer++;
                    }

                    break;
                case 1://静止
                    {
                        Vector2 targetPos = InMousePos;
                        if (Target.GetNPCOwner(out NPC owner))
                            targetPos = owner.Center;

                        TargetRot = (targetPos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);

                        ApplyRecoil(0);

                        if (Item.type != ModContent.ItemType<WhiteGardenia>())
                            TurnToFade();
                    }

                    break;
                case 2://射击
                    {
                        Vector2 targetPos = InMousePos;
                        if (Target.GetNPCOwner(out NPC owner))
                            targetPos = owner.Center;

                        TargetRot = (targetPos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);
                        float factor = Ease();
                        ApplyRecoil(factor);

                        if (Timer > MaxTime)
                        {
                            Timer = 0;
                            State = 1;
                        }

                        Timer++;
                    }

                    break;
                case 3://消失动画
                    {
                        Lighting.AddLight(Projectile.Center, WhiteGardenia.LightGreen.ToVector3());

                        SpawnDust();
                        ApplyRecoil(0);

                        if (Timer % 2 == 0)
                        {
                            Projectile.frame++;
                            if (Projectile.frame > 7)
                            {
                                Projectile.Kill();
                                break;
                            }
                        }

                        Timer++;
                    }
                    break;
                case 4:
                    {
                        Owner.itemTime = Owner.itemAnimation = 2;
                        Vector2 targetPos = InMousePos;
                        if (Target.GetNPCOwner(out NPC owner))
                            targetPos = owner.Center;

                        if (Projectile.frame < 19)
                            TargetRot = (targetPos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);

                        ApplyRecoil(0);

                        if (Projectile.frame < 19)
                        {
                            Projectile.frame++;
                            if (Projectile.frame == 19)
                            {
                                Helper.PlayPitched(CoraliteSoundID.Gun_Item11, Owner.Center);
                                Helper.PlayPitched(CoraliteSoundID.AetheriumBlock, Owner.Center);
                                Helper.PlayPitched(CoraliteSoundID.TerraBlade_Item60, Owner.Center, pitch: 0.8f);

                                Projectile.NewProjectileFromThis<WhiteGardeniaPowerfulShoot>(Projectile.Center, (targetPos - Owner.MountedCenter).SafeNormalize(Vector2.Zero) * 16
                                    , Owner.GetWeaponDamage(Item) * 5, Item.knockBack);
                            }
                        }

                        if (Timer > MaxTime)
                        {
                            Timer = 0;
                            State = 1;
                        }

                        Timer++;
                    }
                    break;
            }

            AimLineAI();

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (Owner.gravDir > 0 ? 0f : MathHelper.Pi) + (DirSign * 0.3f);
        }

        public void AimLineAI()
        {
            int? target = FindTarget();
            if (target.HasValue)
                Target = target.Value;
            else
                Target = -1;

            if (Projectile.IsOwnedByLocalPlayer())//找目标
            {
                float length = ToMouse.Length();
                Vector2 dir = UnitToMouseV;
                Vector2 basePos = Projectile.Center - (Projectile.rotation + 1.57f).ToRotationVector2() * 14;
                bool collide = false;

                for (int i = 0; i < length; i += 8)
                {
                    Vector2 pos = basePos + dir * i;
                    Tile t = Framing.GetTileSafely(pos);

                    if (t.HasSolidTile())
                    {
                        AimPosition = pos;
                        collide = true;
                        break;
                    }
                }

                if (!collide)
                    AimPosition = InMousePos;
            }
        }

        public int? FindTarget()
        {
            Vector2 mouseWorld = InMousePos;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.CanBeChasedBy())
                    continue;

                if (Collision.CanHitLine(Projectile.Center, 1, 1, npc.Center, 1, 1) &&
                    (npc.Hitbox.Contains((int)mouseWorld.X, (int)mouseWorld.Y)
                    || Collision.CheckAABBvLineCollision(npc.TopLeft - npc.Size / 2, npc.Size * 2, Projectile.Center, InMousePos)))
                    return i;
            }

            return null;
        }

        public void SpawnDust()
        {
            //if (Main.rand.NextBool())
            //    return;

            Vector2 dir = TargetRot.ToRotationVector2();

            Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.NextFloat(-40, 40) + Main.rand.NextVector2Circular(32, 32), ModContent.DustType<PixelPoint>()
                , dir * (Main.rand.NextFromList(-3, 3) + Main.rand.NextFloat(-1, 1)), newColor: Main.rand.NextFromList(WhiteGardenia.LightGreen, Color.Silver)
                , Scale: Main.rand.NextFloat(1, 2));
        }

        public void Attack()
        {
            MaxTime = Owner.itemTimeMax;
            Timer = 0;
            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;
            TargetRot = (InMousePos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);

            Projectile.netUpdate = true;
            State = 2;
        }

        public void SpecialAttack()
        {
            MaxTime = 30;

            Timer = 0;
            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;
            TargetRot = (InMousePos - Owner.Center).ToRotation() + (DirSign > 0 ? 0f : MathHelper.Pi);
            Projectile.frame = 0;

            Projectile.netUpdate = true;
            State = 4;
        }

        public void TurnToFade()
        {
            State = 3;
            Projectile.frame = 0;
            Timer = 0;
        }

        public override float Ease()
        {
            float x = 1.772f * (1 - Timer / MaxTime);
            return x * MathF.Sin(x * x) / 1.3076f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State != 0 && State != 3)
            {
                #region 绘制光标
                Texture2D aimTex = AimMouse.Value;

                Vector2 aimPos = AimPosition;
                int frame = 0;

                if (Main.npc.IndexInRange((int)Target))
                {
                    aimPos = Main.npc[(int)Target].Center;
                    frame = 1;
                }

                Rectangle frameBox = aimTex.Frame(2, 1, frame);

                Main.spriteBatch.Draw(aimTex, aimPos - Main.screenPosition, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);

                #endregion

                #region 绘制线
                Texture2D lineTex = TextureAssets.FishingLine.Value;
                Vector2 basePos = Projectile.Center - (Projectile.rotation + 1.57f).ToRotationVector2() * 14;

                float length = (aimPos - basePos).Length();
                if (length < 20)
                    length = 0;
                else
                    length -= 20;

                Vector2 linePos = basePos - Main.screenPosition;
                Rectangle targetRect = new Rectangle((int)linePos.X, (int)linePos.Y, lineTex.Width, (int)length);

                float a = 0.4f;
                if (Target != -1)
                    a = 0.15f;

                Main.spriteBatch.Draw(lineTex, targetRect, null, WhiteGardenia.LightGreen * a, (aimPos - basePos).ToRotation() - MathHelper.PiOver2
                    , new Vector2(lineTex.Width / 2, 0), 0, 0);
                #endregion
            }

            #region 绘制玩家数字条

            Texture2D numberTex = NumberTex.Value;

            if (Item.ModItem is WhiteGardenia gardenia)
            {
                int count = gardenia.shootCount;

                if (count > 0 && count < 11)
                {
                    float scale = 1f + 0.5f * count / 10f;

                    Rectangle number = numberTex.Frame(10, 1, count - 1);

                    Main.spriteBatch.Draw(numberTex, Owner.Center - Main.screenPosition + new Vector2(0, -42)
                        , number, Color.White, 0, new Vector2(number.Width / 2, number.Height), scale, 0, 0);
                }
            }

            #endregion

            switch (State)
            {
                default:
                case 0:
                    DrawSpawn(lightColor);
                    break;
                case 1:
                case 2:
                    {
                        base.PreDraw(ref lightColor);

                        Texture2D mainTex = Highlight.Value;

                        GetFrame(mainTex, out Rectangle? frame2, out Vector2 origin);

                        SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                        Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frame2, Color.White
                            , Projectile.rotation, origin, Projectile.scale, effects, 0f);
                    }
                    break;
                case 3:
                    DrawKill(lightColor);
                    break;
                case 4:
                    {
                        base.PreDraw(ref lightColor);

                        Texture2D mainTex = Highlight.Value;

                        GetFrame(mainTex, out Rectangle? frame2, out Vector2 origin);

                        SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                        Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frame2, Color.White
                            , Projectile.rotation, origin, Projectile.scale, effects, 0f);

                        float factor = Timer / MaxTime;

                        float scale = 1;
                        if (factor < 0.1f)
                            scale = Helper.Lerp(0, 1, factor / 0.1f);

                        float alpha = 1;
                        if (factor > 0.8f)
                            alpha = Coralite.Instance.X3Smoother.Smoother(1 - (factor - 0.8f) / 0.2f);

                        mainTex = SpecialAttackTex.Value;
                        var SPframeBox = mainTex.Frame(1, 20, 0, Projectile.frame);

                        Main.spriteBatch.Draw(mainTex, Projectile.Center + UnitToMouseV * 80 - Main.screenPosition, SPframeBox, Color.White * alpha
                            , Projectile.rotation, SPframeBox.Size() / 2, new Vector2(1, scale), effects, 0f);
                    }

                    break;
            }

            return false;
        }

        public void DrawSpawn(Color lightColor)
        {
            Texture2D mainTex = SpawnAnmi.Value;

            var frameBox = mainTex.Frame(1, 13, 0, Projectile.frame);

            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, effects, 0f);
            lightColor.A = 200;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor * 0.5f * (1 - Projectile.frame / 12f)
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, effects, 0f);
        }

        public void DrawKill(Color lightColor)
        {
            Texture2D mainTex = KillAnmi.Value;

            var frameBox = mainTex.Frame(1, 9, 0, Projectile.frame);

            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, effects, 0f);
            lightColor.A = 200;
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor * 0.5f
                , Projectile.rotation, frameBox.Size() / 2, Projectile.scale, effects, 0f);
        }
    }

    public class WhiteGardeniaFloat : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public ref float State => ref Projectile.ai[0];

        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float RandomAngle => ref Projectile.localAI[0];
        public ref float Length => ref Projectile.localAI[1];
        public ref float AttackCount => ref Projectile.localAI[2];

        private bool init = true;

        public Vector2 offset;
        public SecondOrderDynamics_Vec2 IdleMove;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 10);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage()
            => false;

        public override void AI()
        {
            const int AttackCountMax = 8;

            if (init)
            {
                init = false;
                Helper.GetMyGroupIndexAndFillBlackList(Projectile, out var i, out _);
                AttackCount = -i * AttackCountMax;
            }

            if (Item.type != ModContent.ItemType<WhiteGardenia>())
            {
                Projectile.Kill();
                return;
            }

            Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case 0://返回玩家身边
                    {
                        Timer = 0f;
                        State = 1;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 1://在玩家身边悬浮，查找敌人
                    {
                        IdleMove ??= new SecondOrderDynamics_Vec2(2.5f, 0.9f, 0.5f, Owner.MountedCenter);

                        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out var index2, out _);

                        Vector2 position = Owner.Center + new Vector2((index2 == 0 ? -1 : 1) * 40, -60);
                        Projectile.Center = IdleMove.Update(1 / 60f, position);

                        Projectile.spriteDirection = Owner.direction;

                        Projectile.rotation = (Owner.Center - Projectile.Center).ToRotation();

                        if (Timer > 15)
                        {
                            Timer = 0;
                            int index = FindEnemy();
                            if (index != -1)
                            {
                                Target = index;
                                Projectile.netUpdate = true;
                                if (!VaultUtils.isClient)
                                {
                                    State = index2 + 2;
                                    Timer = -index2 * 8;

                                    RandomAngle = Main.rand.NextFloat(-0.9f, 0.9f);
                                    Length = Main.rand.Next(45, 80);
                                    Projectile.netUpdate = true;
                                }
                                return;
                            }
                        }

                        Timer++;
                    }
                    break;
                case 2://飞到敌人头上
                case 3:
                case 4:
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Timer = 0;
                            State = 0;
                            break;
                        }

                        Projectile.Center = IdleMove.Update(1 / 60f, target.Center + new Vector2(0, -target.height / 2 - Length)
                            .RotatedBy(RandomAngle + (State - 3) * 0.8f));
                        Projectile.spriteDirection = Projectile.Center.X > target.Center.X ? -1 : 1;

                        if (Timer == 15)
                        {
                            if (State != 3)
                                Helper.PlayPitched(CoraliteSoundID.LaserGun_Item158, Projectile.Center);

                            Projectile.NewProjectileFromThis<WhiteGardeniaFloatLaser>(Projectile.Center, Vector2.Zero
                                , (int)(Owner.GetWeaponDamage(Item) * 0.7f), Projectile.knockBack, Projectile.whoAmI, Target);

                            AttackCount++;

                            if (AttackCount > AttackCountMax - 1
                                && Item.ModItem is WhiteGardenia gardenia && gardenia.shootCount < 10)
                            {
                                AttackCount = -AttackCountMax;
                                gardenia.shootCount++;
                                //gardenia.shootCount = 10;
                            }
                        }

                        if (Timer > 28)
                        {
                            int index = FindEnemy();
                            if (index != -1)
                            {
                                Timer = 0;
                                Target = index;
                                Projectile.netUpdate = true;
                                if (!VaultUtils.isClient)
                                {
                                    State++;
                                    if (State > 4)
                                        State = 2;

                                    RandomAngle = Main.rand.NextFloat(-0.4f, 0.4f);
                                    Length = Main.rand.Next(45, 80);
                                    Projectile.netUpdate = true;
                                }
                                return;
                            }
                            else
                                State = 0;
                        }

                        Timer++;
                    }
                    break;
            }

            if (Timer % 2 == 0 && Main.rand.NextBool(3, 4))
                Dust.NewDustPerfect(Projectile.Center + new Vector2(-Projectile.spriteDirection * 8, 0) + Main.rand.NextVector2Circular(4, 4), ModContent.DustType<PixelPoint>()
                    , new Vector2(-Projectile.spriteDirection, -0.5f).RotateByRandom(-0.2f, 0.2f), newColor: WhiteGardenia.LightGreen
                    , Scale: 1f);
        }

        public int FindEnemy()
        {
            Projectile aimProj = Main.projectile.FirstOrDefault(
                p => p.active && p.friendly && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<WhiteGardeniaHeldProj>());

            if (aimProj != null && Main.npc.IndexInRange((int)aimProj.ai[2]))
                return (int)aimProj.ai[2];

            return Projectile.MinionFindTarget();
        }

        public override void NetHeldSend(BinaryWriter writer)
        {
            writer.Write(RandomAngle);
            writer.Write(Length);
        }

        public override void NetHeldReceive(BinaryReader reader)
        {
            RandomAngle = reader.ReadSingle();
            Length = reader.ReadSingle();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Projectile.DrawShadowTrailsSacleStep(lightColor, 0.4f, 0.4f / 10, 0, 10, 1, 0.4f / 10, null, effect);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, 0, mainTex.Size() / 2, Projectile.scale, effect, 0);
            return false;
        }
    }

    /// <summary>
    /// ai0传入弹幕持有者，ai1传入目标
    /// </summary>
    public class WhiteGardeniaFloatLaser : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Owner => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.whoAmI == (int)Target;
        }

        public override void AI()
        {
            if (!Owner.GetProjectileOwner(out Projectile owner, Projectile.Kill))
                return;

            if (!Target.GetNPCOwner(out NPC target, Projectile.Kill))
                return;

            Projectile.Center = target.Center;
            Lighting.AddLight(Projectile.Center, TorchID.Green);

            if (Timer > 20)
                Projectile.Kill();

            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = Helper.NextVec2Dir(1, 2);
                Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.NextFloat(2, 10), ModContent.DustType<PixelPoint>(), dir, newColor: WhiteGardenia.LightGreen
                    , Scale: 2f);
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 dir = Helper.NextVec2Dir(2, 4);
                var p = PRTLoader.NewParticle<PixelLine>(Projectile.Center, dir, newColor: WhiteGardenia.LightGreen
                      , Scale: 1);
                p.TrailCount = 16;
                p.fadeFactor = 0.85f;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lineTex = TextureAssets.FishingLine.Value;

            if (!Owner.GetProjectileOwner(out Projectile owner, Projectile.Kill))
                return false;

            float length = (owner.Center - Projectile.Center).Length();

            Vector2 linePos = Projectile.Center - Main.screenPosition;
            Rectangle targetRect = new Rectangle((int)linePos.X, (int)linePos.Y, lineTex.Width, (int)length);

            Main.spriteBatch.Draw(lineTex, targetRect, null, WhiteGardenia.LightGreen * (1 - Timer / 20), (owner.Center - Projectile.Center).ToRotation() - MathHelper.PiOver2
                , new Vector2(lineTex.Width / 2, 0), 0, 0);

            return false;
        }
    }

    public class WhiteGardeniaPowerfulShoot : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 15);
        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 3;

            Projectile.timeLeft = Projectile.MaxUpdates * 60;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < 2; i++)
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<PixelPoint>(), Projectile.velocity * Main.rand.NextFloat(-0.4f, 0.4f), newColor: WhiteGardenia.LightGreen
                , Scale: 1f);

            var p = PRTLoader.NewParticle<PixelLine>(Projectile.Center + Main.rand.NextVector2Circular(8, 8)
                , Projectile.velocity * Main.rand.NextFloat(0.1f, 0.25f), newColor: WhiteGardenia.LightGreen
                  , Scale: 1);
            p.TrailCount = Main.rand.Next(16, 24);
            p.fadeFactor = Main.rand.NextFloat(0.8f, 0.95f);
            p.useStartLimit = false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.Next(12, 14), ModContent.DustType<PixelPoint>()
                    , dir * Main.rand.NextFloat(2.5f, 3), newColor: WhiteGardenia.LightGreen
                , Scale: Main.rand.NextFloat(1, 1.5f));
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.Next(32, 36), ModContent.DustType<PixelPoint>()
                        , dir * Main.rand.NextFloat(2.5f, 3), newColor: WhiteGardenia.LightGreen
                    , Scale: Main.rand.NextFloat(1.5f, 2f));
            }

            for (int i = 0; i < 7; i++)
            {
                Vector2 dir = Helper.NextVec2Dir(2, 4);
                Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.NextFloat(2, 10), ModContent.DustType<PixelPoint>(), dir, newColor: Color.Silver
                    , Scale: 2f);

                var p = PRTLoader.NewParticle<PixelLine>(Projectile.Center + Main.rand.NextVector2Circular(8, 8)
                    , Projectile.velocity.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(0.2f, 0.5f), newColor: WhiteGardenia.LightGreen
                      , Scale: 1);
                p.TrailCount = Main.rand.Next(12, 20);
                p.fadeFactor = Main.rand.NextFloat(0.8f, 0.9f);
                p.useStartLimit = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(Color.White, 0.5f, 0.5f / 15, 0, 15, 2);
            Projectile.QuickDraw(Color.White, 0);
            return false;
        }
    }
}