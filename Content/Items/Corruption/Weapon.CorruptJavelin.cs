using Coralite.Content.Items.Crimson;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Corruption
{
    public class CorruptJavelin : ModItem
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<BloodyHook>();
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 30;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 4;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<CorruptJavelinProj>();

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed)
            {
                //检测扎在NPC身上的投矛的数量，并让投矛消失，之后射出2倍数量的小投矛
                int howMany = 0;
                foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.owner == player.whoAmI && p.type == type && p.ai[0] != 3))
                {
                    howMany++;
                    (proj.ModProjectile as CorruptJavelinProj).SpecialKill();
                }

                Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<CorruptJavelinSpecial>(), damage, knockback, Main.myPlayer, howMany);
                SoundEngine.PlaySound(CoraliteSoundID.SummonStaff_Item44, player.Center);

                return false;
            }

            float total = 0;

            //有些丑陋的写法 每次射投矛都得遍历一次
            IEnumerable<Projectile> minions = Main.projectile.Where(p => p.active && p.friendly && p.owner == player.whoAmI && p.minion && p.type != ProjectileID.StardustGuardian);

            foreach (var proj in minions)
                total += proj.minionSlots;

            if (player.maxMinions - total < 1)  //如果当前召唤物达到上限那么就随机一位幸运召唤物把它kill了
            {
                List<Projectile> selectedMinions = minions.ToList();
                selectedMinions.RemoveAll(p => p.type == type && p.ai[0] != 2);
                if (selectedMinions.Count < 1)
                    goto lastShoot;
                Projectile first = selectedMinions[Main.rand.Next(selectedMinions.Count)];
                first?.Kill();
            }

        lastShoot:
            var projectile = Projectile.NewProjectileDirect(source, player.Center + new Vector2(player.direction * Main.rand.Next(24, 32), -64 + Main.rand.Next(8, 8)),
                Vector2.Zero, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
            SoundEngine.PlaySound(CoraliteSoundID.HoneyStaffSummon_Item76, player.Center);

            return false;
        }
    }

    public class CorruptJavelinProj : ModProjectile
    {
        public override string Texture => AssetDirectory.CorruptionItems + "CorruptJavelinProj";

        public const int MaxTimeLeft = 1200;

        public ref float State => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];

        private Vector2 offset;

        private float alpha;
        private float shadowScale;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 8);
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = MaxTimeLeft;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 12;
            Projectile.extraUpdates = 1;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        private enum AIStates
        {
            onSpawn = 0,
            shoot = 1,
            onHit = 2,
            specialKill = 3
        }

        public override void AI()
        {
            //状态0：在手中短暂蓄力，有一个短暂的出场动画
            //状态1：射出
            //状态2：扎在NPC身上时
            //状态3：死亡时，向后退并消失

            switch ((int)State)
            {
                default:
                case (int)AIStates.onSpawn:
                    do
                    {
                        Projectile.spriteDirection = Main.MouseWorld.X > Projectile.Center.X ? 1 : -1;

                        if (Timer < 1 && Projectile.IsOwnedByLocalPlayer())
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 1.5f;
                            Projectile.rotation = Projectile.velocity.ToRotation();
                            Projectile.velocity *= -1;
                            alpha = 0;
                            shadowScale = 1.4f;
                        }

                        if (Timer < 24)
                        {
                            alpha += 1 / 24f;
                            shadowScale -= 0.4f / 24;
                            Projectile.timeLeft = MaxTimeLeft;
                            Projectile.rotation = Projectile.rotation.AngleTowards((Main.MouseWorld - Projectile.Center).ToRotation(), 0.4f);

                            if (Main.rand.NextBool(3))
                            {
                                Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                                    DustID.Corruption, Projectile.velocity * Main.rand.NextFloat(3f, 6f), Scale: Main.rand.NextFloat(1f, 1.5f));
                                dust2.noGravity = true;
                            }
                            break;
                        }

                        Projectile.tileCollide = true;
                        State = 1;
                        Timer = 0;
                        Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
                        SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);

                    } while (false);

                    Timer++;
                    break;
                case (int)AIStates.shoot:
                    //仅仅是生成粒子而已
                    int type = DustID.Shadowflame;
                    Color color = default;
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            break;
                        default:
                            type = DustID.Enchanted_Gold;
                            color = Color.DarkBlue;
                            break;
                    }

                    int howMany = (Projectile.timeLeft % 2) + 1;
                    for (int i = 0; i < howMany; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                            type, Vector2.Zero, newColor: color, Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                    }

                    break;
                case (int)AIStates.onHit:
                    if (Target < 0 || Target > Main.maxNPCs)
                        Projectile.Kill();

                    NPC npc = Main.npc[(int)Target];
                    if (!npc.active || npc.dontTakeDamage)
                        Projectile.Kill();

                    Projectile.Center = npc.Center + offset;
                    Timer = 0;

                    if (Main.rand.NextBool(30))
                    {
                        Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                                DustID.Corruption, -Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust3.noGravity = true;
                    }
                    if (Projectile.timeLeft < 11)
                        SpecialKill();
                    break;
                case (int)AIStates.specialKill:
                    Projectile.timeLeft = 3;
                    alpha -= 0.05f;

                    if (alpha < 0)
                        Projectile.Kill();

                    break;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)State == (int)AIStates.shoot)
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((int)State == (int)AIStates.shoot)
            {
                Projectile.tileCollide = false;
                Projectile.timeLeft = MaxTimeLeft;
                Projectile.velocity *= 0;
                Target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                State = (int)AIStates.onHit;
                Timer = 0;
                Projectile.netUpdate = true;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    Vector2 direction = -Projectile.rotation.ToRotationVector2();
                    Vector2 center = Vector2.Lerp(Projectile.Center, target.Center, 0.2f);

                    Helpers.Helper.SpawnDirDustJet(center, () => direction.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)), 2, 6, (i) => Main.rand.NextFloat(0.5f, 5f),
                        DustID.Corruption, Scale: Main.rand.NextFloat(1f, 1.5f), noGravity: false, extraRandRot: 0.1f);
                }
            }
        }

        public void SpecialKill()
        {
            State = 3;
            Projectile.velocity = -Projectile.rotation.ToRotationVector2() * 0.4f;
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dir = -Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                    DustID.CorruptionThorns, dir * Main.rand.NextFloat(0.5f, 4f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var origin = new Vector2(3 * mainTex.Width / 4, mainTex.Height / 4);
            float Rot = Projectile.rotation + (Projectile.spriteDirection * 0.9f);
            SpriteEffects effect = SpriteEffects.None;

            if (Projectile.spriteDirection < 0)
            {
                Rot += MathHelper.Pi;
                effect = SpriteEffects.FlipHorizontally;
                origin = new Vector2(mainTex.Width / 4, mainTex.Height / 4);
            }

            if ((int)State == 1)//残影绘制
            {
                Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);
                Color color = Color.MediumPurple;
                color.A = 0;
                for (int i = 1; i < 8; i += 2)
                {
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null, color * (0.8f - (i * 0.07f)), Rot, mainTex.Size() / 2, 1 - (i * 0.03f), effect, 0);
                }
            }
            else if ((int)State == 0)
                Main.spriteBatch.Draw(mainTex, pos, null, Color.White * 0.5f, Rot, origin, shadowScale, effect, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor * alpha, Rot, origin, Projectile.scale, effect, 0);
            return false;
        }
    }

    public class CorruptJavelinSpecial : BaseHeldProj
    {
        public override string Texture => AssetDirectory.CorruptionItems + "CorruptJavelinProj";

        public ref float HowMany => ref Projectile.ai[0];
        public ref float ShootCount => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        private float distanceToOwner;
        private float shadowRot;
        private float shadowDistance;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.alpha = 0;
            Projectile.timeLeft = 1000;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = (Main.MouseWorld - Owner.Center).ToRotation() + MathHelper.Pi;
            shadowRot = Main.rand.NextFloat(MathHelper.TwoPi);
            distanceToOwner = 10;
            shadowDistance = 25;
        }

        public override void AI()
        {
            Vector2 targetDir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
            Projectile.Center = Owner.Center + (targetDir * distanceToOwner);
            Projectile.rotation = Projectile.rotation.AngleTowards(targetDir.ToRotation(), 0.3f);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
            Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            Projectile.spriteDirection = Main.MouseWorld.X > Projectile.Center.X ? 1 : -1;

            do
            {
                if (Timer < 20)
                {
                    shadowDistance -= 15 / 20f;
                    shadowRot += 0.25f;
                    break;
                }

                if ((Timer - 20) % 12 < 6)
                {
                    distanceToOwner += 5;
                }
                else
                {
                    distanceToOwner -= 5;
                }

                if (Timer % 6 == 0)
                {
                    if (ShootCount >= HowMany * 2)
                    {
                        Projectile.Kill();
                    }

                    //生成小投矛弹幕
                    if (Projectile.IsOwnedByLocalPlayer())
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)) * 12,
                            ProjectileType<SmallCorruptJavelin>(), (int)(Projectile.damage * 0.85f), Projectile.knockBack * 0.5f, Projectile.owner);
                    SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                    ShootCount++;
                }
            } while (false);
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;
            float rot = Projectile.rotation + (Projectile.spriteDirection * 0.9f);
            SpriteEffects effect = SpriteEffects.None;

            if (Projectile.spriteDirection < 0)
            {
                rot += MathHelper.Pi;
                effect = SpriteEffects.FlipHorizontally;
            }

            if (Timer < 20)
            {
                Color drawColor = lightColor * 0.5f;
                for (int i = 0; i < HowMany; i++)
                {
                    Vector2 offset = (shadowRot + (i * MathHelper.TwoPi / HowMany)).ToRotationVector2() * shadowDistance;
                    Main.spriteBatch.Draw(mainTex, pos + offset, null, drawColor, rot, origin, Projectile.scale, effect, 0);
                }
            }

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, rot, origin, Projectile.scale, effect, 0);
            return false;
        }
    }

    public class SmallCorruptJavelin : ModProjectile
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];

        private Vector2 offset;
        private float alpha;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.alpha = 0;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void AI()
        {
            switch ((int)State)
            {
                default:
                case 0:
                    if (alpha < 0.97f)
                        alpha += 0.1f;
                    break;
                case 1:
                    if (Target < 0 || Target > Main.maxNPCs)
                        Projectile.Kill();

                    NPC npc = Main.npc[(int)Target];
                    if (!npc.active || npc.dontTakeDamage)
                        Projectile.Kill();

                    Projectile.Center = npc.Center + offset;
                    if (Projectile.timeLeft < 40)
                        alpha -= 1 / 40f;

                    break;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return State < 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == 0)
            {
                Projectile.tileCollide = false;
                Projectile.timeLeft = 300;
                Projectile.velocity *= 0;
                Target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                State = 1;
                Projectile.netUpdate = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dir = -Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                    DustID.CorruptionThorns, dir * Main.rand.NextFloat(0.5f, 3f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float Rot = Projectile.rotation + (MathHelper.Pi / 4);
            SpriteEffects effect = SpriteEffects.None;

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor * alpha, Rot, mainTex.Size() / 2, Projectile.scale, effect, 0);
            return false;
        }
    }
}
