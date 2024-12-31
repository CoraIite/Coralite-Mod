using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.MTBStructure;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Turbulence : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        private int shootCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(33, 4f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 23, 12f);

            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 2);

            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<TurbulenceHeldProj>(), damage, knockback, player.whoAmI, rot);

            if (Main.rand.NextBool(shootCount,7))
            {
                shootCount = 0;
                type = ProjectileType<TurbulenceArrow>();
                Helper.PlayPitched(CoraliteSoundID.ShadowflameApparition_NPCDeath55, player.Center,pitchAdjust:0.3f);
            }

            shootCount++;

            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<TurbulenceCore>()
                .AddIngredient(ItemID.GraniteBlock,8)
                .AddIngredient(ItemID.LeadBrick, 13)
                .AddIngredient(ItemID.SapphireGemsparkBlock, 7)
                .AddCondition(CoraliteConditions.UseMultiBlockStructure)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 2;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 70;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 12;
            Player.velocity = newVelocity;
            //Player.direction = (int)dashDirection;
            Player.AddImmuneTime(ImmunityCooldownID.General, 14);
            Player.immune = true;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                //Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);
                Helper.PlayPitched(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<TurbulenceHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, new Vector2(dashDirection,0), ProjectileType<TurbulenceHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 12);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class TurbulenceHeldProj : BaseDashBow, IDrawPrimitive, IDrawAdditive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(Turbulence);

        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public PrimitivePRTGroup group;
        public int SPTimer;
        public float handOffset;
        public float trailAlpha;

        [AutoLoadTexture(Name = "TurbulenceArrow")]
        public static ATex TurbulenceArrow1 { get; private set; }

        public override int GetItemType()
            => ItemType<Turbulence>();

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        #region 冲刺攻击

        public override void DashAttackAI()
        {
            Lighting.AddLight(Projectile.Center, (new Color(125, 190, 255) * 0.75f).ToVector3());

            if (Timer < DashTime + 2)
            {
                group ??= [];
                if (Main.myPlayer == Projectile.owner)
                {
                    Owner.itemTime = Owner.itemAnimation = 2;
                    Rotation = Helper.Lerp(RecordAngle, DirSign > 0 ? 0 : 3.141f, Timer / DashTime);
                }

                Owner.velocity.X = Projectile.velocity.X * 10;
                LockOwnerItemTime();

                if (Main.rand.NextBool(3))
                {
                    PRTLoader.NewParticle<SpeedLine>(Projectile.Center + Main.rand.NextVector2Circular(10, 10)
                        , -Owner.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 5f), TurbulenceArrow.RandomColor(), Main.rand.NextFloat(0.2f, 0.4f));
                }
            }
            else if (Timer==DashTime+2)
            {
                Owner.velocity.X = Projectile.velocity.X * 2;
                LockOwnerItemTime();
            }
            else
            {
                if (DownLeft&&SPTimer==0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                    }

                    Projectile.timeLeft = 30;
                    LockOwnerItemTime();
                }
                else
                {
                    if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                    {
                        Helper.PlayPitched(CoraliteSoundID.ShadowflameApparition_NPCDeath55, Projectile.Center,pitchAdjust:-0.5f);
                        Helper.PlayPitched(CoraliteSoundID.Bow_Item5, Projectile.Center);

                        Projectile.NewProjectileFromThis<TurbulenceArrow>(Owner.Center, ToMouse.SafeNormalize(Vector2.Zero) * 16
                            , (int)(Owner.GetWeaponDamage(Owner.HeldItem) * 1.5f), Projectile.knockBack, 1);

                        Rotation = ToMouseAngle;

                        Vector2 dir = Rotation.ToRotationVector2();
                        TurbulenceTornadoParticle.Spawn(Projectile.Center + dir * 16, dir * 2
                            , new Color(134, 229, 251), Rotation, 0.35f);
                        TurbulenceTornadoParticle.Spawn(Projectile.Center + dir * 32, dir * 2
                            , new Color(125, 190, 255), Rotation, 0.2f);

                        if (VisualEffectSystem.HitEffect_ScreenShaking)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 8, 7, 4, 800));

                        handOffset = -20;
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        if (SPTimer < 8)
                            Rotation -= Owner.direction * 0.05f;
                        else
                            Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                    }

                    handOffset = Helper.Lerp(handOffset, 0, 0.1f);
                    SPTimer++;

                    if (SPTimer > 22)
                        Projectile.Kill();
                }
            }

            trailAlpha = Helper.Lerp(trailAlpha, 1, 0.1f);
            if (SPTimer == 0)
                SpawnCircleParticle();

            Projectile.rotation = Rotation;
            Timer++;
        }

        public override void SetCenter()
        {
            base.SetCenter();

            if (Special == 1)
                group?.Update();
        }

        public void SpawnCircleParticle()
        {
            if (Timer % 16 == 0)
            {
                float r = 20 + Main.rand.Next(2) * 8;
                float startRot = Main.rand.NextFloat(-0.6f, -0.4f);
                //总旋转路程除以转速
                float time = (Main.rand.NextFloat(3.6f, 5.2f) - startRot) / TurbulenceCircle.MaxRotateSpeed;

                var p = TurbulenceCircle.Spawn(Projectile.Center,
                         r, time, startRot, Main.rand.NextFromList(0.6f, MathHelper.Pi - 0.6f) + Main.rand.NextFloat(-0.4f, 0.4f)
                         , Main.rand.Next(4) * MathHelper.TwoPi * 0.6f + Main.rand.NextFloat(-0.4f, 0.4f), this);
                p.Color = Color.White;

                group.Add(p);
            }

            if (Timer % 20 == 0)
            {
                float r = 24 + Main.rand.Next(2) * 12;
                float startRot = Main.rand.NextFloat(-0.6f, -0.4f);
                //总旋转路程除以转速
                float time = (Main.rand.NextFloat(4.6f, 7.2f) - startRot) / TurbulenceCircle.MaxRotateSpeed;

                var p = TurbulenceCircle.Spawn(Projectile.Center,
                         r, time, startRot, Main.rand.NextFromList(0.7f, MathHelper.Pi - 0.7f) + Main.rand.NextFloat(-0.3f, 0.3f)
                         , Main.rand.Next(4) * MathHelper.TwoPi * 0.6f + Main.rand.NextFloat(-0.4f, 0.4f), this);
                p.Color = new Color(125, 190, 255) * 0.55f;
                group.Add(p);
            }
        }

        #endregion

        public override Vector2 GetOffset()
            => new Vector2(14 + handOffset, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Special == 0 || SPTimer > 0)
                return false;

            Texture2D tex = TurbulenceArrow.ArrowHighlight.Value;

            Vector2 scale = new Vector2(0.8f, 0.75f * trailAlpha) * 0.55f;
            Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(1, 1) - Projectile.rotation.ToRotationVector2() * 8;
            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(2f);
            effect.Parameters["uBaseImage"].SetValue(TurbulenceArrow.ArrowHighlight.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(TurbulenceArrow.TurbulenceGradient2.Value);
            effect.Parameters["uDissolve"].SetValue(TurbulenceArrow.TurbulenceFlow.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos, null
                , new Color(108, 133, 161), Projectile.rotation, tex.Size() / 2, scale * 1.1f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(255, 255, 255, 120)
                , Projectile.rotation, tex.Size() / 2, scale, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            //绘制箭
            Texture2D arrowTex = TurbulenceArrow1.Value;
            Main.spriteBatch.Draw(arrowTex, center, null, lightColor, Projectile.rotation + 1.57f
                , new Vector2(arrowTex.Width / 2, arrowTex.Height * 5 / 6), 1, 0, 0f);

            return false;
        }

        public void DrawPrimitives()
        {
            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            group?.DrawPrimitive();
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (Special == 0 || SPTimer>0)
                return;

        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class TurbulenceArrow : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied, IDrawWarp, IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        private bool init = true;
        public Trail trail;
        public Trail warpTrail;

        [AutoLoadTexture(Path = AssetDirectory.Particles, Name = "LightShot2")]
        public static ATex ArrowHighlight { get; private set; }
        public static ATex TurbulencePin { get; private set; }
        public static ATex TurbulenceGradient { get; private set; }
        public static ATex TurbulenceGradient2 { get; private set; }
        public static ATex TurbulenceFlow { get; private set; }

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float HitFreeze => ref Projectile.localAI[2];

        public ref float FadeTimer => ref Projectile.localAI[0];
        public ref float HitCount => ref Projectile.ai[2];

        public int trailCount = 10;
        public int trailWidth = 8;
        public float trailAlpha = 1;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * Projectile.MaxUpdates * 4;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition()
            => HitFreeze < 1;

        public override bool? CanDamage()
            => FadeTimer < 1;

        public override void AI()
        {
            Initialize();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;
            if (HitFreeze > 0)
            {
                HitFreeze--;
                return;
            }

            bool normal = State == 0;

            UpdateOldPos(normal);

            if (FadeTimer > 0)
            {
                FadeTimer++;

                Projectile.velocity *= 0.85f;
                trailAlpha = 1 - FadeTimer / 25;

                if (FadeTimer > 25)
                    Projectile.Kill();

                return;
            }

            SpawnParticle(normal);
            SpawnDust(normal);
        }

        private void UpdateOldPos(bool normal)
        {
            if (Timer % 2 == 0)
            {
                if (!VaultUtils.isServer)
                {
                    Projectile.UpdateOldPosCache();

                    Vector2[] pos2 = new Vector2[trailCount + 4];

                    //延长一下拖尾数组，因为使用的贴图比较特别
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                        pos2[i] = Projectile.oldPos[i] + Projectile.velocity;

                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    int exLength = normal ? 4 : 12;

                    for (int i = 1; i < 5; i++)
                        pos2[trailCount + i - 1] = Projectile.oldPos[^1] + dir * i * exLength + Projectile.velocity;

                    trail.TrailPositions = pos2;

                    if (!normal)
                        warpTrail.TrailPositions = Projectile.oldPos;
                }
            }
        }

        private void SpawnDust(bool normal)
        {
            if (Main.rand.NextBool(3))
            {
                int width = normal ? 8 : 24;

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(width, width), DustID.IceTorch
                    , Projectile.velocity * Main.rand.NextFloat(0.4f, 0.8f), 75, Scale: Main.rand.NextFloat(1, 1.4f));
                d.noGravity = true;
            }
        }

        private void SpawnParticle(bool normal)
        {
            if (!normal)
            {
                if (Timer % 8 == 0 && Main.rand.NextBool(3))
                    TurbulenceTornadoParticle.Spawn(Projectile.Center, Projectile.velocity / 6
                        , RandomColor(), Projectile.rotation, Main.rand.NextFloat(0.1f, 0.3f));

                if (Timer % 4 == 0 && Main.rand.NextBool())
                {
                    int width = normal ? 6 : 12;

                    PRTLoader.NewParticle<SpeedLine>(Projectile.Center + Main.rand.NextVector2Circular(width, width)
                        , Projectile.velocity * Main.rand.NextFloat(0.4f, 1f), RandomColor(), Main.rand.NextFloat(0.1f, 0.3f));
                }
            }
        }

        public void Initialize()
        {
            if (init)
            {
                init = false;

                if (State == 1)
                {
                    trailWidth = 25;
                    trailCount = 16;
                }

                if (!VaultUtils.isServer)
                {
                    Projectile.InitOldPosCache(trailCount);
                    trail = new Trail(Main.instance.GraphicsDevice, trailCount + 4, new EmptyMeshGenerator()
                        , f => trailWidth * trailAlpha, f => new Color(255, 255, 255, 170));//=> Color.Lerp(Color.Transparent, Color.White,f.X));
                    if (State == 1)
                        warpTrail = new Trail(Main.instance.GraphicsDevice, trailCount, new EmptyMeshGenerator()
                            , f => (trailWidth + 30) * trailAlpha, f =>
                            {
                                float r = (Projectile.rotation) % 6.18f;
                                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                                float p = 1 - f.X;
                                return new Color(dir, p, 0f, p);
                            });
                }
            }
        }

        public static Color RandomColor()
        {
            return Main.rand.Next(3) switch
            {
                0 => new Color(190, 170, 251),
                1 => new Color(134, 229, 251),
                _ => new Color(125, 190, 255)
            };
        }

        public void TurnToFade()
        {
            Projectile.tileCollide = false;
            if (FadeTimer == 0)
                FadeTimer = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitFreeze = Projectile.MaxUpdates;

            HitCount++;
            if (State == 0)
            {
                if (HitCount > 2)
                    TurnToFade();
            }
            else
            {
                if (HitCount > 4)
                    TurnToFade();
            }

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                float rot = Projectile.rotation + MathHelper.Pi;
                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextFromList(DustID.ApprenticeStorm, DustID.IceTorch)
                        , (rot + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(2f, 8f), 50, Scale: Main.rand.NextFloat(1f, 2f));
                    d.noGravity = true;
                }

                rot -= MathHelper.Pi;
                for (int i = 0; i < 8; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextFromList(DustID.ApprenticeStorm, DustID.IceTorch)
                        , (rot + Main.rand.NextFloat(-0.1f, 0.1f)).ToRotationVector2() * Main.rand.NextFloat(1f, 6f), 50, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            TurnToFade();
            Projectile.velocity = oldVelocity;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();

            if (State == 0)
                mainTex = TurbulencePin.Value;

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, (byte)(trailAlpha * 255)), Projectile.rotation + 1.57f,
                mainTex.Size() / 2, Projectile.scale, 0, 0);
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.08f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(TurbulenceGradient2.Value);
            effect.Parameters["uDissolve"].SetValue(TurbulenceFlow.Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            trail?.DrawTrail(effect);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            trail?.DrawTrail(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            //effect = Filters.Scene["TurbulenceWarp"].GetShader().Shader;

            //effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            //effect.Parameters["uFlow"].SetValue(TurbulenceFlow.Value);
            //effect.Parameters["uTransform"].SetValue(Helper.GetTransfromMatrix());
            //effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);

            //warpTrail?.Render(effect);
        }

        public void DrawWarp()
        {
            if (State == 0 || warpTrail == null)
                return;

            Effect effect = Filters.Scene["TurbulenceWarp"].GetShader().Shader;

            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(TurbulenceFlow.Value);
            effect.Parameters["uTransform"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);

            warpTrail?.DrawTrail(effect);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (State == 0)
                return;

            Texture2D tex = ArrowHighlight.Value;

            Vector2 scale = new Vector2(0.8f, 0.55f * trailAlpha) * 0.65f;
            Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2() * 12;
            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(2f);
            effect.Parameters["uBaseImage"].SetValue(ArrowHighlight.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(TurbulenceGradient2.Value);
            effect.Parameters["uDissolve"].SetValue(TurbulenceFlow.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos, null
                , new Color(108, 133, 161), Projectile.rotation, tex.Size() / 2, scale * 1.1f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(255, 255, 255, 120)
                , Projectile.rotation, tex.Size() / 2, scale, 0, 0);
        }
    }

    public class TurbulenceTornadoParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "Tornado2";
        //public override bool ShouldUpdatePosition() => false;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
            Frame = new Rectangle(0, Main.rand.Next(8) * 64, 128, 64);
        }

        public override void AI()
        {
            if (Opacity % 2 == 0)
            {
                Frame.Y += 64;
                if (Frame.Y > 448)
                    Frame.Y = 0;
            }

            if (Opacity < 8)
                Scale *= 1.15f;
            else
                Scale *= 0.965f;

            if (Opacity > 10)
                Color.A = (byte)(Color.A * 0.75f);

            Opacity++;
            if (Opacity > 30 || Color.A < 10)
                active = false;
        }

        public static void Spawn(Vector2 center, Vector2 velocity, Color color, float rotation, float scale = 1f)
        {
            if (VaultUtils.isServer)
                return;

            TurbulenceTornadoParticle particle = PRTLoader.NewParticle<TurbulenceTornadoParticle>(center, velocity, color, scale);
            TurbulenceTornadoParticle particle2 = PRTLoader.NewParticle<TurbulenceTornadoParticle>(center, velocity, color, scale);

            if (particle != null)
            {
                particle.Rotation = rotation + 1.57f;
                particle2.Rotation = rotation + 1.57f;

                particle2.Frame = particle.Frame;
                particle2.PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            }
        }
    }

    public class TurbulenceCircle : TrailParticle
    {
        public override string Texture => AssetDirectory.Particles + "CyanLine";

        static BasicEffect effect;
        public TurbulenceHeldProj proj;
        public float zRot;
        public float alpha = 0;
        public float exRot;
        public float startRot;

        public static float MaxRotateSpeed = 0.25f;

        public TurbulenceCircle()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
                effect.TextureEnabled = true;
            });
        }

        public override void SetProperty()
        {
            MaxRotateSpeed = 0.3f;
            int trailCount = 10;
            trail = new Trail(Main.instance.GraphicsDevice, trailCount, new EmptyMeshGenerator(), factor => 10 * Scale, factor =>
            {
                return new Color(Color.R, Color.G, Color.B, (byte)(255 * alpha));
            });

            oldPositions = new Vector2[trailCount];
            float r = Rotation - trailCount * MaxRotateSpeed;
            for (int i = 0; i < oldPositions.Length; i++)
            {
                float length2 = Helper.EllipticalEase(r, zRot, out float overrideAngle2) * Velocity.X;

                oldPositions[i] = (overrideAngle2 + exRot).ToRotationVector2() * length2;
                r += MaxRotateSpeed;
            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Rotation += MaxRotateSpeed;

            Vector2 pos = proj.Projectile.Center + proj.Projectile.rotation.ToRotationVector2() * 16;//特殊判定，需要手动给这个oldPos赋值

            Velocity.X *= 1.015f;
            float length = Helper.EllipticalEase(Rotation, zRot, out float overrideAngle) * Velocity.X;

            for (int i = 0; i < oldPositions.Length - 1; i++)
                oldPositions[i] = oldPositions[i + 1];

            oldPositions[^1] = (overrideAngle + exRot).ToRotationVector2() * length;

            //for (int i = 0; i < oldPositions.Length; i++)
            //    oldPositions[i] += offset;

            float time = Velocity.Y;

            if (Opacity < (int)(time * 0.4f))
            {
                float factor = Opacity / (time * 0.4f);
                alpha = factor;
            }
            else if (Opacity == (int)(time * 0.4f))
            {
                alpha = 1;
            }

            if (Opacity > (int)(time * 0.9f))
            {
                alpha *= 0.9f;

                if (alpha < 0.02f)
                {
                    active = false;
                }
            }

            Opacity++;

            Vector2[] pos2 = new Vector2[oldPositions.Length];
            for (int i = 0; i < pos2.Length; i++)
                pos2[i] = pos + oldPositions[i];

            trail.TrailPositions = pos2;
        }

        public static TurbulenceCircle Spawn(Vector2 center, float r, float time, float startRot, float zRot, float exRot, TurbulenceHeldProj proj)
        {
            if (VaultUtils.isServer)
                return null;

            TurbulenceCircle p = PRTLoader.PRT_IDToInstances[CoraliteContent.ParticleType<TurbulenceCircle>()].Clone() as TurbulenceCircle;
            p.Position = center;
            p.Velocity = new Vector2(r, time);
            p.Rotation = startRot;
            p.active = true;
            p.ShouldKillWhenOffScreen = false;
            p.Scale = 1;
            p.proj = proj;
            p.zRot = zRot;
            p.exRot = exRot;

            p.SetProperty();

            return p;
        }

        public override void DrawPrimitive()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //effect.Texture = Texture2D.Value;
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
            effect.Texture = TexValue;

            trail?.DrawTrail(effect);
        }
    }

    public class TurbulenceCore() : BasePlaceableItem(Item.sellPrice(gold: 1), ItemRarityID.Orange, TileType<TurbulenceCoreTile>(), AssetDirectory.ThyphionSeriesItems)
    { }

    public class TurbulenceCoreTile : ModTile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);

            AddMapEntry(Color.DarkBlue);

            DustType = DustID.IceTorch;
        }

        public override bool RightClick(int i, int j)
        {
            CoraliteContent.GetMTBS<TurbulenceMultiBlock>().CheckStructure(new Point(i, j + 1));

            return true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            int type = CoraliteContent.MTBSType<TurbulenceMultiBlock>();
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type ==ProjectileType<PreviewMultiblockPeoj>() && p.ai[0] == type))
                p.Kill();
        }
    }

    public class TurbulenceMultiBlock : PreviewMultiblock
    {
        /// <summary>
        /// 铅砖
        /// </summary>
        private int lb => TileID.LeadBrick;
        /// <summary>
        /// 蓝宝石块
        /// </summary>
        private int s => TileID.SapphireGemspark;
        /// <summary>
        /// 花岗岩
        /// </summary>
        private int g => TileID.GraniteBlock;
        /// <summary>
        /// 核心方块
        /// </summary>
        private int core => TileType<TurbulenceCoreTile>();

        public override int[,] StructureTile =>
            new int[5, 11]
            {
                {-1, -1, -1, g, -1, -1, -1, g, -1, -1, -1 },
                {-1, -1, g ,lb, lb,core,lb,lb, g ,-1 ,-1  },
                {-1, g ,lb ,-1,lb ,lb ,lb ,-1, lb, g ,-1  },
                {g ,lb ,-1 ,-1,-1 ,-1 ,-1 ,-1,-1 ,lb ,g   },
                {lb,-1 ,s  ,s  ,s ,s  ,s  ,s ,s  ,-1 ,lb  },
            };

        public override void OnSuccess(Point origin)
        {
            base.OnSuccess(origin);

            KillAll(origin);

            Item.NewItem(new EntitySource_TileBreak(origin.X, origin.Y), origin.ToWorldCoordinates(), ItemType<Turbulence>());
        }
    }
}
