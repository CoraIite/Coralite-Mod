using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class BoneRing : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<BoneHand>();
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(320, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.channel = true;
            Item.shootSpeed = 24;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(BoneRing));
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, Item);
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), position, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 24, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }

    /// <summary>
    /// 主体弹幕
    /// </summary>
    public class BoneHand : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float NPCIndex => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float ShootCount => ref Projectile.localAI[1];

        public Player Owner => Main.player[Projectile.owner];

        private Vector2 offset;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.55f;
        }

        public override bool? CanDamage()
        {
            if (State == 0)
                return null;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            NPCIndex = target.whoAmI;
            State = 1;
            Projectile.velocity = Vector2.Zero;
            offset = Projectile.Center - target.Center;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;

            switch (State)
            {
                default:
                case 0://射出，直接挪到鼠标旁边
                    {
                        if (!Owner.channel)
                            State = 2;

                        Owner.itemTime = Owner.itemAnimation = 4;
                        Owner.itemRotation = (Projectile.Center - Owner.Center).ToRotation();
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

                        NPCIndex = -1;
                        Vector2 center = Main.MouseWorld + ((Main.GlobalTimeWrappedHourly / 6 * MathHelper.TwoPi).ToRotationVector2() * 32);
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 80f, 0, 1) * 30;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.5f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.15f);
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Main.rand.NextBool(4))
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(48, 48), DustID.VilePowder,
                                  Projectile.velocity * 0.4f, newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }
                    }
                    break;
                case 1://打到NPC，黏到NPC身上后不断射爪击弹幕
                    {
                        if (!Main.npc.IndexInRange((int)NPCIndex) || !Owner.channel)
                            State = 2;

                        Owner.itemTime = Owner.itemAnimation = 4;
                        Owner.itemRotation = (Projectile.Center - Owner.Center).ToRotation();
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        NPC npc = Main.npc[(int)NPCIndex];

                        if (!npc.active || npc.friendly || !npc.CanBeChasedBy())
                            State = 2;

                        Projectile.scale = Helper.Lerp(Projectile.scale, 0.55f, 0.1f);
                        Projectile.rotation = (npc.Center - Projectile.Center).ToRotation();
                        Projectile.Center = npc.Center + offset;

                        if (Main.mouseRight && Main.mouseRightRelease
                            && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy >= 5)
                        {
                            cp.nightmareEnergy -= 5;

                            IEntitySource source = Projectile.GetSource_FromAI();
                            Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                            Vector2 position = Owner.Center + (dir * 80) + (dir.RotatedBy(MathHelper.PiOver2) * 10 * 15);
                            Vector2 velocity = dir.RotatedBy(-MathHelper.PiOver2) * 10;

                            Projectile.NewProjectile(source, position, velocity, ProjectileType<BoneSilt>(), Owner.GetWeaponDamage(Owner.HeldItem), 0, Projectile.owner);

                            State = 3;
                        }

                        if (Timer > Owner.itemTimeMax)
                        {
                            Timer = 0;
                            //生成弹幕
                            int dir = Main.rand.NextFromList(-1, 1);
                            Vector2 position = Projectile.Center + ((Projectile.rotation + (dir * 2.2f)).ToRotationVector2() * Main.rand.Next(60, 80));
                            Vector2 velocity = (Projectile.rotation - (dir * Main.rand.NextFloat(0.2f, 0.45f))).ToRotationVector2() * Main.rand.Next(16, 20);

                            int state = Main.rand.Next(2);

                            if (state == 1)
                            {
                                position = Projectile.Center + ((Projectile.rotation + (dir * 1.8f)).ToRotationVector2() * Main.rand.Next(70, 90));
                            }

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ProjectileType<BoneClaw>(), Projectile.damage, 0, Projectile.owner,
                              state, ShootCount, -dir);

                            Projectile.scale = 0.25f;
                            ShootCount++;
                            if (ShootCount > 4)
                                ShootCount = 0;
                        }
                        Timer++;
                    }
                    break;
                case 2://返回玩家
                    {
                        Projectile.tileCollide = false;
                        Owner.itemTime = Owner.itemAnimation = 2;
                        Owner.itemRotation = (Projectile.Center - Owner.Center).ToRotation();
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

                        Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One) * 20;
                        if (Projectile.Distance(Owner.Center) < 48 || Timer > 120)
                            Projectile.Kill();

                        Timer++;
                    }
                    break;
                case 3://撕开裂隙
                    {
                        Owner.itemTime = Owner.itemAnimation = 4;
                        Owner.itemRotation = (Projectile.Center - Owner.Center).ToRotation();

                        Projectile.rotation += 0.2f;
                        if (Timer > 20)
                            Projectile.Kill();

                        Timer++;
                    }
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.InsanityShadowFriendly);
            Texture2D mainTex = TextureAssets.Projectile[ProjectileID.InsanityShadowFriendly].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            Color c = NightmarePlantera.nightmareRed * 0.5f;

            Vector2 scale = new(0.55f, Projectile.scale);

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (((Main.GlobalTimeWrappedHourly / 5) + (i * MathHelper.TwoPi / 3)).ToRotationVector2() * 6)
                    , null, c, Projectile.rotation, origin, scale, 0, 0);
            }

            Main.spriteBatch.Draw(mainTex, pos, null, Color.Black, Projectile.rotation, origin, scale, 0, 0);
            return false;
        }
    }

    /// <summary>
    /// 鬼手爪击<br></br>
    /// 使用ai0控制状态，0：直线爪，1：转圈抓<br></br>
    /// 使用ai1传入颜色，为1时可变成红色并可以获得梦魇光能<br></br>
    /// 使用ai2传入攻击方向
    /// </summary>
    public class BoneClaw : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "ClawsTrail2";

        public ref float State => ref Projectile.ai[0];
        public ref float ColorState => ref Projectile.ai[1];
        public ref float RotDir => ref Projectile.ai[2];

        public ref float TrailWidth => ref Projectile.localAI[0];
        public ref float Alpha => ref Projectile.localAI[1];
        public ref float Timer => ref Projectile.localAI[2];

        private Trail trail;

        public static Asset<Texture2D> redGradient;
        public static Asset<Texture2D> purpleGradient;

        private float HowManyRot;
        private bool hited = true;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            redGradient = Request<Texture2D>(AssetDirectory.NightmareItems + "BoneClawGradient1");
            purpleGradient = Request<Texture2D>(AssetDirectory.NightmareItems + "BoneClawGradient2");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            redGradient = null;
            purpleGradient = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 40;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[16];
            for (int i = 0; i < 16; i++)
                Projectile.oldPos[i] = Projectile.Center;

            TrailWidth = Main.rand.Next(26, 32);
            HowManyRot = Main.rand.NextFloat(2f / 18, 3f / 18);
        }

        public override void AI()
        {
            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new NoTip(), WidthFunction, ColorFunction);

            switch (State)
            {
                default:
                case 0://直线
                    {
                        if (Timer < 12)
                        {
                            Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

                            for (int i = 0; i < 15; i++)
                                Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[15], i / 15f);

                            Alpha += 1 / 12f;
                            break;
                        }

                        if (Timer == 18)
                            Projectile.velocity = Vector2.Zero;

                        if (Timer > 18)
                        {
                            Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

                            for (int i = 0; i < 15; i++)
                                Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[15], i / 15f);

                            Alpha -= 0.1f;
                            if (Timer > 34 || Alpha < 0)
                            {
                                Projectile.Kill();
                                return;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 15; i++)
                                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                            Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;
                        }
                    }
                    break;
                case 1://转圈
                    {
                        if (Timer < 18)
                        {
                            for (int i = 0; i < 15; i++)
                                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                            Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

                            float rot;
                            if (Timer < 9)
                            {
                                rot = Helper.Lerp(HowManyRot / 3, HowManyRot * 2, Timer / 9);
                            }
                            else
                            {
                                rot = Helper.Lerp(HowManyRot * 2, HowManyRot / 3, (Timer - 9) / 9);
                            }
                            Projectile.velocity = Projectile.velocity.RotatedBy(RotDir * rot);
                            if (Alpha < 1)
                                Alpha += 0.1f;
                            break;
                        }

                        if (Timer == 18)
                            Projectile.velocity = Vector2.Zero;

                        if (Timer > 18)
                        {
                            Alpha -= 0.05f;
                            if (Timer > 44 || Alpha < 0)
                            {
                                Projectile.Kill();
                                return;
                            }
                        }
                    }
                    break;
            }

            trail.Positions = Projectile.oldPos;
            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hited)
            {
                if (ColorState == 1 && Main.player[Projectile.owner].TryGetModPlayer(out CoralitePlayer cp))
                    cp.GetNightmareEnergy(1);
                hited = false;
            }

            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public float WidthFunction(float factor)
        {
            return Helper.Lerp(TrailWidth / 3, TrailWidth, factor);
        }

        public Color ColorFunction(Vector2 factor)
        {
            return Color.White;
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["AlphaGradientTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D colotTex = ColorState == 1 ? redGradient.Value : purpleGradient.Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
            effect.Parameters["gradientTexture"].SetValue(colotTex);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail.Render(effect);
        }
    }

    /// <summary>
    /// 追踪鬼手
    /// </summary>
    public class BoneHands : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        private Vector2 offset;
        private float alpha;
        private bool init = true;
        public int npcIndex;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 12;
            Projectile.timeLeft = 1200;

            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (init)
            {
                alpha = 1f;
                init = false;
            }

            switch (State)
            {
                default:
                    OnShoot();
                    break;
                case 1:
                    {
                        if (Target < 0 || Target > Main.maxNPCs)
                            Projectile.Kill();

                        NPC npc = Main.npc[(int)Target];
                        if (!npc.active || npc.dontTakeDamage)
                            Projectile.Kill();

                        Projectile.Center = npc.Center + offset;

                        if (Main.rand.NextBool(30))
                        {
                            Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 16),
                                    DustID.Corruption, -Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 1.5f));
                            dust3.noGravity = true;
                        }

                        alpha -= 1 / 45f;
                        if (alpha < 0)
                            Projectile.Kill();
                    }
                    break;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if ((int)State == 0)
                return null;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((int)State == 0)
            {
                Projectile.tileCollide = false;
                Projectile.timeLeft = 120;
                Projectile.velocity *= 0;
                Target = target.whoAmI;
                offset = Projectile.Center - target.Center;
                State = 1;
                Projectile.netUpdate = true;
                alpha = 1;
                init = false;

            }
        }

        public void OnShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                    DustID.VilePowder, Vector2.Zero, newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            bool flag2 = false;
            float num4 = 30f;

            if (Projectile.timeLeft > 20)
                flag2 = true;

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

                Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, value3, 0.15f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (State == 0 && VisualEffectSystem.HitEffect_Dusts)
            {
                Vector2 direction = -Projectile.rotation.ToRotationVector2();

                Helper.SpawnDirDustJet(Projectile.Center, () => direction.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)), 2, 6, (i) => Main.rand.NextFloat(0.5f, 10f),
                    DustID.VilePowder, Scale: Main.rand.NextFloat(1f, 1.5f), noGravity: true, extraRandRot: 0.15f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.InsanityShadowFriendly);
            Texture2D mainTex = TextureAssets.Projectile[ProjectileID.InsanityShadowFriendly].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            Color c = NightmarePlantera.nightmareRed * 0.5f * alpha;

            float scale = 0.45f;

            if ((int)State == 0)//残影绘制
            {
                Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

                for (int i = 1; i < 0; i++)
                    Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    c * (0.5f - (i * 0.5f / 8)), Projectile.oldRot[i], origin, scale, 0, 0);
            }

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (((Main.GlobalTimeWrappedHourly / 5) + (i * MathHelper.TwoPi / 3)).ToRotationVector2() * 4)
                    , null, c, Projectile.rotation, origin, scale, 0, 0);
            }

            Main.spriteBatch.Draw(mainTex, pos, null, Color.DarkGray * alpha, Projectile.rotation, origin, scale, 0, 0);
            return false;
        }
    }

    /// <summary>
    /// 裂隙
    /// </summary>
    public class BoneSilt : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public Vector2 originCenter;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float ExHandAlpha => ref Projectile.localAI[1];

        private NightmareTentacle tentacle;
        public float tentacleWidth;
        public Color tencleColor = NightmarePlantera.nightmareRed;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            originCenter = Projectile.Center;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            tentacle ??= new NightmareTentacle(30, factor => tencleColor, factor =>
            {
                if (factor > 0.5f)
                    return Helper.Lerp(tentacleWidth, 0, (factor - 0.5f) / 0.5f);

                return Helper.Lerp(0, tentacleWidth, factor / 0.5f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.tentacleFlowTex);

            Vector2 dir = Projectile.Center - originCenter;
            tentacle.rotation = dir.ToRotation();
            tentacle.pos = originCenter;
            tentacle.UpdateTentacle(dir.Length() / 30, (i) => 2 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

            switch ((int)State)
            {
                default:
                case 0://撕裂开一个小口子
                    {
                        Projectile.Center = originCenter + (Projectile.velocity * 30);
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        if (Timer < 5)
                        {
                            ExHandAlpha += 1 / 5f;
                            break;
                        }
                        if (Timer == 5)
                        {
                            tentacleWidth = 8;
                        }

                        if (Timer == 35)
                        {
                            Helper.PlayPitched(CoraliteSoundID.BigBOOM_Item62, Projectile.Center, pitch: -0.5f);

                            tentacleWidth = 20;
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://爆开并不断射出小鬼手
                    {
                        if (ExHandAlpha > 0)
                        {
                            ExHandAlpha -= 0.02f;
                            if (ExHandAlpha < 0)
                                ExHandAlpha = 0;
                        }

                        do
                        {
                            if (Timer < 9 * 8)
                            {
                                if ((int)Timer % 9 == 0)
                                {
                                    float factor = Timer / (9 * 12);
                                    float length = dir.Length() * factor;

                                    Vector2 slitCenter = (originCenter + Projectile.Center) / 2;
                                    float angle = factor * MathHelper.TwoPi;
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), slitCenter, Helper.NextVec2Dir() * 22,
                                         ProjectileType<BoneHands>(), Projectile.damage, 0);

                                    SoundEngine.PlaySound(CoraliteSoundID.DeathCalling_Item103, Projectile.Center);
                                }
                                break;
                            }

                            tencleColor *= 0.95f;
                            tentacleWidth *= 0.95f;
                            if (tencleColor.A < 20)
                                Projectile.Kill();
                        } while (false);

                    }
                    break;
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(ProjectileID.InsanityShadowFriendly);
            Texture2D mainTex = TextureAssets.Projectile[ProjectileID.InsanityShadowFriendly].Value;
            Vector2 origin = mainTex.Size() / 2;

            Color c = NightmarePlantera.nightmareRed * 0.5f * ExHandAlpha;

            if (ExHandAlpha > 0)
            {
                float rot = Projectile.rotation - MathHelper.PiOver2;
                Vector2 dir = Projectile.rotation.ToRotationVector2();
                float length = (originCenter - Projectile.Center).Length();
                Vector2 scale = new(0.35f, 0.55f);
                Vector2 handPosLeft = originCenter + (dir * length * 7 / 20f) - (rot.ToRotationVector2() * tentacleWidth) - Main.screenPosition;

                for (int i = 0; i < 3; i++)
                {
                    Main.spriteBatch.Draw(mainTex, handPosLeft + (((Main.GlobalTimeWrappedHourly / 5) + (i * MathHelper.TwoPi / 3)).ToRotationVector2() * 6)
                        , null, c, rot, origin, scale, 0, 0);
                }

                Main.spriteBatch.Draw(mainTex, handPosLeft, null, Color.Black * ExHandAlpha, rot, origin, scale, 0, 0);

                rot += MathHelper.Pi;
                Vector2 handPosRight = originCenter + (dir * length * 13 / 20f) - (rot.ToRotationVector2() * tentacleWidth) - Main.screenPosition;

                for (int i = 0; i < 3; i++)
                {
                    Main.spriteBatch.Draw(mainTex, handPosRight + (((Main.GlobalTimeWrappedHourly / 5) + (i * MathHelper.TwoPi / 3)).ToRotationVector2() * 6)
                        , null, c, rot, origin, scale, 0, 0);
                }

                Main.spriteBatch.Draw(mainTex, handPosRight, null, Color.Black * ExHandAlpha, rot, origin, scale, 0, 0);
            }

            tentacle?.DrawTentacle();

            return false;
        }
    }

    public class BoneRingDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HandOnAcc);
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer.handon == EquipLoader.GetEquipSlot(Mod, "BoneRing", EquipType.HandsOn);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Vector2 pos = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f))
                + drawInfo.drawPlayer.bodyPosition
                + (drawInfo.drawPlayer.bodyFrame.Size() / 2);
            DrawData item = new(Request<Texture2D>(AssetDirectory.NightmareItems + "BoneRing_HandsOn").Value,
                pos,
                drawInfo.drawPlayer.bodyFrame,
                Color.White/*drawInfo.colorArmorBody*/,
                drawInfo.drawPlayer.bodyRotation,
                drawInfo.bodyVect, 1f,
                drawInfo.playerEffect);
            item.shader = drawInfo.cHandOn;
            drawInfo.DrawDataCache.Add(item);
        }
    }
}
