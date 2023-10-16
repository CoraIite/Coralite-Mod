using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class Lycoris : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 45;
            Item.reuseDelay = 15;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Bullet;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(320, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 24;
        }

        public override bool RangedPrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

                int projType = ProjectileType<LycorisBullet>();
                int heldProjType = ProjectileType<LycorisHeldProj>();

                if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy >= 7)
                {
                    cp.nightmareEnergy -= 7;
                    Helpers.Helper.PlayPitched("Misc/Zaphkiel", 0.9f, 0f, position);
                    Projectile.NewProjectile(source, position, velocity, projType, damage, knockback, player.whoAmI, 1);
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, heldProjType, 1, 1, player.whoAmI);

                    return false;
                }

                Helpers.Helper.PlayPitched("Misc/Gun", 0.3f, 0f, position);
                Projectile.NewProjectile(source, position, velocity, projType, damage, knockback, player.whoAmI);
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, heldProjType, 1, 1, player.whoAmI);
            }

            return false;
        }

    }

    public class LycorisHeldProj : BaseGunHeldProj
    {
        public LycorisHeldProj() : base(1f, 26, -10, AssetDirectory.NightmareItems) { }

        public override float Ease()
        {
            float x = 1.465f * Projectile.timeLeft / MaxTime;
            return x * MathF.Sin(x * x * x) / 1.186f;
        }

        public override void Initialize()
        {
            base.Initialize();
            float rotation = TargetRot + (OwnerDirection > 0 ? 0 : MathHelper.Pi);
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 center = Projectile.Center + dir * 32;
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(8, 8), DustType<NightmareStar>(), dir.RotatedBy(Main.rand.NextFloat(-0.45f, 0.45f)) * Main.rand.NextFloat(1f, 6f),
                    newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                dust.noGravity = true;
                dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }
    }

    /// <summary>
    /// 使用ai0传入状态，为1时是强化攻击<br></br>
    /// 使用ai1传入射击距离，建议输入鼠标与玩家间的距离
    /// </summary>
    public class LycorisBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Length => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public bool CanSpawnSmallBullet
        {
            get => Projectile.ai[2] == 0;
            set
            {
                if (!value)
                {
                    Projectile.ai[2] = 1;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 3;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 30;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                cp.GetNightmareEnergy(1);
            CanSpawnSmallBullet = false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)//生成一圈小子弹
            {

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class LycorisSmallBullet : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.penetrate = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            //从夜光的弹幕里抄来的，删除了为敌对弹幕时使用的特殊ai
            bool flag = false;
            bool flag2 = false;
            float num = 180f;
            float num2 = 20f;
            float num3 = 0.97f;
            float value = 0.075f;
            float value2 = 0.125f;
            float num4 = 30f;

            if (Projectile.timeLeft == 238)
            {
                int num5 = Projectile.alpha;
                Projectile.alpha = 0;
                Color C = NightmarePlantera.nightmareRed;
                Projectile.alpha = num5;
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 267, Main.rand.NextVector2CircularEdge(3f, 3f) * (Main.rand.NextFloat() * 0.5f + 0.5f), 0, C);
                    dust.scale *= 1.2f;
                    dust.noGravity = true;
                }
            }

            if (Projectile.timeLeft > num)
                flag = true;
            else if (Projectile.timeLeft > num2)
                flag2 = true;

            if (flag)
            {
                float num6 = (float)Math.Cos((float)Projectile.whoAmI % 6f / 6f + Projectile.position.X / 320f + Projectile.position.Y / 160f);
                Projectile.velocity *= num3;
                Projectile.velocity = Projectile.velocity.RotatedBy(num6 * ((float)Math.PI * 2f) * 0.125f * 1f / 30f);
            }

            int num7 = (int)Projectile.ai[0];
            if (Main.npc.IndexInRange(num7) && !Main.npc[num7].CanBeChasedBy(this))
            {
                num7 = -1;
                Projectile.ai[0] = -1f;
                Projectile.netUpdate = true;
            }

            if (num7 == -1)
            {
                int num8 = Projectile.FindTargetWithLineOfSight();
                if (num8 != -1)
                {
                    num7 = num8;
                    Projectile.ai[0] = num8;
                    Projectile.netUpdate = true;
                }
            }

            if (flag2)
            {
                int num9 = (int)Projectile.ai[0];
                Vector2 value3 = Projectile.velocity;

                if (Main.npc.IndexInRange(num9))
                {
                    if (Projectile.timeLeft < 10)
                        Projectile.timeLeft = 10;

                    NPC nPC = Main.npc[num9];
                    value3 = Projectile.DirectionTo(nPC.Center) * num4;
                }
                else
                {
                    Projectile.timeLeft--;
                }

                float amount = MathHelper.Lerp(value, value2, Utils.GetLerpValue(num, 30f, Projectile.timeLeft, clamped: true));
                Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, value3, amount);
                Projectile.velocity *= MathHelper.Lerp(0.85f, 1f, Utils.GetLerpValue(0f, 90f, Projectile.timeLeft, clamped: true));
            }

            Projectile.Opacity = Utils.GetLerpValue(240f, 220f, Projectile.timeLeft, clamped: true);
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            int num45 = Projectile.FindTargetWithLineOfSight();
            if (num45 != -1)
            {
                Projectile.ai[0] = num45;
                Projectile.netUpdate = true;
            }
        }
    }
}
