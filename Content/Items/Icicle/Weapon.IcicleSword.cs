using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.GlobalItems;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleSword : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();

        public byte useCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(30, 2.5f);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 1));

            Item.DamageType = DamageClass.Melee;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<IcicleSwordSplash>();
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override float UseTimeMultiplier(Player player)
        {
            if (useCount == 3)
                return 16 / 17f;
            return 16 / 20f;
        }

        public override float UseAnimationMultiplier(Player player)
        {
            if (useCount == 3)
                return 16 / 17f;

            return 16 / 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.One);
            float factor = Math.Clamp(1 - (player.itemTimeMax * 2 / 50f), 0f, 1f);
            float rotate = dir.ToRotation();

            if (useCount == 3)
            {
                Projectile.NewProjectile(source, player.Center, dir * 14, ProjectileType<IcicleSpurt>(), damage, knockback, player.whoAmI, player.direction, (int)(22 * (1f + factor)));
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleSpurtHeldProj>(), damage * 2, knockback, player.whoAmI, 0, rotate);

                Helper.PlayPitched("Icicle/IcicleSword", 0.4f, 0f, player.Center);
                useCount = 0;
                return false;
            }

            //生成普通斩击和剑气
            //将角度限制在一定范围
            //            \     X        X     /         OK
            //                 \           /        OK
            //         OK         O           <---这个角是90°
            //                /             \       OK
            //            /    X       X       \        OK
            if (rotate < 0)
                rotate += 6.282f;           //全是Magic Number。。。。
            if (rotate > 3.926f && rotate < 5.497f)
                rotate = player.direction > 0 ? 5.497f : 3.926f;
            if (rotate > 0.785f && rotate < 2.355f)
                rotate = player.direction > 0 ? 0.785f : 2.355f;

            Projectile.NewProjectile(source, player.Center, rotate.ToRotationVector2() * 9, type, damage, knockback, player.whoAmI, player.direction, (int)(20 * (1f + factor)));
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<IcicleSwordHeldProj>(), damage * 2, knockback, player.whoAmI, useCount);
            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, player.Center);

            useCount++;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>(2)
            .AddIngredient<IcicleScale>()
            .AddIngredient<IcicleBreath>(2)
            .AddTile(TileID.IceMachine)
            .Register();
        }
    }

    public class IcicleSwordHeldProj : BaseSwingProj
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSword";

        public ref float Combo => ref Projectile.ai[0];

        public IcicleSwordHeldProj() : base(new Vector2(46, 56).ToRotation())
        {

        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 34;
            Projectile.height = 68;
            distanceToOwner = 2;
            minTime = 0;
            onHitFreeze = 4;
            Projectile.coldDamage = true;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 1;

            switch (Combo)
            {
                default:
                case 0:
                    startAngle = 2.2f;
                    totalAngle = 3.6f;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    break;
                case 1:
                    startAngle = 1.4f;
                    totalAngle = 3.8f;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    Projectile.scale = 0.9f;

                    break;
                case 2:
                    startAngle = -1.6f;
                    totalAngle = -4.2f;
                    maxTime = (int)(Owner.itemTimeMax * 1.5f);

                    Smoother = Coralite.Instance.SqrtSmoother;
                    break;
            }

            base.InitializeSwing();
        }

        protected override void OnSlash()
        {
            if (Timer < 3 * maxTime / 4f)
            {
                Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
                Dust dust = Dust.NewDustPerfect(Top - (24 * RotateVec2) + Main.rand.NextVector2Circular(30, 30), DustID.ApprenticeStorm,
                       dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(Top - (14 * RotateVec2) + Main.rand.NextVector2Circular(10, 10), DustID.ApprenticeStorm,
                       dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            switch (Combo)
            {
                default:
                case 0:
                    if (Timer < maxTime / 4f)
                        Projectile.scale += 0.04f;
                    else
                        Projectile.scale -= 0.01f;

                    break;
                case 1:
                    if (Timer < maxTime / 8f)
                        Projectile.scale += 0.10f;
                    else
                        Projectile.scale -= 0.015f;

                    break;
                case 2:
                    if (Timer < maxTime / 2f)
                        Projectile.scale += 0.03f;
                    else
                        Projectile.scale -= 0.03f;

                    break;
            }
            base.OnSlash();
        }

        protected override float GetStartAngle() => Owner.direction > 0 ? 0f : MathHelper.Pi;

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FrostStaff, RotateVec2.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(6f, 8f));
                    dust.noGravity = true;
                }
        }
    }

    /// <summary>
    /// ai0 用于存储玩家方向
    /// ai1 用于存储玩家的使用物品时间
    /// </summary>
    public class IcicleSwordSplash : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public ref float OwnerDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[0];
        public bool fadeIn = true;
        public bool canDamage = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.timeLeft = (int)MaxTime;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }

                Alpha += 0.15f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(44, 44), DustID.FrostStaff, -Projectile.velocity * 0.6f);
                dust.noGravity = true;
            }

            if (canDamage)
            {
                Vector2 targetDir = Projectile.rotation.ToRotationVector2();
                for (int i = 0; i < 4; i++)
                {
                    if (Framing.GetTileSafely(Projectile.Center + (targetDir * i * 16)).HasReallySolidTile())
                    {
                        Projectile.timeLeft = 10;
                        canDamage = false;
                        Projectile.netUpdate = true;
                        break;
                    }
                }
            }

            if (Projectile.timeLeft < 10)
            {
                Projectile.velocity *= 0.8f;
                if (Alpha > 0f)
                {
                    if (!fadeIn)
                        Alpha -= 0.1f;
                    Projectile.timeLeft += 1;
                }
            }

            Timer += 1;
        }

        public override bool? CanDamage() => canDamage;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CanHitLine(Projectile.Center, 1, 1, targetHitbox.Center.ToVector2(), 1, 1))
                return base.Colliding(projHitbox, targetHitbox);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.timeLeft > 15)
                Projectile.timeLeft -= 10;

            Projectile.damage = (int)(Projectile.damage * 0.65f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> mainTex = TextureAssets.Projectile[Type];
            Vector2 center = Projectile.Center - Main.screenPosition;
            SpriteEffects effects = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float factor = Timer / MaxTime;
            float num3 = Utils.Remap(factor, 0f, 0.6f, 0f, 1f) * Utils.Remap(factor, 0.6f, 1f, 1f, 0f);

            Main.spriteBatch.Draw(mainTex.Value, center, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, effects, 0f);

            float rotation = Projectile.rotation - (OwnerDirection * 0.4f);
            for (int i = -1; i < 2; i++)
            {
                float scale = 2 - Math.Abs(i);
                Vector2 drawPos = center + ((rotation + (i * 0.6f) + (Utils.Remap(factor, 0f, 2f, 0f, (float)Math.PI / 2f) * OwnerDirection)).ToRotationVector2() * ((mainTex.Width() * 0.5f) - 4f) * Projectile.scale);
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, drawPos, new Color(255, 255, 255, 0) * num3 * 0.5f, Coralite.IcicleCyan, factor, 0f, 0.5f, 0.5f, 1f, (float)Math.PI / 4f, new Vector2(scale, scale), Vector2.One);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(canDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            canDamage = reader.ReadBoolean();
        }
    }

    /// <summary>
    /// ai0 用于存储玩家方向
    /// ai1 用于存储玩家的使用物品时间
    /// </summary>
    public class IcicleSpurt : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public ref float OwnerDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[1];
        public bool fadeIn = true;
        public bool canDamage = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.timeLeft = (int)MaxTime;
                    Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                }

                Alpha += 0.15f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            if (canDamage)
            {
                Vector2 targetDir = (Projectile.rotation - 1.57f).ToRotationVector2();
                for (int i = 0; i < 2; i++)
                {
                    if (Framing.GetTileSafely(Projectile.Center + (targetDir * i * 16)).HasReallySolidTile())
                    {
                        Projectile.timeLeft = 10;
                        canDamage = false;
                        Projectile.netUpdate = true;
                        break;
                    }
                }

                Color lightColor = Lighting.GetColor((Projectile.Center / 16).ToPoint());
                PRTLoader.NewParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 0.15f, CoraliteContent.ParticleType<Fog>(), lightColor * Alpha, Main.rand.NextFloat(0.6f, 0.8f));
                if (Projectile.timeLeft % 3 == 0)
                    PRTLoader.NewParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 0.3f, CoraliteContent.ParticleType<SnowFlower>(), lightColor * Alpha, Main.rand.NextFloat(0.2f, 0.4f));
            }

            Lighting.AddLight(Projectile.Center, Coralite.IcicleCyan.ToVector3());

            if (Projectile.timeLeft < 10)
            {
                Projectile.velocity *= 0.65f;
                if (Alpha > 0f)
                {
                    if (!fadeIn)
                        Alpha -= 0.1f;
                    Projectile.timeLeft += 1;
                }
            }
        }

        public override bool? CanDamage() => canDamage;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
            if (Projectile.damage > 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(140, 220)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(8, 8) - center).SafeNormalize(Vector2.UnitY) * 12;
                Projectile.NewProjectileFromThis<IcicleFalling>(center, velocity,
                     (int)(Projectile.damage * 0.9f), Projectile.knockBack);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            SpriteEffects effects = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, 1.2f, effects, 0f);
            return false;
        }
    }

    public class IcicleSpurtHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSword";

        public ref float DistanceToOwner => ref Projectile.ai[0];

        public ref float _Rotation => ref Projectile.ai[1];
        public ref float Alpha => ref Projectile.localAI[0];
        public bool fadeIn = true;

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = 24;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.width = 34;
            Projectile.height = 68;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
            Projectile.coldDamage = true;
        }

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = _Rotation + (Owner.direction > 0 ? 0 : MathHelper.Pi);
            Owner.itemTime = 2;
            Projectile.Center = Owner.Center + (_Rotation.ToRotationVector2() * DistanceToOwner);

            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.timeLeft = Math.Clamp(Owner.itemTimeMax, 12, 20);
                    Projectile.rotation = _Rotation + 0.785f;
                    DistanceToOwner = 8;
                }

                Alpha += 0.2f;
                if (Alpha > 0.99f)
                {
                    Alpha = 1;
                    fadeIn = false;
                    DistanceToOwner += 8;
                }
            }
            else if (DistanceToOwner < 42)
                DistanceToOwner += 6;

            if (Projectile.timeLeft < 5)
            {
                if (Alpha > 0)
                    Alpha -= 0.2f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Collision.CanHitLine(Owner.MountedCenter, 1, 1, targetHitbox.Center.ToVector2(), 1, 1))
            {
                float a = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + (_Rotation.ToRotationVector2() * Projectile.height / 2), Owner.MountedCenter, Projectile.width / 2, ref a);
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    /// <summary>
    /// 冰柱坠击
    /// </summary>
    public class IcicleFalling : BaseHeldProj
    {
        public override string Texture => AssetDirectory.IcicleProjectiles + "Old_IcicleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 25;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.1f;

            Projectile.coldDamage = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Vector2.UnitY.RotatedBy(i * 0.785f) * 1.5f);
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - 1.57f;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 4; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frozen, 180);
        }
    }
}
