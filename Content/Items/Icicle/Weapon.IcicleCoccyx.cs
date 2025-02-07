using Coralite.Content.GlobalItems;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleCoccyx : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public int useCount;
        public bool canDash;

        public float Priority => IDashable.HeldItemDash;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 24;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.knockBack = 1f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Expert;
            Item.shoot = ProjectileType<IcicleCoccyxSpurt_N>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.expert = true;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 0.75f), knockback, player.whoAmI, player.itemTimeMax);

                if (canDash)
                {
                    Projectile.NewProjectile(source, player.Center, player.Center.DirectionTo(Main.MouseWorld) * 12f, ProjectileType<IcicleCoccyx_Ex2>(), damage * 2, knockback, player.whoAmI);
                    canDash = false;
                }

                return false;
            }

            //戳戳戳-扫-反向扫-戳-突刺
            switch (useCount)
            {
                default:
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 0.75f), knockback, player.whoAmI, player.itemTimeMax);
                    break;
                case 3:
                case 10:
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleCoccyx_Ex1>(), damage, knockback, player.whoAmI, (int)(player.itemTimeMax * 0.9f), 0.35f);
                    break;
                case 4:
                case 9:
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleCoccyx_Ex1>(), damage, knockback, player.whoAmI, (int)(player.itemTimeMax * 0.9f), -0.35f);
                    break;
                case 5:
                case 11:
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleCoccyx_Ex1>(), damage, knockback, player.whoAmI, (int)(player.itemTimeMax * 0.8f));
                    if (!canDash)
                    {
                        //生成粒子
                        SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, player.Center);
                        for (int i = 0; i < 4; i++)//生成冰晶粒子
                        {
                            Vector2 center = player.Center + ((-1.57f + (i * 1.57f)).ToRotationVector2() * 64);
                            Vector2 vel = (i * 1.57f).ToRotationVector2() * 4;
                            IceStarLight.Spawn(center, vel, 1f, () => player.Center, 16);
                        }
                        canDash = true;
                    }
                    break;
            }

            useCount++;
            if (useCount > 11)
                useCount = 0;
            return false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            if (!canDash)
                return false;

            (Player.HeldItem.ModItem as IcicleCoccyx).canDash = false;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    break;
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;

            if (Player.heldProj > 0 && Main.projectile[Player.heldProj].active && Main.projectile[Player.heldProj].owner == Player.whoAmI)
                Main.projectile[Player.heldProj].Kill();

            if (Player.whoAmI == Main.myPlayer)//生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ModContent.ProjectileType<IcicleCoccyx_Ex3>(),
                    Player.HeldItem.damage * 2, Player.HeldItem.knockBack, Player.whoAmI, DashDir);

            return true;
        }
    }

    /// <summary>
    /// 普通的戳 ai[0]输入总时间
    /// </summary>
    public class IcicleCoccyxSpurt_N : BaseHeldProj, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleCoccyx";

        public ref float TimeMax => ref Projectile.ai[0];
        public ref float Distance2Owner => ref Projectile.ai[1];

        public ref float RotOffset => ref Projectile.localAI[0];

        private bool initialize = true;

        public override void SetDefaults()
        {
            Projectile.coldDamage = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.width = 34;
            Projectile.height = 68;
            Projectile.timeLeft = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CanHitLine(Owner.Center, 1, 1, targetHitbox.Center.ToVector2(), 1, 1)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Owner.Center + (Projectile.velocity * (Distance2Owner + 36)), 30, ref a);
        }

        public override void AI()
        {
            if (initialize)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                Projectile.timeLeft = (int)TimeMax;
                Distance2Owner = 0;
                RotOffset = Main.rand.NextFloat(-0.2f, 0.2f);
                initialize = false;
            }

            if (Projectile.IsOwnedByLocalPlayer())
            {
                Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One);
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
            }

            if (Projectile.timeLeft > (int)TimeMax / 2)
                Distance2Owner += 64 / (TimeMax / 2);
            else
                Distance2Owner -= 50 / (TimeMax / 2);

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.75f, 0.75f));
            Projectile.Center = Owner.Center + ((Projectile.rotation + RotOffset).ToRotationVector2() * Distance2Owner);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = mainTex.Size() / 2f;

            //绘制本体
            float rot = Projectile.rotation + RotOffset + ((float)Math.PI / 4f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.rotation < -(float)Math.PI / 2f || Projectile.rotation > (float)Math.PI / 2f)
            {
                rot += (float)Math.PI / 2f;
                spriteEffects |= SpriteEffects.FlipHorizontally;
            }

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, rot, origin, 1f, spriteEffects, 0f);
            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D extraTex = Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleCoccyx_Extra").Value;
            Vector2 origin = extraTex.Size() / 2;
            Color color2 = Color.White;
            color2.A = (byte)(color2.A * 0.5f);

            float currentRot = Projectile.rotation + RotOffset;
            float rot2 = currentRot + (DirSign * 0.1f);
            Vector2 dir = currentRot.ToRotationVector2() * Distance2Owner;

            for (int j = -1; j < 2; j += 2)
            {
                float scale = Main.rand.NextFloat(0.5f, 0.7f);
                float randomPos = (Math.Abs(Projectile.timeLeft - (TimeMax / 2)) + 1f) * 1f;
                Vector2 position2 = Owner.Center + (currentRot.ToRotationVector2().RotatedBy(j * 1.57f) * randomPos) + Main.rand.NextVector2Circular(randomPos, randomPos) + dir - Main.screenPosition;
                Main.spriteBatch.Draw(extraTex, position2, null, color2, rot2 + 0.785f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }

    /// <summary>
    /// 将骨头分成节变成类似鞭子一样的挥动 ai[0]输入时间，ai[1]输入每帧的挥舞角度，为0就是直线突刺
    /// </summary>
    public class IcicleCoccyx_Ex1 : BaseHeldProj
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleCoccyx";

        public ref float TimeMax => ref Projectile.ai[0];
        public ref float SwingAngle => ref Projectile.ai[1];
        public ref float PerPartLength => ref Projectile.ai[2];

        public ref float FinalRotationOffset => ref Projectile.localAI[0];

        public const int CACHE_LENGTH = 16;
        private bool initialize = true;

        public static Asset<Texture2D> handleTex;
        public static Asset<Texture2D> boneTex;
        public static Asset<Texture2D> boneTipTex;

        public override void SetDefaults()
        {
            Projectile.coldDamage = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.width = 34;
            Projectile.height = 68;
            Projectile.timeLeft = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void Load()
        {
            handleTex = Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleCoccyx_Handle");
            boneTex = Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleCoccyx_Bone");
            boneTipTex = Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleCoccyx_Tip");
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CanHitLine(Owner.Center, 1, 1, targetHitbox.Center.ToVector2(), 1, 1)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Projectile.oldPos[CACHE_LENGTH - 1], 30, ref a);
        }

        public override void AI()
        {
            if (initialize)
            {
                if (Projectile.IsOwnedByLocalPlayer())  //初始化鞭子节点，以及其他信息
                {
                    Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One);
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Projectile.netUpdate = true;
                }
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                FinalRotationOffset = SwingAngle * ((int)TimeMax / 2);
                Projectile.timeLeft = (int)TimeMax;
                Projectile.oldPos = new Vector2[CACHE_LENGTH];
                Projectile.oldRot = new float[CACHE_LENGTH];
                for (int i = 0; i < CACHE_LENGTH; i++)
                {
                    Projectile.oldPos[i] = Owner.Center;
                    Projectile.oldRot[i] = Projectile.rotation;
                }
                PerPartLength = 0.1f;
                initialize = false;
            }

            FinalRotationOffset -= SwingAngle;  //更新额外角度
            if (SwingAngle == 0)    //更新每一节点的长度
            {
                if (Projectile.timeLeft > (int)TimeMax / 2)
                    PerPartLength += 16 / (TimeMax / 2);
                else
                    PerPartLength -= 14 / (TimeMax / 2);
            }
            else
            {
                if (Projectile.timeLeft > (int)TimeMax / 2)
                    PerPartLength += 12 / (TimeMax / 2);
                else
                    PerPartLength -= 10 / (TimeMax / 2);
            }

            Projectile.oldPos[0] = Owner.Center;    //更新节点位置及旋转
            Projectile.oldRot[0] = Projectile.rotation + FinalRotationOffset;
            for (int i = 1; i < CACHE_LENGTH; i++)
            {
                float rot = Helper.Lerp(0, FinalRotationOffset, (i - 1) / (float)CACHE_LENGTH);
                Projectile.oldRot[i] = Projectile.rotation + rot;
                Projectile.oldPos[i] = Projectile.oldPos[i - 1] + (Projectile.velocity.RotatedBy(rot) * PerPartLength);
            }

            Lighting.AddLight(Projectile.oldPos[2], new Vector3(0.4f, 0.75f, 0.75f));
            Lighting.AddLight(Projectile.oldPos[CACHE_LENGTH - 1], new Vector3(0.4f, 0.75f, 0.75f));
            Projectile.Center = Owner.Center;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            Owner.itemTime = Owner.itemAnimation = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制线条
            Helper.DrawLine(Projectile.oldPos.ToList(), Coralite.IcicleCyan);

            //绘制骨头节
            Texture2D _boneTex = boneTex.Value;
            Vector2 boneOrigin = _boneTex.Size() / 2;
            for (int i = 3; i < CACHE_LENGTH - 2; i += 2)
            {
                Main.spriteBatch.Draw(_boneTex, Projectile.oldPos[i] - Main.screenPosition, null, lightColor, Projectile.oldRot[i] + 0.785f, boneOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            //绘制骨头尖
            Main.spriteBatch.Draw(boneTipTex.Value, Projectile.oldPos[CACHE_LENGTH - 1] - Main.screenPosition, null, lightColor, Projectile.oldRot[CACHE_LENGTH - 1] + 0.785f, boneTipTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            //绘制握把
            SpriteEffects effect = SpriteEffects.None;
            float rot = Projectile.oldRot[2] + 0.785f;
            if (DirSign < 0)
            {
                rot -= MathHelper.Pi / 2;
                effect = SpriteEffects.FlipVertically;
            }
            Vector2 originCenter = Owner.Center - Main.screenPosition;
            Main.spriteBatch.Draw(handleTex.Value, originCenter + (Projectile.velocity * 14), null, lightColor, rot, handleTex.Size() / 2, Projectile.scale, effect, 0);

            return false;
        }
    }

    /// <summary>
    /// 追踪龙卷风
    /// </summary>
    public class IcicleCoccyx_Ex2 : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.coldDamage = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 500;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 3;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 25 == 0)
                Helper.PlayPitched("Icicle/Wind" + Main.rand.Next(1, 3).ToString(), 0.2f, 0f, Projectile.Center);

            if (Projectile.timeLeft % 2 == 0)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.FrostStaff, -Projectile.velocity * 0.3f, Scale: Main.rand.NextFloat(1.8f, 2f));
                dust.noGravity = true;
            }

            Color tornadoColor = Main.rand.Next(3) switch
            {
                0 => new Color(217, 248, 255, 200),
                1 => new Color(120, 211, 231, 200),
                _ => new Color(252, 255, 255, 200)
            };
            Tornado.Spawn(Projectile.Center, Projectile.velocity * 0.2f, tornadoColor, 60, Projectile.velocity.ToRotation(), Main.rand.NextFloat(0.3f, 0.4f));
            Helper.AutomaticTracking(Projectile, 6, 12);
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }

    /// <summary>
    /// 冲刺攻击 ai[0输入冲刺方向]
    /// </summary>
    public class IcicleCoccyx_Ex3 : BaseHeldProj
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleCoccyx";

        public ref float DashDir => ref Projectile.ai[0];

        public bool onStart = true;

        public override void SetDefaults()
        {
            Projectile.coldDamage = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 48;
            Projectile.timeLeft = 15;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.timeLeft < 10)
                return false;

            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.velocity, 48, ref a);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0)
            {
                Owner.immuneTime += 20;
                Owner.immune = true;
                Projectile.ai[1] = 1;
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            if (onStart && Projectile.IsOwnedByLocalPlayer())
            {
                Vector2 dashDir = Owner.Center.DirectionTo(Main.MouseWorld);
                switch ((int)DashDir)
                {
                    case CoralitePlayer.DashLeft:   //向左双击时如果此时鼠标在玩家右侧，那么固定向左冲刺
                        if (dashDir.X > 0)
                            dashDir = new Vector2(-1, 0);
                        break;
                    case CoralitePlayer.DashRight:
                        if (dashDir.X < 0)
                            dashDir = new Vector2(1, 0);
                        break;
                    default:
                        break;
                }

                Vector2 targetCenter = Owner.Center;

                for (int i = 0; i < 18; i++)    //检测与物块的碰撞
                {
                    for (int j = -1; j < 2; j++)
                    {
                        Vector2 position = targetCenter + (j * 16 * dashDir.RotatedBy(1.57f));
                        Tile tile = Framing.GetTileSafely(position);
                        if (tile.HasSolidTile())
                        {
                            targetCenter -= dashDir * 32;
                            goto checkEnd;
                        }
                    }
                    targetCenter += dashDir * 16;
                }

            checkEnd:
                for (int j = -1; j < 2; j++)    //避免在最后判定之后穿墙
                {
                    Vector2 position = targetCenter + (dashDir * 16) + (j * 16 * dashDir.RotatedBy(1.57f));
                    Tile tile = Framing.GetTileSafely(position);
                    if (tile.HasSolidTile())
                        targetCenter -= dashDir * 32;
                }

                //设置这两个检测伤害时需要用到的坐标
                Projectile.Center = Owner.Center;
                Projectile.velocity = targetCenter;

                float dashLength = Vector2.Distance(Owner.Center, targetCenter);
                int lineCount = (int)(dashLength / 16);

                if (dashLength > 64)     //冲刺一定距离才会有速度线产生
                {
                    for (int i = 0; i < lineCount - 2; i++)
                    {
                        PRTLoader.NewParticle(Vector2.Lerp(targetCenter, Owner.Center, (float)i / lineCount) + Main.rand.NextVector2Circular(32, 32), -dashDir * Main.rand.NextFloat(6f, 13f),
                            CoraliteContent.ParticleType<SpeedLine>(), Coralite.IcicleCyan, Main.rand.NextFloat(0.1f, 0.4f));
                    }
                    Helper.PlayPitched("Icicle/Spurt", 0.8f, 0f, Owner.Center);
                }
                lineCount *= 2;
                for (int i = 0; i < lineCount; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Vector2.Lerp(Owner.Center, targetCenter, (float)i / lineCount) + Main.rand.NextVector2Circular(32, 32),
                        DustID.FrostStaff, -dashDir * Main.rand.NextFloat(2f, 10f));
                    dust.noGravity = true;
                }

                Owner.Center = targetCenter;
                Owner.velocity = dashDir * Owner.maxRunSpeed * 3;
                Projectile.rotation = dashDir.ToRotation();
                onStart = false;
            }

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            Owner.itemTime = Owner.itemAnimation = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = mainTex.Size() / 2f;

            float rot = Projectile.rotation + ((float)Math.PI / 4f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.rotation < -(float)Math.PI / 2f || Projectile.rotation > (float)Math.PI / 2f)
            {
                rot += (float)Math.PI / 2f;
                spriteEffects |= SpriteEffects.FlipHorizontally;
            }

            Main.spriteBatch.Draw(mainTex, Owner.Center + (Projectile.rotation.ToRotationVector2() * 28) - Main.screenPosition, null, lightColor, rot, origin, 1f, spriteEffects, 0f);

            return false;
        }
    }
}
