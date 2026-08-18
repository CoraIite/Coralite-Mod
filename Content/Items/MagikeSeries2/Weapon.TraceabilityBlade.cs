using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.DamageClasses;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class TraceabilityBlade : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page4>();

        public override void SetDefaults()
        {
            Item.SetWeaponValues(50, 5, 4);
            Item.DamageType = MagikeDamage.Instance;
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Swing2_Item7;

            Item.useTurn = false;
            Item.noUseGraphic = true;

            Item.GetMagikeItem().MagikeMax = 7500;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //int sp = 0;
            //int count = 2;
            //if (MagikeHelper.TryCosumeMagike(15, Item, player))
            //{
            //    sp = 1;

            //    type = ModContent.ProjectileType<CrystallineSentinelBullet2>();
            //    damage = (int)(damage * 1.35f);
            //    count = 4;

            //    Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, position, pitch: 0.5f);
            //}

            //Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CrystallineTriggerScatterHeldProj>(), 0, knockback, player.whoAmI, ai2: sp);

            //for (int i = 0; i < count; i++)
            //{
            //    Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.1f, 0.1f) * Main.rand.NextFloat(0.8f, 1.15f), type, damage, knockback, player.whoAmI);
            //}
            //Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineEngram>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.MagikeSeries2Item)]
    public class TraceabilityBladeSwing : BaseSwingProj
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];

        [VaultLoaden("{@classPath}" + "TraceabilityBladeSPattack")]
        public static ATex SPattackTex { get; set; }

        [VaultLoaden("{@classPath}" + "TraceabilityBladeGradient")]
        public static ATex GradientTexture { get; set; }

        public TraceabilityBladeSwing() : base(MathHelper.PiOver2+0.3f, trailCount: 62) { }

        public int delay;
        public int alpha;

        public float dir;
        public float offsetLength;
        public float maxLength;
        public Vector2 velocity;

        public override void SetSwingProperty()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 20;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 30 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                dir = Projectile.velocity.ToRotation();//(Projectile.Center - npc.Center).ToRotation();
                maxLength = Vector2.Distance(Main.player[npc.target].Center, npc.Center) + 140;

                if (maxLength > 480)
                    maxLength = 480;
            }

            switch (State)
            {
                default:
                case 0://左键挥舞
                    {

                        Projectile.extraUpdates = 2;
                        alpha = 0;
                        maxTime = int.MaxValue;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        distanceToOwner = -Projectile.height / 2;
                        Projectile.localNPCHitCooldown = 60;
                        Projectile.InitOldPosCache(62);
                    }
                    break;
            }

            base.InitializeSwing();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
        }

        protected override void OnSlash()
        {
            //int timer = (int)Timer - minTime;

            //if (timer % 30 == 0)
            //    onHitTimer = 0;

            //alpha = (int)(Helper.SinEase(timer, maxTime) * 255);
            //if (timer < maxTime / 2)
            //    offsetLength = Helper.SqrtEase(timer, maxTime / 2) * maxLength;
            //else
            //    offsetLength = Helper.SinEase(timer, maxTime) * maxLength;

            switch (State)
            {
                default:
                case 0:
                    {
                        _Rotation = startAngle + Timer * 0.02f;
                        Slasher();
                    }
                    break;
            }

        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 10;
            if (Projectile.scale > 0.8f)
            {
                Projectile.scale *= 0.999f;
            }

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void AIAfter()
        {
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

            if (useShadowTrail || useSlashTrail)
            {
                UpdateCaches();
                UpdateOldPosCaches();
            }
        }
        public void UpdateOldPosCaches()
        {
            for (int i = oldRotate.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            }
            Projectile.oldPos[0] = Top;
        }
        protected override Vector2 OwnerCenter()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                CrystallineSentinel cs = npc.ModNPC as CrystallineSentinel;
                Vector2 pos = (npc.ai[2] > 0 ? cs.P2LeftHandPos : cs.P2RightHandPos);

                return pos + dir.ToRotationVector2() * offsetLength;
            }

            return base.OwnerCenter();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            base.DrawSelf(SPattackTex.Value, origin, Color.White * (alpha / 255f), extraRot);
        }

        protected override void DrawSlashTrail()
        {
            if (oldRotate == null)
                return;

            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = Projectile.oldPos[i];
                Vector2 Top = Center /*+ (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]))*/;
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]))
                    - (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Vanilla.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }
    }

    public class TraceabilityBladeController:ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
    }
}
