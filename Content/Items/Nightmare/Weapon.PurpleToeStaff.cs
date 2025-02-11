using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class PurpleToeStaff : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 175;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.reuseDelay = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 15;
            Item.knockBack = 3;

            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.rare = RarityType<NightmareRarity>();
            Item.shoot = ProjectileType<PurpleToeProj>();
            Item.UseSound = CoraliteSoundID.TerraprismaSummon_Item82;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.Center + player.velocity + new Vector2(player.direction * 24, 16);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }
    }

    public class PurpleToeProj : BaseHeldProj, INightmareMinion
    {
        public override string Texture => AssetDirectory.NightmareItems + "PurpleToe";

        public ref float Timer => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Powerful => ref Projectile.ai[2];

        public ref float AttackTimer => ref Projectile.localAI[2];

        private RotateTentacle tentacle;
        private Color tentacleColor;
        private Vector2 exVec2 = new(0, -300);

        private const int NormalAttackTime = 50;
        private const int PowerfulAttackTime = 35;

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

            Main.projFrames[Type] = 4;
        }

        public override bool MinionContactDamage() => Timer > 0;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Powerful > 0)
                modifiers.SourceDamage += 0.25f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.alpha = 0;
            Projectile.timeLeft = 300;
            Projectile.minionSlots = 1.5f;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 16;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void Initialize()
        {
            tentacleColor = NightmarePlantera.nightmareSparkleColor;
        }

        public override void AI()
        {
            //在玩家头顶摇摆
            //找到敌人后检测距离，然后伸到敌人面前之后冲刺，再快速返回

            Player player = Main.player[Projectile.owner];
            Vector2 vector = player.Top + new Vector2(0f, -80f);

            if (!CheckActive(player))
                return;

            Projectile.timeLeft = 2;
            Owner.AddBuff(BuffType<PurpleToeBuff>(), 2);

            if (Timer == 0f)
            {
                Helper.GetMyProjIndexWithSameType(Projectile.type, Projectile.whoAmI, Projectile.owner, out int index, out int totalIndexesInGroup);
                float num2 = (float)Math.PI * 2f / totalIndexesInGroup;
                float num3 = totalIndexesInGroup * 0.66f;
                Vector2 vector2 = new Vector2(50f, 8f) / 3f * (totalIndexesInGroup - 1);
                Vector2 vector3 = Vector2.UnitY.RotatedBy((num2 * index) + (Main.GlobalTimeWrappedHourly % num3 / num3 * ((float)Math.PI * 2f)));
                vector += vector3 * vector2;
                vector.Y += player.gfxOffY;
                vector = vector.Floor();

                Vector2 vector4 = vector - Projectile.Center;
                float num4 = 12f;
                float lerpValue = Utils.GetLerpValue(200f, 600f, vector4.Length(), clamped: true);
                num4 += lerpValue * 30f;
                if (vector4.Length() >= 3000f)
                    Projectile.Center = vector;

                Projectile.velocity = vector4;
                if (Projectile.velocity.Length() > num4)
                    Projectile.velocity *= num4 / Projectile.velocity.Length();

                Projectile.UpdateFrameNormally(12, Main.projFrames[Projectile.type] - 1);

                float targetAngle = Projectile.velocity.SafeNormalize(Vector2.UnitY).ToRotation();
                if (vector4.Length() < 40f)
                    targetAngle = -Vector2.UnitY.ToRotation();

                Projectile.rotation = Projectile.rotation.AngleLerp(targetAngle, 0.2f);

                if (Main.rand.NextBool(10))
                    StartAttack(false);
                return;
            }

            if (Timer == -1f)
            {
                if (Target == 0f)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.position);
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.VilePowder, Projectile.oldVelocity.X * 0.2f, Projectile.oldVelocity.Y * 0.2f, 0, default, 1.4f);
                        if (!Main.rand.NextBool(3))
                        {
                            dust.scale *= 1.3f;
                            dust.velocity *= 1.1f;
                        }

                        dust.noGravity = true;
                        dust.fadeIn = 0f;
                    }

                    Projectile.velocity += Main.rand.NextVector2CircularEdge(4f, 4f);
                }

                Projectile.UpdateFrameNormally(4, Main.projFrames[Projectile.type] - 1);
                Target += 1f;
                Projectile.rotation += (Projectile.velocity.X * 0.1f) + (Projectile.velocity.Y * 0.05f);
                Projectile.velocity *= 0.92f;
                if (Target >= 9f)
                {
                    Timer = 0f;
                    Target = 0f;
                }

                return;
            }

            NPC target = null;
            int targetIndex = (int)Target;
            if (Main.npc.IndexInRange(targetIndex) && Main.npc[targetIndex].CanBeChasedBy(this))
                target = Main.npc[targetIndex];

            if (target == null)
            {
                Timer = -1f;
                Target = 0f;
                Projectile.netUpdate = true;
            }
            else if (player.Distance(target.Center) >= 900f)
            {
                Timer = 0f;
                Target = 0f;
                Projectile.netUpdate = true;
            }
            else
            {
                int resetTimer = 1;

                if (Timer < -1)
                {
                    Vector2 dir = (target.Center - Owner.Center).SafeNormalize(Vector2.Zero);
                    Vector2 idlePos = Owner.Center + (dir * 64);
                    Projectile.Center = Projectile.Center.MoveTowards(idlePos, 36);

                    Projectile.rotation = Projectile.rotation.AngleTowards(dir.ToRotation(), (float)Math.PI / 5f);

                    Timer--;
                    if (Vector2.Distance(idlePos, Projectile.Center) < 30 || Timer < -40)
                        StartAttack();
                    return;
                }

                Timer -= 1f;
                if (Timer >= AttackTimer - 1)
                {
                    Projectile.direction = (Projectile.Center.X < target.Center.X) ? 1 : (-1);
                    Vector2 center = Projectile.Center;
                    Projectile.localAI[0] = center.X;
                    Projectile.localAI[1] = center.Y;
                    exVec2 = Helper.NextVec2Dir() * Main.rand.Next(200, 240);
                }

                if (Timer == (int)(AttackTimer / 3))
                {
                    Projectile.frame = 1;
                }
                else if (Timer == (int)(AttackTimer / 4))
                {
                    Projectile.frame = 0;
                }

                float lerpValue2 = Utils.GetLerpValue(AttackTimer, resetTimer, Timer, clamped: true);

                Vector2 originCenter = new(Projectile.localAI[0], Projectile.localAI[1]);
                originCenter += Utils.GetLerpValue(0f, 0.4f, lerpValue2, clamped: true) * exVec2;
                Vector2 v = target.Center - originCenter;
                Vector2 vector6 = v.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(v.Length(), 60f, 150f);
                Vector2 value = target.Center + vector6;
                float lerpValue3 = Utils.GetLerpValue(0.4f, 0.6f, lerpValue2, clamped: true);
                float lerpValue4 = Utils.GetLerpValue(0.6f, 1f, lerpValue2, clamped: true);
                float targetAngle = v.SafeNormalize(Vector2.Zero).ToRotation();
                Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle, (float)Math.PI / 5f);
                Projectile.Center = Vector2.Lerp(originCenter, target.Center, lerpValue3);
                if (lerpValue4 > 0f)
                    Projectile.Center = Vector2.Lerp(target.Center, value, lerpValue4);

                if (Timer <= resetTimer)
                {
                    StartAttack(onAttack: true);
                }
            }

            if (tentacle == null)
            {
                tentacle = new RotateTentacle(20, TentacleColor, TentacleWidth, NightmarePlantera.tentacleTex, NightmareSpike.FlowTex);
                Vector2 dir = Owner.Center - Projectile.Center;
                float distance = dir.Length();
                float tentacleLength = distance * 0.8f / 20f;

                tentacle.SetValue(Projectile.Center, Owner.Center, Projectile.rotation + MathHelper.Pi);
                tentacle.UpdateTentacle(tentacleLength);
            }
            else
            {
                Vector2 dir = Owner.Center - Projectile.Center;
                float distance = dir.Length();
                float tentacleLength = distance * 0.8f / 20f;

                tentacle.SetValue(Projectile.Center, Owner.Center, Projectile.rotation + MathHelper.Pi);
                if (Timer > 0)
                    tentacle.UpdateTentacle(tentacleLength);
                else
                    tentacle.UpdateTentacleSmoothly(tentacleLength);
            }
        }

        public void StartAttack(bool RandomRot = true, bool onAttack = false)
        {
            int startAttackRange = 800;
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(startAttackRange, ref attackTarget, skipIfCannotHitWithOwnBody: false);
            if (attackTarget != -1)
            {
                AttackTimer = Timer = Powerful > 0 ? PowerfulAttackTime : NormalAttackTime;
                if (Powerful > 0)
                {
                    if (!onAttack)
                        Powerful--;
                    tentacleColor = NightmarePlantera.nightmareRed;
                }
                else
                {
                    tentacleColor = NightmarePlantera.nightmareSparkleColor;
                }

                Target = attackTarget;
                Projectile.netUpdate = true;
                Projectile.frame = 2;
                if (onAttack)
                {
                    Projectile.frame = 0;
                    Timer = -2;
                }
            }
            else if (RandomRot)
            {
                Timer = -1f;
                Projectile.netUpdate = true;
            }
        }

        public Color TentacleColor(float factor)
        {
            return Color.Lerp(tentacleColor, Color.Transparent, factor);
        }

        public static float TentacleWidth(float factor)
        {
            if (factor > 0.6f)
                return Helper.Lerp(16, 0, (factor - 0.6f) / 0.4f);

            return Helper.Lerp(0, 16, factor / 0.6f);
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<PurpleToeBuff>());
                return false;
            }

            if (owner.HasBuff(BuffType<PurpleToeBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            tentacle?.DrawTentacle((i) => 4 * MathF.Sin(i / 3 * Main.GlobalTimeWrappedHourly));

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            Vector2 selforigin = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation + MathHelper.PiOver2,
                selforigin, Projectile.scale, 0, 0);

            return false;
        }

        public void GetPower(int howMany)
        {
            Powerful += howMany;
            if (Powerful > Owner.GetModPlayer<CoralitePlayer>().nightmareEnergyMax)
                Powerful = Owner.GetModPlayer<CoralitePlayer>().nightmareEnergyMax;

            if (Powerful > 0)
                tentacleColor = NightmarePlantera.nightmareRed;

            if (howMany > 0)
            {
                float angle = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 12; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, angle.ToRotationVector2() * Main.rand.NextFloat(1f, 4f), newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                    angle += MathHelper.TwoPi / 12;
                }
            }
        }
    }

    public class PurpleToeBuff : ModBuff
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<PurpleToeProj>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            for (int i = 0; i < 1000; i++)
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<PurpleToeProj>() && Main.projectile[i].owner == Main.myPlayer)
                    Main.projectile[i].Kill();

            return true;
        }

    }

}
