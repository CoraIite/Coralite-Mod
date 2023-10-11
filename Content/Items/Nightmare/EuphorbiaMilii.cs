using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    /// <summary>
    /// 刺刺=》下挥后再次向下挥=》上至下挥砍后下至上挥舞2圈=》在手里转一圈后向前刺出<br></br>
    /// 右键消耗能量释放特殊攻击
    /// </summary>
    public class EuphorbiaMilii : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 23;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<EuphorbiaMiliiProj>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(175, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

                if (player.altFunctionUse == 2)
                {
                    //生成弹幕
                    if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy >= 7)//射出特殊弹幕
                    {
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsCut>(), (int)(damage * 14f), knockback,
                            player.whoAmI, 1);
                        cp.nightmareEnergy = 0;
                    }
                    else
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsCut>(), (int)(damage * 1.4f), knockback, player.whoAmI, 0);
                    return false;
                }

                // 生成弹幕
                switch (combo)
                {
                    default:
                    case 0:
                    case 1://刺
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, combo);
                        break;
                    case 2://下挥*2
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, (int)(damage * 1.2f), knockback, player.whoAmI,combo);
                        break;
                    case 3://下挥+上挥*2
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsCut>(), (int)(damage * 1.4f), knockback, player.whoAmI, 0);
                        break;
                    case 4://转圈刺
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsCut>(), (int)(damage * 1.4f), knockback, player.whoAmI, 0);
                        break;
                }

                combo++;
                if (combo > 2)
                    combo = 0;
            }

            return false;
        }

    }

    public class EuphorbiaMiliiProj : BaseSwingProj,IDrawWarp
    {
        public override string Texture => AssetDirectory.NightmareItems + "EuphorbiaMiliiProj";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> GradientTexture;

        public EuphorbiaMiliiProj() : base(0.785f, trailLength: 26) { }

        public int alpha;
        public int delay;
        public int innerCombo;
        public float nextStartAngle;
        public float firstStartAngle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "NormalSlashTrail2a");
            GradientTexture = Request<Texture2D>(AssetDirectory.NightmareItems + "EuphorbiaMiliiGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTexture = null;
            GradientTexture = null;
        }

        public override void SetDefs()
        {
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 50;
            Projectile.height = (int)(118 * 1.414f);
            trailTopWidth = 2;
            minTime = 0;
            onHitFreeze = 8;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 55 * Projectile.scale;
        }

        protected override void Initializer()
        {
            Projectile.extraUpdates = 3;
            alpha = 0;

            switch (Combo)
            {
                default:
                case 0: //刺1
                    startAngle = Main.rand.NextFloat(-0.2f,0.2f);
                    totalAngle =0.001f;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.ReverseX2Smoother;
                    distanceToOwner = -Projectile.height / 2;
                    useSlashTrail = false;
                    break;
                case 1://刺2
                    startAngle = Main.rand.NextFloat(-0.1f, 0.1f);
                    totalAngle = 0.001f;
                    maxTime = Owner.itemTimeMax * 2;
                    Smoother = Coralite.Instance.ReverseX2Smoother;
                    distanceToOwner = -Projectile.height / 2;

                    useSlashTrail = false;
                    break;
                case 2 when innerCombo == 0://下挥1 小幅度转圈
                    startAngle = 1.9f;
                    totalAngle = 4f;
                    maxTime = Owner.itemTimeMax * 3;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    distanceToOwner = -Projectile.height / 2;
                    nextStartAngle = GetStartAngle() - OwnerDirection * 2.4f;
                    delay = 20;
                    useTurnOnStart = false;
                    break;
                case 2 when innerCombo == 1://下挥2 转圈并稍微伸出
                    startAngle = 2.4f;
                    totalAngle = 4.9f;
                    maxTime = Owner.itemTimeMax * 4;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    distanceToOwner = -Projectile.height / 2;
                    delay = 0;
                    break;
                case 3 when innerCombo == 0://下挥 伸出，更类似于挥砍，不会挥到身体后方
                    break;
                case 3 when innerCombo == 1://上挥，转圈
                    break;
                case 3 when innerCombo == 2://上挑
                    break;
                case 4 when innerCombo == 0://在手里转圈圈
                    break;
                case 4 when innerCombo == 1://大力刺出
                    break;
            }

            base.Initializer();
        }

        protected override float GetStartAngle()
        {
            if (innerCombo == 0)
            {
                firstStartAngle = base.GetStartAngle();
                return firstStartAngle;
            }

            return firstStartAngle;
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, NightmarePlantera.nightPurple.ToVector3());
            base.AIBefore();
        }

        protected override void OnSlash()
        {
            //Vector2 dir = RotateVec2.RotatedBy(1.57f * Math.Sign(totalAngle));
            //Dust dust = Dust.NewDustPerfect((Top + Projectile.Center) / 2 + Main.rand.NextVector2Circular(50, 50), DustID.RedMoss,
            //       dir * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
            //dust.noGravity = true;

            int timer = (int)Timer - minTime;
            alpha = (int)(Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime) * 200) + 50;

            switch ((int)Combo)
            {
                default:
                case 0:
                    distanceToOwner = -Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 180;
                    break;
                case 1:
                    distanceToOwner = -Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 180;
                    break;
                case 2 when innerCombo == 0://下挥1 小幅度转圈
                    distanceToOwner = -Projectile.height / 2 + Coralite.Instance.SqrtSmoother.Smoother(timer, maxTime - minTime) * 40;
                    Projectile.scale = 1 + Coralite.Instance.SinSmoother.Smoother(timer, maxTime - minTime) * 0.3f;
                    break;
                case 2 when innerCombo == 1://下挥2 转圈并稍微伸出
                    distanceToOwner = -Projectile.height / 2 + Smoother.Smoother(timer, maxTime - minTime) * 60;
                    //Projectile.scale = 1 + Smoother.Smoother(timer, maxTime - minTime) * 0.3f;
                    Projectile.scale = Helper.EllipticalEase(2.4f - 4.9f * Smoother.Smoother(timer, maxTime - minTime), 1f, 1.4f);

                    break;
                case 3 when innerCombo == 0://下挥 伸出，更类似于挥砍，不会挥到身体后方
                    break;
                case 3 when innerCombo == 1://上挥，转圈
                    break;
                case 3 when innerCombo == 2://上挑
                    break;
                case 4 when innerCombo == 0://在手里转圈圈
                    break;
                case 4 when innerCombo == 1://大力刺出
                    break;
            }
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;

            switch ((int)Combo)
            {
                default:
                    if (Timer > maxTime + delay)
                        Projectile.Kill();

                    break;
                case 2 when innerCombo == 0://下挥1 小幅度转圈
                    _Rotation = _Rotation.AngleTowards(nextStartAngle, 0.15f);
                    distanceToOwner = Helper.Lerp( distanceToOwner,-Projectile.height / 2,0.15f);

                    if (Timer > maxTime + delay)
                    {
                        innerCombo++;
                        Timer = 0;
                        Initializer();
                    }
                    break;
            }

            Slasher();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                Owner.immuneTime += 8;

                if (Owner.TryGetModPlayer(out CoralitePlayer cp2) && cp2.nightmareEnergy < 7)//获得能量
                    cp2.nightmareEnergy++;
                if (Main.netMode == NetmodeID.Server)
                    return;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, 3, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.RedTorch, dir * Main.rand.NextFloat(4f, 12f), Scale: Main.rand.NextFloat(1f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        public void DrawWarp()
        {
            if (Combo != 0 && Combo != 1)
                WarpDrawer(0.75f);
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            Rectangle frameBox = mainTex.Frame(1, 3, 0, 0);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition,frameBox ,
                                                lightColor, Projectile.rotation + extraRot, frameBox.Size()/2, Projectile.scale, CheckEffect(), 0f);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / count;
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
