using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Turbulence : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(30, 4f, 4);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 24, 9f);

            Item.rare = ItemRarityID.Pink;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 4);

            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<TurbulenceHeldProj>(), damage, knockback, player.whoAmI, rot);

            Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<TurbulenceArrow>(), damage, knockback, player.whoAmI,1);

            return false;
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<FarAwaySky>()
            //    .AddIngredient(ItemID.SoulofSight, 15)
            //    .AddIngredient(ItemID.CrystalShard, 12)
            //    .AddTile(TileID.WorkBenches)
            //    .Register();
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
                        newVelocity.X = dashDirection * 8;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 26;
            Player.velocity = newVelocity;
            Player.AddImmuneTime(ImmunityCooldownID.General, 25);
            Player.immune = true;
            //Player.direction = (int)dashDirection;

            Main.instance.CameraModifiers.Add(new MoveModifyer(3, 12));

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
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<TurbulenceHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 20);
            }

            return true;
        }
    }

    public class TurbulenceHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(Turbulence);

        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public override int GetItemType()
            => ItemType<Turbulence>();

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        public override void DashAttackAI()
        {
            if (Timer < DashTime + 2)
            {
                Owner.itemTime = Owner.itemAnimation = 2;
                Rotation = Helper.Lerp(RecordAngle, DirSign > 0 ? 0 : 3.141f, Timer / DashTime);

            }
            else
            {

            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Special == 0 || Timer < DashTime / 2)
                return false;

            //绘制箭
            //Texture2D arrowTex = HorizonArcArrow.Value;
            //Vector2 dir = Rotation.ToRotationVector2();
            //Main.spriteBatch.Draw(arrowTex, center, null, lightColor, Projectile.rotation + 1.57f
            //    , new Vector2(arrowTex.Width / 2, arrowTex.Height * 5 / 6), 1, 0, 0f);

            return false;
        }
    }

    [AutoLoadTexture(Path =AssetDirectory.ThyphionSeriesItems)]
    public class TurbulenceArrow : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied,IDrawWarp,IPostDrawAdditive
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
        public ref float HitFreeze => ref Projectile.ai[2];

        public int trailCount = 10;
        public int trailWidth = 8;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * Projectile.MaxUpdates * 4;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition()
            => HitFreeze < 1;

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

                    trail.Positions = pos2;

                    if (!normal)
                        warpTrail.Positions = Projectile.oldPos;
                }
            }

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

            if (Main.rand.NextBool(3))
            {
                int width = normal ? 8 : 24;

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(width, width), DustID.IceTorch
                    , Projectile.velocity * Main.rand.NextFloat(0.4f, 0.8f), 75, Scale: Main.rand.NextFloat(1, 1.4f));
                d.noGravity = true;
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
                    trail = new Trail(Main.instance.GraphicsDevice, trailCount + 4, new NoTip()
                        , f => trailWidth, f => new Color(255,255,255,170));//=> Color.Lerp(Color.Transparent, Color.White,f.X));
                    if (State == 1)
                        warpTrail = new Trail(Main.instance.GraphicsDevice, trailCount, new NoTip()
                            , f => trailWidth + 30, f =>
                            {
                                float r = (Projectile.rotation) % 6.18f;
                                float dir = (r >= 3.14f ? r - 3.14f : r + 3.14f) / MathHelper.TwoPi;
                                return new Color(dir, 1 - f.X, 0f,1- f.X);
                            });
                }
            }
        }

        public Color RandomColor()
        {
            return Main.rand.Next(3) switch
            {
                0 => new Color(190, 170, 251),
                1 => new Color(134, 229, 251),
                _ => new Color(125, 190, 255)
            };
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitFreeze = Projectile.MaxUpdates;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();

            if (State == 0)
                mainTex = TurbulencePin.Value;

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + 1.57f,
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
            trail?.Render(effect);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            trail?.Render(effect);

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

            warpTrail?.Render(effect);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (State==0)
                return;

            Texture2D tex = ArrowHighlight.Value;

            Vector2 scale = new Vector2(0.8f, 0.55f) * 0.65f;
            Vector2 pos = Projectile.Center - Projectile.rotation.ToRotationVector2()*12;
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
                , new Color(108, 133, 161), Projectile.rotation, tex.Size() / 2, scale*1.1f, 0, 0);

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
            if (Opacity>30||Color.A<10)
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

    public class TurbulencePin : ModProjectile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;
    }
}
