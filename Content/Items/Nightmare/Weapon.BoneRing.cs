using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class BoneRing : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 45;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
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

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.equippedBoneRing = true;

            player.handon = 25;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
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

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0.75f;
        }

        public override bool? CanDamage()
        {
            if (State == 1)
                return true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            NPCIndex = target.whoAmI;
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

                        NPCIndex = -1;
                        Vector2 center = Main.MouseWorld + (Main.GlobalTimeWrappedHourly / 6 * MathHelper.TwoPi).ToRotationVector2() * 32;
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 80f, 0, 1) * 24;

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

                        NPC npc = Main.npc[(int)NPCIndex];

                        if (!npc.active || npc.active || !npc.CanBeChasedBy())
                            State = 2;

                        if (Timer > 20)
                        {

                        }
                        Timer++;
                    }
                    break;
                case 2://返回玩家
                    {
                        Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.One) * 24;
                        if (Projectile.Distance(Owner.Center) < 48)
                            Projectile.Kill();
                    }
                    break;
                case 3://撕开裂隙
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

            Vector2 scale = new Vector2(0.75f, Projectile.scale);

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly / 5 + i * MathHelper.TwoPi / 3).ToRotationVector2() * 6
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
    public class BoneClaw : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
        public ref float State => ref Projectile.ai[0];
        public ref float ColorState => ref Projectile.ai[1];
        public ref float RotDir => ref Projectile.ai[2];

        private Trail trail;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new NoTip(), WidthFunction, ColorFunction);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public float WidthFunction(float factor)
        {
            if (factor < 0.3f)
                return Helper.Lerp(0, TrailWidth, factor / 0.3f);
            return Helper.Lerp(TrailWidth, 0, (factor - 0.3f) / 0.7f);
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

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(TextureAssets.Projectile[Projectile.type].Value);
            effect.Parameters["gradientTexture"].SetValue(LostSevensideSlash.GradientTexture.Value);
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

        public ref float State => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];

        private Vector2 offset;
        private float alpha;
        private bool init = true;
        public int npcIndex;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
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

            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (init)
            {
                alpha += 0.05f;
                if (alpha > 1)
                {
                    alpha = 1f;
                    init = false;
                }
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

                        if (Projectile.timeLeft < 30)
                        {
                            alpha -= 1 / 32f;
                        }
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
                    DustID.PlatinumCoin, Vector2.Zero, newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            #region 同叶绿弹的追踪，但是范围更大
            float velLength = Projectile.velocity.Length();
            float localAI0 = Projectile.localAI[0];
            if (localAI0 == 0f)
            {
                Projectile.localAI[0] = velLength;
                localAI0 = velLength;
            }

            float num186 = Projectile.position.X;
            float num187 = Projectile.position.Y;
            float chasingLength = 900f;
            bool flag5 = false;
            int targetIndex = 0;
            if (npcIndex == 0)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile))
                    {
                        float targetX = Main.npc[i].Center.X;
                        float targetY = Main.npc[i].Center.Y;
                        float num193 = Math.Abs(Projectile.Center.X - targetX) + Math.Abs(Projectile.Center.Y - targetY);
                        if (num193 < chasingLength)
                        {
                            chasingLength = num193;
                            num186 = targetX;
                            num187 = targetY;
                            flag5 = true;
                            targetIndex = i;
                        }
                    }
                }

                if (flag5)
                    npcIndex = targetIndex + 1;

                flag5 = false;
            }

            if (npcIndex > 0f)
            {
                int targetIndex2 = npcIndex - 1;
                if (Main.npc[targetIndex2].active && Main.npc[targetIndex2].CanBeChasedBy(this, ignoreDontTakeDamage: true) && !Main.npc[targetIndex2].dontTakeDamage)
                {
                    float num195 = Main.npc[targetIndex2].Center.X;
                    float num196 = Main.npc[targetIndex2].Center.Y;
                    if (Math.Abs(Projectile.Center.X - num195) + Math.Abs(Projectile.Center.Y - num196) < 1000f)
                    {
                        flag5 = true;
                        num186 = Main.npc[targetIndex2].Center.X;
                        num187 = Main.npc[targetIndex2].Center.Y;
                    }
                }
                else
                    npcIndex = 0;

                Projectile.netUpdate = true;
            }

            if (flag5)
            {
                float num197 = localAI0;
                Vector2 center = Projectile.Center;
                float num198 = num186 - center.X;
                float num199 = num187 - center.Y;
                float dis2Target = MathF.Sqrt(num198 * num198 + num199 * num199);
                dis2Target = num197 / dis2Target;
                num198 *= dis2Target;
                num199 *= dis2Target;
                int chase = 24;

                Projectile.velocity.X = (Projectile.velocity.X * (chase - 1) + num198) / chase;
                Projectile.velocity.Y = (Projectile.velocity.Y * (chase - 1) + num199) / chase;
            }

            #endregion
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

            Color c = NightmarePlantera.nightmareRed * 0.5f;

            Vector2 scale = new Vector2(0.75f, Projectile.scale);

            if ((int)State == 0)//残影绘制
                Projectile.DrawShadowTrails(lightColor * alpha, 0.5f, 0.5f / 8f, 1, 8, 1, 0.785f, -1f);

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly / 5 + i * MathHelper.TwoPi / 3).ToRotationVector2() * 6
                    , null, c, Projectile.rotation, origin, scale, 0, 0);
            }

            Main.spriteBatch.Draw(mainTex, pos, null, Color.DarkGray, Projectile.rotation, origin, scale, 0, 0);
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

        private NightmareTentacle tentacle;
        public float tentacleWidth = 30;
        public Color tencleColor = NightmarePlantera.lightPurple;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1000;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, originCenter);
        }

        public override void OnSpawn(IEntitySource source)
        {
            originCenter = Projectile.Center;
        }

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
            tentacle.UpdateTentacle(dir.Length() / 30, (i) => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

            switch ((int)State)
            {
                default:
                case 0://撕裂开一个小口子
                    {

                    }
                    break;
                case 1:
                    break;
                case 2://爆开并不断射出小鬼手
                    {
                        do
                        {
                            if (Timer < 9 * 12)
                            {
                                if ((int)Timer % 9 == 0)
                                {
                                    float factor = Timer / (9 * 12);
                                    float length = dir.Length() * factor;

                                    Vector2 targetDirection = dir.SafeNormalize(Vector2.Zero);
                                    for (int i = -1; i < 2; i += 2)
                                    {
                                        Vector2 position = originCenter + targetDirection * length;
                                        Vector2 velDir = targetDirection.RotatedBy(i * MathHelper.PiOver2);
                                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), position,
                                           velDir * 15, ModContent.ProjectileType<DarkLeaf>(), Projectile.damage, 0);

                                        for (int j = 0; j < 5; j++)
                                        {
                                            Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<NightmarePetal>(),
                                                    velDir.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(2, 10), newColor: NightmarePlantera.nightPurple);
                                            dust.noGravity = true;
                                        }
                                    }

                                    Vector2 slitCenter = (originCenter + Projectile.Center) / 2;
                                    float angle = factor * MathHelper.TwoPi;
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), slitCenter, (angle + i * MathHelper.Pi).ToRotationVector2() * 12,
                                             ModContent.ProjectileType<DarkLeaf>(), Projectile.damage, 0, ai0: 1);
                                    }

                                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_BlowgunPlus_Item65, Projectile.Center);
                                }
                                break;
                            }

                            tencleColor *= 0.95f;
                            tentacleWidth *= 0.95f;
                            if (tencleColor.A < 20)
                                Projectile.Kill();
                        } while (false);

                        Timer++;
                    }
                    break;
            }

        }

        public static void Exposion()
        {
            foreach (var proj in Main.projectile.Where(p => p.active && p.hostile && p.type == ProjectileType<NightmareSlit>() && p.ai[0] == 1))
            {
                proj.ai[0] = 2;
                (proj.ModProjectile as NightmareSlit).tentacleWidth += 30;

                SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
                st.Pitch = -0.5f;
                SoundEngine.PlaySound(st, proj.Center);

                Vector2 selfCenter = (proj.ModProjectile as NightmareSlit).originCenter;
                Vector2 targetCenter = proj.Center;
                var modifyer = new PunchCameraModifier((targetCenter + selfCenter) / 2, Vector2.One, 20, 8, 20, 1000);
                Main.instance.CameraModifiers.Add(modifyer);

                float maxLength = Vector2.Distance(selfCenter, targetCenter);
                Vector2 dir = (selfCenter - targetCenter).SafeNormalize(Vector2.UnitY);

                for (int i = 0; i < maxLength; i += 8)
                {
                    Vector2 pos = targetCenter + dir * i;
                    for (int j = -1; j < 2; j += 2)
                    {
                        Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(16, 16), DustID.SpookyWood,
                            new Vector2(j, 0) * Main.rand.NextFloat(1, 8), Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
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
                + drawInfo.drawPlayer.bodyFrame.Size() / 2;
            DrawData item = new DrawData(Request<Texture2D>(AssetDirectory.NightmareItems + "BoneRing_HandsOn").Value,
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
