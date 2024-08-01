using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinStaff : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 57;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 4f;
            Item.maxStack = 1;
            Item.mana = 25;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;

            Item.shoot = ProjectileType<ThunderMinion>();
            Item.UseSound = CoraliteSoundID.SummonStaff_Item44;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer);
                projectile.originalDamage = Item.damage;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ZapCrystal>(2)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }

    public class ThunderveinStaffBuff : ModBuff
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<ThunderMinion>()] > 0)
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
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<ThunderMinion>() && Main.projectile[i].owner == Main.myPlayer)
                    Main.projectile[i].Kill();

            return true;
        }
    }

    public class ThunderMinion : BaseThunderProj
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderProj";

        public ThunderTrail[] trails;

        public ref float State => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float Recorder => ref Projectile.localAI[0];

        private Player Owner => Main.player[Projectile.owner];

        public Vector2 BasePos;

        public bool CanDrawTrail;
        public float alpha = 1;
        public bool Init = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.timeLeft = 300;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void AI()
        {
            //发现敌人时先直接朝敌人冲刺，之后如果能够攻击到敌人那么就直接瞬移到目标头顶再向下戳
            if (!Projectile.CheckMinionOwnerActive<ThunderveinStaffBuff>())
                return;

            if (Init)
            {
                Projectile.InitOldPosCache(10);
                Init = false;
            }

            Owner.AddBuff(BuffType<ThunderveinStaffBuff>(), 2);

            switch (State)
            {
                default:
                case -1://回到玩家头顶
                    {
                        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out var index, out var totalIndexesInGroup);

                        Vector2 idleSpot = CircleMovement(32 + totalIndexesInGroup * 4, 36, accelFactor: 0.6f, angleFactor: 0.9f, baseRot: index * MathHelper.TwoPi / totalIndexesInGroup);
                        if (Projectile.Distance(idleSpot) < 32f)
                        {
                            Timer = 0f;
                            State = 0;
                            Projectile.netUpdate = true;
                            CanDrawTrail = false;
                        }

                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;
                case 0://在玩家头顶盘旋
                    {
                        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out var index2, out var totalIndexesInGroup2);
                        CircleMovement(32 + totalIndexesInGroup2 * 4, 28, accelFactor: 0.4f, angleFactor: 0.9f, baseRot: index2 * MathHelper.TwoPi / totalIndexesInGroup2);
                        Projectile.rotation = (Owner.Center - Projectile.Center).ToRotation();

                        if (Main.rand.NextBool(20))
                        {
                            int index = Projectile.MinionFindTarget();
                            if (index != -1)
                            {
                                Projectile.StartAttack();
                                Projectile.InitOldPosCache(10);
                                State = 1;
                                Timer = 0;
                                Target = index;
                                Projectile.netUpdate = true;
                                CanDrawTrail = false;
                                return;
                            }
                        }
                    }
                    break;
                case 1://发现目标并初次展开攻击，向目标戳刺
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Timer = 0;
                            State = -1;
                            CanDrawTrail = false;
                            alpha = 1;
                            break;
                        }

                        const int AttackTime = 30;
                        float lerpValue2 = Timer / AttackTime;

                        if (Timer == 0)
                            BasePos = Projectile.Center;

                        if (Timer == AttackTime / 2)
                        {
                            InitCaches();
                            ResetCaches();
                            CanDrawTrail = true;
                        }
                        else if (Timer > AttackTime / 2)
                        {
                            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
                                Projectile.oldPos[i] = Vector2.Lerp(Projectile.Center, BasePos, i / (float)Projectile.oldPos.Length);
                            UpdateCaches();
                            Projectile.SpawnTrailDust(DustID.PortalBoltTrail, Main.rand.NextFloat(0.2f, 0.5f), newColor: Coralite.ThunderveinYellow);
                            alpha -= 1f / (AttackTime / 2);
                        }

                        Vector2 originCenter = BasePos;
                        originCenter += new Vector2(0f, Utils.GetLerpValue(0f, 0.4f, lerpValue2, clamped: true) * -100f);
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

                        Timer++;
                        if (Timer > AttackTime)
                        {
                            alpha = 1;

                            int targetIndex = Projectile.MinionFindTarget();
                            if (targetIndex != -1)
                            {
                                State = 2;
                                Timer = 0;
                                Target = targetIndex;
                                Recorder = -1.57f + Main.rand.NextFloat(-0.7f, 0.7f);
                                CanDrawTrail = false;
                                Projectile.Center = target.Center + Recorder.ToRotationVector2() * target.height;
                            }
                            else
                            {
                                State = -1;
                                Timer = 0;
                            }
                        }
                    }
                    break;
                case 2://在目标头顶向下戳刺
                    {
                        const int ReadyTime = 25;
                        const int AttackTime = 10;
                        const int DelayTime = 8;

                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Timer = 0;
                            State = -1;
                            CanDrawTrail = false;
                            alpha = 1;

                            break;
                        }

                        if (Timer == 2)
                            Projectile.InitOldPosCache(10);
                        if (Timer < ReadyTime)
                        {
                            Projectile.velocity = Vector2.Zero;
                            Projectile.Center = target.Center + Recorder.ToRotationVector2() * (Timer * 4 + target.height);
                            Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
                        }
                        else if (Timer == ReadyTime)
                        {
                            CanDrawTrail = true;
                            BasePos = Projectile.Center;
                            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * (Timer * 4 + target.height) / AttackTime;
                            InitCaches();
                            ResetCaches();
                        }
                        else if (Timer < ReadyTime + AttackTime)
                        {
                            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
                                Projectile.oldPos[i] = Vector2.Lerp(Projectile.Center, BasePos, i / (float)Projectile.oldPos.Length);

                            UpdateCaches();

                            Projectile.SpawnTrailDust(DustID.PortalBoltTrail, Main.rand.NextFloat(0.2f, 0.5f), newColor: Coralite.ThunderveinYellow);
                        }
                        else if (Timer < ReadyTime + AttackTime + DelayTime)
                        {
                            Projectile.velocity *= 0.95f;
                            alpha -= 1f / DelayTime;
                            for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
                                Projectile.oldPos[i] = Vector2.Lerp(Projectile.Center, BasePos, i / (float)Projectile.oldPos.Length);

                            UpdateCaches();
                        }
                        else
                        {
                            CanDrawTrail = false;
                            alpha = 1;

                            int targetIndex = Projectile.MinionFindTarget();
                            if (targetIndex != -1)
                            {
                                State = 2;
                                Timer = 0;
                                Target = targetIndex;
                                Recorder = -1.57f + Main.rand.NextFloat(-0.7f, 0.7f);
                                Projectile.Center = target.Center + Recorder.ToRotationVector2() * target.height;
                            }
                            else
                            {
                                State = -1;
                                Timer = 0;
                            }
                        }

                        Timer++;
                    }
                    break;
            }
        }

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public Color ThunderColorFunc_Fade(float factor)
        {
            return ThunderveinDragon.ThunderveinYellowAlpha * ThunderAlpha * (1 - factor);
        }

        public void InitCaches()
        {
            if (trails == null)
            {
                trails = new ThunderTrail[3];
                Asset<Texture2D> thunderTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");

                for (int i = 0; i < trails.Length; i++)
                {
                    trails[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Fade);
                    trails[i].SetRange((0, 6));
                    trails[i].SetExpandWidth(4);
                }

                foreach (var trail in trails)
                {
                    trail.BasePositions = Projectile.oldPos;
                    trail.RandomThunder();
                }
            }
        }

        public void UpdateCaches()
        {
            ThunderWidth = 12;
            ThunderAlpha = alpha;
            foreach (var trail in trails)
            {
                trail.CanDraw = Main.rand.NextBool();
                if (trail.CanDraw)
                {
                    trail.BasePositions = Projectile.oldPos;
                    trail.RandomThunder();
                }
            }
        }

        public void ResetCaches()
        {
            foreach (var trail in trails)
            {
                trail.BasePositions = Projectile.oldPos;
                trail.RandomThunder();
            }
        }

        public Vector2 CircleMovement(float distance, float speedMax, float accelFactor = 0.25f, float rollingFactor = 5f, float angleFactor = 0.9f, float baseRot = 0f)
        {
            Vector2 offset = (baseRot + Main.GlobalTimeWrappedHourly / rollingFactor * MathHelper.TwoPi).ToRotationVector2() * distance;
            offset.Y /= 4;
            Vector2 center = Owner.Center + new Vector2(0, -48) + offset;
            Vector2 dir = center - Projectile.Center;

            if (dir.Length() > 2000)
                Projectile.Center = center;

            float velRot = Projectile.velocity.ToRotation();
            float targetRot = dir.ToRotation();

            float speed = Projectile.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 100f, 0, 1) * speedMax;

            Projectile.velocity = velRot.AngleTowards(targetRot, angleFactor).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, accelFactor);
            return center;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (CanDrawTrail)
            {
                foreach (var trail in trails)
                    if (trail.CanDraw)
                        trail.DrawThunder(Main.instance.GraphicsDevice);
            }

            Projectile.QuickDraw(Color.White * alpha, Projectile.scale * 1.1f, 1.57f);
            Projectile.QuickDraw(lightColor * alpha, 1.57f);

            return false;
        }
    }
}
